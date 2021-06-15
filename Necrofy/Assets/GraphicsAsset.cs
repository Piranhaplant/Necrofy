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

        private const AssetCategory AssetCat = AssetCategory.Graphics;

        static GraphicsAsset() {
            AddCreator(new GraphicsCreator());
        }
        
        public static string GetAssetName(NStream romStream, ROMInfo romInfo, int pointer, string tileset) {
            return GetAssetName(romStream, romInfo, new GraphicsCreator(), AssetCat, pointer, tileset);
        }

        private readonly GraphicsNameInfo nameInfo;
        public readonly byte[] data;

        public static GraphicsAsset FromProject(Project project, string fullName) {
            ParsedName parsedName = new ParsedName(fullName);
            return new GraphicsCreator().FromProject(project, parsedName.Folder, parsedName.FinalName);
        }

        private GraphicsAsset(GraphicsNameInfo nameInfo, byte[] data) {
            this.nameInfo = nameInfo;
            this.data = data;
        }

        public override void WriteFile(Project project) {
            File.WriteAllBytes(nameInfo.GetFilename(project.path, createDirectories: true), data);
        }
        
        public override void Insert(NStream rom, ROMInfo romInfo, Project project) {
            if (nameInfo.compressed) {
                InsertCompressedByteArray(rom, romInfo, data, nameInfo.GetFilename(project.path), nameInfo.pointer);
            } else {
                InsertByteArray(rom, romInfo, data, nameInfo.pointer);
            }
        }

        protected override AssetCategory Category => nameInfo.Category;
        protected override string Name => nameInfo.Name;

        class GraphicsCreator : Creator
        {
            public GraphicsAsset FromProject(Project project, string folder, string graphicsName) {
                NameInfo nameInfo = new GraphicsNameInfo(folder, graphicsName, null);
                string filename = nameInfo.FindFilename(project.path);
                return (GraphicsAsset)FromFile(nameInfo, filename);
            }

            public override NameInfo GetNameInfo(NameInfo.PathParts pathParts, Project project) {
                return GraphicsNameInfo.FromPath(pathParts);
            }

            public override Asset FromFile(NameInfo nameInfo, string filename) {
                return new GraphicsAsset((GraphicsNameInfo)nameInfo, File.ReadAllBytes(filename));
            }

            public override List<DefaultParams> GetDefaults() {
                return new List<DefaultParams>() {
                    new DefaultParams(0x20000, new GraphicsNameInfo(SpritesFolder, DefaultName, 0x20000), 0x5d300, extractFromNecrofyROM: true),
                    new DefaultParams(0x94f80, new GraphicsNameInfo(LevelTitleFolder, DefaultName, 0x94f80, compressed: true), extractFromNecrofyROM: true),
                    new DefaultParams(0x90000, new GraphicsNameInfo(TitleScreenFolder, DefaultName, 0x90000), 0x2800, extractFromNecrofyROM: true),

                    new DefaultParams(0xc8000, new GraphicsNameInfo(GetTilesetFolder(Castle), DefaultName), 0x4000),
                    new DefaultParams(0xc0000, new GraphicsNameInfo(GetTilesetFolder(Grass), DefaultName), 0x4000),
                    new DefaultParams(0xc4000, new GraphicsNameInfo(GetTilesetFolder(Sand), DefaultName), 0x4000),
                    new DefaultParams(0xd0000, new GraphicsNameInfo(GetTilesetFolder(Office), DefaultName), 0x4000),
                    new DefaultParams(0xcc000, new GraphicsNameInfo(GetTilesetFolder(Mall), DefaultName), 0x4000),
                };
            }

            public override Asset FromRom(NameInfo nameInfo, NStream romStream, ROMInfo romInfo, int? size, out bool trackFreespace) {
                GraphicsNameInfo graphicsNameInfo = (GraphicsNameInfo)nameInfo;
                trackFreespace = graphicsNameInfo.pointer == null;
                if (graphicsNameInfo.compressed) {
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
            private const string Extension = "gfx";

            public readonly string folder;
            public readonly string name;
            public readonly int? pointer;
            public readonly bool compressed;

            public GraphicsNameInfo(string folder, string name, int? pointer = null, bool compressed = false) : base(folder, name) {
                this.folder = folder;
                this.name = name;
                this.pointer = pointer;
                this.compressed = compressed;
            }
            
            public override string DisplayName => name;
            public override AssetCategory Category => AssetCat;

            protected override PathParts GetPathParts() {
                return new PathParts(folder, name, Extension, pointer, compressed);
            }

            public static GraphicsNameInfo FromPath(PathParts parts) {
                if (parts.fileExtension != Extension) return null;
                return new GraphicsNameInfo(parts.folder, parts.name, parts.pointer, parts.compressed);
            }
        }
    }
}
