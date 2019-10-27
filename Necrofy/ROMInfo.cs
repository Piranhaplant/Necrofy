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
        public Freespace Freespace;

        /// <summary>Loads the ROMInfo data from an already opened stream.</summary>
        /// <param name="s">A stream to a ROM file</param>
        public ROMInfo(NStream s) {
            Freespace = new Freespace((int)s.Length);

            // First get a list of all the level pointers
            s.Seek(ROMPointers.LevelPointers, SeekOrigin.Begin);
            int levelCount = s.ReadInt16();
            s.Seek(ROMPointers.BonusLevelNums, SeekOrigin.Begin);
            int maxBonusLevel = levelCount;
            // Assume that there aren't any skipped level numbers
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

            // Don't try to get default assets for ROMs built with Necrofy, since everything is moved around
            // TODO: Some stuff doesn't move around and those still need to be extracted
            if (!fullLevelPointers) {
                Asset.AddAllDefaults(s, this);
            }

            Freespace.AddSize(ROMPointers.LevelPointers + 2, (maxBonusLevel + 1) * (fullLevelPointers ? 4 : 2));

            // Load all levels. Even if they aren't going to be used, this is still necessary to track the space used by them.
            for (int i = 0; i <= maxBonusLevel; i++) {
                if (fullLevelPointers)
                    s.GoToPointerPush();
                else
                    s.GoToRelativePointerPush();
                Level level = new Level(this, s);
                assets.Add(new LevelAsset(i, level));
                s.PopPosition();
            }
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
