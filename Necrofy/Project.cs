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
    public class Project
    {
        public const string baseROMFilename = "base.sfc";
        public const string buildFilename = "build.sfc";

        public readonly string path;
        public readonly string settingsFilename;
        public string settingsPath {
            get {
                return Path.Combine(path, settingsFilename);
            }
        }
        public readonly ProjectSettings settings;

        /// <summary>Creates a new project from the given base ROM.</summary>
        /// <param name="baseROM">The path to a ROM that the files in the project will be extracted from.</param>
        /// <param name="path">The path to a directory in which all of the project files will be placed.</param>
        public Project(string baseROM, string path) {
            this.path = FixPath(path);

            if (Directory.Exists(path)) {
                Directory.Delete(path, true);
            }
            Directory.CreateDirectory(path);
            string newBaseROM = Path.Combine(path, baseROMFilename);
            File.Copy(baseROM, newBaseROM);
            // TODO: Remove header if it exists

            NStream s = new NStream(new FileStream(newBaseROM, FileMode.Open, FileAccess.Read, FileShare.Read));
            ROMInfo info = new ROMInfo(s);

            foreach (Asset asset in info.assets) {
                asset.WriteFile(this);
            }

            s.Close();
            settingsFilename = "project.nfyp";
            settings = new ProjectSettings();
            WriteSettings();
        }

        /// <summary>Loads an existing project from the given settings file.</summary>
        /// <param name="path">The filename of the settings file.</param>
        public Project(string settingsFile) {
            path = FixPath(Path.GetDirectoryName(settingsFile));
            settingsFilename = Path.GetFileName(settingsFile);
            settings = JsonConvert.DeserializeObject<ProjectSettings>(File.ReadAllText(settingsFile));
        }

        private static string FixPath(string path) {
            if (!path.EndsWith(Path.DirectorySeparatorChar.ToString())) {
                path += Path.DirectorySeparatorChar;
            }
            return path;
        }

        private void WriteSettings() {
            File.WriteAllText(settingsPath, JsonConvert.SerializeObject(settings));
        }

        public string GetRelativePath(string filename) {
            Debug.Assert(filename.StartsWith(path));
            return filename.Substring(path.Length);
        }

        /// <summary>Builds the project</summary>
        public void Build() {
            string outputROM = Path.Combine(path, buildFilename);
            File.Copy(Path.Combine(path, baseROMFilename), outputROM, true);
            NStream s = new NStream(new FileStream(outputROM, FileMode.Open, FileAccess.ReadWrite, FileShare.Read));
            ROMInfo info = new ROMInfo(s);
            info.assets.Clear();

            foreach (string filename in Directory.GetFiles(path, "*", SearchOption.AllDirectories)) {
                string relativeFilename = GetRelativePath(filename);
                Asset asset = Asset.FromFile(this, relativeFilename);
                if (asset == null) {
                    // TODO: some sort of error
                    Debug.WriteLine("No asset handled for filename " + relativeFilename);
                    continue;
                }
                info.assets.Add(asset);
            }

            info.assets.Sort();
            foreach (Asset asset in info.assets) {
                asset.ReserveSpace(info.Freespace);
            }
            foreach (Asset asset in info.assets) {
                asset.Insert(s, info);
            }

            // Round size up to the nearest bank
            s.SetLength((long)Math.Ceiling(s.Length / (double)Freespace.BankSize) * Freespace.BankSize);
            byte sizeValue = (byte)(Math.Ceiling(Math.Log(s.Length, 2)) - 10);
            s.Seek(ROMPointers.ROMSize, SeekOrigin.Begin);
            s.WriteByte(sizeValue);

            s.Close();

            ProcessStartInfo process = new ProcessStartInfo(Path.Combine("Tools", "xkas.exe"));
            // TODO: Check that this works with spaces in the path
            process.Arguments = String.Format("\"{0}\" \"{1}\"", Path.Combine("Tools", "ROMExpand.asm"), outputROM);
            process.CreateNoWindow = true;
            process.UseShellExecute = false;
            process.RedirectStandardOutput = true;

            Process p = Process.Start(process);
            p.WaitForExit();
            Console.Out.WriteLine(p.StandardOutput.ReadToEnd());
        }
    }
}
