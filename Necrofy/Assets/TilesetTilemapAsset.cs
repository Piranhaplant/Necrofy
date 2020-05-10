using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Necrofy
{
    class TilesetTilemapAsset : TilesetAsset
    {
        private const AssetCategory AssetCat = AssetCategory.TilesetTilemap;

        public static void RegisterLoader() {
            AddCreator(new TilesetTilemapCreator());
        }

        public static string GetAssetName(NStream romStream, ROMInfo romInfo, int pointer) {
            return GetAssetName(romStream, romInfo, pointer, new TilesetTilemapCreator(), AssetCat);
        }

        private TilesetFixedNameInfo nameInfo;
        public byte[] data;

        public static TilesetTilemapAsset FromProject(Project project, string tilesetName) {
            return new TilesetTilemapCreator().FromProject(project, tilesetName);
        }

        private TilesetTilemapAsset(TilesetFixedNameInfo nameInfo, byte[] data) {
            this.nameInfo = nameInfo;
            this.data = data;
        }

        public override void WriteFile(Project project) {
            File.WriteAllBytes(nameInfo.GetFilename(project.path, createDirectories: true), data);
        }

        public override void Insert(NStream rom, ROMInfo romInfo, Project project) {
            InsertCompressedByteArray(rom, romInfo, data, nameInfo.GetFilename(project.path));
        }

        protected override AssetCategory Category => nameInfo.Category;
        protected override string Name => nameInfo.Name;

        class TilesetTilemapCreator : Creator
        {
            public TilesetTilemapAsset FromProject(Project project, string tilesetName) {
                NameInfo nameInfo = new TilesetTilemapNameInfo(tilesetName);
                string filename = nameInfo.GetFilename(project.path);
                return (TilesetTilemapAsset)FromFile(nameInfo, filename);
            }

            public override NameInfo GetNameInfo(NameInfo.PathParts pathParts, Project project) {
                return TilesetTilemapNameInfo.FromPath(pathParts);
            }

            public override Asset FromFile(NameInfo nameInfo, string filename) {
                return new TilesetTilemapAsset((TilesetFixedNameInfo)nameInfo, File.ReadAllBytes(filename));
            }

            public override List<DefaultParams> GetDefaults() {
                return new List<DefaultParams>() {
                    new DefaultParams(0xd4000, new TilesetTilemapNameInfo(Castle)),
                    new DefaultParams(0xd8000, new TilesetTilemapNameInfo(Grass)),
                    new DefaultParams(0xdbcb5, new TilesetTilemapNameInfo(Sand)),
                    new DefaultParams(0xe0000, new TilesetTilemapNameInfo(Office)),
                    new DefaultParams(0xe36ef, new TilesetTilemapNameInfo(Mall))
                };
            }

            public override Asset FromRom(NameInfo nameInfo, NStream romStream, int? size) {
                return new TilesetTilemapAsset((TilesetFixedNameInfo)nameInfo, ZAMNCompress.Decompress(romStream));
            }

            public override NameInfo GetNameInfoForName(string name) {
                return new TilesetTilemapNameInfo(name);
            }
        }

        class TilesetTilemapNameInfo : TilesetFixedNameInfo
        {
            private const string Filename = "tilemap";
            private const string Extension = "bin";

            public TilesetTilemapNameInfo(string tilesetName) : base(tilesetName, Filename, Extension) { }

            public override AssetCategory Category => AssetCat;

            public static TilesetFixedNameInfo FromPath(PathParts parts) {
                return FromPath(parts, Filename, Extension, s => new TilesetTilemapNameInfo(s));
            }
        }
    }
}
