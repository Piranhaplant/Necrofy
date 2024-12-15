using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Windows.Forms;
using System.Net.Configuration;

namespace Necrofy
{
    /// <summary>Manages aspects of a ROM hack project including creation, modification, and building.</summary>
    class Project
    {
        public const string defaultProjectFilename = "project.nfyp";
        public const string buildDirectoryName = "build";
        public const string baseROMFilename = "base.sfc";
        public const string buildFilename = "build.sfc";
        public const string runFromLevelFilename = "runFromLevel.sfc";
        public const string recordDemoFilename = "recordDemo.sfc";
        public static readonly string internalProjectFilesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ProjectFiles");
        public static readonly string internalPatchesPath = Path.Combine(internalProjectFilesPath, "Patches");
        private static readonly HashSet<string> ignoredFileExtensions = new HashSet<string>() { ".sfc", ".nfyz", ".nfyp", ".asm", ".user" };

        private const byte FreespaceFillByte = 0x33;

        public const string ROMExpandPatchName = "ROMExpand.asm";
        public const string OtherExpandPatchName = "OtherExpand.asm";
        public const string RunFromLevelPatchName = "RunFromLevel.asm";
        public const string RecordDemoPatchName = "RecordDemo.asm";

        public readonly string path;
        public readonly string settingsFilename;
        public string SettingsPath => Path.Combine(path, settingsFilename);
        public string UserSettingsPath => SettingsPath + ".user";
        public string BuildDirectory => Path.Combine(path, buildDirectoryName);

        public readonly ProjectSettings settings;
        public readonly ProjectUserSettings userSettings;

        public AssetTree Assets { get; private set; }
        private Dictionary<AssetCategory, Dictionary<string, WeakReference<Asset>>> assetCache = new Dictionary<AssetCategory, Dictionary<string, WeakReference<Asset>>>();

        /// <summary>Creates a new project from the given base ROM.</summary>
        /// <param name="baseROM">The path to a ROM that the files in the project will be extracted from.</param>
        /// <param name="path">The path to a directory in which all of the project files will be placed.</param>
        public Project(string baseROM, string path, ISynchronizeInvoke synchronizingObject) {
            this.path = FixPath(path);

            Directory.CreateDirectory(path);
            string newBaseROM = Path.Combine(path, baseROMFilename);

            byte[] romData = File.ReadAllBytes(baseROM);
            int extraBytes = romData.Length % 0x2000;
            if (extraBytes != 0x200) {
                extraBytes = 0;
            }
            
            using (NStream s = new NStream(new FileStream(newBaseROM, FileMode.Create, FileAccess.ReadWrite, FileShare.Read))) {
                s.Write(romData, extraBytes, romData.Length - extraBytes);

                ROMInfo info = new ROMInfo(s);

                foreach (Asset asset in info.assets) {
                    asset.Save(this);
                }

                settingsFilename = defaultProjectFilename;
                settings = ProjectSettings.CreateNew();
                settings.WinLevel = info.WinLevel;
                settings.EndGameLevel = info.EndGameLevel;
                settings.AssetOptions = info.assetOptions;
            }

            userSettings = new ProjectUserSettings();
            WriteSettings();

            ReadAssets(synchronizingObject);
        }

        /// <summary>Loads an existing project from the given settings file.</summary>
        /// <param name="path">The filename of the settings file.</param>
        public Project(string settingsFile, ISynchronizeInvoke synchronizingObject) {
            path = FixPath(Path.GetDirectoryName(settingsFile));
            settingsFilename = Path.GetFileName(settingsFile);
            settings = JsonConvert.DeserializeObject<ProjectSettings>(File.ReadAllText(settingsFile), new AssetOptions.OptionJsonConverter());
            if (File.Exists(UserSettingsPath)) {
                userSettings = JsonConvert.DeserializeObject<ProjectUserSettings>(File.ReadAllText(UserSettingsPath));
            } else {
                userSettings = new ProjectUserSettings();
            }

            if (settings.MajorVersion > ProjectSettings.CurMajorVersion) {
                throw new Exception($"Project was created with a newer version of {Application.ProductName}.");
            }

            if (settings.Version.CompareTo(ProjectSettings.CurVersion) < 0 || settings.AssetOptions.entries.Count == 0) {
                Upgrade();
            }

            ReadAssets(synchronizingObject);
        }

        private static string FixPath(string path) {
            if (!path.EndsWith(Path.DirectorySeparatorChar.ToString())) {
                path += Path.DirectorySeparatorChar;
            }
            return path;
        }

