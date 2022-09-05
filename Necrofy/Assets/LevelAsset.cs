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
        public const string Folder = "Levels";
        private const string Extension = "json";

        private const AssetCategory AssetCat = AssetCategory.Level;

        static LevelAsset() {
            AddCreator(new LevelCreator());
        }

        private readonly LevelNameInfo levelNameInfo;
        public Level level;

        public static LevelAsset FromProject(Project project, int levelNum) {
            return new LevelCreator().FromProject(project, levelNum);
        }

        public LevelAsset(int levelNum, Level level) : this(new LevelNameInfo(levelNum, n => level.displayName), level) { }

        private LevelAsset(LevelNameInfo nameInfo, Level level) : base(nameInfo) {
            this.levelNameInfo = nameInfo;
            this.level = level;
        }

        private LevelAsset(LevelNameInfo nameInfo, string filename) : base(nameInfo) {
            this.levelNameInfo = nameInfo;
            Reload(filename);
        }

        public int LevelNumber => levelNameInfo.levelNum;

        private static int GetPointerPosition(int levelNum) {
            return ROMPointers.LevelPointers + 2 + levelNum * 4;
        }

        protected override void Reload(string filename) {
            level = JsonConvert.DeserializeObject<Level>(File.ReadAllText(filename), new LevelJsonConverter());
        }

        protected override void WriteFile(string filename) {
            File.WriteAllText(filename, JsonConvert.SerializeObject(level, Formatting.Indented));
        }

        public override void Insert(NStream rom, ROMInfo romInfo, Project project) {
            MovableData levelData = level.Build(romInfo);
            int pointer = romInfo.Freespace.Claim(levelData.GetSize());
            byte[] data = levelData.Build(pointer);

            rom.Seek(pointer);
            rom.Write(data, 0, data.Length);
            rom.Seek(GetPointerPosition(levelNameInfo.levelNum));
            rom.WritePointer(pointer);
        }

        public override void ReserveSpace(ROMInfo romInfo) {
            romInfo.Freespace.Reserve(GetPointerPosition(levelNameInfo.levelNum), 4);
        }
        
        class LevelCreator : Creator
        {
            public LevelAsset FromProject(Project project, int levelNum) {
                NameInfo nameInfo = new LevelNameInfo(levelNum, n => GetDisplayName(n, project));
                string filename = nameInfo.GetFilename(project.path);
                return (LevelAsset)FromFile(nameInfo, filename);
            }

            public override NameInfo GetNameInfo(NameInfo.PathParts pathParts, Project project) {
                return LevelNameInfo.FromPath(pathParts, n => GetDisplayName(n, project));
            }

            public override Asset FromFile(NameInfo nameInfo, string filename) {
                return new LevelAsset((LevelNameInfo)nameInfo, filename);
            }

            private string GetDisplayName(LevelNameInfo nameInfo, Project project) {
                try {
                    using (FileStream fs = new FileStream(nameInfo.GetFilename(project.path), FileMode.Open, FileAccess.Read, FileShare.Read))
                    using (StreamReader sr = new StreamReader(fs))
                    using (JsonReader reader = new JsonTextReader(sr)) {
                        while (reader.Read()) {
                            if (reader.TokenType == JsonToken.PropertyName && (string)reader.Value == "displayName") {
                                return reader.ReadAsString();
                            }
                        }
                    }
                } catch (Exception e) {
                    Console.Write(e);
                }
                return "";
            }
        }

        class LevelNameInfo : NameInfo
        {
            public readonly int levelNum;
            private readonly Func<LevelNameInfo, string> displayNameGetter;
            private string displayName = null;

            private LevelNameInfo(PathParts parts, int levelNum, Func<LevelNameInfo, string> displayNameGetter) : base(parts) {
                this.levelNum = levelNum;
                this.displayNameGetter = displayNameGetter;
            }
            public LevelNameInfo(int levelNum, Func<LevelNameInfo, string> displayNameGetter) : this(new PathParts(Folder, levelNum.ToString(), Extension, null, false), levelNum, displayNameGetter) { }
            
            public override string DisplayName {
                get {
                    if (displayName == null) {
                        displayName = displayNameGetter(this);
                    }
                    return levelNum.ToString() + " " + displayName;
                }
            }

            public override AssetCategory Category => AssetCat;

            public override bool Editable => true;
            public override EditorWindow GetEditor(Project project) {
                return new LevelEditor(new LoadedLevel(project, levelNum));
            }

            public override void Refresh() {
                displayName = displayNameGetter(this);
            }

            public static LevelNameInfo FromPath(PathParts parts, Func<LevelNameInfo, string> displayNameGetter) {
                if (parts.folder != Folder) return null;
                if (parts.fileExtension != Extension) return null;
                if (parts.pointer != null) return null;
                if (parts.compressed) return null;
                if (!int.TryParse(parts.name, out int levelNum)) return null;
                return new LevelNameInfo(parts, levelNum, displayNameGetter);
            }
        }
    }
}
