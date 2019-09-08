using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Necrofy
{
    class TilesetSuggestionsAsset : TilesetAsset
    {
        private const AssetCategory AssetCat = AssetCategory.Editor;

        public static void RegisterLoader() {
            AddCreator(new TilesetSuggestionsCreator());
        }
        
        private TilesetFixedNameInfo nameInfo;
        public TilesetSuggestions data;

        public static TilesetSuggestionsAsset FromProject(Project project, string tilesetName) {
            return new TilesetSuggestionsCreator().FromProject(project, tilesetName);
        }

        private TilesetSuggestionsAsset(TilesetFixedNameInfo nameInfo, TilesetSuggestions data) {
            this.nameInfo = nameInfo;
            this.data = data;
        }

        public override void WriteFile(Project project) {
            // TODO: data would need to be converted back to JSON format first
            //File.WriteAllText(nameInfo.GetFilename(project.path), JsonConvert.SerializeObject(data));
        }

        public override void Insert(NStream rom, ROMInfo romInfo) { }

        protected override AssetCategory Category => nameInfo.Category;
        protected override string Name => nameInfo.Name;

        class TilesetSuggestionsCreator : Creator
        {
            public TilesetSuggestionsAsset FromProject(Project project, string tilesetName) {
                NameInfo nameInfo = new TilesetSuggestionsNameInfo(tilesetName);
                string filename = nameInfo.GetFilename(project.path);
                if (!File.Exists(filename)) {
                    filename = nameInfo.GetFilename(Project.internalProjectFilesPath);
                }
                return (TilesetSuggestionsAsset)FromFile(nameInfo, filename);
            }

            public override NameInfo GetNameInfo(NameInfo.PathParts pathParts, Project project) {
                return TilesetSuggestionsNameInfo.FromPath(pathParts);
            }

            public override Asset FromFile(NameInfo nameInfo, string filename) {
                return new TilesetSuggestionsAsset((TilesetFixedNameInfo)nameInfo, new TilesetSuggestions(File.ReadAllText(filename)));
            }
        }

        class TilesetSuggestionsNameInfo : TilesetFixedNameInfo
        {
            private const string Filename = "suggestions";
            private const string Extension = "json";

            public TilesetSuggestionsNameInfo(string tilesetName) : base(tilesetName, Filename, Extension) { }

            public override AssetCategory Category => AssetCat;

            public static TilesetFixedNameInfo FromPath(PathParts parts) {
                return FromPath(parts, Filename, Extension, s => new TilesetSuggestionsNameInfo(s));
            }
        }
    }
}
