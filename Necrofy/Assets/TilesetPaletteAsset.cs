using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Necrofy
{
    class TilesetPaletteAsset : TilesetAsset
    {
        private const AssetCategory AssetCat = AssetCategory.Palette;
        private const string AssetExtension = "plt";

        public static void RegisterLoader() {
            AddCreator(new TilesetPaletteCreator());
        }

        public static string GetAssetName(NStream romStream, ROMInfo romInfo, int pointer) {
            return Asset.GetAssetName(romStream, romInfo, pointer, new TilesetPaletteCreator());
        }

        private TilesetNameInfo nameInfo;
        public byte[] data;

        public static TilesetPaletteAsset FromProject(Project project, string fullPaletteName) {
            return new TilesetPaletteCreator().FromProject(project, fullPaletteName);
        }

        private TilesetPaletteAsset(TilesetNameInfo nameInfo, byte[] data) {
            this.nameInfo = nameInfo;
            this.data = data;
        }

        public override void WriteFile(Project project) {
            File.WriteAllBytes(nameInfo.GetFilename(project.path), data);
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

        class TilesetPaletteCreator : Creator
        {
            public TilesetPaletteAsset FromProject(Project project, string fullPaletteName) {
                NameInfo nameInfo = new TilesetNameInfo(fullPaletteName, AssetExtension);
                string filename = nameInfo.GetFilename(project.path);
                return (TilesetPaletteAsset)FromFile(nameInfo, filename);
            }

            public override NameInfo GetNameInfo(NameInfo.PathParts pathParts) {
                return TilesetNameInfo.FromPath(pathParts, AssetExtension);
            }

            public override Asset FromFile(NameInfo nameInfo, string filename) {
                return new TilesetPaletteAsset((TilesetNameInfo)nameInfo, File.ReadAllBytes(filename));
            }

            public override AssetCategory GetCategory() {
                return AssetCat;
            }

            public override List<DefaultParams> GetDefaults() {
                return new TilesetDefaultList(AssetExtension) {
                    { 0xf0e76, "Grass", "Normal" }, { 0xf1076, "Grass", "Autumn" }, { 0xf1176, "Grass", "Winter" }, { 0xf1276, "Grass", "Night" },
                    { 0xf1476, "Desert", "Pyramid" }, { 0xf1576, "Desert", "Beach" }, { 0xf1676, "Desert", "DarkBeach" }, { 0xf1776, "Desert", "Cave" },
                    { 0xf1876, "Castle", "Normal" }, { 0xf1976, "Castle", "Night" }, { 0xf1a76, "Castle", "Bright" }, { 0xf1b76, "Castle", "Dark" },
                    { 0xf1c76, "Mall", "Normal" }, { 0xf1d76, "Mall", "Alternate" },
                    { 0xf1e76, "Office", "Normal" }, { 0xf1f76, "Office", "DarkFireCave" }, { 0xf2076, "Office", "Light" }, { 0xf2176, "Office", "Dark" }, { 0xf2276, "Office", "FireCave" }
                };
            }

            public override Asset FromRom(NameInfo nameInfo, NStream romStream, int? size) {
                return new TilesetPaletteAsset((TilesetNameInfo)nameInfo, romStream.ReadBytes(0x100));
            }

            public override NameInfo GetNameInfoForName(string name) {
                return new TilesetNameInfo(name, name, AssetExtension);
            }
        }
    }
}
