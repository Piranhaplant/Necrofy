using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Necrofy
{
    class TilesetGraphicsAsset : TilesetAsset
    {
        private const AssetCategory AssetCat = AssetCategory.TilesetGraphics;

        public static void RegisterLoader() {
            AddCreator(new TilesetGraphicsCreator());
        }

        public static string GetAssetName(NStream romStream, ROMInfo romInfo, int pointer) {
            return GetAssetName(romStream, romInfo, pointer, new TilesetGraphicsCreator(), AssetCat);
        }

        private TilesetFixedNameInfo nameInfo;
        public byte[] data;

        public static TilesetGraphicsAsset FromProject(Project project, string tilesetName) {
            return new TilesetGraphicsCreator().FromProject(project, tilesetName);
        }

        private TilesetGraphicsAsset(TilesetFixedNameInfo nameInfo, byte[] data) {
            this.nameInfo = nameInfo;
            this.data = data;
        }

        public override void WriteFile(Project project) {
            File.WriteAllBytes(nameInfo.GetFilename(project.path, createDirectories: true), data);
        }

        public override void Insert(NStream rom, ROMInfo romInfo, Project project) {
            InsertByteArray(rom, romInfo, data);
        }

        protected override AssetCategory Category => nameInfo.Category;
        protected override string Name => nameInfo.Name;

        class TilesetGraphicsCreator : Creator
        {
            public TilesetGraphicsAsset FromProject(Project project, string tilesetName) {
                NameInfo nameInfo = new TilesetGraphicsNameInfo(tilesetName);
                string filename = nameInfo.GetFilename(project.path);
                return (TilesetGraphicsAsset)FromFile(nameInfo, filename);
            }

            public override NameInfo GetNameInfo(NameInfo.PathParts pathParts, Project project) {
                return TilesetGraphicsNameInfo.FromPath(pathParts);
            }

            public override Asset FromFile(NameInfo nameInfo, string filename) {
                return new TilesetGraphicsAsset((TilesetFixedNameInfo)nameInfo, File.ReadAllBytes(filename));
            }

            public override List<DefaultParams> GetDefaults() {
                return new List<DefaultParams>() {
                    new DefaultParams(0xc8000, new TilesetGraphicsNameInfo(Castle)),
                    new DefaultParams(0xc0000, new TilesetGraphicsNameInfo(Grass)),
                    new DefaultParams(0xc4000, new TilesetGraphicsNameInfo(Sand)),
                    new DefaultParams(0xd0000, new TilesetGraphicsNameInfo(Office)),
                    new DefaultParams(0xcc000, new TilesetGraphicsNameInfo(Mall))
                };
            }

            public override Asset FromRom(NameInfo nameInfo, NStream romStream, int? size) {
                return new TilesetGraphicsAsset((TilesetFixedNameInfo)nameInfo, romStream.ReadBytes(0x4000));
            }

            public override NameInfo GetNameInfoForName(string name) {
                return new TilesetGraphicsNameInfo(name);
            }
        }

        class TilesetGraphicsNameInfo : TilesetFixedNameInfo
        {
            private const string Filename = "graphics";
            private const string Extension = "bin";

            public TilesetGraphicsNameInfo(string tilesetName) : base(tilesetName, Filename, Extension) { }

            public override AssetCategory Category => AssetCat;

            public static TilesetFixedNameInfo FromPath(PathParts parts) {
                return FromPath(parts, Filename, Extension, s => new TilesetGraphicsNameInfo(s));
            }
        }
    }
}
