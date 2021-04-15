using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Newtonsoft.Json;

namespace Necrofy
{
    /// <summary>Stores a ZAMN level</summary>
    class Level
    {
        // This needs to be first because this field is read to show the level name in the project browser
        // Keeping it first ensures that it is read quickly. See LevelAsset
        /// <summary>Human readable name of the level. For use in Necrofy only</summary>
        public string displayName { get; set; }
        /// <summary>The background tiles making up the level</summary>
        public ushort[,] background { get; set; }
        [JsonIgnore]
        public int width => background.GetWidth();
        [JsonIgnore]
        public int height => background.GetHeight();

        public string tilesetTilemapName { get; set; }
        public string tilesetCollisionName { get; set; }
        public string tilesetGraphicsName { get; set; }
        public string paletteName { get; set; }
        public string spritePaletteName { get; set; }
        /// <summary>The palette animation used for the level</summary>
        public int paletteAnimationPtr { get; set; }
        /// <summary>The number of the background track that plays during the level</summary>
        public ushort music { get; set; }
        /// <summary>The number corresponding to which extra sounds are loaded in the level</summary>
        public ushort sounds { get; set; }
        /// <summary>Tiles numbered strictly less than this value will be displayed on top of sprites</summary>
        public ushort priorityTileCount { get; set; }
        /// <summary>Tiles numbered strictly greater than this value will not be displayed</summary>
        public ushort visibleTilesEnd { get; set; }
        public ushort p1startX { get; set; }
        public ushort p1startY { get; set; }
        public ushort p2startX { get; set; }
        public ushort p2startY { get; set; }

        public List<Monster> monsters { get; set; }
        public List<OneShotMonster> oneShotMonsters { get; set; }
        public List<Item> items { get; set; }
        public List<ushort> bonuses { get; set; }
        public List<LevelMonster> levelMonsters { get; set; }

        public TitlePage title1 { get; set; }
        public TitlePage title2 { get; set; }

        public ushort secretBonusCodePointer { get; set; }
        public ushort bonusLevelNumber { get; set; }
        
        public Level() { }

        /// <summary>Loads a level from a ROM file that has not been modified by Necrofy</summary>
        /// <param name="r">The ROMInfo that freespace, etc. will be saved to</param>
        /// <param name="s">A stream for the ROM file that is positioned at the beginning of the level</param>
        /// <param name="secretBonusCodePointer">The secret bonus code pointer</param>
        /// <param name="bonusLevelNumber">The bonus level number</param>
        public Level(ROMInfo r, NStream s, ushort secretBonusCodePointer, ushort bonusLevelNumber) : this(r, s, false) {
            this.secretBonusCodePointer = secretBonusCodePointer;
            this.bonusLevelNumber = bonusLevelNumber;
        }

        /// <summary>Loads a level from a ROM file that has been modified by Necrofy</summary>
        /// <param name="r">The ROMInfo that freespace, etc. will be saved to</param>
        /// <param name="s">A stream for the ROM file that is positioned at the beginning of the level</param>
        public Level(ROMInfo r, NStream s) : this(r, s, true) { }

        private Level(ROMInfo r, NStream s, bool necrofy) {
            monsters = new List<Monster>();
            oneShotMonsters = new List<OneShotMonster>();
            items = new List<Item>();
            bonuses = new List<ushort>();
            levelMonsters = new List<LevelMonster>();

            s.StartBlock(); // Track the freespace used by the level

            s.TogglePauseBlock(); // Don't want to track space used if a new asset needs to be extracted from the ROM
            tilesetTilemapName = TilemapAsset.GetAssetName(s, r, s.ReadPointer());
            string tileset = new Asset.ParsedName(tilesetTilemapName).Tileset;
            int backgroundPtr = s.ReadPointer();
            tilesetCollisionName = CollisionAsset.GetAssetName(s, r, s.ReadPointer(), tileset);
            tilesetGraphicsName = GraphicsAsset.GetAssetName(s, r, s.ReadPointer(), tileset);
            paletteName = PaletteAsset.GetAssetName(s, r, s.ReadPointer(), tileset);
            spritePaletteName = PaletteAsset.GetAssetName(s, r, s.ReadPointer(), Asset.SpritesFolder);
            paletteAnimationPtr = s.ReadPointer();
            s.TogglePauseBlock();

            AddAllObjects(s, () => {
                Monster m = new Monster(s);
                // In level 29, there are some invalid monsters from the monster data running into the victim data, so remove them
                if (m.type > -1) {
                    monsters.Add(m);
                }
            }, stream => stream.PeekByte());
            AddAllObjects(s, () => oneShotMonsters.Add(new OneShotMonster(s)));
            AddAllObjects(s, () => items.Add(new Item(s)));

            int width = s.ReadInt16();
            int height = s.ReadInt16();
            priorityTileCount = s.ReadInt16();
            visibleTilesEnd = s.ReadInt16();
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
            displayName = GenerateDisplayName();

            // Bonus pointer is optional
            if (s.PeekInt16() > 0) {
                AddAllObjects(s, () => bonuses.Add(s.ReadInt16()));
            } else {
                s.ReadInt16();
            }

            if (necrofy) {
                secretBonusCodePointer = s.ReadInt16();
                bonusLevelNumber = s.ReadInt16();
            }

            while (s.PeekPointer() > -1) {
                levelMonsters.Add(LevelMonster.FromROM(r, s, tileset));
            }
            // Stop tracking the freespace since the background data is separate from the rest
            s.EndBlock(r.Freespace);

            background = new ushort[width, height];
            s.Seek(backgroundPtr);
            for (int y = 0; y < height; y++) {
                for (int x = 0; x < width; x++) {
                    background[x, y] = s.ReadInt16();
                }
            }
            r.Freespace.AddSize(backgroundPtr, width * height * 2);
        }

