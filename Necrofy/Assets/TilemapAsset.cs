using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Necrofy
{
    class TilemapAsset : Asset
    {
        public const string LevelTitleName = "Level Title";

        private const AssetCategory AssetCat = AssetCategory.Tilemap;

        public static void RegisterLoader() {
            AddCreator(new TilemapCreator());
        }

        public static string GetAssetName(NStream romStream, ROMInfo romInfo, int pointer) {
            return GetAssetName(romStream, romInfo, pointer, new TilemapCreator(), AssetCat);
        }

        private readonly TilemapNameInfo nameInfo;
        public readonly byte[] data;

        public static TilemapAsset FromProject(Project project, string tilemapName) {
            return new TilemapCreator().FromProject(project, tilemapName);
        }

        private TilemapAsset(TilemapNameInfo nameInfo, byte[] data) {
            this.nameInfo = nameInfo;
            this.data = data;
        }

        public override void WriteFile(Project project) {
            File.WriteAllBytes(nameInfo.GetFilename(project.path, createDirectories: true), data);
        }
        
        public override void Insert(NStream rom, ROMInfo romInfo, Project project) {
            InsertByteArray(rom, romInfo, data, nameInfo.pointer);
        }

        protected override AssetCategory Category => nameInfo.Category;
        protected override string Name => nameInfo.Name;

        class TilemapCreator : Creator
        {
            public TilemapAsset FromProject(Project project, string tilemapName) {
                NameInfo nameInfo = new TilemapNameInfo(tilemapName, null);
                string filename = nameInfo.FindFilename(project.path);
                return (TilemapAsset)FromFile(nameInfo, filename);
            }

            public override NameInfo GetNameInfo(NameInfo.PathParts pathParts, Project project) {
                return TilemapNameInfo.FromPath(pathParts);
            }

            public override Asset FromFile(NameInfo nameInfo, string filename) {
                return new TilemapAsset((TilemapNameInfo)nameInfo, File.ReadAllBytes(filename));
            }

            public override List<DefaultParams> GetDefaults() {
                return new List<DefaultParams>() {
                    new DefaultParams(0xb5641, new TilemapNameInfo(LevelTitleName, 0xb5641), 0x1200),
                };
            }

            public override Asset FromRom(NameInfo nameInfo, NStream romStream, int? size) {
                return new TilemapAsset((TilemapNameInfo)nameInfo, romStream.ReadBytes((int)size));
            }

            public override NameInfo GetNameInfoForName(string name) {
                return new TilemapNameInfo(name, null);
            }

            public override bool AutoTrackFreespace => false;
        }

        class TilemapNameInfo : NameInfo
        {
            private const string Folder = "Tilemaps";
            private const string Extension = "bin";

            public readonly string name;
            public readonly int? pointer;

            public TilemapNameInfo(string name, int? pointer) {
                this.name = name;
                this.pointer = pointer;
            }

            public override string Name => name;
            public override string DisplayName => name;
            public override AssetCategory Category => AssetCat;

            protected override PathParts GetPathParts() {
                return new PathParts(Folder, null, name, Extension, pointer, false);
            }

            public static TilemapNameInfo FromPath(PathParts parts) {
                if (parts.topFolder != Folder) return null;
                if (parts.subFolder != null) return null;
                if (parts.fileExtension != Extension) return null;
                return new TilemapNameInfo(parts.name, parts.pointer);
            }
        }
    }
}
