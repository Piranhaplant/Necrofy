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

        public static void RegisterLoader() {
            AddCreator(new PaletteCreator());
        }
        
        public static string GetAssetName(NStream romStream, ROMInfo romInfo, int pointer) {
            return Asset.GetAssetName(romStream, romInfo, pointer, new PaletteCreator());
        }

        private readonly PaletteNameInfo nameInfo;
        private readonly byte[] data;

        private PaletteAsset(PaletteNameInfo nameInfo, byte[] data) {
            this.nameInfo = nameInfo;
            this.data = data;
        }

        public override void WriteFile(string projectDir) {
            File.WriteAllBytes(nameInfo.GetFilename(projectDir), data);
        }

        protected override Inserter GetInserter(ROMInfo romInfo) {
            return new ByteArrayInserter(data);
        }

        protected override AssetCategory Category {
            get { return AssetCat; }
        }

        protected override string Name {
            get { return nameInfo.Name; }
        }

        class PaletteCreator : Creator
        {
            public override NameInfo GetNameInfo(string path) {
                return PaletteNameInfo.FromPath(path);
            }

            public override Asset FromFile(NameInfo nameInfo, string filename) {
                return new PaletteAsset((PaletteNameInfo)nameInfo, File.ReadAllBytes(filename));
            }

            public override AssetCategory GetCategory() {
                return AssetCat;
            }

            public override List<DefaultParams> GetDefaults() {
                return new List<DefaultParams>() { new DefaultParams(0xf0f76, new PaletteNameInfo("Sprites", null)) };
            }

            public override Asset FromRom(NameInfo nameInfo, NStream romStream) {
                return new PaletteAsset((PaletteNameInfo)nameInfo, romStream.ReadBytes(0x100));
            }

            public override NameInfo GetNameInfoForName(string name) {
                return new PaletteNameInfo(name, null);
            }
        }

        class PaletteNameInfo : NameInfo
        {
            private const string Folder = "Palettes";
            private const string Extension = "plt";

            public readonly string name;
            public readonly int? pointer;

            public PaletteNameInfo(string name, int? pointer) {
                this.name = name;
                this.pointer = pointer;
            }

            public override string Name {
                get { return name; }
            }

            protected override NameInfo.PathParts GetPathParts() {
                return new NameInfo.PathParts(Folder, null, name, Extension, pointer);
            }

            public static PaletteNameInfo FromPath(string path) {
                PathParts parts = NameInfo.ParsePath(path);
                if (parts.topFolder != Folder) return null;
                if (parts.subFolder != null) return null;
                if (parts.fileExtension != Extension) return null;
                return new PaletteNameInfo(parts.name, parts.pointer);
            }
        }
    }
}