        private void ReadAssets(ISynchronizeInvoke synchronizingObject) {
            Assets = new AssetTree(this, synchronizingObject);
            Assets.AssetChanged += Assets_AssetChanged;
            Assets.RenameCompleted += Assets_RenameCompleted;
        }

        private void Assets_AssetChanged(object sender, AssetEventArgs e) {
            Asset asset = GetCachedAsset(e.Asset.Asset, () => (Asset)null);
            if (asset != null) {
                asset.Reload(this);
            }
        }

        public void WriteSettings() {
            // Remove asset options for assets that no longer exist
            List<AssetOptions.Entry> keepEntries = Assets.Root.Enumerate().Select(a => settings.AssetOptions.GetEntry(a.Category, a.Name)).Where(e => e != null).ToList();
            settings.AssetOptions.entries = keepEntries;

            File.WriteAllText(SettingsPath, JsonConvert.SerializeObject(settings, Formatting.Indented));
            File.WriteAllText(UserSettingsPath, JsonConvert.SerializeObject(userSettings, Formatting.Indented));
        }

        public string GetRelativePath(string filename) {
            Debug.Assert(filename.StartsWith(path));
            return filename.Substring(path.Length);
        }

        public IEnumerable<Asset.NameInfo> GetAssetsInCategory(AssetCategory category) {
            return Assets.Root.Enumerate().Where(a => a.Category == category);
        }

        public T GetCachedAsset<T>(Asset.NameInfo nameInfo, Func<T> getter) where T : Asset {
            if (!assetCache.ContainsKey(nameInfo.Category)) {
                assetCache[nameInfo.Category] = new Dictionary<string, WeakReference<Asset>>();
            }
            if (assetCache[nameInfo.Category].TryGetValue(nameInfo.Name, out WeakReference<Asset> assetRef) && assetRef.TryGetTarget(out Asset cachedAsset)) {
                return (T)cachedAsset;
            } else {
                T asset = getter();
                if (asset != null) {
                    assetCache[nameInfo.Category][nameInfo.Name] = new WeakReference<Asset>(asset);
                }
                return asset;
            }
        }

        private void Assets_RenameCompleted(object sender, RenameCompletedEventArgs e) {
            foreach (KeyValuePair<AssetCategory, Dictionary<string, string>> category in e.Results.renamedAssets) {
                // Update cached asset names
                if (assetCache.TryGetValue(category.Key, out Dictionary<string, WeakReference<Asset>> cache)) {
                    foreach (KeyValuePair<string, string> names in category.Value) {
                        if (cache.TryGetValue(names.Key, out WeakReference<Asset> asset)) {
                            cache.Remove(names.Key);
                            cache[names.Value] = asset;
                        }
                    }
                }

                // Update asset options names
                foreach (KeyValuePair<string, string> names in category.Value) {
                    settings.AssetOptions.SetOptions(category.Key, names.Value, settings.AssetOptions.GetOptions(category.Key, names.Key));
                }
            }
        }

        private void Upgrade() {
            using (NStream s = new NStream(new FileStream(Path.Combine(path, baseROMFilename), FileMode.Open, FileAccess.ReadWrite, FileShare.Read))) {
                ROMInfo info = new ROMInfo(s, new Version(settings.MajorVersion, settings.MinorVersion));
                foreach (Asset asset in info.assets) {
                    asset.Save(this, overriteExisting: false);
                }
                settings.AssetOptions.Merge(info.assetOptions);
            }

            if (settings.MajorVersion == ProjectSettings.CurMajorVersion) {
                // If upgrading only for asset options and project is from a newer version, then don't override minor version
                settings.MinorVersion = Math.Max(settings.MinorVersion, ProjectSettings.CurMinorVersion);
            } else {
                settings.MinorVersion = ProjectSettings.CurMinorVersion;
            }
            settings.MajorVersion = ProjectSettings.CurMajorVersion;
            WriteSettings();
        }

