using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Necrofy
{
    class TilesetCollisionAsset : TilesetAsset
    {
        private const AssetCategory AssetCat = AssetCategory.Collision;
        private const string Filename = "collision";
        private const string AssetExtension = "bin";

        public static void RegisterLoader() {
            AddCreator(new TilesetCollisionCreator());
        }

        public static string GetAssetName(NStream romStream, ROMInfo romInfo, int pointer) {
            return Asset.GetAssetName(romStream, romInfo, pointer, new TilesetCollisionCreator());
        }

        private TilesetFixedNameInfo nameInfo;
        private byte[] data;

        private TilesetCollisionAsset(TilesetFixedNameInfo nameInfo, byte[] data) {
            this.nameInfo = nameInfo;
            this.data = data;
        }

        public override void WriteFile(string projectDir) {
            File.WriteAllBytes(nameInfo.GetFilename(projectDir), data);
        }

        protected override Asset.Inserter GetInserter(ROMInfo romInfo) {
            return new ByteArrayInserter(data);
        }

        protected override AssetCategory Category {
            get { return AssetCat; }
        }

        protected override string Name {
            get { return nameInfo.Name; }
        }

        class TilesetCollisionCreator : Creator
        {
            public override NameInfo GetNameInfo(string path) {
                return TilesetFixedNameInfo.FromPath(path, Filename, AssetExtension);
            }

            public override Asset FromFile(NameInfo nameInfo, string filename) {
                return new TilesetCollisionAsset((TilesetFixedNameInfo)nameInfo, File.ReadAllBytes(filename));
            }

            public override AssetCategory GetCategory() {
                return AssetCat;
            }

            public override List<DefaultParams> GetDefaults() {
                return new TilesetFixedDefaultList(Filename, AssetExtension) {
                    { 0xe6aab, "Castle" }, { 0xdf4d1, "Grass" }, { 0xdf8d1, "Desert" }, { 0xe72ab, "Office" }, { 0xe6eab, "Mall" }
                };
            }

            public override Asset FromRom(NameInfo nameInfo, NStream romStream) {
                return new TilesetCollisionAsset((TilesetFixedNameInfo)nameInfo, romStream.ReadBytes(0x400));
            }

            public override NameInfo GetNameInfoForName(string name) {
                return new TilesetFixedNameInfo(name, Filename, AssetExtension);
            }
        }
    }
}
