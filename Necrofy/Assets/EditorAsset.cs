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
        
        public T data;
        
        public static EditorAsset<T> FromProject(Project project, string name) {
            return new EditorCreator().FromProject(project, name);
        }

        private EditorAsset(EditorNameInfo nameInfo, string filename) : base(nameInfo, filename) { }

        protected override void Reload(string filename) {
            data = JsonConvert.DeserializeObject<T>(File.ReadAllText(filename));
        }

        protected override void WriteFile(string filename) {
            File.WriteAllText(filename, JsonConvert.SerializeObject(data));
        }

        public override void Insert(NStream rom, ROMInfo romInfo, Project project) { }
        
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
                return new EditorAsset<T>((EditorNameInfo)nameInfo, filename);
            }
        }

        class EditorNameInfo : NameInfo
        {
            private const string Folder = "Editor";
            private const string Extension = "json";
            
            public EditorNameInfo(string name) : base(new PathParts(Folder, name, Extension, null, false)) { }
            
            public override AssetCategory Category => AssetCat;
        }
    }
}
