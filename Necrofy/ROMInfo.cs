using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Necrofy
{
    /// <summary>Holds information about a ROM that will be used to create a new project and compile a project</summary>
    class ROMInfo
    {
        /// <summary>A list of assets for the ROM</summary>
        public List<Asset> assets = new List<Asset>();

        public Dictionary<AssetCategory, Dictionary<int, string>> assetNames = new Dictionary<AssetCategory, Dictionary<int, string>>();
        public Dictionary<AssetCategory, Dictionary<string, int>> assetPointers = new Dictionary<AssetCategory, Dictionary<string, int>>();

        /// <summary>The freespace that was found in the ROM</summary>
        public readonly Freespace Freespace;
        /// <summary>Indicates whether or not the ROM was built with Necrofy</summary>
        public bool NecrofyROM { get; private set; }
        /// <summary>The level after which the winner screen will be shown</summary>
        public int WinLevel { get; private set; }
        /// <summary>The level after which the game will end and the player will be sent back to the main menu loop</summary>
        public int EndGameLevel { get; private set; }
        /// <summary>Definitions that will be available to patches</summary>
        public readonly Dictionary<string, string> exportedDefines = new Dictionary<string, string>();

        /// <summary>Loads the ROMInfo data from an already opened stream.</summary>
        /// <param name="s">A stream to a ROM file</param>
        public ROMInfo(NStream s) {
            Freespace = new Freespace((int)s.Length);

            s.Seek(ROMPointers.WinLevel);
            WinLevel = s.ReadInt16();
            
            s.Seek(ROMPointers.LevelPointers);
            int levelCount = s.ReadInt16();
            EndGameLevel = levelCount - 1;
            NecrofyROM = s.PeekPointer() > 0;
            
            Asset.AddAllDefaults(s, this);
            
            for (int i = 0; i <= levelCount; i++) {
                Level level;
                if (NecrofyROM) {
                    s.GoToPointerPush();
                    level = new Level(this, s);
                } else {
                    s.PushPosition();
                    s.Seek(ROMPointers.SecretBonusCodePointers + i * 2);
                    ushort secretBonusCodePointer = s.ReadInt16();
                    s.Seek(ROMPointers.BonusLevelNums + i * 2);
                    ushort bonusLevelNum = s.ReadInt16();
                    s.PopPosition();

                    s.GoToRelativePointerPush();
                    level = new Level(this, s, secretBonusCodePointer, bonusLevelNum);
                }
                assets.Add(new LevelAsset(i, level));
                s.PopPosition();

                if (level.bonusLevelNumber > levelCount && level.bonusLevelNumber - levelCount <= 0x100) { // filter out likely invalid entries
                    levelCount = Math.Max(levelCount, level.bonusLevelNumber);
                }
            }

            Freespace.AddSize(ROMPointers.LevelPointers + 2, (levelCount + 1) * (NecrofyROM ? 4 : 2));
        }

        public void AddAssetName(AssetCategory category, int pointer, string name) {
            if (!assetNames.ContainsKey(category)) {
                assetNames.Add(category, new Dictionary<int, string>());
            }
            assetNames[category].Add(pointer, name);
        }

        public void AddAssetPointer(AssetCategory category, string name, int pointer) {
            if (!assetPointers.ContainsKey(category)) {
                assetPointers.Add(category, new Dictionary<string, int>());
            }
            assetPointers[category].Add(name, pointer);
        }

        public string GetAssetName(AssetCategory category, int pointer) {
            if (!assetNames.ContainsKey(category)) {
                return null;
            }
            if (!assetNames[category].ContainsKey(pointer)) {
                return null;
            }
            return assetNames[category][pointer];
        }

        public int GetAssetPointer(AssetCategory category, string name) {
            if (!assetPointers.ContainsKey(category)) {
                throw new Exception("No asset found for category " + category + " with name " + name);
            }
            if (!assetPointers[category].ContainsKey(name)) {
                throw new Exception("No asset found for category " + category + " with name " + name);
            }
            return assetPointers[category][name];
        }
    }
}
