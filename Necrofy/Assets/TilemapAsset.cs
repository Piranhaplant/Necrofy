using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Necrofy
{
    class TilemapAsset : Asset
    {
        public enum Type
        {
            Normal,
            Sized,
        }

        public const string DefaultName = "Tilemap";
        public static readonly Dictionary<string, Type> Extensions = new Dictionary<string, Type>() {
            {"tlm", Type.Normal },
            {"tlms", Type.Sized },
        };
        public static readonly Dictionary<Type, string> TypeToExtension = Extensions.Reverse();

        private const AssetCategory AssetCat = AssetCategory.Tilemap;

        static TilemapAsset() {
            AddCreator(new TilemapCreator());
        }

        public static string GetAssetName(NStream romStream, ROMInfo romInfo, int pointer) {
            return GetAssetName(romStream, romInfo, new TilemapCreator(), AssetCat, pointer);
        }

        private readonly TilemapNameInfo tilemapNameInfo;
        public byte[] data;

        public static TilemapAsset FromProject(Project project, string fullName, Type type) {
            ParsedName parsedName = new ParsedName(fullName);
            return new TilemapCreator().FromProject(project, parsedName.Folder, parsedName.FinalName, type);
        }

        private TilemapAsset(TilemapNameInfo nameInfo, byte[] data) : base(nameInfo) {
            this.tilemapNameInfo = nameInfo;
            this.data = data;
        }

        private TilemapAsset(TilemapNameInfo nameInfo, string filename) : base(nameInfo) {
            this.tilemapNameInfo = nameInfo;
            Reload(filename);
        }

        public Type TilemapType => tilemapNameInfo.type;

        protected override void Reload(string filename) {
            data = File.ReadAllBytes(filename);
        }

        protected override void WriteFile(string filename) {
            File.WriteAllBytes(filename, data);
        }
        
        public override void Insert(NStream rom, ROMInfo romInfo, Project project) {
            if (nameInfo.Parts.compressed) {
                InsertCompressedByteArray(rom, romInfo, data, nameInfo.GetFilename(project.path), nameInfo.Parts.pointer);
            } else {
                InsertByteArray(rom, romInfo, data, nameInfo.Parts.pointer);
            }
        }
        
        class TilemapCreator : Creator
        {
            public TilemapAsset FromProject(Project project, string folder, string tilemapName, Type type) {
                NameInfo nameInfo = new TilemapNameInfo(folder, tilemapName, type: type);
                return project.GetCachedAsset(nameInfo, () => {
                    string filename = nameInfo.FindFilename(project.path);
                    return (TilemapAsset)FromFile(nameInfo, filename);
                });
            }

            public override NameInfo GetNameInfo(PathParts pathParts, Project project) {
                return TilemapNameInfo.FromPath(pathParts);
            }

            public override Asset FromFile(NameInfo nameInfo, string filename) {
                return new TilemapAsset((TilemapNameInfo)nameInfo, filename);
            }

            public override List<DefaultParams> GetDefaults() {
                return new List<DefaultParams>() {
                    new DefaultParams(0xb5641, new TilemapNameInfo(LevelTitleFolder, DefaultName, 0xb5641), 0x1200, extractFromNecrofyROM: true, options: new AssetOptions.TilemapOptions(128, true, false, Hinting.Type.LevelTitle), reserved: true),
                    new DefaultParams(0xf5700, new TilemapNameInfo(TitleScreenFolder, DefaultName, 0xf5700), 0x780, extractFromNecrofyROM: true, options: new AssetOptions.TilemapOptions(32, true, false)),

                    new DefaultParams(0xd4000, new TilemapNameInfo(GetTilesetFolder(Castle), DefaultName, compressed: true), options: new AssetOptions.TilemapOptions(8, false, false, Hinting.Type.Tileset)),
                    new DefaultParams(0xd8000, new TilemapNameInfo(GetTilesetFolder(Grass), DefaultName, compressed: true), options: new AssetOptions.TilemapOptions(8, false, false, Hinting.Type.Tileset)),
                    new DefaultParams(0xdbcb5, new TilemapNameInfo(GetTilesetFolder(Sand), DefaultName, compressed: true), options: new AssetOptions.TilemapOptions(8, false, false, Hinting.Type.Tileset)),
                    new DefaultParams(0xe0000, new TilemapNameInfo(GetTilesetFolder(Office), DefaultName, compressed: true), options: new AssetOptions.TilemapOptions(8, false, false, Hinting.Type.Tileset)),
                    new DefaultParams(0xe36ef, new TilemapNameInfo(GetTilesetFolder(Mall), DefaultName, compressed: true), options: new AssetOptions.TilemapOptions(8, false, false, Hinting.Type.Tileset)),

                    new DefaultParams(0, new TilemapNameInfo(ScratchPadFolder, DefaultName, skipped: true), extractFromNecrofyROM: true, versionAdded: new Version(2, 0)),
                };
            }

            public override Asset FromRom(NameInfo nameInfo, NStream romStream, ROMInfo romInfo, int? size, out bool trackFreespace) {
                TilemapNameInfo tilemapNameInfo = (TilemapNameInfo)nameInfo;
                trackFreespace = tilemapNameInfo.Parts.pointer == null;
                if (romStream.Position == 0) {
                    // Special pointer used for scratch pad asset
                    trackFreespace = false;
                    return new TilemapAsset(tilemapNameInfo, new byte[0x800]);
                } else if (tilemapNameInfo.Parts.compressed) {
                    if (trackFreespace) {
                        romStream.PushPosition();
                        ZAMNCompress.AddToFreespace(romStream, romInfo.Freespace);
                        romStream.PopPosition();
                        trackFreespace = false;
                    }
                    return new TilemapAsset(tilemapNameInfo, ZAMNCompress.Decompress(romStream));
                } else {
                    if (tilemapNameInfo.type == Type.Sized) {
                        ushort width = romStream.ReadInt16();
                        ushort height = romStream.ReadInt16();
                        romStream.Seek(-4, SeekOrigin.Current);
                        size = width * height * 2 + 4;
                    }
                    return new TilemapAsset(tilemapNameInfo, romStream.ReadBytes((int)size));
                }
            }

            public override NameInfo GetNameInfoForName(string name, string group) {
                return new TilemapNameInfo(GetTilesetFolder(group), name, null, compressed: true);
            }

            public override NameInfo GetNameInfoForExtraction(ExtractionPreset preset) {
                if (preset.Type == ExtractionPreset.AssetType.Tilemap || preset.Type == ExtractionPreset.AssetType.TilemapSized) {
                    Type tilemapType = preset.Type == ExtractionPreset.AssetType.Tilemap ? Type.Normal : Type.Sized;
                    return new TilemapNameInfo(preset.Category, preset.Filename, preset.Address, tilemapType, preset.Compressed);
                }
                return null;
            }
        }

        class TilemapNameInfo : NameInfo
        {
            public Type type { get; private set; }

            private TilemapNameInfo(PathParts parts) : base(parts) {
                type = Extensions[parts.fileExtension];
            }
            public TilemapNameInfo(string folder, string name, int? pointer = null, Type type = Type.Normal, bool compressed = false, bool skipped = false)
                : this(new PathParts(folder, name, TypeToExtension[type], pointer, compressed, skipped)) { }
            
            public override AssetCategory Category => AssetCat;

            public override bool Editable => true;
            public override bool CanRename => true;
            public override EditorWindow GetEditor(Project project) {
                return new TilemapEditor(new LoadedTilemap(project, Name, type), project);
            }

            public static TilemapNameInfo FromPath(PathParts parts) {
                if (parts.fileExtension == null || !Extensions.ContainsKey(parts.fileExtension)) return null;
                return new TilemapNameInfo(parts);
            }
        }
    }
}
