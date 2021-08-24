using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Necrofy
{
    class EditorAsset<T> : Asset
    {
        private const AssetCategory AssetCat = AssetCategory.Editor;
        
        private readonly EditorNameInfo nameInfo;
        public readonly T data;
        
        public static EditorAsset<T> FromProject(Project project, string name) {
            return new EditorCreator().FromProject(project, name);
        }

        private EditorAsset(EditorNameInfo nameInfo, T data) {
            this.nameInfo = nameInfo;
            this.data = data;
        }

        protected override void WriteFile(Project project) {
            File.WriteAllText(nameInfo.GetFilename(project.path, createDirectories: true), JsonConvert.SerializeObject(data));
        }

        public override void Insert(NStream rom, ROMInfo romInfo, Project project) { }

        protected override AssetCategory Category => nameInfo.Category;
        protected override string Name => nameInfo.Name;

        class EditorCreator : Creator
        {
            public EditorAsset<T> FromProject(Project project, string name) {
                NameInfo nameInfo = new EditorNameInfo(name);
                string filename = nameInfo.GetFilename(project.path);
                if (!File.Exists(filename)) {
                    filename = nameInfo.GetFilename(Project.internalProjectFilesPath);
                }
                return (EditorAsset<T>)FromFile(nameInfo, filename);
            }

            public override NameInfo GetNameInfo(NameInfo.PathParts pathParts, Project project) {
                throw new NotImplementedException();
            }

            public override Asset FromFile(NameInfo nameInfo, string filename) {
                return new EditorAsset<T>((EditorNameInfo)nameInfo, JsonConvert.DeserializeObject<T>(File.ReadAllText(filename)));
            }
        }

        class EditorNameInfo : NameInfo
        {
            private const string Folder = "Editor";
            private const string Extension = "json";

            public readonly string name;

            public EditorNameInfo(string name) : base(name) {
                this.name = name;
            }

            public override string DisplayName => name;
            public override AssetCategory Category => AssetCat;

            protected override PathParts GetPathParts() {
                return new PathParts(Folder, name, Extension, null, false);
            }
        }
    }
}
