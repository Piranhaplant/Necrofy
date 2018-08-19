﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Necrofy
{
    class PaletteAsset : Asset
    {
        private const AssetCategory AssetCat = AssetCategory.Palette;
        private const string AssetExtension = "plt";

        public static void RegisterLoader() {
            AddCreator(new PaletteCreator());
        }
        
        public static string GetAssetName(NStream romStream, ROMInfo romInfo, int pointer) {
            return Asset.GetAssetName(romStream, romInfo, pointer, new PaletteCreator());
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
            File.WriteAllBytes(nameInfo.GetFilename(project.path), data);
        }

        protected override Inserter GetInserter(ROMInfo romInfo) {
            return new ByteArrayInserter(data);
        }

        protected override int? FixedPointer {
            get { return nameInfo.pointer; }
        }

        protected override AssetCategory Category {
            get { return AssetCat; }
        }

        protected override string Name {
            get { return nameInfo.Name; }
        }

        class PaletteCreator : Creator
        {
            public PaletteAsset FromProject(Project project, string paletteName) {
                NameInfo nameInfo = new PaletteNameInfo(paletteName, null);
                string filename = nameInfo.FindFilename(project.path);
                return (PaletteAsset)FromFile(nameInfo, filename);
            }

            public override NameInfo GetNameInfo(NameInfo.PathParts pathParts) {
                return PaletteNameInfo.FromPath(pathParts);
            }

            public override Asset FromFile(NameInfo nameInfo, string filename) {
                return new PaletteAsset((PaletteNameInfo)nameInfo, File.ReadAllBytes(filename));
            }

            public override AssetCategory GetCategory() {
                return AssetCat;
            }

            public override List<DefaultParams> GetDefaults() {
                return new List<DefaultParams>() {
                    new DefaultParams(0xf0f76, new PaletteNameInfo("Sprites", 0xf0f76)),
                    new DefaultParams(0xf0e96, new PaletteNameInfo("Copyright", 0xf0e96), 0x20)
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

            public readonly string name;
            public readonly int? pointer;

            public PaletteNameInfo(string name, int? pointer) {
                this.name = name;
                this.pointer = pointer;
            }

            public override string Name {
                get { return name; }
            }

            public override string DisplayName {
                get { return name; }
            }

            public override System.Drawing.Bitmap DisplayImage {
                get { return Properties.Resources.color; }
            }

            protected override NameInfo.PathParts GetPathParts() {
                return new NameInfo.PathParts(Folder, null, name, AssetExtension, pointer);
            }

            public static PaletteNameInfo FromPath(NameInfo.PathParts parts) {
                if (parts.topFolder != Folder) return null;
                if (parts.subFolder != null) return null;
                if (parts.fileExtension != AssetExtension) return null;
                return new PaletteNameInfo(parts.name, parts.pointer);
            }
        }
    }
}
