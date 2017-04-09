﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Necrofy
{
    /// <summary>Holds information about a ROM that will be used to create a new project and compile a project</summary>
    class ROMInfo
    {
        private Project project;
        // The following dicionaries map different parts of the tilesets from their address in the default ZAMN ROM to their names
        private static Dictionary<int, string> DefaultTSTilemapNames = new Dictionary<int, string>() { { 0xd4000, "Castle" }, { 0xd8000, "Grass" }, { 0xdbcb5, "Desert" }, { 0xe0000, "Office" }, { 0xe36ef, "Mall" } };
        private static Dictionary<int, string> DefaultTSCollisionNames = new Dictionary<int, string>() { { 0xe6aab, "Castle" }, { 0xdf4d1, "Grass" }, { 0xdf8d1, "Desert" }, { 0xe72ab, "Office" }, { 0xe6eab, "Mall" } };
        private static Dictionary<int, string> DefaultTSGraphicsNames = new Dictionary<int, string>() { { 0xc8000, "Castle" }, { 0xc0000, "Grass" }, { 0xc4000, "Desert" }, { 0xd0000, "Office" }, { 0xcc000, "Mall" } };
        // Maps the address of the tileset palettes' in the default ZAMN ROM to their names
        private static Dictionary<int, string> DefaultPaletteNames =
            new Dictionary<int, string>() { { 0xf0e76, "Grass/Normal" }, { 0xf1076, "Grass/Autumn" }, { 0xf1176, "Grass/Winter" }, { 0xf1276, "Grass/Night" },
                                            { 0xf1476, "Desert/Pyramid" }, { 0xf1576, "Desert/Beach" }, { 0xf1676, "Desert/DarkBeach" }, { 0xf1776, "Desert/Cave" },
                                            { 0xf1876, "Castle/Normal" }, { 0xf1976, "Castle/Night" }, { 0xf1a76, "Castle/Bright" }, { 0xf1b76, "Castle/Dark" },
                                            { 0xf1c76, "Mall/Normal" }, { 0xf1d76, "Mall/Alternate" },
                                            { 0xf1e76, "Office/Normal" }, { 0xf1f76, "Office/DarkFireCave" }, { 0xf2076, "Office/Light" }, { 0xf2176, "Office/Dark" }, { 0xf2276, "Office/FireCave" },
                                            { 0xf0f76, "Sprites" } };

        /// <summary>A list of levels loaded from the ROM</summary>
        public List<Level> Levels = new List<Level>();

        public Dictionary<int, string> TilesetTilemapNames = new Dictionary<int, string>();
        public Dictionary<int, string> TilesetCollisionNames = new Dictionary<int, string>();
        public Dictionary<int, string> TilesetGraphicsNames = new Dictionary<int, string>();
        public Dictionary<int, string> PaletteNames = new Dictionary<int, string>();

        private Dictionary<string, int> TilesetTilemapPointers = new Dictionary<string, int>();
        private Dictionary<string, int> TilesetCollisionPointers = new Dictionary<string, int>();
        private Dictionary<string, int> TilesetGraphicsPointers = new Dictionary<string, int>();
        private Dictionary<string, int> PalettePointers = new Dictionary<string, int>();

        /// <summary>The freespace that was found in the ROM</summary>
        public Freespace Freespace;

        /// <summary>Loads the ROMInfo data from an already opened stream.</summary>
        /// <param name="project">The project</param>
        /// <param name="s">A stream to a ROM file</param>
        public ROMInfo(Project project, NStream s) {
            this.project = project;
            Freespace = new Freespace((int)s.Length);

            // First get a list of all the level pointers
            s.Seek(ROMPointers.LevelPointers, SeekOrigin.Begin);
            int levelCount = s.ReadInt16();
            s.Seek(ROMPointers.BonusLevelNums, SeekOrigin.Begin);
            int maxBonusLevel = levelCount;
            // I am going to assume that there aren't any skipped level numbers
            for (int i = 0; i <= levelCount; i++) {
                int bonusLevelNum = s.ReadInt16();
                if (bonusLevelNum > maxBonusLevel)
                    maxBonusLevel = bonusLevelNum;
            }
            s.Seek(ROMPointers.LevelPointers + 2, SeekOrigin.Begin); // Skip past the first 2 bytes which indicate how many levels there are

            // If the fourth byte is zero, then the ROM is using 4-byte level pointers
            s.Seek(3, SeekOrigin.Current);
            bool fullLevelPointers = s.ReadByte() == 0;
            s.Seek(-4, SeekOrigin.Current);

            Freespace.AddSize(ROMPointers.LevelPointers + 2, (maxBonusLevel + 1) * (fullLevelPointers ? 4 : 2));

            // Load all levels. Even if they aren't going to be used, this is still necessary to track the space used by them.
            for (int i = 0; i <= maxBonusLevel; i++) {
                if (fullLevelPointers)
                    s.GoToPointerPush();
                else
                    s.GoToRelativePointerPush();
                Levels.Add(new Level(this, s));
                s.PopPosition();
            }

            // Add the freespace for everything from the levels
            foreach (int ptr in TilesetTilemapNames.Keys) {
                // They're compressed, so the first two bytes of the data determines the size
                s.Seek(ptr, SeekOrigin.Begin);
                Freespace.AddSize(ptr, s.ReadInt16() + 2); // +2 for the size bytes themselves
            }
            foreach (int ptr in TilesetCollisionNames.Keys) {
                Freespace.AddSize(ptr, 0x400);
            }
            foreach (int ptr in TilesetGraphicsNames.Keys) {
                Freespace.AddSize(ptr, 0x4000);
            }
            foreach (int ptr in PaletteNames.Keys) {
                Freespace.AddSize(ptr, 0x100);
            }
        }

        // These functions are used to convert internal pointers to human readable names that will be used in a project.
        // Default values are used when they exist, but otherwise they are just numbered.
        private string GetName(int pointer, Dictionary<int, string> nameDict, Dictionary<int, string> defaultsDict, string defaultName) {
            if (!nameDict.ContainsKey(pointer)) {
                if (defaultsDict.ContainsKey(pointer))
                    nameDict.Add(pointer, defaultsDict[pointer]);
                else
                    nameDict.Add(pointer, defaultName + nameDict.Count.ToString());
            }
            return nameDict[pointer];
        }

        private const string UnknownName = "unknown";

        // TODO: Remove UnknownName from all of these
        public string GetTilesetTilemapName(int pointer) {
            return GetName(pointer, TilesetTilemapNames, DefaultTSTilemapNames, UnknownName);
        }

        public string GetTilesetCollisionName(int pointer) {
            return GetName(pointer, TilesetCollisionNames, DefaultTSCollisionNames, UnknownName);
        }

        public string GetTilesetGraphicsName(int pointer) {
            return GetName(pointer, TilesetGraphicsNames, DefaultTSGraphicsNames, UnknownName);
        }

        public string GetPaletteName(int pointer) {
            return GetName(pointer, PaletteNames, DefaultPaletteNames, UnknownName);
        }

        private int GetPointer(Dictionary<string, int> dict, string name) {
            if (dict.ContainsKey(name)) {
                return dict[name];
            }
            throw new Exception("Asset not found: " + name);
        }

        public int GetTilesetTilemapPointer(string name) {
            return GetPointer(TilesetTilemapPointers, name);
        }

        public int GetTilesetCollisionPointer(string name) {
            return GetPointer(TilesetCollisionPointers, name);
        }

        public int GetTilesetGraphicsPointer(string name) {
            return GetPointer(TilesetGraphicsPointers, name);
        }

        public int GetPalettePointer(string name) {
            return GetPointer(PalettePointers, name);
        }

        private void AddPointer(Dictionary<string, int> dict, string name, byte[] data, NStream s) {
            int pointer = Freespace.Claim(data.Length);
            dict.Add(name, pointer);
            s.Seek(pointer, SeekOrigin.Begin);
            s.Write(data, 0, data.Length);
            Console.Out.WriteLine(string.Format("Inserted {0:X} bytes at {1:X}", data.Length, pointer));
        }

        public void AddTilesetTilemap(string name, byte[] data, NStream s) {
            AddPointer(TilesetTilemapPointers, name, data, s);
        }

        public void AddTilesetCollision(string name, byte[] data, NStream s) {
            AddPointer(TilesetCollisionPointers, name, data, s);
        }

        public void AddTilesetGraphics(string name, byte[] data, NStream s) {
            AddPointer(TilesetGraphicsPointers, name, data, s);
        }

        public void AddPalette(string name, byte[] data, NStream s) {
            AddPointer(PalettePointers, name, data, s);
        }
    }
}
