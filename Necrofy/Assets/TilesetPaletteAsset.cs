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
        private const string AssetExtension = ".plt";
        private const char NameSeparator = '/';
        private static Dictionary<int, string> Defaults = 
            new Dictionary<int, string>() { { 0xf0e76, "Grass/Normal" }, { 0xf1076, "Grass/Autumn" }, { 0xf1176, "Grass/Winter" }, { 0xf1276, "Grass/Night" },
                                            { 0xf1476, "Desert/Pyramid" }, { 0xf1576, "Desert/Beach" }, { 0xf1676, "Desert/DarkBeach" }, { 0xf1776, "Desert/Cave" },
                                            { 0xf1876, "Castle/Normal" }, { 0xf1976, "Castle/Night" }, { 0xf1a76, "Castle/Bright" }, { 0xf1b76, "Castle/Dark" },
                                            { 0xf1c76, "Mall/Normal" }, { 0xf1d76, "Mall/Alternate" },
                                            { 0xf1e76, "Office/Normal" }, { 0xf1f76, "Office/DarkFireCave" }, { 0xf2076, "Office/Light" }, { 0xf2176, "Office/Dark" }, { 0xf2276, "Office/FireCave" } };
        
        public static void RegisterLoader() {
            Asset.AddLoader(
                (projectDir, path) => {
                    string tilesetName, paletteName;
                    if (CheckPathEndsWith(path, AssetExtension, out tilesetName, out paletteName)) {
                        return new TilesetPaletteAsset(tilesetName, paletteName, File.ReadAllBytes(Path.Combine(projectDir, path)));
                    }
                    return null;
                },
                (romStream, romInfo) => {
                    foreach (KeyValuePair<int, string> def in Defaults) {
                        string[] parts = def.Value.Split(NameSeparator);
                        CreateAsset(romStream, romInfo, def.Key, parts[0], parts[1]);
                    }
                });
        }

        private static String GetFullName(string tilesetName, string paletteName) {
            return tilesetName + NameSeparator + paletteName;
        }

        public static string GetAssetName(NStream romStream, ROMInfo romInfo, int pointer, string tilesetName) {
            string name = romInfo.GetAssetName(AssetCat, pointer);
            if (name == null) {
                name = pointer.ToString("X6");
                CreateAsset(romStream, romInfo, pointer, tilesetName, name);
                return GetFullName(tilesetName, name);
            }
            return name;
        }

        private string paletteName;
        private byte[] data;

        public TilesetPaletteAsset(string tilesetName, string paletteName, byte[] data) {
            this.tilesetName = tilesetName;
            this.paletteName = paletteName;
            this.data = data;
        }

        private static void CreateAsset(NStream romStream, ROMInfo romInfo, int pointer, string tilesetName, string paletteName) {
            romStream.PushPosition();

            romStream.Seek(pointer, SeekOrigin.Begin);
            byte[] data = new byte[0x100];
            romStream.Read(data, 0, data.Length);
            romInfo.Freespace.AddSize(pointer, data.Length);

            Asset asset = new TilesetPaletteAsset(tilesetName, paletteName, data);
            romInfo.assets.Add(asset);
            romInfo.AddAssetName(AssetCat, pointer, GetFullName(tilesetName, paletteName));

            romStream.PopPosition();
        }

        public override void WriteFile(string projectDir) {
            string filename = CreateDirectories(projectDir, paletteName + AssetExtension);
            File.WriteAllBytes(filename, data);
        }

        public override void Insert(NStream rom, ROMInfo romInfo) {
            int pointer = romInfo.Freespace.Claim(data.Length);
            rom.Seek(pointer, SeekOrigin.Begin);
            rom.Write(data, 0, data.Length);
            romInfo.AddAssetPointer(AssetCat, GetFullName(tilesetName, paletteName), pointer);
        }

        public override AssetCategory Category {
            get { return AssetCat; }
        }
    }
}
