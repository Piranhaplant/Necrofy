using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Necrofy
{
    class CollisionAsset : Asset
    {
        public const string DefaultName = "Collision";
        private const AssetCategory AssetCat = AssetCategory.Collision;
        
        static CollisionAsset() {
            AddCreator(new CollisionCreator());
        }

        public static string GetAssetName(NStream romStream, ROMInfo romInfo, int pointer, string tileset) {
            return GetAssetName(romStream, romInfo, new CollisionCreator(), AssetCat, pointer, tileset);
        }

        public byte[] data;

        public static CollisionAsset FromProject(Project project, string fullName) {
            ParsedName parsedName = new ParsedName(fullName);
            return new CollisionCreator().FromProject(project, parsedName.Folder, parsedName.FinalName);
        }

        private CollisionAsset(CollisionNameInfo nameInfo, byte[] data) : base(nameInfo) {
            this.data = data;
        }

        private CollisionAsset(CollisionNameInfo nameInfo, string filename) : base(nameInfo, filename) { }

        protected override void Reload(string filename) {
            data = File.ReadAllBytes(filename);
        }

        protected override void WriteFile(string filename) {
            File.WriteAllBytes(filename, data);
        }
        
        public override void Insert(NStream rom, ROMInfo romInfo, Project project) {
            InsertByteArray(rom, romInfo, data);
        }
        
        class CollisionCreator : Creator
        {
            public CollisionAsset FromProject(Project project, string folder, string CollisionName) {
                NameInfo nameInfo = new CollisionNameInfo(folder, CollisionName);
                return project.GetCachedAsset(nameInfo, () => {
                    string filename = nameInfo.FindFilename(project.path);
                    return (CollisionAsset)FromFile(nameInfo, filename);
                });
            }

            public override NameInfo GetNameInfo(NameInfo.PathParts pathParts, Project project) {
                return CollisionNameInfo.FromPath(pathParts);
            }

            public override Asset FromFile(NameInfo nameInfo, string filename) {
                return new CollisionAsset((CollisionNameInfo)nameInfo, filename);
            }

            public override List<DefaultParams> GetDefaults() {
                return new List<DefaultParams>() {
                    new DefaultParams(0xe6aab, new CollisionNameInfo(GetTilesetFolder(Castle), DefaultName)),
                    new DefaultParams(0xdf4d1, new CollisionNameInfo(GetTilesetFolder(Grass), DefaultName)),
                    new DefaultParams(0xdf8d1, new CollisionNameInfo(GetTilesetFolder(Sand), DefaultName)),
                    new DefaultParams(0xe72ab, new CollisionNameInfo(GetTilesetFolder(Office), DefaultName)),
                    new DefaultParams(0xe6eab, new CollisionNameInfo(GetTilesetFolder(Mall), DefaultName)),
                };
            }

            public override Asset FromRom(NameInfo nameInfo, NStream romStream, ROMInfo romInfo, int? size, out bool trackFreespace) {
                trackFreespace = true;
                return new CollisionAsset((CollisionNameInfo)nameInfo, romStream.ReadBytes(0x400));
            }

            public override NameInfo GetNameInfoForName(string name, string group) {
                return new CollisionNameInfo(GetTilesetFolder(group), name);
            }
        }

        class CollisionNameInfo : NameInfo
        {
            private const string Extension = "col";
            
            private CollisionNameInfo(PathParts parts) : base(parts) { }
            public CollisionNameInfo(string folder, string name) : this(new PathParts(folder, name, Extension, null, false)) { }
            
            public override AssetCategory Category => AssetCat;
            
            public static CollisionNameInfo FromPath(PathParts parts) {
                if (parts.fileExtension != Extension) return null;
                if (parts.pointer != null) return null;
                if (parts.compressed) return null;
                return new CollisionNameInfo(parts);
            }
        }
    }
}
