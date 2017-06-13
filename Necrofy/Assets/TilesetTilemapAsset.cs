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
        private const string AssetFilename = "tilemap.bin";
        private static Dictionary<int, string> Defaults = new Dictionary<int, string>() { { 0xd4000, "Castle" }, { 0xd8000, "Grass" }, { 0xdbcb5, "Desert" }, { 0xe0000, "Office" }, { 0xe36ef, "Mall" } };

        public static void RegisterLoader() {
            Asset.AddLoader(
                (projectDir, path) => {
                    string tilesetName;
                    if (CheckPath(path, AssetFilename, out tilesetName)) {
                        return new TilesetTilemapAsset(tilesetName, File.ReadAllBytes(Path.Combine(projectDir, path)));
                    }
                    return null;
                },
                (romStream, romInfo) => {
                    foreach (KeyValuePair<int, string> def in Defaults) {
                        CreateAsset(romStream, romInfo, def.Key, def.Value);
                    }
                });
        }

        public static string GetAssetName(NStream romStream, ROMInfo romInfo, int pointer) {
            string name = romInfo.GetAssetName(AssetCat, pointer);
            if (name == null) {
                name = pointer.ToString("X6");
                CreateAsset(romStream, romInfo, pointer, name);
            }
            return name;
        }

        private byte[] data;

        public TilesetTilemapAsset(string tilesetName, byte[] data) {
            this.tilesetName = tilesetName;
            this.data = data;
        }

        private static void CreateAsset(NStream romStream, ROMInfo romInfo, int pointer, string tilesetName) {
            romStream.PushPosition();

            romStream.Seek(pointer, SeekOrigin.Begin);
            ZAMNCompress.AddToFreespace(romStream, romInfo.Freespace);
            byte[] data = ZAMNCompress.Decompress(romStream);

            Asset asset = new TilesetTilemapAsset(tilesetName, data);
            romInfo.assets.Add(asset);
            romInfo.AddAssetName(AssetCat, pointer, tilesetName);

            romStream.PopPosition();
        }

        public override void WriteFile(string projectDir) {
            string filename = CreateDirectories(projectDir, AssetFilename);
            File.WriteAllBytes(filename, data);
        }

        public override void Insert(NStream rom, ROMInfo romInfo) {
            int pointer = romInfo.Freespace.Claim(data.Length);
            rom.Seek(pointer, SeekOrigin.Begin);
            byte[] compressedData = ZAMNCompress.Compress(data);
            rom.Write(compressedData, 0, compressedData.Length);
            romInfo.AddAssetPointer(AssetCat, tilesetName, pointer);
        }

        public override AssetCategory Category {
            get { return AssetCat; }
        }
    }
}
