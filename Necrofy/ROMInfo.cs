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

        private Dictionary<AssetCategory, Dictionary<int, string>> assetNames = new Dictionary<AssetCategory, Dictionary<int, string>>();
        private Dictionary<AssetCategory, Dictionary<string, PointerAndSize>> assetPointers = new Dictionary<AssetCategory, Dictionary<string, PointerAndSize>>();

        /// <summary>The freespace that was found in the ROM</summary>
        public readonly Freespace Freespace;
        /// <summary>Indicates whether or not the ROM was built with Necrofy</summary>
        public bool NecrofyROM { get; private set; }
        /// <summary>The level after which the winner screen will be shown</summary>
        public int WinLevel { get; private set; }
        /// <summary>The level after which the game will end and the player will be sent back to the main menu loop</summary>
        public int EndGameLevel { get; private set; }
        /// <summary>Definitions that will be available to all patches</summary>
        public readonly Dictionary<string, string> globalDefines = new Dictionary<string, string>();
        /// <summary>Definitions that will be available to patches in a specific folder </summary>
        public readonly Dictionary<string, Dictionary<string, string>> folderDefines = new Dictionary<string, Dictionary<string, string>>();
        /// <summary>Various per-asset settings</summary>
        public readonly AssetOptions assetOptions = new AssetOptions();

        private const string PropertiesHeader = "NFY";
        private const byte PropertiesVersion = 2;
        /// <summary>The total number of bytes that will be used by custom sprite graphics</summary>
        public int ExtraSpriteGraphicsSize { get; set; }
        /// <summary>A pointer to the start of the custom sprite graphics</summary>
        public int ExtraSpriteGraphicsBasePointer { get; set; }
        /// <summary>The current pointer that custom sprite graphics will be inserted at</summary>
        public int ExtraSpriteGraphicsCurrentPointer { get; set; }
        /// <summary>The starting index for custom sprite tiles</summary>
        public int ExtraSpriteGraphicsStartIndex { get; set; }
        /// <summary>Size of the extra sprite graphics that needs to be inserted at the beginning of the extra graphics block.
        /// This is used for projects that were created from a Necrofy ROM that already contained extra sprite graphics.</summary>
        public int ExtraSpriteGraphicsFirstSize { get; set; }

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
                } else {
                    throw new Exception("ROM was created with a newer version of Necrofy");
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

        public void AddAssetPointer(AssetCategory category, string name, int pointer, int size) {
            if (!assetPointers.ContainsKey(category)) {
                assetPointers.Add(category, new Dictionary<string, PointerAndSize>());
            }
            assetPointers[category].Add(name, new PointerAndSize(pointer, size));
            Asset.ParsedName parsedName = new Asset.ParsedName(name);
            AddFolderDefine(parsedName.Folder, category.ToString() + "_" + parsedName.FinalName, ROMPointers.PointerToHexString(pointer));
        }

        public void AddGlobalDefine(string key, string value) {
            globalDefines[FixDefineName(key)] = value;
        }

        public void AddFolderDefine(string folder, string key, string value) {
            if (!folderDefines.ContainsKey(folder)) {
                folderDefines[folder] = new Dictionary<string, string>();
            }
            folderDefines[folder][FixDefineName(key)] = value;
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

        public Dictionary<string, string> GetFolderDefines(string folder) {
            if (folderDefines.TryGetValue(folder, out Dictionary<string, string> defines)) {
                return defines;
            }
            return new Dictionary<string, string>();
        }

        public void LogDefines() {
            foreach (KeyValuePair<string, string> define in globalDefines) {
                Console.WriteLine("!" + define.Key + "=" + define.Value);
            }
            foreach (KeyValuePair<string, Dictionary<string, string>> folder in folderDefines) {
                Console.WriteLine(folder.Key);
                foreach (KeyValuePair<string, string> define in folder.Value) {
                    Console.WriteLine("    !" + define.Key + "=" + define.Value);
                }
            }
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

        public PointerAndSize GetAssetPointerAndSize(AssetCategory category, string name) {
            if (!assetPointers.ContainsKey(category)) {
                throw new Exception("No asset found for category " + category + " with name " + name);
            }
            if (!assetPointers[category].ContainsKey(name)) {
                throw new Exception("No asset found for category " + category + " with name " + name);
            }
            return assetPointers[category][name];
        }

        public int GetAssetPointer(AssetCategory category, string name) {
            return GetAssetPointerAndSize(category, name).Pointer;
        }

        public int GetAssetSize(AssetCategory category, string name) {
            return GetAssetPointerAndSize(category, name).Size;
        }

        public void WriteToBuild(NStream s, BuildResults results) {
            WriteProperties(s);

            if (ExtraSpriteGraphicsStartIndex + (ExtraSpriteGraphicsCurrentPointer - ExtraSpriteGraphicsBasePointer + ExtraSpriteGraphicsSize) / 0x80 > 0x1000) {
                results.AddEntry(new BuildResults.Entry(BuildResults.Entry.Level.ERROR, "", "Number of sprite tiles has exceeded the 0x1000 tile limit"));
            }
        }
    }
}
