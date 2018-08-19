using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;

namespace Necrofy
{
    class TilesetPaletteAsset : TilesetAsset
    {
        private const AssetCategory AssetCat = AssetCategory.Palette;
        private const string AssetExtension = "plt";
        private static readonly Bitmap displayImage = Properties.Resources.spectrum;

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
                NameInfo nameInfo = new TilesetPaletteNameInfo(fullPaletteName);
                string filename = nameInfo.GetFilename(project.path);
                return (TilesetPaletteAsset)FromFile(nameInfo, filename);
            }

            public override NameInfo GetNameInfo(NameInfo.PathParts pathParts) {
                return TilesetPaletteNameInfo.FromPath(pathParts);
            }

            public override Asset FromFile(NameInfo nameInfo, string filename) {
                return new TilesetPaletteAsset((TilesetNameInfo)nameInfo, File.ReadAllBytes(filename));
            }

            public override AssetCategory GetCategory() {
                return AssetCat;
            }

            public override List<DefaultParams> GetDefaults() {
                return new List<DefaultParams>() {
                    new DefaultParams(0xf0e76, new TilesetPaletteNameInfo(Grass, "Normal")),
                    new DefaultParams(0xf1076, new TilesetPaletteNameInfo(Grass, "Autumn")),
                    new DefaultParams(0xf1176, new TilesetPaletteNameInfo(Grass, "Winter")),
                    new DefaultParams(0xf1276, new TilesetPaletteNameInfo(Grass, "Night")),

                    new DefaultParams(0xf1476, new TilesetPaletteNameInfo(Desert, "Pyramid")),
                    new DefaultParams(0xf1576, new TilesetPaletteNameInfo(Desert, "Beach")),
                    new DefaultParams(0xf1676, new TilesetPaletteNameInfo(Desert, "DarkBeach")),
                    new DefaultParams(0xf1776, new TilesetPaletteNameInfo(Desert, "Cave")),

                    new DefaultParams(0xf1876, new TilesetPaletteNameInfo(Castle, "Normal")),
                    new DefaultParams(0xf1976, new TilesetPaletteNameInfo(Castle, "Night")),
                    new DefaultParams(0xf1a76, new TilesetPaletteNameInfo(Castle, "Bright")),
                    new DefaultParams(0xf1b76, new TilesetPaletteNameInfo(Castle, "Dark")),

                    new DefaultParams(0xf1c76, new TilesetPaletteNameInfo(Mall, "Normal")),
                    new DefaultParams(0xf1d76, new TilesetPaletteNameInfo(Mall, "Alternate")),

                    new DefaultParams(0xf1e76, new TilesetPaletteNameInfo(Office, "Normal")),
                    new DefaultParams(0xf1f76, new TilesetPaletteNameInfo(Office, "DarkFireCave")),
                    new DefaultParams(0xf2076, new TilesetPaletteNameInfo(Office, "Light")),
                    new DefaultParams(0xf2176, new TilesetPaletteNameInfo(Office, "Dark")),
                    new DefaultParams(0xf2276, new TilesetPaletteNameInfo(Office, "FireCave")),
                };
            }

            public override Asset FromRom(NameInfo nameInfo, NStream romStream, int? size) {
                return new TilesetPaletteAsset((TilesetNameInfo)nameInfo, romStream.ReadBytes(0x100));
            }

            public override NameInfo GetNameInfoForName(string name) {
                return new TilesetPaletteNameInfo(name, name);
            }
        }

        class TilesetPaletteNameInfo : TilesetNameInfo
        {
            public TilesetPaletteNameInfo(string fullName) : base(fullName, AssetExtension) { }
            public TilesetPaletteNameInfo(string tilesetName, string name) : base(tilesetName, name, AssetExtension) { }

            public override Bitmap DisplayImage {
                get { return Properties.Resources.color; }
            }

            public static TilesetNameInfo FromPath(NameInfo.PathParts parts) {
                return TilesetNameInfo.FromPath(parts, AssetExtension, (t, n) => new TilesetPaletteNameInfo(t, n));
            }
        }
    }
}
