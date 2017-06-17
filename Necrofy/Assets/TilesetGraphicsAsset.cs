using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Necrofy
{
    class TilesetGraphicsAsset : TilesetAsset
    {
        private const AssetCategory AssetCat = AssetCategory.Graphics;
        private const string Filename = "graphics";
        private const string AssetExtension = "bin";

        public static void RegisterLoader() {
            AddCreator(new TilesetGraphicsCreator());
        }

        public static string GetAssetName(NStream romStream, ROMInfo romInfo, int pointer) {
            return Asset.GetAssetName(romStream, romInfo, pointer, new TilesetGraphicsCreator());
        }

        private TilesetFixedNameInfo nameInfo;
        private byte[] data;

        private TilesetGraphicsAsset(TilesetFixedNameInfo nameInfo, byte[] data) {
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

        class TilesetGraphicsCreator : Creator
        {
            public override NameInfo GetNameInfo(string path) {
                return TilesetFixedNameInfo.FromPath(path, Filename, AssetExtension);
            }

            public override Asset FromFile(NameInfo nameInfo, string filename) {
                return new TilesetGraphicsAsset((TilesetFixedNameInfo)nameInfo, File.ReadAllBytes(filename));
            }

            public override AssetCategory GetCategory() {
                return AssetCat;
            }

            public override List<DefaultParams> GetDefaults() {
                return new TilesetFixedDefaultList(Filename, AssetExtension) {
                    { 0xc8000, "Castle" }, { 0xc0000, "Grass" }, { 0xc4000, "Desert" }, { 0xd0000, "Office" }, { 0xcc000, "Mall" }
                };
            }

            public override Asset FromRom(NameInfo nameInfo, NStream romStream) {
                return new TilesetGraphicsAsset((TilesetFixedNameInfo)nameInfo, romStream.ReadBytes(0x4000));
            }

            public override NameInfo GetNameInfoForName(string name) {
                return new TilesetFixedNameInfo(name, Filename, AssetExtension);
            }
        }
    }
}
