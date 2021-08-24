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

        private readonly PasswordsNameInfo nameInfo;
        public readonly PasswordData data;

        public static PasswordsAsset FromProject(Project project) {
            return new PasswordsCreator().FromProject(project);
        }

        private PasswordsAsset(PasswordsNameInfo nameInfo, PasswordData data) {
            this.nameInfo = nameInfo;
            this.data = data;
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

        protected override AssetCategory Category => nameInfo.Category;
        protected override string Name => nameInfo.Name;

        class PasswordsCreator : Creator
        {
            public PasswordsAsset FromProject(Project project) {
                NameInfo nameInfo = new PasswordsNameInfo();
                string filename = nameInfo.FindFilename(project.path);
                return (PasswordsAsset)FromFile(nameInfo, filename);
            }

            public override NameInfo GetNameInfo(NameInfo.PathParts pathParts, Project project) {
                return PasswordsNameInfo.FromPath(pathParts);
            }

            public override Asset FromFile(NameInfo nameInfo, string filename) {
                return new PasswordsAsset((PasswordsNameInfo)nameInfo, JsonConvert.DeserializeObject<PasswordData>(File.ReadAllText(filename)));
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
            
            public PasswordsNameInfo() : base(Folder, Filename) { }

            public override string DisplayName => Filename;
            public override AssetCategory Category => AssetCat;

            protected override PathParts GetPathParts() {
                return new PathParts(Folder, Filename, Extension, null, false);
            }

            public override bool Editable => true;
            public override EditorWindow GetEditor(Project project) {
                return new PasswordEditor(PasswordsAsset.FromProject(project));
            }

            public static PasswordsNameInfo FromPath(PathParts parts) {
                if (parts.folder != Folder) return null;
                if (parts.name != Filename) return null;
                if (parts.fileExtension != Extension) return null;
                return new PasswordsNameInfo();
            }
        }
    }
}
