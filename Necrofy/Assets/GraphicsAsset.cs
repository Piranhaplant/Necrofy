using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Necrofy
{
    class GraphicsAsset : Asset
    {
        public const string DefaultName = "Graphics";
        public const string Extension = "gfx";

        private const AssetCategory AssetCat = AssetCategory.Graphics;

        static GraphicsAsset() {
            AddCreator(new GraphicsCreator());
        }
        
        public static string GetAssetName(NStream romStream, ROMInfo romInfo, int pointer, string tileset) {
            return GetAssetName(romStream, romInfo, new GraphicsCreator(), AssetCat, pointer, tileset);
        }

        public byte[] data;

        public static GraphicsAsset FromProject(Project project, string fullName) {
            ParsedName parsedName = new ParsedName(fullName);
            return new GraphicsCreator().FromProject(project, parsedName.Folder, parsedName.FinalName);
        }

        private GraphicsAsset(GraphicsNameInfo nameInfo, byte[] data) : base(nameInfo) {
            this.data = data;
        }

        private GraphicsAsset(GraphicsNameInfo nameInfo, string filename) : base(nameInfo, filename) { }

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
        
        class GraphicsCreator : Creator
        {
            public GraphicsAsset FromProject(Project project, string folder, string graphicsName) {
                NameInfo nameInfo = new GraphicsNameInfo(folder, graphicsName, null);
                return project.GetCachedAsset(nameInfo, () => {
                    string filename = nameInfo.FindFilename(project.path);
                    return (GraphicsAsset)FromFile(nameInfo, filename);
                });
            }

            public override NameInfo GetNameInfo(NameInfo.PathParts pathParts, Project project) {
                return GraphicsNameInfo.FromPath(pathParts);
            }

            public override Asset FromFile(NameInfo nameInfo, string filename) {
                return new GraphicsAsset((GraphicsNameInfo)nameInfo, filename);
            }

            public override List<DefaultParams> GetDefaults() {
                return new List<DefaultParams>() {
                    new DefaultParams(0x20000, new GraphicsNameInfo(SpritesFolder, DefaultName, 0x20000), 0x5d300, extractFromNecrofyROM: true, options: new AssetOptions.GraphicsOptions(16, true, true)),
                    new DefaultParams(0x94f80, new GraphicsNameInfo(LevelTitleFolder, DefaultName, 0x94f80, compressed: true), extractFromNecrofyROM: true, options: new AssetOptions.GraphicsOptions(16, true, false)),
                    new DefaultParams(0x90000, new GraphicsNameInfo(TitleScreenFolder, DefaultName, 0x90000), 0x2800, extractFromNecrofyROM: true, options: new AssetOptions.GraphicsOptions(16, true, false)),

                    new DefaultParams(0xc8000, new GraphicsNameInfo(GetTilesetFolder(Castle), DefaultName), 0x4000),
                    new DefaultParams(0xc0000, new GraphicsNameInfo(GetTilesetFolder(Grass), DefaultName), 0x4000),
                    new DefaultParams(0xc4000, new GraphicsNameInfo(GetTilesetFolder(Sand), DefaultName), 0x4000),
                    new DefaultParams(0xd0000, new GraphicsNameInfo(GetTilesetFolder(Office), DefaultName), 0x4000),
                    new DefaultParams(0xcc000, new GraphicsNameInfo(GetTilesetFolder(Mall), DefaultName), 0x4000),

                    new DefaultParams(0, new GraphicsNameInfo(ScratchPadFolder, DefaultName, skipped: true), extractFromNecrofyROM: true, versionAdded: new Version(2, 0), options: new AssetOptions.GraphicsOptions(32, false, false)),
                };
            }

            public override Asset FromRom(NameInfo nameInfo, NStream romStream, ROMInfo romInfo, int? size, out bool trackFreespace) {
                GraphicsNameInfo graphicsNameInfo = (GraphicsNameInfo)nameInfo;
                trackFreespace = graphicsNameInfo.Parts.pointer == null;
                if (romStream.Position == 0) {
                    // Special pointer used for scratch pad asset
                    trackFreespace = false;
                    return new GraphicsAsset(graphicsNameInfo, new byte[0x8000]);
                } else if (graphicsNameInfo.Parts.compressed) {
                    if (trackFreespace) {
                        romStream.PushPosition();
                        ZAMNCompress.AddToFreespace(romStream, romInfo.Freespace);
                        romStream.PopPosition();
                        trackFreespace = false;
                    }
                    return new GraphicsAsset(graphicsNameInfo, ZAMNCompress.Decompress(romStream));
                } else {
                    return new GraphicsAsset(graphicsNameInfo, romStream.ReadBytes(size ?? 0x4000));
                }
            }

            public override NameInfo GetNameInfoForName(string name, string group) {
                return new GraphicsNameInfo(GetTilesetFolder(group), name, null);
            }
        }

        class GraphicsNameInfo : NameInfo
        {
            private GraphicsNameInfo(PathParts parts) : base(parts) { }
            public GraphicsNameInfo(string folder, string name, int? pointer = null, bool compressed = false, bool skipped = false) : this(new PathParts(folder, name, Extension, pointer, compressed, skipped)) { }
            
            public override AssetCategory Category => AssetCat;
            
            public override bool Editable => true;
            public override EditorWindow GetEditor(Project project) {
                return new GraphicsEditor(new LoadedGraphics(project, Name), project);
            }
            
            public static GraphicsNameInfo FromPath(PathParts parts) {
                if (parts.fileExtension != Extension) return null;
                return new GraphicsNameInfo(parts);
            }
        }
    }
}
