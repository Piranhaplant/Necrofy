using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Necrofy
{
    class DataAsset : Asset
    {
        public const string LevelTitleCharacterMapName = "Character Map";

        private const AssetCategory AssetCat = AssetCategory.Data;

        static DataAsset() {
            AddCreator(new DataCreator());
        }

        private readonly DataNameInfo nameInfo;
        public readonly byte[] data;

        public static DataAsset FromProject(Project project, string folder, string dataName) {
            return new DataCreator().FromProject(project, folder, dataName);
        }

        private DataAsset(DataNameInfo nameInfo, byte[] data) {
            this.nameInfo = nameInfo;
            this.data = data;
        }

        public override void WriteFile(Project project) {
            File.WriteAllBytes(nameInfo.GetFilename(project.path, createDirectories: true), data);
        }

        public override void Insert(NStream rom, ROMInfo romInfo, Project project) {
            InsertByteArray(rom, romInfo, data, nameInfo.pointer);
        }

        protected override AssetCategory Category => nameInfo.Category;
        protected override string Name => nameInfo.Name;

        class DataCreator : Creator
        {
            public DataAsset FromProject(Project project, string folder, string name) {
                NameInfo nameInfo = new DataNameInfo(folder, name, null);
                string filename = nameInfo.FindFilename(project.path);
                return (DataAsset)FromFile(nameInfo, filename);
            }

            public override NameInfo GetNameInfo(NameInfo.PathParts pathParts, Project project) {
                return DataNameInfo.FromPath(pathParts);
            }

            public override Asset FromFile(NameInfo nameInfo, string filename) {
                return new DataAsset((DataNameInfo)nameInfo, File.ReadAllBytes(filename));
            }

            public override List<DefaultParams> GetDefaults() {
                return new List<DefaultParams>() {
                    new DefaultParams(0x12f37, new DataNameInfo(LevelTitleFolder, LevelTitleCharacterMapName, 0x12f37), 0xbc),
                };
            }

            public override Asset FromRom(NameInfo nameInfo, NStream romStream, ROMInfo romInfo, int? size, out bool trackFreespace) {
                DataNameInfo dataNameInfo = (DataNameInfo)nameInfo;
                trackFreespace = dataNameInfo.pointer == null;
                return new DataAsset(dataNameInfo, romStream.ReadBytes((int)size));
            }
        }

        class DataNameInfo : NameInfo
        {
            private const string Extension = "bin";

            public readonly string folder;
            public readonly string name;
            public readonly int? pointer;
            
            public DataNameInfo(string folder, string name, int? pointer) : base(folder, name) {
                this.folder = folder;
                this.name = name;
                this.pointer = pointer;
            }

            public override string DisplayName => name;
            public override AssetCategory Category => AssetCat;

            protected override PathParts GetPathParts() {
                return new PathParts(folder, name, Extension, pointer, false);
            }

            public static DataNameInfo FromPath(PathParts parts) {
                if (parts.fileExtension != Extension) return null;
                return new DataNameInfo(parts.folder, parts.name, parts.pointer);
            }
        }
    }
}
