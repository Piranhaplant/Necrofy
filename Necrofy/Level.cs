using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Newtonsoft.Json;

namespace Necrofy
{
    /// <summary>Represents a ZAMN level</summary>
    class Level
    {
        // The current version of the level format
        private const int curVersion = 1;
        /// <summary>The version of the level format used for loading/saving.</summary>
        public int version { get; set; }
        /// <summary>The loaded tileset used for this level.</summary>
        [JsonIgnore]
        public Tileset tileset;
        /// <summary>The graphics loaded for items, victims, monsters, etc.</summary>
        [JsonIgnore]
        public SpriteGFX sprites;
        /// <summary>The background tiles making up the level</summary>
        public ushort[,] background { get; set; }
        public string tilesetTilemapName { get; set; }
        public string tilesetCollisionName { get; set; }
        public string tilesetGraphicsName { get; set; }
        public string paletteName { get; set; }
        public string spritePaletteName { get; set; }
        /// <summary>The palette animation used for the level. Since I don't know how this works yet, it is stored as a pointer.</summary>
        public int paletteAnimationPtr { get; set; }
        /// <summary>The number of the background track that plays during the level</summary>
        public ushort music { get; set; }
        /// <summary>The number corresponding to which extra sounds are loaded in the level</summary>
        public ushort sounds { get; set; }
        /// <summary>Some unknown level setting</summary>
        public ushort unknown1 { get; set; }
        /// <summary>Some unknown level setting</summary>
        public ushort unknown2 { get; set; }
        public ushort p1startX { get; set; }
        public ushort p1startY { get; set; }
        public ushort p2startX { get; set; }
        public ushort p2startY { get; set; }

        public List<Monster> monsters { get; set; }
        public List<OneTimeMonster> oneTimeMonsters { get; set; }
        public List<Item> items { get; set; }
        public List<ushort> bonuses { get; set; }
        public List<LevelMonster> levelMonsters { get; set; }

        public TitlePage title1 { get; set; }
        public TitlePage title2 { get; set; }

        public Level() { }

        /// <summary>Loads a level from a ROM file</summary>
        /// <param name="r">The ROMInfo that freespace, etc. will be saved to</param>
        /// <param name="s">A stream for the ROM file that is positioned at the beginning of the level</param>
        public Level(ROMInfo r, NStream s) {
            version = curVersion;
            monsters = new List<Monster>();
            oneTimeMonsters = new List<OneTimeMonster>();
            items = new List<Item>();
            bonuses = new List<ushort>();
            levelMonsters = new List<LevelMonster>();

            s.StartBlock(); // Track the freespace used by the level
            tilesetTilemapName = r.GetTilesetTilemapName(s.ReadPointer());
            int backgroundPtr = s.ReadPointer();
            tilesetCollisionName = r.GetTilesetCollisionName(s.ReadPointer());
            tilesetGraphicsName = r.GetTilesetGraphicsName(s.ReadPointer());
            paletteName = r.GetPaletteName(s.ReadPointer());
            spritePaletteName = r.GetPaletteName(s.ReadPointer());
            paletteAnimationPtr = s.ReadPointer();

            AddAllObjects(s, () => monsters.Add(new Monster(s)));
            AddAllObjects(s, () => oneTimeMonsters.Add(new OneTimeMonster(s)));
            AddAllObjects(s, () => items.Add(new Item(s)));

            int width = s.ReadInt16();
            int height = s.ReadInt16();
            unknown1 = s.ReadInt16();
            unknown2 = s.ReadInt16();
            p1startX = s.ReadInt16();
            p1startY = s.ReadInt16();
            p2startX = s.ReadInt16();
            p2startY = s.ReadInt16();
            music = s.ReadInt16();
            sounds = s.ReadInt16();

            s.GoToRelativePointerPush();
            title1 = new TitlePage(s);
            s.PopPosition();
            s.GoToRelativePointerPush();
            title2 = new TitlePage(s);
            s.PopPosition();

            // Bonus pointer is optional
            if (s.PeekInt16() > 0) {
                AddAllObjects(s, () => bonuses.Add(s.ReadInt16()));
            }

            while (s.PeekPointer() > -1) {
                levelMonsters.Add(LevelMonster.FromROM(r, s));
            }
            // Stop tracking the freespace since the background data is separate from the rest
            s.EndBlock(r.Freespace);

            background = new ushort[width, height];
            s.Seek(backgroundPtr, SeekOrigin.Begin);
            for (int y = 0; y < height; y++) {
                for (int x = 0; x < width; x++) {
                    background[x, y] = s.ReadInt16();
                }
            }
            r.Freespace.AddSize(backgroundPtr, width * height * 2);
        }

        private void AddAllObjects(NStream s, Action add) {
            s.GoToRelativePointerPush();
            while (s.PeekInt16() > 0) {
                add();
            }
            s.PopPosition();
        }
    }
}
