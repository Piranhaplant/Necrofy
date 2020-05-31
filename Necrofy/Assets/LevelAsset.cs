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

        public static void RegisterLoader() {
            AddCreator(new LevelCreator());
        }

        private readonly LevelNameInfo nameInfo;
        public readonly Level level;

        public static LevelAsset FromProject(Project project, int levelNum) {
            return new LevelCreator().FromProject(project, levelNum);
        }

        public LevelAsset(int levelNum, Level level) : this(new LevelNameInfo(levelNum, n => level.displayName), level) { }

        private LevelAsset(LevelNameInfo nameInfo, Level level) {
            this.nameInfo = nameInfo;
            this.level = level;
        }

        public int LevelNumber => nameInfo.levelNum;

        private static int GetPointerPosition(int levelNum) {
            return ROMPointers.LevelPointers + 2 + levelNum * 4;
        }

        public override void WriteFile(Project project) {
            File.WriteAllText(nameInfo.GetFilename(project.path, createDirectories: true), JsonConvert.SerializeObject(level));
        }

        public override void Insert(NStream rom, ROMInfo romInfo, Project project) {
            MovableData levelData = level.Build(romInfo);
            int pointer = romInfo.Freespace.Claim(levelData.GetSize());
            byte[] data = levelData.Build(pointer);

            rom.Seek(pointer, SeekOrigin.Begin);
            rom.Write(data, 0, data.Length);
            rom.Seek(GetPointerPosition(nameInfo.levelNum), SeekOrigin.Begin);
            rom.WritePointer(pointer);
        }

        public override void ReserveSpace(Freespace freespace) {
            freespace.Reserve(GetPointerPosition(nameInfo.levelNum), 4);
        }

        protected override AssetCategory Category => nameInfo.Category;
        protected override string Name => nameInfo.Name;

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
                return new LevelAsset((LevelNameInfo)nameInfo, JsonConvert.DeserializeObject<Level>(File.ReadAllText(filename), new LevelJsonConverter()));
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

            public LevelNameInfo(int levelNum, Func<LevelNameInfo, string> displayNameGetter) {
                this.levelNum = levelNum;
                this.displayNameGetter = displayNameGetter;
            }

            public override string Name => levelNum.ToString();

            public override string DisplayName {
                get {
                    if (displayName == null) {
                        displayName = displayNameGetter(this);
                    }
                    return levelNum.ToString() + " " + displayName;
                }
            }

            public override AssetCategory Category => AssetCat;

            protected override PathParts GetPathParts() {
                return new PathParts(Folder, null, levelNum.ToString(), Extension, null, false);
            }

            public override EditorWindow GetEditor(Project project) {
                return new LevelEditor(new LoadedLevel(project, levelNum));
            }

            public override void Refresh() {
                displayName = displayNameGetter(this);
            }

            public static LevelNameInfo FromPath(PathParts parts, Func<LevelNameInfo, string> displayNameGetter) {
                if (parts.topFolder != Folder) return null;
                if (parts.subFolder != null) return null;
                if (parts.fileExtension != Extension) return null;
                if (parts.pointer != null) return null;
                if (!int.TryParse(parts.name, out int levelNum)) return null;
                return new LevelNameInfo(levelNum, displayNameGetter);
            }
        }
    }
}
