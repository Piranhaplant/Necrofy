﻿using System;
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
        /// <summary>The background tiles making up the level</summary>
        public ushort[,] background { get; set; }
        public int width { get { return background.GetLength(0); } }
        public int height { get { return background.GetLength(1); } }

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
        /// <summary>Tiles numbered strictly less than this value will be displayed on top of sprites</summary>
        public ushort tilePriorityEnd { get; set; }
        /// <summary>Tiles numbered strictly greater than this value will not be displayed</summary>
        public ushort hiddenTilesStart { get; set; }
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
            monsters = new List<Monster>();
            oneTimeMonsters = new List<OneTimeMonster>();
            items = new List<Item>();
            bonuses = new List<ushort>();
            levelMonsters = new List<LevelMonster>();

            s.StartBlock(); // Track the freespace used by the level
            tilesetTilemapName = TilesetTilemapAsset.GetAssetName(s, r, s.ReadPointer());
            int backgroundPtr = s.ReadPointer();
            tilesetCollisionName = TilesetCollisionAsset.GetAssetName(s, r, s.ReadPointer());
            tilesetGraphicsName = TilesetGraphicsAsset.GetAssetName(s, r, s.ReadPointer());
            paletteName = TilesetPaletteAsset.GetAssetName(s, r, s.ReadPointer());
            spritePaletteName = PaletteAsset.GetAssetName(s, r, s.ReadPointer());
            paletteAnimationPtr = s.ReadPointer();

            AddAllObjects(s, () => {
                Monster m = new Monster(s);
                // In level 29, there are some invalid monsters, so remove them now
                if (m.type > 0) {
                    monsters.Add(m);
                }
            });
            AddAllObjects(s, () => oneTimeMonsters.Add(new OneTimeMonster(s)));
            AddAllObjects(s, () => items.Add(new Item(s)));

            int width = s.ReadInt16();
            int height = s.ReadInt16();
            tilePriorityEnd = s.ReadInt16();
            hiddenTilesStart = s.ReadInt16();
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

        private static void AddAllObjects(NStream s, Action add) {
            s.GoToRelativePointerPush();
            while (s.PeekInt16() > 0) {
                add();
            }
            s.PopPosition();
        }

        /// <summary>Builds the level for inserting into a ROM.</summary>
        /// <returns>The level data</returns>
        public MovableData Build(ROMInfo rom) {
            MovableData data = new MovableData();

            data.data.AddPointer(rom.GetAssetPointer(AssetCategory.Tilemap, tilesetTilemapName));

            MovableData backgroundData = new MovableData();
            int width = background.GetLength(0);
            int height = background.GetLength(1);
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
            BuildAll(oneTimeMonsters, data, (o, d) => o.Build(d));
            BuildAll(items, data, (o, d) => o.Build(d));

            data.data.AddInt16((ushort)background.GetLength(0));
            data.data.AddInt16((ushort)background.GetLength(1));
            data.data.AddInt16(tilePriorityEnd);
            data.data.AddInt16(hiddenTilesStart);
            data.data.AddInt16(p1startX);
            data.data.AddInt16(p1startY);
            data.data.AddInt16(p2startX);
            data.data.AddInt16(p2startY);
            data.data.AddInt16(music);
            data.data.AddInt16(sounds);

            MovableData title1Data = new MovableData();
            title1.Build(title1Data, 0);
            data.AddPointer(MovableData.PointerSize.TwoBytes, title1Data);

            MovableData title2Data = new MovableData();
            title2.Build(title2Data, 1);
            data.AddPointer(MovableData.PointerSize.TwoBytes, title2Data);

            BuildAll(bonuses, data, (o, d) => d.data.AddInt16(o));

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
