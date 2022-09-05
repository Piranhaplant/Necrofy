using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Necrofy
{
    class PaletteAsset : Asset
    {
        public const string DefaultName = "Palette";
        public const string DefaultSpritePaletteName = "Normal";

        private const AssetCategory AssetCat = AssetCategory.Palette;

        static PaletteAsset() {
            AddCreator(new PaletteCreator());
        }
        
        public static string GetAssetName(NStream romStream, ROMInfo romInfo, int pointer, string tileset = null) {
            return GetAssetName(romStream, romInfo, new PaletteCreator(), AssetCat, pointer, tileset);
        }

        public byte[] data;

        public static PaletteAsset FromProject(Project project, string fullName) {
            ParsedName parsedName = new ParsedName(fullName);
            return new PaletteCreator().FromProject(project, parsedName.Folder, parsedName.FinalName);
        }
        
        private PaletteAsset(PaletteNameInfo nameInfo, byte[] data) : base(nameInfo) {
            this.data = data;
        }

        private PaletteAsset(PaletteNameInfo nameInfo, string filename) : base(nameInfo) {
            Reload(filename);
        }

        protected override void Reload(string filename) {
            data = File.ReadAllBytes(filename);
        }

        protected override void WriteFile(string filename) {
            File.WriteAllBytes(filename, data);
        }

        public override void ReserveSpace(ROMInfo romInfo) {
            if (nameInfo.Parts.pointer != null) {
                romInfo.Freespace.Reserve((int)nameInfo.Parts.pointer, data.Length);
            }
        }

        public override void Insert(NStream rom, ROMInfo romInfo, Project project) {
            InsertByteArray(rom, romInfo, data, nameInfo.Parts.pointer);
        }
        
        class PaletteCreator : Creator
        {
            public PaletteAsset FromProject(Project project, string folder, string paletteName) {
                NameInfo nameInfo = new PaletteNameInfo(folder, paletteName, null);
                return project.GetCachedAsset(nameInfo, () => {
                    string filename = nameInfo.FindFilename(project.path);
                    return (PaletteAsset)FromFile(nameInfo, filename);
                });
            }

            public override NameInfo GetNameInfo(NameInfo.PathParts pathParts, Project project) {
                return PaletteNameInfo.FromPath(pathParts);
            }

            public override Asset FromFile(NameInfo nameInfo, string filename) {
                return new PaletteAsset((PaletteNameInfo)nameInfo, filename);
            }

            public override List<DefaultParams> GetDefaults() {
                return new List<DefaultParams>() {
                    new DefaultParams(0xf0f76, new PaletteNameInfo(SpritesFolder, DefaultSpritePaletteName, 0xf0f76), extractFromNecrofyROM: true),
                    new DefaultParams(0x1f08c, new PaletteNameInfo(LevelTitleFolder, DefaultName, 0x1f08c), extractFromNecrofyROM: true),
                    new DefaultParams(0x1f28c, new PaletteNameInfo(TitleScreenFolder, DefaultName, 0x1f28c), extractFromNecrofyROM: true),

                    new DefaultParams(0xf0e76, new PaletteNameInfo(GetTilesetFolder(Grass), "Normal", 0xf0e76), extractFromNecrofyROM: true),
                    new DefaultParams(0xf1076, new PaletteNameInfo(GetTilesetFolder(Grass), "Autumn")),
                    new DefaultParams(0xf1176, new PaletteNameInfo(GetTilesetFolder(Grass), "Winter")),
                    new DefaultParams(0xf1276, new PaletteNameInfo(GetTilesetFolder(Grass), "Night")),

                    new DefaultParams(0xf1476, new PaletteNameInfo(GetTilesetFolder(Sand), "Pyramid")),
                    new DefaultParams(0xf1576, new PaletteNameInfo(GetTilesetFolder(Sand), "Beach")),
                    new DefaultParams(0xf1676, new PaletteNameInfo(GetTilesetFolder(Sand), "Dark Beach")),
                    new DefaultParams(0xf1776, new PaletteNameInfo(GetTilesetFolder(Sand), "Mines")),

                    new DefaultParams(0xf1876, new PaletteNameInfo(GetTilesetFolder(Castle), "Normal")),
                    new DefaultParams(0xf1976, new PaletteNameInfo(GetTilesetFolder(Castle), "Night")),
                    new DefaultParams(0xf1a76, new PaletteNameInfo(GetTilesetFolder(Castle), "Bright")),
                    new DefaultParams(0xf1b76, new PaletteNameInfo(GetTilesetFolder(Castle), "Dark")),

                    new DefaultParams(0xf1c76, new PaletteNameInfo(GetTilesetFolder(Mall), "Mall")),
                    new DefaultParams(0xf1d76, new PaletteNameInfo(GetTilesetFolder(Mall), "Factory")),

                    new DefaultParams(0xf1e76, new PaletteNameInfo(GetTilesetFolder(Office), "Normal")),
                    new DefaultParams(0xf1f76, new PaletteNameInfo(GetTilesetFolder(Office), "Dark Fire Cave")),
                    new DefaultParams(0xf2076, new PaletteNameInfo(GetTilesetFolder(Office), "Light")),
                    new DefaultParams(0xf2176, new PaletteNameInfo(GetTilesetFolder(Office), "Dark")),
                    new DefaultParams(0xf2276, new PaletteNameInfo(GetTilesetFolder(Office), "Fire Cave")),

                    new DefaultParams(0, new PaletteNameInfo(ScratchPadFolder, DefaultName, skipped: true), extractFromNecrofyROM: true, versionAdded: new Version(2, 0)),
                };
            }

            public override Asset FromRom(NameInfo nameInfo, NStream romStream, ROMInfo romInfo, int? size, out bool trackFreespace) {
                PaletteNameInfo paletteNameInfo = (PaletteNameInfo)nameInfo;
                if (romStream.Position == 0) {
                    // Special pointer used for scratch pad asset
                    trackFreespace = false;
                    return new PaletteAsset(paletteNameInfo, new byte[0x100]);
                } else {
                    trackFreespace = paletteNameInfo.Parts.pointer == null;
                    return new PaletteAsset(paletteNameInfo, romStream.ReadBytes(size ?? 0x100));
                }
            }

            public override NameInfo GetNameInfoForName(string name, string group) {
                if (group == null || group == SpritesFolder) {
                    return new PaletteNameInfo(SpritesFolder, name);
                } else {
                    return new PaletteNameInfo(GetTilesetFolder(group), name);
                }
            }

            public override NameInfo GetNameInfoForExtraction(ExtractionPreset preset) {
                if (preset.Type == ExtractionPreset.AssetType.Palette) {
                    return new PaletteNameInfo(preset.Category, preset.Filename, preset.Address);
                }
                return null;
            }
        }

        class PaletteNameInfo : NameInfo
        {
            private const string Extension = "plt";
            
            private PaletteNameInfo(PathParts parts) : base(parts) { }
            public PaletteNameInfo(string folder, string name, int? pointer = null, bool skipped = false) : this(new PathParts(folder, name, Extension, pointer, false, skipped)) { }
            
            public override AssetCategory Category => AssetCat;
            
            public override bool Editable => true;
            public override EditorWindow GetEditor(Project project) {
                return new PaletteEditor(new LoadedPalette(project, Name));
            }
            
            public static PaletteNameInfo FromPath(PathParts parts) {
                if (parts.fileExtension != Extension) return null;
                if (parts.compressed) return null;
                return new PaletteNameInfo(parts);
            }
        }
    }
}
