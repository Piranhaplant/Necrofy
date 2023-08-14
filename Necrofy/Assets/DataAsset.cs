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

        public byte[] data;

        public static DataAsset FromProject(Project project, string folder, string dataName) {
            return new DataCreator().FromProject(project, folder, dataName);
        }

        private DataAsset(DataNameInfo nameInfo, byte[] data) : base(nameInfo) {
            this.data = data;
        }

        private DataAsset(DataNameInfo nameInfo, string filename) : base(nameInfo) {
            Reload(filename);
        }

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
        
        class DataCreator : Creator
        {
            public DataAsset FromProject(Project project, string folder, string name) {
                NameInfo nameInfo = new DataNameInfo(folder, name, null);
                return project.GetCachedAsset(nameInfo, () => {
                    string filename = nameInfo.FindFilename(project.path);
                    return (DataAsset)FromFile(nameInfo, filename);
                });
            }

            public override NameInfo GetNameInfo(PathParts pathParts, Project project) {
                return DataNameInfo.FromPath(pathParts);
            }

            public override Asset FromFile(NameInfo nameInfo, string filename) {
                return new DataAsset((DataNameInfo)nameInfo, filename);
            }

            public override List<DefaultParams> GetDefaults() {
                return new List<DefaultParams>() {
                    new DefaultParams(0x12f37, new DataNameInfo(LevelTitleFolder, LevelTitleCharacterMapName, 0x12f37), 0xbc, extractFromNecrofyROM: true, reserved: true),
                };
            }

            public override Asset FromRom(NameInfo nameInfo, NStream romStream, ROMInfo romInfo, int? size, out bool trackFreespace) {
                DataNameInfo dataNameInfo = (DataNameInfo)nameInfo;
                trackFreespace = dataNameInfo.Parts.pointer == null;
                if (nameInfo.Parts.compressed) {
                    if (trackFreespace) {
                        romStream.PushPosition();
                        ZAMNCompress.AddToFreespace(romStream, romInfo.Freespace);
                        romStream.PopPosition();
                        trackFreespace = false;
                    }
                    return new DataAsset(dataNameInfo, ZAMNCompress.Decompress(romStream));
                } else {
                    return new DataAsset(dataNameInfo, romStream.ReadBytes((int)size));
                }
            }

            public override NameInfo GetNameInfoForExtraction(ExtractionPreset preset) {
                if (preset.Type == ExtractionPreset.AssetType.Binary) {
                    return new DataNameInfo(preset.Category, preset.Filename, preset.Address, preset.Compressed);
                }
                return null;
            }
        }

        class DataNameInfo : NameInfo
        {
            private const string Extension = "bin";
            
            private DataNameInfo(PathParts parts) : base(parts) { }
            public DataNameInfo(string folder, string name, int? pointer, bool compressed = false) : this(new PathParts(folder, name, Extension, pointer, compressed)) { }
            
            public override AssetCategory Category => AssetCat;

            public override bool CanRename => true;

            public static DataNameInfo FromPath(PathParts parts) {
                if (parts.fileExtension != Extension) return null;
                return new DataNameInfo(parts);
            }
        }
    }
}
