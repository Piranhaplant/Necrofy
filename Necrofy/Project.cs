using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using System.Diagnostics;

namespace Necrofy
{
    /// <summary>Manages aspects of a ROM hack project including creation, modification, and building.</summary>
    class Project
    {
        private const string TilesetTilemapFilename = "tilemap.bin";
        private const string TilesetCollisionFilename = "collision.bin";
        private const string TilesetGraphicsFilename = "graphics.bin";
        private const string PaletteExtension = ".pal";

        public string path { get; private set; }

        /// <summary>Creates a new project from the given base ROM.</summary>
        /// <param name="baseROM">The path to a ROM that the files in the project will be extracted from.</param>
        /// <param name="path">The path to a directory in which all of the project files will be placed.</param>
        /// <param name="extractLevels">Sets whether the levels will be extracted from the base ROM.</param>
        public Project(string baseROM, string path, bool extractLevels) {
            this.path = path;
            NStream s = new NStream(new FileStream(baseROM, FileMode.Open, FileAccess.Read, FileShare.Read));
            ROMInfo info = new ROMInfo(this, s);
            // Extract all files from the ROM needed for a project
            if (extractLevels) {
                Directory.CreateDirectory(GetLevelFolderPath());
                for (int i = 0; i < info.Levels.Count; i++) {
                    Level l = info.Levels[i];
                    File.WriteAllText(GetLevelFilename(i), JsonConvert.SerializeObject(l));
                }
            }

            Directory.CreateDirectory(GetTilesetFolderPath());
            foreach (KeyValuePair<int, string> tilemap in info.TilesetTilemapNames) {
                s.Seek(tilemap.Key, SeekOrigin.Begin);
                byte[] tilemapData = ZAMNCompress.Decompress(s);
                string fileName = GetTilesetTilemapFilename(tilemap.Value);
                Directory.CreateDirectory(Path.GetDirectoryName(fileName));
                File.WriteAllBytes(fileName, tilemapData);
            }
            foreach (KeyValuePair<int, string> collision in info.TilesetCollisionNames) {
                s.Seek(collision.Key, SeekOrigin.Begin);
                byte[] collisionData = new byte[0x400];
                s.Read(collisionData, 0, collisionData.Length);
                string fileName = GetTilesetCollisionFilename(collision.Value);
                Directory.CreateDirectory(Path.GetDirectoryName(fileName));
                File.WriteAllBytes(fileName, collisionData);
            }
            foreach (KeyValuePair<int, string> graphics in info.TilesetGraphicsNames) {
                s.Seek(graphics.Key, SeekOrigin.Begin);
                byte[] graphicsData = new byte[0x4000];
                s.Read(graphicsData, 0, graphicsData.Length);
                string fileName = GetTilesetGraphicsFilename(graphics.Value);
                Directory.CreateDirectory(Path.GetDirectoryName(fileName));
                File.WriteAllBytes(fileName, graphicsData);
            }
            foreach (KeyValuePair<int, string> palette in info.PaletteNames) {
                s.Seek(palette.Key, SeekOrigin.Begin);
                byte[] paletteData = new byte[0x100];
                s.Read(paletteData, 0, paletteData.Length);
                string fileName = GetPaletteFilename(palette.Value);
                Directory.CreateDirectory(Path.GetDirectoryName(fileName));
                File.WriteAllBytes(fileName, paletteData);
            }
        }

        /// <summary>Loads an existing project from the given directory.</summary>
        /// <param name="path">The directory from which to load the project files.</param>
        public Project(string path) {
            this.path = path;
        }