        /// <summary>Builds the project</summary>
        public BuildResults Build() {
            BuildResults results = new BuildResults();
            try {
                if (!Directory.Exists(BuildDirectory)) {
                    Directory.CreateDirectory(BuildDirectory);
                }
                string outputROM = Path.Combine(BuildDirectory, buildFilename);
                File.Copy(Path.Combine(path, baseROMFilename), outputROM, true);

                ROMInfo info;
                using (NStream s = new NStream(new FileStream(outputROM, FileMode.Open, FileAccess.ReadWrite, FileShare.Read))) {
                    info = new ROMInfo(s);
                    info.assets.Clear();
                    AddEndOfBankFreespace(s, info.Freespace);

                    foreach (string filename in Directory.GetFiles(path, "*", SearchOption.AllDirectories)) {
                        string relativeFilename = GetRelativePath(filename);
                        try {
                            Asset asset = Asset.FromFile(this, relativeFilename);
                            if (asset == null) {
                                if (!ignoredFileExtensions.Contains(Path.GetExtension(filename)) && !relativeFilename.StartsWith(buildDirectoryName + Path.DirectorySeparatorChar)) {
                                    results.AddEntry(new BuildResults.Entry(BuildResults.Entry.Level.WARNING, relativeFilename, "Unknown asset"));
                                }
                            } else if (asset.IsSkipped) {
                                Debug.WriteLine("Skipping asset " + relativeFilename);
                            } else {
                                info.assets.Add(asset);
                            }
                        } catch (Exception ex) {
                            results.AddEntry(new BuildResults.Entry(BuildResults.Entry.Level.ERROR, relativeFilename, ex.Message, ex.StackTrace));
                        }
                    }

                    info.assets.Sort();
                    foreach (Asset asset in info.assets) {
                        asset.ReserveSpace(info);
                    }
                    foreach (Asset asset in info.assets) {
                        try {
                            asset.Insert(s, info, this);
                        } catch (Exception ex) {
                            results.AddEntry(new BuildResults.Entry(BuildResults.Entry.Level.ERROR, asset.Filename, ex.Message, ex.StackTrace));
                        }
                    }

                    info.WriteToBuild(s, results);

                    // Round size up to the nearest whole megabyte
                    s.SetLength(((s.Length - 1) / 0x100000 + 1) * 0x100000);
                    info.Freespace.ExpandSize((int)s.Length);
                    info.Freespace.Fill(s, FreespaceFillByte);

                    byte sizeValue = (byte)(Math.Ceiling(Math.Log(s.Length, 2)) - 10);
                    s.Seek(ROMPointers.ROMSize);
                    s.WriteByte(sizeValue);
                }

                info.AddGlobalDefine("win_level", settings.WinLevel.ToString());
                info.AddGlobalDefine("end_game_level", settings.EndGameLevel.ToString());
                info.AddGlobalDefine("extra_sprite_graphics_base", ROMPointers.PointerToHexString(info.ExtraSpriteGraphicsBasePointer));
                info.AddGlobalDefine("extra_sprite_graphics_start_index", info.ExtraSpriteGraphicsStartIndex.ToString());
                info.LogDefines();

                foreach (ProjectSettings.Patch patch in settings.EnabledPatches) {
                    ApplyInternalPatch(outputROM, patch.Name, results, info.globalDefines);
                }

                foreach (string filename in Directory.GetFiles(path, "*.asm", SearchOption.AllDirectories)) {
                    ApplyPatch(outputROM, filename, results, info.globalDefines, info.GetFolderDefines(Path.GetDirectoryName(GetRelativePath(filename))));
                }

                results.symbols.WriteBSNES(outputROM);
            } catch (Exception ex) {
                results.AddEntry(new BuildResults.Entry(BuildResults.Entry.Level.ERROR, "", ex.Message, ex.StackTrace));
            }
            return results;
        }

        public BuildResults Run() {
            // TODO: Don't build if not necessary
            BuildResults results = Build();
            if (results.Success) {
                RunEmulator(Path.Combine(BuildDirectory, buildFilename));
            }
            return results;
        }

        public BuildResults RunFromLevel(int level, RunSettings settings) {
            Dictionary<string, string> defines = new Dictionary<string, string> {
                ["LEVEL"] = level.ToString(),
                ["VICTIMS"] = "$" + settings.victimCount.ToString(),
                ["WEAPON_COUNT"] = settings.weaponAmounts.Length.ToString(),
                ["SPECIAL_COUNT"] = settings.specialAmounts.Length.ToString(),
            };
            for (int i = 0; i < settings.weaponAmounts.Length; i++) {
                defines["WEAPON" + i.ToString()] = "$" + settings.weaponAmounts[i].ToString();
            }
            for (int i = 0; i < settings.specialAmounts.Length; i++) {
                defines["SPECIAL" + i.ToString()] = "$" + settings.specialAmounts[i].ToString();
            }
            return RunWithPatch(runFromLevelFilename, RunFromLevelPatchName, defines).BuildResults;
        }

