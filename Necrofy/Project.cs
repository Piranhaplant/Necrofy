using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using Newtonsoft.Json;

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
        public static readonly string internalProjectFilesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ProjectFiles");
        public static readonly string internalPatchesPath = Path.Combine(internalProjectFilesPath, "Patches");
        private static readonly HashSet<string> ignoredFileExtensions = new HashSet<string>() { ".sfc", ".nfyz", ".nfyp", ".asm", ".user" };

        public const string ROMExpandPatchName = "ROMExpand.asm";
        public const string OtherExpandPatchName = "OtherExpand.asm";
        public const string RunFromLevelPatchName = "RunFromLevel.asm";

        public readonly string path;
        public readonly string settingsFilename;
        public string SettingsPath => Path.Combine(path, settingsFilename);
        public string UserSettingsPath => SettingsPath + ".user";
        public string BuildDirectory => Path.Combine(path, buildDirectoryName);

        public readonly ProjectSettings settings;
        public readonly ProjectUserSettings userSettings;

        public AssetTree Assets { get; private set; }

        /// <summary>Creates a new project from the given base ROM.</summary>
        /// <param name="baseROM">The path to a ROM that the files in the project will be extracted from.</param>
        /// <param name="path">The path to a directory in which all of the project files will be placed.</param>
        public Project(string baseROM, string path) {
            this.path = FixPath(path);

            Directory.CreateDirectory(path);
            string newBaseROM = Path.Combine(path, baseROMFilename);

            byte[] romData = File.ReadAllBytes(baseROM);
            int extraBytes = romData.Length % 0x2000;
            if (extraBytes != 0x200) {
                extraBytes = 0;
            }
            NStream s = new NStream(new FileStream(newBaseROM, FileMode.Create, FileAccess.ReadWrite, FileShare.Read));
            s.Write(romData, extraBytes, romData.Length - extraBytes);

            ROMInfo info = new ROMInfo(s);

            foreach (Asset asset in info.assets) {
                asset.WriteFile(this);
            }

            s.Close();

            settingsFilename = defaultProjectFilename;
            settings = ProjectSettings.CreateNew();
            settings.WinLevel = info.WinLevel;
            settings.EndGameLevel = info.EndGameLevel;

            userSettings = new ProjectUserSettings();
            WriteSettings();

            ReadAssets();
        }

        /// <summary>Loads an existing project from the given settings file.</summary>
        /// <param name="path">The filename of the settings file.</param>
        public Project(string settingsFile) {
            path = FixPath(Path.GetDirectoryName(settingsFile));
            settingsFilename = Path.GetFileName(settingsFile);
            settings = JsonConvert.DeserializeObject<ProjectSettings>(File.ReadAllText(settingsFile));
            if (File.Exists(UserSettingsPath)) {
                userSettings = JsonConvert.DeserializeObject<ProjectUserSettings>(File.ReadAllText(UserSettingsPath));
            } else {
                userSettings = new ProjectUserSettings();
            }
            ReadAssets();
        }

        private static string FixPath(string path) {
            if (!path.EndsWith(Path.DirectorySeparatorChar.ToString())) {
                path += Path.DirectorySeparatorChar;
            }
            return path;
        }

        private void ReadAssets() {
            Assets = new AssetTree(this);
        }
        
        public void WriteSettings() {
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
                                if (!ignoredFileExtensions.Contains(Path.GetExtension(filename))) {
                                    results.AddEntry(new BuildResults.Entry(BuildResults.Entry.Level.WARNING, relativeFilename, "Unknown asset"));
                                }
                                continue;
                            }
                            info.assets.Add(asset);
                        } catch (Exception ex) {
                            results.AddEntry(new BuildResults.Entry(BuildResults.Entry.Level.ERROR, relativeFilename, ex.Message, ex.StackTrace));
                        }
                    }

                    info.assets.Sort();
                    foreach (Asset asset in info.assets) {
                        asset.ReserveSpace(info.Freespace);
                    }
                    foreach (Asset asset in info.assets) {
                        asset.Insert(s, info, this);
                    }

                    // Round size up to the nearest bank
                    s.SetLength((long)Math.Ceiling(s.Length / (double)Freespace.BankSize) * Freespace.BankSize);
                    byte sizeValue = (byte)(Math.Ceiling(Math.Log(s.Length, 2)) - 10);
                    s.Seek(ROMPointers.ROMSize);
                    s.WriteByte(sizeValue);

                    info.Freespace.Fill(s, 0x33);
                }

                info.exportedDefines["win_level"] = settings.WinLevel.ToString();
                info.exportedDefines["end_game_level"] = settings.EndGameLevel.ToString();

                foreach (ProjectSettings.Patch patch in settings.EnabledPatches) {
                    ApplyInternalPatch(outputROM, patch.Name, results, info.exportedDefines);
                }

                foreach (string filename in Directory.GetFiles(path, "*.asm", SearchOption.AllDirectories)) {
                    ApplyPatch(outputROM, filename, results, info.exportedDefines);
                }
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
            // TODO: Don't build if not necessary
            BuildResults results = Build();
            if (results.Success) {
                string runROM = Path.Combine(BuildDirectory, runFromLevelFilename);
                try {
                    File.Copy(Path.Combine(BuildDirectory, buildFilename), runROM, true);

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

                    ApplyInternalPatch(runROM, RunFromLevelPatchName, results, defines);
                } catch (Exception ex) {
                    results.AddEntry(new BuildResults.Entry(BuildResults.Entry.Level.ERROR, "", ex.Message, ex.StackTrace));
                }
                if (results.Success) {
                    RunEmulator(runROM);
                }
            }
            return results;
        }

        private void RunEmulator(string romFile) {
            if (Properties.Settings.Default.useSystemEmulator) {
                Process.Start(romFile);
            } else {
                Process.Start(Properties.Settings.Default.emulator, $"\"{romFile}\"");
            }
        }

        public static void AddEndOfBankFreespace(Stream s, Freespace freespace) {
            AddEndOfBankFreespace(s, freespace, 0xff);
            AddEndOfBankFreespace(s, freespace, 0x00);
        }

        private static void AddEndOfBankFreespace(Stream s, Freespace freespace, byte searchByte) {
            for (int bankEndPos = Freespace.BankSize - 1; bankEndPos < s.Length; bankEndPos += Freespace.BankSize) {
                s.Seek(bankEndPos);
                int length = 0;
                while (s.ReadByte() == searchByte && length < Freespace.BankSize) {
                    length++;
                    s.Seek(-2, SeekOrigin.Current);
                }
                // Leave 2 bytes in case they were part of the end of some data
                if (length >= 2) {
                    freespace.AddSize((int)s.Position + 2, length - 2);
                }
            }
        }

        private static void ApplyInternalPatch(string rom, string patch, BuildResults results, Dictionary<string, string> defines = null) {
            ApplyPatch(rom, Path.Combine(internalPatchesPath, patch), results, defines);
        }

        private static void ApplyPatch(string rom, string patch, BuildResults results, Dictionary<string, string> defines = null) {
            string args = "";
            if (defines != null) {
                foreach (KeyValuePair<string, string> define in defines) {
                    args += string.Format("\"-D{0}={1}\" ", define.Key, define.Value);
                }
            }
            args += string.Format("\"{0}\" \"{1}\"", patch, rom);

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
                }
            } catch (Exception ex) {
                results.AddEntry(new BuildResults.Entry(BuildResults.Entry.Level.ERROR, patch, ex.Message, ex.StackTrace));
            }
        }
    }
}
