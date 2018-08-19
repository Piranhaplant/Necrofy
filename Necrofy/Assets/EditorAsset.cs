using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Necrofy
{
    class EditorAsset : Asset
    {
        private const string Extension = "json";
        private const AssetCategory AssetCat = AssetCategory.Editor;

        public const string SpriteGraphics = "SpriteGraphics";

        public static void RegisterLoader() {
            AddCreator(new EditorCreator());
        }

        private readonly EditorNameInfo nameInfo;
        public readonly string text;

        public static EditorAsset FromProject(Project project, string name) {
            return new EditorCreator().FromProject(project, name);
        }

        private EditorAsset(EditorNameInfo nameInfo, string text) {
            this.nameInfo = nameInfo;
            this.text = text;
        }

        public override void WriteFile(Project project) {
            File.WriteAllText(nameInfo.GetFilename(project.path), text);
        }

        protected override Inserter GetInserter(ROMInfo romInfo) {
            return new EditorInserter();
        }

        protected override AssetCategory Category {
            get { return AssetCat; }
        }

        protected override string Name {
            get { return nameInfo.Name; }
        }

        class EditorCreator : Creator
        {
            public EditorAsset FromProject(Project project, string name) {
                NameInfo nameInfo = new EditorNameInfo(name);
                string filename = nameInfo.GetFilename(project.path);
                if (!File.Exists(filename)) {
                    filename = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Tools", name + "." + Extension);
                }
                return (EditorAsset)FromFile(nameInfo, filename);
            }

            public override NameInfo GetNameInfo(NameInfo.PathParts pathParts) {
                return EditorNameInfo.FromPath(pathParts);
            }

            public override Asset FromFile(NameInfo nameInfo, string filename) {
                return new EditorAsset((EditorNameInfo)nameInfo, File.ReadAllText(filename));
            }

            public override AssetCategory GetCategory() {
                return AssetCat;
            }
        }

        class EditorNameInfo : NameInfo
        {
            private const string Folder = "Editor";

            public readonly string name;

            public EditorNameInfo(string name) {
                this.name = name;
            }

            public override string Name {
                get { return name; }
            }

            public override string DisplayName {
                get { return name; }
            }

            public override System.Drawing.Bitmap DisplayImage {
                get { return Properties.Resources.gear; }
            }

            protected override PathParts GetPathParts() {
                return new PathParts(Folder, null, name, Extension, null);
            }

            public static EditorNameInfo FromPath(NameInfo.PathParts parts) {
                if (parts.topFolder != Folder) return null;
                if (parts.subFolder != null) return null;
                if (parts.fileExtension != Extension) return null;
                if (parts.pointer != null) return null;
                return new EditorNameInfo(parts.name);
            }
        }

        class EditorInserter : Inserter
        {
            public EditorInserter() { }

            public override int GetSize() {
                return 0;
            }

            public override byte[] GetData(int pointer) {
                return new byte[0];
            }
        }
    }
}