        public BuildAndRunResults RecordDemo(int level) {
            Dictionary<string, string> defines = new Dictionary<string, string> {
                ["DEMO_LEVEL"] = level.ToString(),
            };
            return RunWithPatch(recordDemoFilename, RecordDemoPatchName, defines);
        }

        private BuildAndRunResults RunWithPatch(string newROMFilename, string internalPatch, params Dictionary<string, string>[] defines) {
            // TODO: Don't build if not necessary
            BuildResults results = Build();
            if (results.Success) {
                string runROM = Path.Combine(BuildDirectory, newROMFilename);
                try {
                    File.Copy(Path.Combine(BuildDirectory, buildFilename), runROM, true);
                    ApplyInternalPatch(runROM, internalPatch, results, defines);
                } catch (Exception ex) {
                    results.AddEntry(new BuildResults.Entry(BuildResults.Entry.Level.ERROR, "", ex.Message, ex.StackTrace));
                }
                if (results.Success) {
                    results.symbols.WriteBSNES(runROM);
                    return new BuildAndRunResults(results, RunEmulator(runROM));
                }
            }
            return new BuildAndRunResults(results);
        }

        private Process RunEmulator(string romFile) {
            if (Properties.Settings.Default.useSystemEmulator) {
                return Process.Start(romFile);
            } else {
                return Process.Start(Properties.Settings.Default.emulator, $"\"{romFile}\"");
            }
        }

        public static string GetEmulatorExecutable(string romFile) {
            if (Properties.Settings.Default.useSystemEmulator) {
                return Win32.FindExecutable(romFile);
            } else {
                return Properties.Settings.Default.emulator;
            }
        }

        public static void AddEndOfBankFreespace(Stream s, Freespace freespace) {
            AddEndOfBankFreespace(s, freespace, 0xff);
            AddEndOfBankFreespace(s, freespace, FreespaceFillByte);
        }

        private static void AddEndOfBankFreespace(Stream s, Freespace freespace, byte searchByte) {
            byte[] bank = new byte[Freespace.BankSize];
            s.Seek(0);
            for (int bankNum = 0; (bankNum + 1) * Freespace.BankSize <= s.Length; bankNum++) {
                s.Read(bank, 0, bank.Length);

                int i = bank.Length;
                while (i > 0 && bank[i - 1] == searchByte) {
                    i--;
                }

                if (i == 0) {
                    freespace.Add(bankNum * Freespace.BankSize, (bankNum + 1) * Freespace.BankSize);
                } else if (i < bank.Length - 2) {
                    // Leave 2 bytes in case they were part of the end of some data
                    freespace.Add(bankNum * Freespace.BankSize + i + 2, (bankNum + 1) * Freespace.BankSize);
                }
            }
        }

        private static void ApplyInternalPatch(string rom, string patch, BuildResults results, params Dictionary<string, string>[] defines) {
            ApplyPatch(rom, Path.Combine(internalPatchesPath, patch), results, defines);
        }

        private static void ApplyPatch(string rom, string patch, BuildResults results, params Dictionary<string, string>[] defines) {
            string args = "";
            foreach (Dictionary <string, string> dict in defines) {
                foreach (KeyValuePair<string, string> define in dict) {
                    args += string.Format("\"-D{0}={1}\" ", define.Key, define.Value);
                }
            }
            string symPath = Path.ChangeExtension(patch, "sym");
            args += $"--symbols=wla --symbols-path=\"{symPath}\" ";
            args += string.Format("\"{0}\" \"{1}\"", patch, rom);
            Console.WriteLine(args);

            ProcessStartInfo processInfo = new ProcessStartInfo(Path.Combine("Tools", "asar.exe")) {
                Arguments = args,
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            };
            try {
                Process p = Process.Start(processInfo);
                p.WaitForExit();
                if (p.ExitCode != 0) {
                    results.AddEntry(new BuildResults.Entry(BuildResults.Entry.Level.ERROR, patch, p.StandardError.ReadToEnd()));
                } else {
                    try {
                        results.symbols.AddWLASymbols(symPath);
                        File.Delete(symPath);
                    } catch (Exception ex) {
                        results.AddEntry(new BuildResults.Entry(BuildResults.Entry.Level.WARNING, patch, ex.Message, ex.StackTrace));
                    }
                }
            } catch (Exception ex) {
                results.AddEntry(new BuildResults.Entry(BuildResults.Entry.Level.ERROR, patch, ex.Message, ex.StackTrace));
            }
        }
    }
}
