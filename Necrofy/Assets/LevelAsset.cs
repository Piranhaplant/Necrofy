using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Necrofy
{
    class LevelAsset : Asset
    {
        private const string Folder = "Levels";
        private const string Extension = "json";

        public static void RegisterLoader() {
            Asset.AddLoader(
                (projectDir, path) => {
                    string[] parts = path.Split(Path.DirectorySeparatorChar);
                    if (parts.Length == 2 && parts[0] == Folder) {
                        string[] nameParts = parts[1].Split('.');
                        if (nameParts.Length == 2 && nameParts[1] == Extension) {
                            int levelNum;
                            if (int.TryParse(nameParts[0], out levelNum)) {
                                Level level = JsonConvert.DeserializeObject<Level>(File.ReadAllText(Path.Combine(projectDir, path)), new LevelJsonConverter());
                                return new LevelAsset(levelNum, level);
                            }
                        }
                    }
                    return null;
                },
                (romStream, romInfo) => { });
        }

        private int levelNum;
        private Level level;

        public LevelAsset(int levelNum, Level level) {
            this.levelNum = levelNum;
            this.level = level;
        }

        public override void WriteFile(string projectDir) {
            string path = Path.Combine(projectDir, Folder);
            if (!Directory.Exists(path)) {
                Directory.CreateDirectory(path);
            }
            path = Path.Combine(path, levelNum.ToString() + "." + Extension);
            File.WriteAllText(path, JsonConvert.SerializeObject(level));
        }

        private int GetPointerPosition() {
            return ROMPointers.LevelPointers + 2 + levelNum * 4;
        }

        public override void Insert(NStream rom, ROMInfo romInfo) {
            MovableData levelData = level.Build(romInfo);
            int pointer = romInfo.Freespace.Claim(levelData.GetSize());
            byte[] levelDataArray = levelData.Build(pointer);

            rom.Seek(GetPointerPosition(), SeekOrigin.Begin);
            rom.WritePointer(pointer);
            
            rom.Seek(pointer, SeekOrigin.Begin);
            rom.Write(levelDataArray, 0, levelDataArray.Length);
        }

        public override void ReserveSpace(Freespace freespace) {
            freespace.Reserve(GetPointerPosition(), 4);
        }

        public override AssetCategory Category {
            get { return AssetCategory.Level; }
        }
    }
}
