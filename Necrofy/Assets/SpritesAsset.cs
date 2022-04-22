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
        private const AssetCategory AssetCat = AssetCategory.Sprites;

        static SpritesAsset() {
            AddCreator(new SpritesCreator());
        }

        public Sprite[] sprites;

        public static SpritesAsset FromProject(Project project, string folder) {
            return new SpritesCreator().FromProject(project, folder);
        }

        private SpritesAsset(SpritesNameInfo nameInfo, Sprite[] sprites) : base(nameInfo) {
            this.sprites = sprites;
        }

        private SpritesAsset(SpritesNameInfo nameInfo, string filename) : base(nameInfo, filename) { }

        protected override void Reload(string filename) {
            sprites = JsonConvert.DeserializeObject<Sprite[]>(File.ReadAllText(filename));
        }

        protected override void WriteFile(Project project) {
            File.WriteAllText(nameInfo.GetFilename(project.path, createDirectories: true), JsonConvert.SerializeObject(sprites, Formatting.Indented));
        }

        public override void Insert(NStream rom, ROMInfo romInfo, Project project) {
            Sprite.WriteToROM(sprites, rom, romInfo.Freespace);
        }
        
        class SpritesCreator : Creator
        {
            public SpritesAsset FromProject(Project project, string folder) {
                NameInfo nameInfo = new SpritesNameInfo(folder);
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
                    new DefaultParams(0, new SpritesNameInfo(SpritesFolder), extractFromNecrofyROM: true),
                };
            }

            public override Asset FromRom(NameInfo nameInfo, NStream romStream, ROMInfo romInfo, int? size, out bool trackFreespace) {
                trackFreespace = false;
                List<Sprite> sprites = new List<Sprite>();
                Sprite.AddFromROM(sprites, romStream, ROMPointers.SpriteData1, romInfo);
                Sprite.AddFromROM(sprites, romStream, ROMPointers.SpriteData2, romInfo);
                return new SpritesAsset((SpritesNameInfo)nameInfo, sprites.ToArray());
            }
        }

        class SpritesNameInfo : NameInfo
        {
            private const string FileName = "Sprites";
            private const string Extension = "spr";
            
            private SpritesNameInfo(PathParts parts) : base(parts) { }
            public SpritesNameInfo(string folder) : this(new PathParts(folder, FileName, Extension, null, false)) { }
            
            public override AssetCategory Category => AssetCat;
            
            public override bool Editable => true;
            public override EditorWindow GetEditor(Project project) {
                return new SpriteEditor(new LoadedSprites(project, Parts.folder));
            }

            public static SpritesNameInfo FromPath(PathParts parts) {
                if (parts.name != FileName) return null;
                if (parts.fileExtension != Extension) return null;
                if (parts.pointer != null) return null;
                if (parts.compressed) return null;
                return new SpritesNameInfo(parts);
            }
        }
    }
}
