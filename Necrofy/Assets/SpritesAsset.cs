using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Necrofy
{
    class SpritesAsset : Asset
    {
        public const string DefaultFileName = "Sprites";
        private const AssetCategory AssetCat = AssetCategory.Sprites;

        static SpritesAsset() {
            AddCreator(new SpritesCreator());
        }

        public SpriteFile sprites;

        public static SpritesAsset FromProject(Project project, string fullName) {
            ParsedName parsedName = new ParsedName(fullName);
            return new SpritesCreator().FromProject(project, parsedName.Folder, parsedName.FinalName);
        }

        private SpritesAsset(SpritesNameInfo nameInfo, SpriteFile sprites) : base(nameInfo) {
            this.sprites = sprites;
        }

        private SpritesAsset(SpritesNameInfo nameInfo, string filename) : base(nameInfo) {
            Reload(filename);
        }

        protected override void Reload(string filename) {
            sprites = JsonConvert.DeserializeObject<SpriteFile>(File.ReadAllText(filename), new SpriteFile.Converter());
        }

        protected override void WriteFile(string filename) {
            File.WriteAllText(filename, JsonConvert.SerializeObject(sprites, Formatting.Indented));
        }

        public override void Insert(NStream rom, ROMInfo romInfo, Project project) {
            Sprite.WriteToROM(sprites, rom, romInfo, nameInfo.Parts.folder);
        }
        
        class SpritesCreator : Creator
        {
            public SpritesAsset FromProject(Project project, string folder, string spritesName) {
                NameInfo nameInfo = new SpritesNameInfo(folder, spritesName);
                return project.GetCachedAsset(nameInfo, () => {
                    string filename = nameInfo.FindFilename(project.path);
                    return (SpritesAsset)FromFile(nameInfo, filename);
                });
            }

            public override NameInfo GetNameInfo(NameInfo.PathParts pathParts, Project project) {
                return SpritesNameInfo.FromPath(pathParts);
            }

            public override Asset FromFile(NameInfo nameInfo, string filename) {
                return new SpritesAsset((SpritesNameInfo)nameInfo, filename);
            }

            public override List<DefaultParams> GetDefaults() {
                return new List<DefaultParams>() {
                    new DefaultParams(0, new SpritesNameInfo(SpritesFolder, DefaultFileName), extractFromNecrofyROM: true),
                };
            }

            public override Asset FromRom(NameInfo nameInfo, NStream romStream, ROMInfo romInfo, int? size, out bool trackFreespace) {
                trackFreespace = false;
                List<Sprite> sprites = new List<Sprite>();
                Sprite.AddFromROM(sprites, romStream, ROMPointers.SpriteData1, romInfo);
                Sprite.AddFromROM(sprites, romStream, ROMPointers.SpriteData2, romInfo);

                return new SpritesAsset((SpritesNameInfo)nameInfo, new SpriteFile(sprites));
            }
        }

        class SpritesNameInfo : NameInfo
        {
            private const string Extension = "spr";
            
            private SpritesNameInfo(PathParts parts) : base(parts) { }
            public SpritesNameInfo(string folder, string name) : this(new PathParts(folder, name, Extension, null, false)) { }
            
            public override AssetCategory Category => AssetCat;
            
            public override bool Editable => true;
            public override EditorWindow GetEditor(Project project) {
                return new SpriteEditor(new LoadedSprites(project, Name));
            }

            public static SpritesNameInfo FromPath(PathParts parts) {
                if (parts.fileExtension != Extension) return null;
                if (parts.pointer != null) return null;
                if (parts.compressed) return null;
                return new SpritesNameInfo(parts);
            }
        }
    }
}
