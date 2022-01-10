using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Necrofy
{
    class TilesetSuggestionsAsset : Asset
    {
        private const AssetCategory AssetCat = AssetCategory.Editor;

        static TilesetSuggestionsAsset() {
            AddCreator(new TilesetSuggestionsCreator());
        }
        
        public TilesetSuggestions data;

        public static TilesetSuggestionsAsset FromProject(Project project, string tilemapName) {
            ParsedName parsedName = new ParsedName(tilemapName);
            return new TilesetSuggestionsCreator().FromProject(project, parsedName.Tileset);
        }

        private TilesetSuggestionsAsset(TilesetSuggestionsNameInfo nameInfo, TilesetSuggestions data) : base(nameInfo) {
            this.data = data;
        }

        protected override void WriteFile(Project project) {
            // TODO: data would need to be converted back to JSON format first
            throw new NotImplementedException();
        }

        public override void Insert(NStream rom, ROMInfo romInfo, Project project) { }

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
                return new TilesetSuggestionsAsset((TilesetSuggestionsNameInfo)nameInfo, new TilesetSuggestions(File.ReadAllText(filename)));
            }
        }

        class TilesetSuggestionsNameInfo : NameInfo
        {
            private const string Filename = "Suggestions";
            private const string Extension = "json";
            
            private TilesetSuggestionsNameInfo(PathParts parts) : base(parts) { }
            public TilesetSuggestionsNameInfo(string tilesetName) : this(new PathParts(GetTilesetFolder(tilesetName), Filename, Extension, null, false)) { }
            
            public override AssetCategory Category => AssetCat;
            
            public static TilesetSuggestionsNameInfo FromPath(PathParts parts) {
                string tilesetName = parts.folder.Substring(parts.folder.LastIndexOf(FolderSeparator) + 1);
                if (parts.folder != GetTilesetFolder(tilesetName)) return null;
                if (parts.name != Filename) return null;
                if (parts.fileExtension != Extension) return null;
                if (parts.pointer != null) return null;
                if (parts.compressed) return null;
                return new TilesetSuggestionsNameInfo(parts);
            }
        }
    }
}
