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
        
        public static EditorAsset<T> FromProject(Project project, string name, params JsonConverter[] converters) {
            return new EditorCreator().FromProject(project, name, converters);
        }

        private readonly JsonConverter[] converters;

        private EditorAsset(EditorNameInfo nameInfo, string filename, JsonConverter[] converters) : base(nameInfo) {
            this.converters = converters;
            Reload(filename);
        }

        protected override void Reload(string filename) {
            data = JsonConvert.DeserializeObject<T>(File.ReadAllText(filename), converters);
        }

        protected override void WriteFile(string filename) {
            File.WriteAllText(filename, JsonConvert.SerializeObject(data));
        }

        public override void Insert(NStream rom, ROMInfo romInfo, Project project) { }
        
        class EditorCreator : Creator
        {
            public EditorAsset<T> FromProject(Project project, string name, JsonConverter[] converters) {
                NameInfo nameInfo = new EditorNameInfo(name);
                string filename = nameInfo.GetFilename(project.path);
                if (!File.Exists(filename)) {
                    filename = nameInfo.GetFilename(Project.internalProjectFilesPath);
                }
                return new EditorAsset<T>((EditorNameInfo)nameInfo, filename, converters);
            }

            public override NameInfo GetNameInfo(PathParts pathParts, Project project) {
                throw new NotImplementedException();
            }

            public override Asset FromFile(NameInfo nameInfo, string filename) {
                throw new NotImplementedException();
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
