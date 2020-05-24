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
        public const string LevelTitleCharsName = "Level Title Chars";

        private const AssetCategory AssetCat = AssetCategory.Data;

        public static void RegisterLoader() {
            AddCreator(new DataCreator());
        }

        public static string GetAssetName(NStream romStream, ROMInfo romInfo, int pointer) {
            return GetAssetName(romStream, romInfo, pointer, new DataCreator(), AssetCat);
        }

        private readonly DataNameInfo nameInfo;
        public readonly byte[] data;

        public static DataAsset FromProject(Project project, string dataName) {
            return new DataCreator().FromProject(project, dataName);
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
            public DataAsset FromProject(Project project, string dataName) {
                NameInfo nameInfo = new DataNameInfo(dataName, null);
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
                    new DefaultParams(0x12f37, new DataNameInfo(LevelTitleCharsName, 0x12f37), 0xbc),
                };
            }

            public override Asset FromRom(NameInfo nameInfo, NStream romStream, int? size) {
                return new DataAsset((DataNameInfo)nameInfo, romStream.ReadBytes((int)size));
            }

            public override NameInfo GetNameInfoForName(string name) {
                return new DataNameInfo(name, null);
            }

            public override bool AutoTrackFreespace => false;
        }

        class DataNameInfo : NameInfo
        {
            private const string Folder = "Data";
            private const string Extension = "bin";

            public readonly string name;
            public readonly int? pointer;

            public DataNameInfo(string name, int? pointer) {
                this.name = name;
                this.pointer = pointer;
            }

            public override string Name => name;
            public override string DisplayName => name;
            public override AssetCategory Category => AssetCat;

            protected override PathParts GetPathParts() {
                return new PathParts(Folder, null, name, Extension, pointer, false);
            }

            public static DataNameInfo FromPath(PathParts parts) {
                if (parts.topFolder != Folder) return null;
                if (parts.subFolder != null) return null;
                if (parts.fileExtension != Extension) return null;
                return new DataNameInfo(parts.name, parts.pointer);
            }
        }
    }
}
