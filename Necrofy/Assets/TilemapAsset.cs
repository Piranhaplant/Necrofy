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
        public const string DefaultName = "Tilemap";

        private const AssetCategory AssetCat = AssetCategory.Tilemap;

        static TilemapAsset() {
            AddCreator(new TilemapCreator());
        }

        public static string GetAssetName(NStream romStream, ROMInfo romInfo, int pointer) {
            return GetAssetName(romStream, romInfo, new TilemapCreator(), AssetCat, pointer);
        }

        public readonly byte[] data;

        public static TilemapAsset FromProject(Project project, string fullName) {
            ParsedName parsedName = new ParsedName(fullName);
            return new TilemapCreator().FromProject(project, parsedName.Folder, parsedName.FinalName);
        }

        private TilemapAsset(TilemapNameInfo nameInfo, byte[] data) : base(nameInfo) {
            this.data = data;
        }

        protected override void WriteFile(Project project) {
            File.WriteAllBytes(nameInfo.GetFilename(project.path, createDirectories: true), data);
        }
        
        public override void Insert(NStream rom, ROMInfo romInfo, Project project) {
            if (nameInfo.Parts.compressed) {
                InsertCompressedByteArray(rom, romInfo, data, nameInfo.GetFilename(project.path), nameInfo.Parts.pointer);
            } else {
                InsertByteArray(rom, romInfo, data, nameInfo.Parts.pointer);
            }
        }

        protected override AssetCategory Category => nameInfo.Category;
        protected override string Name => nameInfo.Name;

        class TilemapCreator : Creator
        {
            public TilemapAsset FromProject(Project project, string folder, string tilemapName) {
                NameInfo nameInfo = new TilemapNameInfo(folder, tilemapName, null);
                string filename = nameInfo.FindFilename(project.path);
                return (TilemapAsset)FromFile(nameInfo, filename);
            }

            public override NameInfo GetNameInfo(NameInfo.PathParts pathParts, Project project) {
                return TilemapNameInfo.FromPath(pathParts);
            }

            public override Asset FromFile(NameInfo nameInfo, string filename) {
                return new TilemapAsset((TilemapNameInfo)nameInfo, File.ReadAllBytes(filename));
            }

            public override List<DefaultParams> GetDefaults() {
                return new List<DefaultParams>() {
                    new DefaultParams(0xb5641, new TilemapNameInfo(LevelTitleFolder, DefaultName, 0xb5641), 0x1200, extractFromNecrofyROM: true, options: new AssetOptions.TilemapOptions(128, true, Hinting.Type.LevelTitle)),
                    new DefaultParams(0xf5700, new TilemapNameInfo(TitleScreenFolder, DefaultName, 0xf5700), 0x800, extractFromNecrofyROM: true, options: new AssetOptions.TilemapOptions(32, true)),

                    new DefaultParams(0xd4000, new TilemapNameInfo(GetTilesetFolder(Castle), DefaultName, compressed: true), options: new AssetOptions.TilemapOptions(8, false, Hinting.Type.Tileset)),
                    new DefaultParams(0xd8000, new TilemapNameInfo(GetTilesetFolder(Grass), DefaultName, compressed: true), options: new AssetOptions.TilemapOptions(8, false, Hinting.Type.Tileset)),
                    new DefaultParams(0xdbcb5, new TilemapNameInfo(GetTilesetFolder(Sand), DefaultName, compressed: true), options: new AssetOptions.TilemapOptions(8, false, Hinting.Type.Tileset)),
                    new DefaultParams(0xe0000, new TilemapNameInfo(GetTilesetFolder(Office), DefaultName, compressed: true), options: new AssetOptions.TilemapOptions(8, false, Hinting.Type.Tileset)),
                    new DefaultParams(0xe36ef, new TilemapNameInfo(GetTilesetFolder(Mall), DefaultName, compressed: true), options: new AssetOptions.TilemapOptions(8, false, Hinting.Type.Tileset)),

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
                    return new TilemapAsset(tilemapNameInfo, romStream.ReadBytes((int)size));
                }
            }

            public override NameInfo GetNameInfoForName(string name, string group) {
                return new TilemapNameInfo(GetTilesetFolder(group), name, null, compressed: true);
            }
        }

        class TilemapNameInfo : NameInfo
        {
            private const string Extension = "tlm";
            
            private TilemapNameInfo(PathParts parts) : base(parts) { }
            public TilemapNameInfo(string folder, string name, int? pointer = null, bool compressed = false, bool skipped = false) : this(new PathParts(folder, name, Extension, pointer, compressed, skipped)) { }
            
            public override AssetCategory Category => AssetCat;

            public override bool Editable => true;

            public override EditorWindow GetEditor(Project project) {
                return new TilemapEditor(new LoadedTilemap(project, Name), project);
            }

            public static TilemapNameInfo FromPath(PathParts parts) {
                if (parts.fileExtension != Extension) return null;
                return new TilemapNameInfo(parts);
            }
        }
    }
}
