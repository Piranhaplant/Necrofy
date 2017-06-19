using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Necrofy
{
    class TilesetTilemapAsset : TilesetAsset
    {
        private const AssetCategory AssetCat = AssetCategory.Tilemap;
        private const string Filename = "tilemap";
        private const string AssetExtension = "bin";

        public static void RegisterLoader() {
            AddCreator(new TilesetTilemapCreator());
        }

        public static string GetAssetName(NStream romStream, ROMInfo romInfo, int pointer) {
            return Asset.GetAssetName(romStream, romInfo, pointer, new TilesetTilemapCreator());
        }

        private TilesetFixedNameInfo nameInfo;
        private byte[] data;

        private TilesetTilemapAsset(TilesetFixedNameInfo nameInfo, byte[] data) {
            this.nameInfo = nameInfo;
            this.data = data;
        }

        public override void WriteFile(string projectDir) {
            File.WriteAllBytes(nameInfo.GetFilename(projectDir), data);
        }

        protected override Asset.Inserter GetInserter(ROMInfo romInfo) {
            return new ByteArrayInserter(ZAMNCompress.Compress(data));
        }

        protected override AssetCategory Category {
            get { return AssetCat; }
        }

        protected override string Name {
            get { return nameInfo.Name; }
        }

        class TilesetTilemapCreator : Creator
        {
            public override NameInfo GetNameInfo(string path) {
                return TilesetFixedNameInfo.FromPath(path, Filename, AssetExtension);
            }

            public override Asset FromFile(NameInfo nameInfo, string filename) {
                return new TilesetTilemapAsset((TilesetFixedNameInfo)nameInfo, File.ReadAllBytes(filename));
            }

            public override AssetCategory GetCategory() {
                return AssetCat;
            }

            public override List<DefaultParams> GetDefaults() {
                return new TilesetFixedDefaultList(Filename, AssetExtension) {
                    { 0xd4000, "Castle" }, { 0xd8000, "Grass" }, { 0xdbcb5, "Desert" }, { 0xe0000, "Office" }, { 0xe36ef, "Mall" }
                };
            }

            public override Asset FromRom(NameInfo nameInfo, NStream romStream, int? size) {
                return new TilesetTilemapAsset((TilesetFixedNameInfo)nameInfo, ZAMNCompress.Decompress(romStream));
            }

            public override NameInfo GetNameInfoForName(string name) {
                return new TilesetFixedNameInfo(name, Filename, AssetExtension);
            }
        }
    }
}
