﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Necrofy
{
    class EditorAsset<T> : Asset
    {
        private const string Extension = "json";
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

        public override void WriteFile(Project project) {
            File.WriteAllText(nameInfo.GetFilename(project.path), JsonConvert.SerializeObject(data));
        }

        public override void Insert(NStream rom, ROMInfo romInfo) { }

        protected override AssetCategory Category => nameInfo.Category;
        protected override string Name => nameInfo.Name;

        class EditorCreator : Creator
        {
            public EditorAsset<T> FromProject(Project project, string name) {
                NameInfo nameInfo = new EditorNameInfo(name);
                string filename = nameInfo.GetFilename(project.path);
                if (!File.Exists(filename)) {
                    filename = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Tools", name + "." + Extension);
                }
                return (EditorAsset<T>)FromFile(nameInfo, filename);
            }

            public override NameInfo GetNameInfo(NameInfo.PathParts pathParts, Project project) {
                return EditorNameInfo.FromPath(pathParts);
            }

            public override Asset FromFile(NameInfo nameInfo, string filename) {
                return new EditorAsset<T>((EditorNameInfo)nameInfo, JsonConvert.DeserializeObject<T>(File.ReadAllText(filename)));
            }
        }

        class EditorNameInfo : NameInfo
        {
            private const string Folder = "Editor";

            public readonly string name;

            public EditorNameInfo(string name) {
                this.name = name;
            }

            public override string Name => name;
            public override string DisplayName => name;
            public override AssetCategory Category => AssetCat;

            protected override PathParts GetPathParts() {
                return new PathParts(Folder, null, name, Extension, null);
            }

            public static EditorNameInfo FromPath(PathParts parts) {
                if (parts.topFolder != Folder) return null;
                if (parts.subFolder != null) return null;
                if (parts.fileExtension != Extension) return null;
                if (parts.pointer != null) return null;
                return new EditorNameInfo(parts.name);
            }
        }
    }
}