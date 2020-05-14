using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Necrofy
{
    class PaletteAsset : Asset
    {
        public const string SpritesName = "Sprites";
        public const string LevelTitleName = "Level Title";

        private const AssetCategory AssetCat = AssetCategory.Palette;

        public static void RegisterLoader() {
            AddCreator(new PaletteCreator());
        }
        
        public static string GetAssetName(NStream romStream, ROMInfo romInfo, int pointer) {
            return GetAssetName(romStream, romInfo, pointer, new PaletteCreator(), AssetCat);
        }

        private readonly PaletteNameInfo nameInfo;
        public readonly byte[] data;

        public static PaletteAsset FromProject(Project project, string paletteName) {
            return new PaletteCreator().FromProject(project, paletteName);
        }

        private PaletteAsset(PaletteNameInfo nameInfo, byte[] data) {
            this.nameInfo = nameInfo;
            this.data = data;
        }

        public override void WriteFile(Project project) {
            File.WriteAllBytes(nameInfo.GetFilename(project.path, createDirectories: true), data);
        }

        public override void ReserveSpace(Freespace freespace) {
            if (nameInfo.pointer != null) {
                freespace.Reserve((int)nameInfo.pointer, data.Length);
            }
        }

        public override void Insert(NStream rom, ROMInfo romInfo, Project project) {
            InsertByteArray(rom, romInfo, data, nameInfo.pointer);
        }

        protected override AssetCategory Category => nameInfo.Category;
        protected override string Name => nameInfo.Name;

        class PaletteCreator : Creator
        {
            public PaletteAsset FromProject(Project project, string paletteName) {
                NameInfo nameInfo = new PaletteNameInfo(paletteName, null);
                string filename = nameInfo.FindFilename(project.path);
                return (PaletteAsset)FromFile(nameInfo, filename);
            }

            public override NameInfo GetNameInfo(NameInfo.PathParts pathParts, Project project) {
                return PaletteNameInfo.FromPath(pathParts);
            }

            public override Asset FromFile(NameInfo nameInfo, string filename) {
                return new PaletteAsset((PaletteNameInfo)nameInfo, File.ReadAllBytes(filename));
            }

            public override List<DefaultParams> GetDefaults() {
                return new List<DefaultParams>() {
                    new DefaultParams(0xf0f76, new PaletteNameInfo(SpritesName, 0xf0f76)),
                    new DefaultParams(0x1f08c, new PaletteNameInfo(LevelTitleName, 0x1f08c)),
                    new DefaultParams(0xf0e76, new PaletteNameInfo("Copyright", 0xf0e76))
                };
            }

            public override Asset FromRom(NameInfo nameInfo, NStream romStream, int? size) {
                int actualSize = size ?? 0x100;
                return new PaletteAsset((PaletteNameInfo)nameInfo, romStream.ReadBytes(actualSize));
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

            public override string Name => name;
            public override string DisplayName => name;
            public override AssetCategory Category => AssetCat;

            protected override PathParts GetPathParts() {
                return new PathParts(Folder, null, name, Extension, pointer, false);
            }

            public static PaletteNameInfo FromPath(PathParts parts) {
                if (parts.topFolder != Folder) return null;
                if (parts.subFolder != null) return null;
                if (parts.fileExtension != Extension) return null;
                return new PaletteNameInfo(parts.name, parts.pointer);
            }
        }
    }
}
