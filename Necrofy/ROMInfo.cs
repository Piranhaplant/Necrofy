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
        /// <summary>Various per-asset settings</summary>
        public readonly AssetOptions assetOptions = new AssetOptions();

        private const string PropertiesHeader = "NFY";
        private const byte PropertiesVersion = 2;
        /// <summary>The total number of bytes that will be used by custom sprite graphics. This is cleared once the space for them has been claimed.</summary>
        public int ExtraSpriteGraphicsSize { get; set; }
        /// <summary>A pointer to the start of the custom sprite graphics</summary>
        public int ExtraSpriteGraphicsBasePointer { get; set; }
        /// <summary>The current pointer that custom sprite graphics will be inserted at.</summary>
        public int ExtraSpriteGraphicsCurrentPointer { get; set; }
        /// <summary>The starting index for custom sprite tiles</summary>
        public int ExtraSpriteGraphicsStartIndex { get; set; }

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

            if (NecrofyROM) {
                ReadProperties(s);
            }
            
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

        /// <summary>Creates a ROMInfo for upgrading a project from the specified version</summary>
        /// <param name="s">A stream to the base ROM file</param>
        /// <param name="originalProjectVersion">The project version to upgrade from</param>
        public ROMInfo(NStream s, Version originalProjectVersion) {
            Freespace = new Freespace((int)s.Length);

            s.Seek(ROMPointers.LevelPointers);
            s.ReadInt16(); // Skip past level count
            NecrofyROM = s.PeekPointer() > 0;

            Asset.AddAllDefaults(s, this, originalProjectVersion);
        }

        /// <summary>Creates an empty ROMInfo to use for extracting single assets</summary>
        public ROMInfo() {
            Freespace = new Freespace(0x100000);
        }

        private void ReadProperties(NStream s) {
            s.PushPosition();
            s.Seek(ROMPointers.SecretBonusCodePointers);

            if (Encoding.ASCII.GetString(s.ReadBytes(PropertiesHeader.Length)) == PropertiesHeader) {
                int version = s.ReadByte();
                if (version == 2) {
                    ExtraSpriteGraphicsBasePointer = s.ReadPointer();
                    ExtraSpriteGraphicsCurrentPointer = s.ReadPointer();
                }
            }

            s.PopPosition();
        }

        private void WriteProperties(NStream s) {
            s.Seek(ROMPointers.SecretBonusCodePointers);
            s.Write(Encoding.ASCII.GetBytes(PropertiesHeader));
            s.WriteByte(PropertiesVersion);
            s.WritePointer(ExtraSpriteGraphicsBasePointer);
            s.WritePointer(ExtraSpriteGraphicsCurrentPointer);
            if (s.Position > 0x1525E) {
                throw new Exception("Properties overflowed available space");
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
            exportedDefines.Add("ASSET_" + category.ToString() + "_" + FixDefineName(name), ROMPointers.PointerToHexString(pointer));
        }

        private static string FixDefineName(string s) {
            StringBuilder builder = new StringBuilder(s);
            for (int i = 0; i < builder.Length; i++) {
                char c = builder[i];
                if (!((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || (c >= '0' && c <= '9'))) {
                    builder[i] = '_';
                }
            }
            return builder.ToString();
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

        public void WriteToBuild(NStream s, ProjectSettings settings, BuildResults results) {
            WriteProperties(s);

            if (settings.ExtraSpriteGraphicsBasePointer != null) {
                ExtraSpriteGraphicsBasePointer = (int)settings.ExtraSpriteGraphicsBasePointer;
            }
            if (settings.ExtraSpriteGraphicsCurrentPointer != null) {
                ExtraSpriteGraphicsCurrentPointer = (int)settings.ExtraSpriteGraphicsCurrentPointer;
            }

            if (ExtraSpriteGraphicsStartIndex + (ExtraSpriteGraphicsCurrentPointer - ExtraSpriteGraphicsBasePointer + ExtraSpriteGraphicsSize) / 0x80 > 0x1000) {
                results.AddEntry(new BuildResults.Entry(BuildResults.Entry.Level.ERROR, "", "Number of sprite tiles has exceeded the 0x1000 tile limit"));
            }
            // Check that all bytes that will be overwritten by custom sprite graphics are the same value (indicating unused data)
            if (ExtraSpriteGraphicsCurrentPointer > 0 && ExtraSpriteGraphicsSize > 0) {
                s.Seek(ExtraSpriteGraphicsCurrentPointer);
                int startByte = s.ReadByte();
                for (int i = 1; i < ExtraSpriteGraphicsSize; i++) {
                    if (s.ReadByte() != startByte) {
                        results.AddEntry(new BuildResults.Entry(BuildResults.Entry.Level.ERROR, "", "Added sprite graphics will overwrite existing data"));
                        break;
                    }
                }
            }
        }
    }
}
