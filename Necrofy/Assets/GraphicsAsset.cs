﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Necrofy
{
    class GraphicsAsset : Asset
    {
        public enum Type
        {
            Normal,
            Sprite,
            TwoBPP,
        }

        public const string DefaultName = "Graphics";
        public const string ExtraSpriteGraphicsName = "GraphicsExtra";
        public static readonly string SpriteGraphics = SpritesFolder + FolderSeparator + DefaultName;
        public static readonly string ExtraSpriteGraphics = SpritesFolder + FolderSeparator + ExtraSpriteGraphicsName;
        public static readonly Dictionary<string, Type> Extensions = new Dictionary<string, Type>() {
            {"gfx", Type.Normal },
            {"gfxs", Type.Sprite },
            {"gfx2", Type.TwoBPP },
        };
        public static readonly Dictionary<Type, string> TypeToExtension = Extensions.Reverse();

        private const AssetCategory AssetCat = AssetCategory.Graphics;

        static GraphicsAsset() {
            AddCreator(new GraphicsCreator());
        }
        
        public static string GetAssetName(NStream romStream, ROMInfo romInfo, int pointer, string tileset) {
            return GetAssetName(romStream, romInfo, new GraphicsCreator(), AssetCat, pointer, tileset);
        }

        public static Type? GetGraphicsType(NameInfo nameInfo) {
            return (nameInfo as GraphicsNameInfo)?.type;
        }

        private readonly GraphicsNameInfo graphicsNameInfo;
        public byte[] data;

        public static GraphicsAsset FromProject(Project project, string fullName, Type type) {
            ParsedName parsedName = new ParsedName(fullName);
            return new GraphicsCreator().FromProject(project, parsedName.Folder, parsedName.FinalName, type);
        }

        private GraphicsAsset(GraphicsNameInfo nameInfo, byte[] data) : base(nameInfo) {
            this.graphicsNameInfo = nameInfo;
            this.data = data;
        }

        private GraphicsAsset(GraphicsNameInfo nameInfo, string filename) : base(nameInfo) {
            this.graphicsNameInfo = nameInfo;
            Reload(filename);
        }

        public Type GraphicsType => graphicsNameInfo.type;

        protected override void Reload(string filename) {
            data = File.ReadAllBytes(filename);
        }

        protected override void WriteFile(string filename) {
            File.WriteAllBytes(filename, data);
        }

        public override void ReserveSpace(ROMInfo romInfo) {
            if (nameInfo.Parts.folder == SpritesFolder && nameInfo.Parts.name == DefaultName) {
                romInfo.ExtraSpriteGraphicsStartIndex = data.Length / 0x80;
            } else if (graphicsNameInfo.type == Type.Sprite) {
                romInfo.ExtraSpriteGraphicsSize += data.Length;
                romInfo.ExtraSpriteGraphicsBasePointer = 0; // Clear this value so it can be reset when the graphics are inserted
                if (nameInfo.Name == ExtraSpriteGraphics) {
                    romInfo.ExtraSpriteGraphicsFirstSize = data.Length;
                }
            }
        }

        public override void Insert(NStream rom, ROMInfo romInfo, Project project) {
            if (graphicsNameInfo.type == Type.Sprite) {
                if (romInfo.ExtraSpriteGraphicsBasePointer == 0) {
                    // TODO: Extra sprite graphics could exceed one bank
                    romInfo.ExtraSpriteGraphicsBasePointer = romInfo.Freespace.Claim(romInfo.ExtraSpriteGraphicsSize, alignment: 0x80);
                    romInfo.ExtraSpriteGraphicsCurrentPointer = romInfo.ExtraSpriteGraphicsBasePointer;
                    if (romInfo.ExtraSpriteGraphicsFirstSize > 0) {
                        romInfo.ExtraSpriteGraphicsCurrentPointer += romInfo.ExtraSpriteGraphicsFirstSize;
                    }
                }
                if (nameInfo.Name == ExtraSpriteGraphics) {
                    InsertByteArray(rom, romInfo, data, romInfo.ExtraSpriteGraphicsBasePointer);
                } else {
                    InsertByteArray(rom, romInfo, data, romInfo.ExtraSpriteGraphicsCurrentPointer);
                    romInfo.ExtraSpriteGraphicsCurrentPointer += data.Length;
                }
            } else if (nameInfo.Parts.compressed) {
                InsertCompressedByteArray(rom, romInfo, data, nameInfo.GetFilename(project.path), nameInfo.Parts.pointer);
            } else {
                InsertByteArray(rom, romInfo, data, nameInfo.Parts.pointer);
            }
        }
        
        class GraphicsCreator : Creator
        {
            public GraphicsAsset FromProject(Project project, string folder, string graphicsName, Type type) {
                NameInfo nameInfo = new GraphicsNameInfo(folder, graphicsName, type: type);
                return project.GetCachedAsset(nameInfo, () => {
                    string filename = nameInfo.FindFilename(project.path);
                    return (GraphicsAsset)FromFile(nameInfo, filename);
                });
            }

            public override NameInfo GetNameInfo(PathParts pathParts, Project project) {
                return GraphicsNameInfo.FromPath(pathParts);
            }

            public override Asset FromFile(NameInfo nameInfo, string filename) {
                return new GraphicsAsset((GraphicsNameInfo)nameInfo, filename);
            }

            public override List<DefaultParams> GetDefaults() {
                return new List<DefaultParams>() {
                    new DefaultParams(0x20000, new GraphicsNameInfo(SpritesFolder, DefaultName, 0x20000), 0x5d300, extractFromNecrofyROM: true, options: new AssetOptions.GraphicsOptions(16, true, true), reserved: true),
                    new DefaultParams(0x94f80, new GraphicsNameInfo(LevelTitleFolder, DefaultName, 0x94f80, compressed: true), extractFromNecrofyROM: true, options: new AssetOptions.GraphicsOptions(16, true, false), reserved: true),
                    new DefaultParams(0x90000, new GraphicsNameInfo(TitleScreenFolder, DefaultName, 0x90000), 0x2800, extractFromNecrofyROM: true, options: new AssetOptions.GraphicsOptions(16, true, false)),

                    new DefaultParams(0xc8000, new GraphicsNameInfo(GetTilesetFolder(Castle), DefaultName), 0x4000),
                    new DefaultParams(0xc0000, new GraphicsNameInfo(GetTilesetFolder(Grass), DefaultName), 0x4000),
                    new DefaultParams(0xc4000, new GraphicsNameInfo(GetTilesetFolder(Sand), DefaultName), 0x4000),
                    new DefaultParams(0xd0000, new GraphicsNameInfo(GetTilesetFolder(Office), DefaultName), 0x4000),
                    new DefaultParams(0xcc000, new GraphicsNameInfo(GetTilesetFolder(Mall), DefaultName), 0x4000),

                    new DefaultParams(0, new GraphicsNameInfo(ScratchPadFolder, DefaultName, skipped: true), extractFromNecrofyROM: true, versionAdded: new Version(2, 0), options: new AssetOptions.GraphicsOptions(32, false, false)),
                    new DefaultParams(1, new GraphicsNameInfo(SpritesFolder, ExtraSpriteGraphicsName, type: Type.Sprite), extractFromNecrofyROM: true, options: new AssetOptions.GraphicsOptions(16, true, true), reserved: true),
                };
            }

            public override Asset FromRom(NameInfo nameInfo, NStream romStream, ROMInfo romInfo, int? size, out bool trackFreespace) {
                GraphicsNameInfo graphicsNameInfo = (GraphicsNameInfo)nameInfo;
                trackFreespace = graphicsNameInfo.Parts.pointer == null;
                if (romStream.Position == 0) {
                    // Special pointer used for scratch pad asset
                    trackFreespace = false;
                    return new GraphicsAsset(graphicsNameInfo, new byte[0x8000]);
                } else if (romStream.Position == 1) {
                    // Special pointer used for extra sprite graphics in a Necrofy ROM
                    trackFreespace = false;
                    if (romInfo.ExtraSpriteGraphicsBasePointer > 0) {
                        romInfo.Freespace.Add(romInfo.ExtraSpriteGraphicsBasePointer, romInfo.ExtraSpriteGraphicsCurrentPointer);
                        romStream.Seek(romInfo.ExtraSpriteGraphicsBasePointer);
                        return new GraphicsAsset(graphicsNameInfo, romStream.ReadBytes(romInfo.ExtraSpriteGraphicsCurrentPointer - romInfo.ExtraSpriteGraphicsBasePointer));
                    } else {
                        return null;
                    }
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
                return new GraphicsNameInfo(GetTilesetFolder(group), name);
            }

            public override NameInfo GetNameInfoForExtraction(ExtractionPreset preset) {
                if (preset.Type == ExtractionPreset.AssetType.Graphics || preset.Type == ExtractionPreset.AssetType.Graphics2BPP) {
                    Type graphicsType = preset.Type == ExtractionPreset.AssetType.Graphics ? Type.Normal : Type.TwoBPP;
                    return new GraphicsNameInfo(preset.Category, preset.Filename, preset.Address, graphicsType, preset.Compressed);
                }
                return null;
            }
        }

        class GraphicsNameInfo : NameInfo
        {
            public Type type { get; private set; }

            private GraphicsNameInfo(PathParts parts) : base(parts) {
                type = Extensions[parts.fileExtension];
            }
            public GraphicsNameInfo(string folder, string name, int? pointer = null, Type type = Type.Normal, bool compressed = false, bool skipped = false)
                : this(new PathParts(folder, name, TypeToExtension[type], pointer, compressed, skipped)) { }
            
            public override AssetCategory Category => AssetCat;
            
            public override bool Editable => true;
            public override bool CanRename => true;
            public override EditorWindow GetEditor(Project project) {
                return new GraphicsEditor(new LoadedGraphics(project, Name, type), project);
            }

            protected override void RenamedTo(NameInfo newNameInfo) {
                type = ((GraphicsNameInfo)newNameInfo).type;
            }

            public static GraphicsNameInfo FromPath(PathParts parts) {
                if (parts.fileExtension == null || !Extensions.ContainsKey(parts.fileExtension)) return null;
                return new GraphicsNameInfo(parts);
            }
        }
    }
}
