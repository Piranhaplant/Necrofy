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
        public const string DefaultName = "Tilemap";

        private const AssetCategory AssetCat = AssetCategory.Tilemap;

        static TilemapAsset() {
            AddCreator(new TilemapCreator());
        }

        public static string GetAssetName(NStream romStream, ROMInfo romInfo, int pointer) {
            return GetAssetName(romStream, romInfo, new TilemapCreator(), AssetCat, pointer);
        }

        private readonly TilemapNameInfo nameInfo;
        public readonly byte[] data;

        public static TilemapAsset FromProject(Project project, string fullName) {
            ParsedName parsedName = new ParsedName(fullName);
            return new TilemapCreator().FromProject(project, parsedName.Folder, parsedName.FinalName);
        }

        private TilemapAsset(TilemapNameInfo nameInfo, byte[] data) {
            this.nameInfo = nameInfo;
            this.data = data;
        }

        public override void WriteFile(Project project) {
            File.WriteAllBytes(nameInfo.GetFilename(project.path, createDirectories: true), data);
        }
        
        public override void Insert(NStream rom, ROMInfo romInfo, Project project) {
            if (nameInfo.compressed) {
                InsertCompressedByteArray(rom, romInfo, data, nameInfo.GetFilename(project.path), nameInfo.pointer);
            } else {
                InsertByteArray(rom, romInfo, data, nameInfo.pointer);
            }
        }

        protected override AssetCategory Category => nameInfo.Category;
        protected override string Name => nameInfo.Name;

        class TilemapCreator : Creator
        {
            public TilemapAsset FromProject(Project project, string folder, string tilemapName) {
                NameInfo nameInfo = new TilemapNameInfo(folder, tilemapName, null);
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
                    new DefaultParams(0xb5641, new TilemapNameInfo(LevelTitleFolder, DefaultName, 0xb5641), 0x1200),

                    new DefaultParams(0xd4000, new TilemapNameInfo(GetTilesetFolder(Castle), DefaultName, compressed: true)),
                    new DefaultParams(0xd8000, new TilemapNameInfo(GetTilesetFolder(Grass), DefaultName, compressed: true)),
                    new DefaultParams(0xdbcb5, new TilemapNameInfo(GetTilesetFolder(Sand), DefaultName, compressed: true)),
                    new DefaultParams(0xe0000, new TilemapNameInfo(GetTilesetFolder(Office), DefaultName, compressed: true)),
                    new DefaultParams(0xe36ef, new TilemapNameInfo(GetTilesetFolder(Mall), DefaultName, compressed: true)),
                };
            }

            public override Asset FromRom(NameInfo nameInfo, NStream romStream, ROMInfo romInfo, int? size, out bool trackFreespace) {
                TilemapNameInfo tilemapNameInfo = (TilemapNameInfo)nameInfo;
                trackFreespace = tilemapNameInfo.pointer == null;
                if (tilemapNameInfo.compressed) {
                    return new TilemapAsset(tilemapNameInfo, ZAMNCompress.Decompress(romStream));
                } else {
                    return new TilemapAsset(tilemapNameInfo, romStream.ReadBytes((int)size));
                }
            }

            public override NameInfo GetNameInfoForName(string name, string group) {
                return new TilemapNameInfo(GetTilesetFolder(group), name, null);
            }
        }

        class TilemapNameInfo : NameInfo
        {
            private const string Extension = "tlm";

            public readonly string folder;
            public readonly string name;
            public readonly int? pointer;
            public readonly bool compressed;

            public TilemapNameInfo(string folder, string name, int? pointer = null, bool compressed = false) : base(folder, name) {
                this.folder = folder;
                this.name = name;
                this.pointer = pointer;
                this.compressed = compressed;
            }

            public override string DisplayName => name;
            public override AssetCategory Category => AssetCat;

            protected override PathParts GetPathParts() {
                return new PathParts(folder, name, Extension, pointer, compressed);
            }

            public static TilemapNameInfo FromPath(PathParts parts) {
                if (parts.fileExtension != Extension) return null;
                return new TilemapNameInfo(parts.folder, parts.name, parts.pointer, parts.compressed);
            }
        }
    }
}
