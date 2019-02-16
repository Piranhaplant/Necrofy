using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Necrofy
{
    class TilesetCollisionAsset : TilesetAsset
    {
        private const AssetCategory AssetCat = AssetCategory.Collision;

        public static void RegisterLoader() {
            AddCreator(new TilesetCollisionCreator());
        }

        public static string GetAssetName(NStream romStream, ROMInfo romInfo, int pointer) {
            return GetAssetName(romStream, romInfo, pointer, new TilesetCollisionCreator(), AssetCat);
        }

        private TilesetFixedNameInfo nameInfo;
        public byte[] data;

        public static TilesetCollisionAsset FromProject(Project project, string tilesetName) {
            return new TilesetCollisionCreator().FromProject(project, tilesetName);
        }

        private TilesetCollisionAsset(TilesetFixedNameInfo nameInfo, byte[] data) {
            this.nameInfo = nameInfo;
            this.data = data;
        }

        public override void WriteFile(Project project) {
            File.WriteAllBytes(nameInfo.GetFilename(project.path), data);
        }

        public override void Insert(NStream rom, ROMInfo romInfo) {
            InsertByteArray(rom, romInfo, data);
        }

        protected override AssetCategory Category => nameInfo.Category;
        protected override string Name => nameInfo.Name;

        class TilesetCollisionCreator : Creator
        {
            public TilesetCollisionAsset FromProject(Project project, string tilesetName) {
                NameInfo nameInfo = new TilesetCollisionNameInfo(tilesetName);
                string filename = nameInfo.GetFilename(project.path);
                return (TilesetCollisionAsset)FromFile(nameInfo, filename);
            }

            public override NameInfo GetNameInfo(NameInfo.PathParts pathParts, Project project) {
                return TilesetCollisionNameInfo.FromPath(pathParts);
            }

            public override Asset FromFile(NameInfo nameInfo, string filename) {
                return new TilesetCollisionAsset((TilesetFixedNameInfo)nameInfo, File.ReadAllBytes(filename));
            }

            public override List<DefaultParams> GetDefaults() {
                return new List<DefaultParams>() {
                    new DefaultParams(0xe6aab, new TilesetCollisionNameInfo(Castle)),
                    new DefaultParams(0xdf4d1, new TilesetCollisionNameInfo(Grass)),
                    new DefaultParams(0xdf8d1, new TilesetCollisionNameInfo(Desert)),
                    new DefaultParams(0xe72ab, new TilesetCollisionNameInfo(Office)),
                    new DefaultParams(0xe6eab, new TilesetCollisionNameInfo(Mall))
                };
            }

            public override Asset FromRom(NameInfo nameInfo, NStream romStream, int? size) {
                return new TilesetCollisionAsset((TilesetFixedNameInfo)nameInfo, romStream.ReadBytes(0x400));
            }

            public override NameInfo GetNameInfoForName(string name) {
                return new TilesetCollisionNameInfo(name);
            }
        }

        class TilesetCollisionNameInfo : TilesetFixedNameInfo
        {
            private const string Filename = "collision";
            private const string Extension = "bin";

            public TilesetCollisionNameInfo(string tilesetName) : base(tilesetName, Filename, Extension) { }

            public override AssetCategory Category => AssetCat;

            public static TilesetFixedNameInfo FromPath(PathParts parts) {
                return FromPath(parts, Filename, Extension, s => new TilesetCollisionNameInfo(s));
            }
        }
    }
}
