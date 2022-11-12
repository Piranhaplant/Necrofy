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
        public readonly List<LoadedGraphics> loadedGraphics = new List<LoadedGraphics>();
        public readonly LoadedPalette loadedPalette;

        public List<Sprite> Sprites { get; private set; }
        public List<string> GraphicsAssets { get; private set; } = new List<string>();
        public Bitmap[] images = new Bitmap[] { };
        public List<Bitmap> tileImages = new List<Bitmap>();

        public event EventHandler Updated;

        public LoadedSprites(Project project, string spritesName) {
            this.spritesName = spritesName;
            spritesAsset = SpritesAsset.FromProject(project, spritesName);
            loadedPalette = new LoadedPalette(project, Asset.SpritesFolder + Asset.FolderSeparator + PaletteAsset.DefaultSpritePaletteName, transparent: true);

            loadedPalette.Updated += Asset_Updated;

            Sprites = spritesAsset.sprites.sprites.JsonClone();
            foreach (string graphicsAsset in spritesAsset.sprites.graphicsAssets) {
                AddGraphics(project, graphicsAsset);
            }
            if (GraphicsAssets.Count > 0) {
                LoadAllSprites();
            }
        }

        private void Asset_Updated(object sender, EventArgs e) {
            Dispose();
            LoadAllGraphics();
            LoadAllSprites();
            Updated?.Invoke(sender, e);
        }

        public void AddGraphics(Project project, string graphicsAsset) {
            GraphicsAssets.Add(graphicsAsset);
            LoadedGraphics graphics = new LoadedGraphics(project, graphicsAsset, GetGraphicsAssetType(graphicsAsset));
            loadedGraphics.Add(graphics);
            graphics.Updated += Asset_Updated;
            Load(graphics);
        }

        public static GraphicsAsset.Type GetGraphicsAssetType(string name) {
            return name == GraphicsAsset.SpriteGraphics ? GraphicsAsset.Type.Normal : GraphicsAsset.Type.Sprite;
        }

        private void LoadAllGraphics() {
            tileImages.Clear();
            foreach (LoadedGraphics graphics in loadedGraphics) {
                Load(graphics);
            }
        }

        private void Load(LoadedGraphics graphics) {
            for (int i = 0; i < graphics.linearGraphics.Length / 4; i++) {
                Bitmap b = new Bitmap(16, 16, PixelFormat.Format8bppIndexed);
                BitmapData data = b.LockBits(new Rectangle(Point.Empty, b.Size), ImageLockMode.WriteOnly, b.PixelFormat);

                SNESGraphics.DrawTile(data, 0, 0, new LoadedTilemap.Tile(i * 4 + 0, 0, false, false, false), graphics.linearGraphics);
                SNESGraphics.DrawTile(data, 8, 0, new LoadedTilemap.Tile(i * 4 + 1, 0, false, false, false), graphics.linearGraphics);
                SNESGraphics.DrawTile(data, 0, 8, new LoadedTilemap.Tile(i * 4 + 2, 0, false, false, false), graphics.linearGraphics);
                SNESGraphics.DrawTile(data, 8, 8, new LoadedTilemap.Tile(i * 4 + 3, 0, false, false, false), graphics.linearGraphics);

                b.UnlockBits(data);
                tileImages.Add(b);
            }
        }

        public void LoadAllSprites() {
            images = new Bitmap[Sprites.Count];
            for (int i = 0; i < images.Length; i++) {
                LoadSprite(i);
            }
        }

        public void LoadSprite(int i) {
            images[i] = Sprites[i].Render(loadedGraphics, loadedPalette.colors, null, out int anchorX, out int anchorY);
        }

        public void Save(Project project) {
            spritesAsset.sprites = Sprite.RemoveUnusedGraphics(Sprites, GraphicsAssets, loadedGraphics);
            spritesAsset.Save(project);
        }

        public void Dispose() {
            foreach (Bitmap image in images) {
                image.Dispose();
            }
            foreach (Bitmap image in tileImages) {
                image.Dispose();
            }
        }
    }
}
