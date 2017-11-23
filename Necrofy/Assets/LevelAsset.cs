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
        private const AssetCategory AssetCat = AssetCategory.Level;

        public static void RegisterLoader() {
            AddCreator(new LevelCreator());
        }

        private readonly LevelNameInfo nameInfo;
        public readonly Level level;

        public static LevelAsset FromProject(Project project, int levelNum) {
            return new LevelCreator().FromProject(project, levelNum);
        }

        public LevelAsset(int levelNum, Level level) : this(new LevelNameInfo(levelNum), level) { }

        private LevelAsset(LevelNameInfo nameInfo, Level level) {
            this.nameInfo = nameInfo;
            this.level = level;
        }

        private static int GetPointerPosition(int levelNum) {
            return ROMPointers.LevelPointers + 2 + levelNum * 4;
        }

        public override void WriteFile(Project project) {
            File.WriteAllText(nameInfo.GetFilename(project.path), JsonConvert.SerializeObject(level));
        }

        protected override Inserter GetInserter(ROMInfo romInfo) {
            return new LevelInserter(nameInfo.levelNum, level.Build(romInfo));
        }

        public override void ReserveSpace(Freespace freespace) {
            freespace.Reserve(GetPointerPosition(nameInfo.levelNum), 4);
        }

        protected override AssetCategory Category {
            get { return AssetCat; }
        }

        protected override string Name {
            get { return nameInfo.Name; }
        }

        class LevelCreator : Creator
        {
            public LevelAsset FromProject(Project project, int levelNum) {
                NameInfo nameInfo = new LevelNameInfo(levelNum);
                string filename = nameInfo.GetFilename(project.path);
                return (LevelAsset)FromFile(nameInfo, filename);
            }

            public override NameInfo GetNameInfo(NameInfo.PathParts pathParts) {
                return LevelNameInfo.FromPath(pathParts);
            }

            public override Asset FromFile(NameInfo nameInfo, string filename) {
                return new LevelAsset((LevelNameInfo)nameInfo, JsonConvert.DeserializeObject<Level>(File.ReadAllText(filename), new LevelJsonConverter()));
            }

            public override AssetCategory GetCategory() {
                return AssetCat;
            }
        }

        class LevelNameInfo : NameInfo
        {
            private const string Folder = "Levels";
            private const string Extension = "json";

            public readonly int levelNum;

            public LevelNameInfo(int levelNum) {
                this.levelNum = levelNum;
            }

            public override string Name {
                get { return levelNum.ToString(); }
            }

            protected override PathParts GetPathParts() {
                return new PathParts(Folder, null, levelNum.ToString(), Extension, null);
            }

            public static LevelNameInfo FromPath(NameInfo.PathParts parts) {
                if (parts.topFolder != Folder) return null;
                if (parts.subFolder != null) return null;
                if (parts.fileExtension != Extension) return null;
                if (parts.pointer != null) return null;
                int levelNum = int.Parse(parts.name); // TODO: Handle failed parse
                return new LevelNameInfo(levelNum);
            }
        }

        class LevelInserter : Inserter
        {
            private readonly int levelNum;
            private readonly MovableData levelData;

            public LevelInserter(int levelNum, MovableData levelData) {
                this.levelNum = levelNum;
                this.levelData = levelData;
            }

            public override int GetSize() {
                return levelData.GetSize();
            }

            public override byte[] GetData(int pointer) {
                return levelData.Build(pointer);
            }

            public override void InsertExtras(int pointer, NStream romStream) {
                romStream.Seek(GetPointerPosition(levelNum), SeekOrigin.Begin);
                romStream.WritePointer(pointer);
            }
        }
    }
}