        /// <summary>Builds the project from the specified base ROM into the specified output ROM</summary>
        /// <param name="baseROM">The base ROM</param>
        /// <param name="outputROM">The output ROM</param>
        public void Build(string baseROM, string outputROM) {
            File.Copy(baseROM, outputROM, true);
            NStream s = new NStream(new FileStream(outputROM, FileMode.Open, FileAccess.ReadWrite, FileShare.Read));
            ROMInfo info = new ROMInfo(this, s);

            int maxLevelNumber = 0;
            foreach (string filepath in Directory.GetFiles(GetLevelFolderPath())) {
                string filename = Path.GetFileNameWithoutExtension(filepath);
                int num;
                if (int.TryParse(filename, out num) && num > maxLevelNumber) {
                    maxLevelNumber = num;
                }
            }
            info.Freespace.Reserve(ROMPointers.LevelPointers + 2, (maxLevelNumber + 1) * 4);

            info.Freespace.Sort();
            Console.Out.WriteLine(info.Freespace.ToString());

            // TODO: Check for errors reading the files
            foreach (string filename in Directory.GetFiles(GetTilesetFolderPath())) {
                if (filename.EndsWith(PaletteExtension)) {
                    string name = Path.GetFileNameWithoutExtension(filename);
                    info.AddPalette(name, File.ReadAllBytes(filename), s);
                }
            }
            foreach (string tileset in Directory.GetDirectories(GetTilesetFolderPath())) {
                string name = Path.GetFileName(tileset);

                // TODO: Check size of the files
                string tilemapFilename = Path.Combine(tileset, TilesetTilemapFilename);
                if (File.Exists(tilemapFilename)) {
                    info.AddTilesetTilemap(name, ZAMNCompress.Compress(File.ReadAllBytes(tilemapFilename)), s);
                }

                string collisionFilename = Path.Combine(tileset, TilesetCollisionFilename);
                if (File.Exists(collisionFilename)) {
                    info.AddTilesetCollision(name, File.ReadAllBytes(collisionFilename), s);
                }

                string graphicsFilename = Path.Combine(tileset, TilesetGraphicsFilename);
                if (File.Exists(graphicsFilename)) {
                    info.AddTilesetGraphics(name, File.ReadAllBytes(graphicsFilename), s);
                }

                foreach (string filename in Directory.GetFiles(tileset)) {
                    if (filename.EndsWith(PaletteExtension)) {
                        string paletteName = name + "/" + Path.GetFileNameWithoutExtension(filename);
                        info.AddPalette(paletteName, File.ReadAllBytes(filename), s);
                    }
                }
            }

            s.Seek(ROMPointers.LevelPointers + 2, SeekOrigin.Begin);
            for (int levelNum = 0; levelNum <= maxLevelNumber; levelNum++) {
                string filename = GetLevelFilename(levelNum);
                // TODO: Check if file exists
                Level level = JsonConvert.DeserializeObject<Level>(File.ReadAllText(filename), new LevelJsonConverter());
                MovableData levelData = level.Build(info);
                int pointer = info.Freespace.Claim(levelData.GetSize());
                byte[] levelDataArray = levelData.Build(pointer);
                s.WritePointer(pointer);
                s.PushPosition();
                s.Seek(pointer, SeekOrigin.Begin);
                s.Write(levelDataArray, 0, levelDataArray.Length);
                s.PopPosition();
                Console.Out.WriteLine(string.Format("Inserting level {0} at {1:X}, {2:X} bytes", levelNum, pointer, levelDataArray.Length));
            }

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

        public string GetLevelFolderPath() {
            return Path.Combine(path, "Levels");
        }

        public string GetLevelFilename(int num) {
            return Path.Combine(GetLevelFolderPath(), num.ToString() + ".json");
        }

        public string GetTilesetFolderPath() {
            return Path.Combine(path, "Tilesets");
        }

        public string GetTilesetTilemapFilename(string name) {
            return Path.Combine(GetTilesetFolderPath(), name, TilesetTilemapFilename);
        }

        public string GetTilesetCollisionFilename(string name) {
            return Path.Combine(GetTilesetFolderPath(), name, TilesetCollisionFilename);
        }

        public string GetTilesetGraphicsFilename(string name) {
            return Path.Combine(GetTilesetFolderPath(), name, TilesetGraphicsFilename);
        }

        public string GetPaletteFilename(string name) {
            return Path.Combine(GetTilesetFolderPath(), name.Replace('/', Path.DirectorySeparatorChar) + PaletteExtension);
        }
    }
}
