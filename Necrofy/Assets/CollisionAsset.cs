using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Necrofy
{
    class CollisionAsset : Asset
    {
        public const string DefaultName = "Collision";
        private const AssetCategory AssetCat = AssetCategory.Collision;

        static CollisionAsset() {
            AddCreator(new CollisionCreator());
        }

        public static string GetAssetName(NStream romStream, ROMInfo romInfo, int pointer, string tileset) {
            return GetAssetName(romStream, romInfo, new CollisionCreator(), AssetCat, pointer, tileset);
        }

        private readonly CollisionNameInfo nameInfo;
        public readonly byte[] data;

        public static CollisionAsset FromProject(Project project, string fullName) {
            ParsedName parsedName = new ParsedName(fullName);
            return new CollisionCreator().FromProject(project, parsedName.Folder, parsedName.FinalName);
        }

        private CollisionAsset(CollisionNameInfo nameInfo, byte[] data) {
            this.nameInfo = nameInfo;
            this.data = data;
        }

        public override void WriteFile(Project project) {
            File.WriteAllBytes(nameInfo.GetFilename(project.path, createDirectories: true), data);
        }
        
        public override void Insert(NStream rom, ROMInfo romInfo, Project project) {
            InsertByteArray(rom, romInfo, data);
        }

        protected override AssetCategory Category => nameInfo.Category;
        protected override string Name => nameInfo.Name;

        class CollisionCreator : Creator
        {
            public CollisionAsset FromProject(Project project, string folder, string CollisionName) {
                NameInfo nameInfo = new CollisionNameInfo(folder, CollisionName);
                string filename = nameInfo.FindFilename(project.path);
                return (CollisionAsset)FromFile(nameInfo, filename);
            }

            public override NameInfo GetNameInfo(NameInfo.PathParts pathParts, Project project) {
                return CollisionNameInfo.FromPath(pathParts);
            }

            public override Asset FromFile(NameInfo nameInfo, string filename) {
                return new CollisionAsset((CollisionNameInfo)nameInfo, File.ReadAllBytes(filename));
            }

            public override List<DefaultParams> GetDefaults() {
                return new List<DefaultParams>() {
                    new DefaultParams(0xe6aab, new CollisionNameInfo(GetTilesetFolder(Castle), DefaultName)),
                    new DefaultParams(0xdf4d1, new CollisionNameInfo(GetTilesetFolder(Grass), DefaultName)),
                    new DefaultParams(0xdf8d1, new CollisionNameInfo(GetTilesetFolder(Sand), DefaultName)),
                    new DefaultParams(0xe72ab, new CollisionNameInfo(GetTilesetFolder(Office), DefaultName)),
                    new DefaultParams(0xe6eab, new CollisionNameInfo(GetTilesetFolder(Mall), DefaultName)),
                };
            }

            public override Asset FromRom(NameInfo nameInfo, NStream romStream, ROMInfo romInfo, int? size, out bool trackFreespace) {
                trackFreespace = true;
                return new CollisionAsset((CollisionNameInfo)nameInfo, romStream.ReadBytes(0x400));
            }

            public override NameInfo GetNameInfoForName(string name, string group) {
                return new CollisionNameInfo(GetTilesetFolder(group), name);
            }
        }

        class CollisionNameInfo : NameInfo
        {
            private const string Extension = "col";

            public readonly string folder;
            public readonly string name;

            public CollisionNameInfo(string folder, string name) : base(folder, name) {
                this.folder = folder;
                this.name = name;
            }

            public override string DisplayName => name;
            public override AssetCategory Category => AssetCat;

            protected override PathParts GetPathParts() {
                return new PathParts(folder, name, Extension, null, false);
            }

            public static CollisionNameInfo FromPath(PathParts parts) {
                if (parts.fileExtension != Extension) return null;
                if (parts.pointer != null) return null;
                return new CollisionNameInfo(parts.folder, parts.name);
            }
        }
    }
}
