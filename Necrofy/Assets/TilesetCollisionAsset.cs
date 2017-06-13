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
        private const string AssetFilename = "collision.bin";
        private static Dictionary<int, string> Defaults = new Dictionary<int, string>() { { 0xe6aab, "Castle" }, { 0xdf4d1, "Grass" }, { 0xdf8d1, "Desert" }, { 0xe72ab, "Office" }, { 0xe6eab, "Mall" } };

        public static void RegisterLoader() {
            Asset.AddLoader(
                (projectDir, path) => {
                    string tilesetName;
                    if (CheckPath(path, AssetFilename, out tilesetName)) {
                        return new TilesetCollisionAsset(tilesetName, File.ReadAllBytes(Path.Combine(projectDir, path)));
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

        public TilesetCollisionAsset(string tilesetName, byte[] data) {
            this.tilesetName = tilesetName;
            this.data = data;
        }

        private static void CreateAsset(NStream romStream, ROMInfo romInfo, int pointer, string tilesetName) {
            romStream.PushPosition();

            romStream.Seek(pointer, SeekOrigin.Begin);
            byte[] data = new byte[0x400];
            romStream.Read(data, 0, data.Length);
            romInfo.Freespace.AddSize(pointer, data.Length);

            Asset asset = new TilesetCollisionAsset(tilesetName, data);
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
            rom.Write(data, 0, data.Length);
            romInfo.AddAssetPointer(AssetCat, tilesetName, pointer);
        }

        public override AssetCategory Category {
            get { return AssetCat; }
        }
    }
}