        private static void AddAllObjects(NStream s, Action add) {
            AddAllObjects(s, add, stream => stream.PeekInt16());
        }

        private static void AddAllObjects(NStream s, Action add, Func<Stream, int> peekValue) {
            s.GoToRelativePointerPush();
            while (peekValue(s) > 0) {
                add();
            }
            s.PopPosition();
        }

        public string GenerateDisplayName() {
            string name = title1.ToString() + " " + title2.ToString();
            int actualNameStart = name.LastIndexOf("Level ");
            if (actualNameStart < 0) {
                return name;
            }
            actualNameStart += 6; // Go past the string "Level" and one space
            if (actualNameStart >= name.Length) {
                return name;
            }
            if (char.IsDigit(name, actualNameStart)) {
                actualNameStart = name.IndexOf(" ", actualNameStart) + 1;
                if (actualNameStart <= 0) {
                    return "";
                }
            }
            return name.Substring(actualNameStart);
        }

        /// <summary>Builds the level for inserting into a ROM.</summary>
        /// <returns>The level data</returns>
        public MovableData Build(ROMInfo rom) {
            MovableData data = new MovableData();

            data.data.AddPointer(rom.GetAssetPointer(AssetCategory.Tilemap, tilesetTilemapName));

            MovableData backgroundData = new MovableData();
            int width = this.width;
            int height = this.height;
            for (int y = 0; y < height; y++) {
                for (int x = 0; x < width; x++) {
                    backgroundData.data.AddInt16(background[x, y]);
                }
            }
            data.AddPointer(MovableData.PointerSize.FourBytes, backgroundData);

            data.data.AddPointer(rom.GetAssetPointer(AssetCategory.Collision, tilesetCollisionName));
            data.data.AddPointer(rom.GetAssetPointer(AssetCategory.Graphics, tilesetGraphicsName));
            data.data.AddPointer(rom.GetAssetPointer(AssetCategory.Palette, paletteName));
            data.data.AddPointer(rom.GetAssetPointer(AssetCategory.Palette, spritePaletteName));
            if (paletteAnimationPtr > 0) {
                data.data.AddPointer(paletteAnimationPtr);
            } else {
                data.data.AddInt16(0);
                data.data.AddInt16(0);
            }

            BuildAll(monsters, data, (o, d) => o.Build(d));
            BuildAll(oneShotMonsters, data, (o, d) => o.Build(d));
            BuildAll(items, data, (o, d) => o.Build(d));

            data.data.AddInt16((ushort)width);
            data.data.AddInt16((ushort)height);
            data.data.AddInt16(priorityTileCount);
            data.data.AddInt16(visibleTilesEnd);
            data.data.AddInt16(p1startX);
            data.data.AddInt16(p1startY);
            data.data.AddInt16(p2startX);
            data.data.AddInt16(p2startY);
            data.data.AddInt16(music);
            data.data.AddInt16(sounds);

            data.AddPointer(MovableData.PointerSize.TwoBytes, title1.Build(0));
            data.AddPointer(MovableData.PointerSize.TwoBytes, title2.Build(1));

            if (bonuses.Count > 0) {
                BuildAll(bonuses, data, (o, d) => d.data.AddInt16(o));
            } else {
                data.data.AddInt16(0);
            }

            data.data.AddInt16(secretBonusCodePointer);
            data.data.AddInt16(bonusLevelNumber);

            foreach (LevelMonster levelMonster in levelMonsters) {
                levelMonster.Build(data, rom);
            }
            data.data.AddInt16(0);
            data.data.AddInt16(0);

            return data;
        }

        private static void BuildAll<T>(List<T> objects, MovableData data, Action<T, MovableData> build) {
            MovableData objectData = new MovableData();
            foreach (T obj in objects) {
                build(obj, objectData);
            }
            objectData.data.AddInt16(0);
            data.AddPointer(MovableData.PointerSize.TwoBytes, objectData);
        }
    }
}
