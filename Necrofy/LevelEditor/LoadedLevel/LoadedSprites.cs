using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Necrofy
{
    class LoadedSprites : IDisposable
    {
        private readonly SpritesAsset spritesAsset;
        public readonly string spritesName;
        public readonly LoadedPalette loadedPalette;

        public List<Sprite> Sprites { get; private set; }
        public Bitmap[] spriteImages = new Bitmap[] { };
        public List<Sprite.Graphics> graphics = new List<Sprite.Graphics>();

        public event EventHandler Updated;

        public LoadedSprites(Project project, string spritesName) {
            this.spritesName = spritesName;
            spritesAsset = SpritesAsset.FromProject(project, spritesName);
            loadedPalette = new LoadedPalette(project, Asset.SpritesFolder + Asset.FolderSeparator + PaletteAsset.DefaultSpritePaletteName, transparent: true);

            loadedPalette.Updated += Asset_Updated;

            Sprites = spritesAsset.sprites.sprites.JsonClone();
            string parentFolder = new Asset.ParsedName(spritesName).Folder;

            List<string> previousGraphicsAssets = spritesAsset.sprites.graphicsAssets;
            List<string> newGraphicsAssets = new List<string>();
            if (project.Assets.Root.FindFolder(parentFolder, out AssetTree.Folder folder)) {
                foreach (AssetTree.AssetEntry asset in folder.Assets) {
                    if (GraphicsAsset.GetGraphicsType(asset.Asset) == GraphicsAsset.Type.Sprite && asset.Asset.Name != GraphicsAsset.ExtraSpriteGraphics) {
                        newGraphicsAssets.Add(asset.Asset.Parts.name);
                    }
                }
                newGraphicsAssets.Sort(NumericStringComparer.instance);
            }
            ReorderGraphics(Sprites, previousGraphicsAssets, newGraphicsAssets);

            AddGraphics(new Sprite.Graphics(project, GraphicsAsset.SpriteGraphics, GraphicsAsset.ExtraSpriteGraphics));
            foreach (string graphicsAsset in newGraphicsAssets) {
                AddGraphics(new Sprite.Graphics(project, parentFolder + Asset.FolderSeparator + graphicsAsset));
            }

            LoadAllGraphics();
            LoadAllSprites();
        }

        private static void ReorderGraphics(List<Sprite> sprites, List<string> previousGraphicsAssets, List<string> newGraphicsAssets) {
            int[] mapping = new int[previousGraphicsAssets.Count + 1];
            mapping[0] = 0;

            for (int i = 0; i < previousGraphicsAssets.Count; i++) {
                int index = newGraphicsAssets.IndexOf(previousGraphicsAssets[i]);
                if (index >= 0) {
                    mapping[i + 1] = index + 1;
                } else {
                    mapping[i + 1] = i + 1;
                }
            }

            foreach (Sprite sprite in sprites) {
                foreach (Sprite.Tile tile in sprite.tiles) {
                    if (tile.graphicsIndex < mapping.Length) {
                        tile.graphicsIndex = mapping[tile.graphicsIndex];
                    }
                }
            }
        }

        private void Asset_Updated(object sender, EventArgs e) {
            Dispose();
            LoadAllGraphics();
            LoadAllSprites();
            Updated?.Invoke(sender, e);
        }

        private void AddGraphics(Sprite.Graphics g) {
            graphics.Add(g);
            foreach (LoadedGraphics lg in g.loadedGraphics) {
                lg.Updated += Asset_Updated;
            }
        }

        private void LoadAllGraphics() {
            foreach (Sprite.Graphics g in graphics) {
                g.LoadTiles();
            }
        }

        public void LoadAllSprites() {
            spriteImages = new Bitmap[Sprites.Count];
            for (int i = 0; i < spriteImages.Length; i++) {
                LoadSprite(i);
            }
        }

        public void LoadSprite(int i) {
            spriteImages[i] = Sprites[i].Render(graphics, loadedPalette.colors, null, out int anchorX, out int anchorY);
        }

        public void Save(Project project) {
            spritesAsset.sprites = new SpriteFile(Sprites.JsonClone(), graphics.Skip(1).Select(g => g.name).ToList());
            spritesAsset.Save(project);
        }

        public void Dispose() {
            foreach (Bitmap image in spriteImages) {
                image.Dispose();
            }
            foreach (Sprite.Graphics g in graphics) {
                g.Dispose();
            }
        }
    }
}
