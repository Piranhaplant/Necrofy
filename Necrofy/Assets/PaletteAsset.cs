using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Necrofy
{
    class PaletteAsset : Asset
    {
        private const AssetCategory AssetCat = AssetCategory.Palette;
        private const string Folder = "Palettes";
        private const string Extension = "plt";

        private static Dictionary<int, string> Defaults = new Dictionary<int, string>() { { 0xf0f76, "Sprites" } };

        public static void RegisterLoader() {
            Asset.AddLoader(
                (projectDir, path) => {
                    string[] parts = path.Split(Path.DirectorySeparatorChar);
                    if (parts.Length == 2 && parts[0] == Folder) {
                        string[] nameParts = parts[1].Split('.');
                        if (nameParts.Length == 2 && nameParts[1] == Extension) {
                            return new PaletteAsset(nameParts[0], File.ReadAllBytes(Path.Combine(projectDir, path)));
                        }
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

        private string name;
        private byte[] data;

        public PaletteAsset(string name, byte[] data) {
            this.name = name;
            this.data = data;
        }

        private static void CreateAsset(NStream romStream, ROMInfo romInfo, int pointer, string name) {
            romStream.PushPosition();

            romStream.Seek(pointer, SeekOrigin.Begin);
            byte[] data = new byte[0x100];
            romStream.Read(data, 0, data.Length);
            romInfo.Freespace.AddSize(pointer, data.Length);

            Asset asset = new PaletteAsset(name, data);
            romInfo.assets.Add(asset);
            romInfo.AddAssetName(AssetCat, pointer, name);

            romStream.PopPosition();
        }

        public override void WriteFile(string projectDir) {
            string path = Path.Combine(projectDir, Folder);
            if (!Directory.Exists(path)) {
                Directory.CreateDirectory(path);
            }
            path = Path.Combine(path, name + "." + Extension);
            File.WriteAllBytes(path, data);
        }

        public override void Insert(NStream rom, ROMInfo romInfo) {
            int pointer = romInfo.Freespace.Claim(data.Length);
            rom.Seek(pointer, SeekOrigin.Begin);
            rom.Write(data, 0, data.Length);
            romInfo.AddAssetPointer(AssetCat, name, pointer);
        }

        public override AssetCategory Category {
            get { return AssetCat; }
        }
    }
}
