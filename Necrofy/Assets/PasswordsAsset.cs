using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Necrofy
{
    class PasswordsAsset : Asset
    {
        private const AssetCategory AssetCat = AssetCategory.Passwords;

        static PasswordsAsset() {
            AddCreator(new PasswordsCreator());
        }

        public PasswordData data;

        public static PasswordsAsset FromProject(Project project) {
            return new PasswordsCreator().FromProject(project);
        }

        private PasswordsAsset(PasswordsNameInfo nameInfo, PasswordData data) : base(nameInfo) {
            this.data = data;
        }

        private PasswordsAsset(PasswordsNameInfo nameInfo, string filename) : base(nameInfo, filename) { }

        protected override void Reload(string filename) {
            data = JsonConvert.DeserializeObject<PasswordData>(File.ReadAllText(filename));
        }

        protected override void WriteFile(Project project) {
            File.WriteAllText(nameInfo.GetFilename(project.path, createDirectories: true), JsonConvert.SerializeObject(data, Formatting.Indented));
        }

        public override void ReserveSpace(Freespace freespace) {
            data.ReserveSpace(freespace);
        }

        public override void Insert(NStream rom, ROMInfo romInfo, Project project) {
            data.WriteToROM(rom, romInfo);
        }
        
        class PasswordsCreator : Creator
        {
            public PasswordsAsset FromProject(Project project) {
                NameInfo nameInfo = new PasswordsNameInfo();
                return project.GetCachedAsset(nameInfo, () => {
                    string filename = nameInfo.FindFilename(project.path);
                    return (PasswordsAsset)FromFile(nameInfo, filename);
                });
            }

            public override NameInfo GetNameInfo(NameInfo.PathParts pathParts, Project project) {
                return PasswordsNameInfo.FromPath(pathParts);
            }

            public override Asset FromFile(NameInfo nameInfo, string filename) {
                return new PasswordsAsset((PasswordsNameInfo)nameInfo, filename);
            }

            public override List<DefaultParams> GetDefaults() {
                return new List<DefaultParams>() {
                    new DefaultParams(ROMPointers.PasswordData, new PasswordsNameInfo(), extractFromNecrofyROM: true),
                };
            }

            public override Asset FromRom(NameInfo nameInfo, NStream romStream, ROMInfo romInfo, int? size, out bool trackFreespace) {
                trackFreespace = false;
                PasswordsNameInfo passwordsNameInfo = (PasswordsNameInfo)nameInfo;
                return new PasswordsAsset(passwordsNameInfo, new PasswordData(romStream, romInfo));
            }
        }

        class PasswordsNameInfo : NameInfo
        {
            private const string Folder = MiscFolder;
            private const string Filename = "Passwords";
            private const string Extension = "json";

            private PasswordsNameInfo(PathParts parts) : base(parts) { }
            public PasswordsNameInfo() : this(new PathParts(Folder, Filename, Extension, null, false)) { }
            
            public override AssetCategory Category => AssetCat;
            
            public override bool Editable => true;
            public override EditorWindow GetEditor(Project project) {
                return new PasswordEditor(PasswordsAsset.FromProject(project));
            }

            public static PasswordsNameInfo FromPath(PathParts parts) {
                if (parts.folder != Folder) return null;
                if (parts.name != Filename) return null;
                if (parts.fileExtension != Extension) return null;
                if (parts.compressed) return null;
                return new PasswordsNameInfo(parts);
            }
        }
    }
}
