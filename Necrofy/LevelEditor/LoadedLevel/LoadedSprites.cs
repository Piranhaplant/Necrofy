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
        public readonly SpritesAsset spritesAsset;
        public readonly LoadedGraphics loadedGraphics;
        public readonly LoadedPalette loadedPalette;

        public Bitmap[] images;
        public Bitmap[] tileImages;

        public event EventHandler Updated;

        public LoadedSprites(Project project, string folder) {
            spritesAsset = SpritesAsset.FromProject(project, folder);
            loadedGraphics = new LoadedGraphics(project, folder + Asset.FolderSeparator + GraphicsAsset.DefaultName);
            loadedPalette = new LoadedPalette(project, Asset.SpritesFolder + Asset.FolderSeparator + PaletteAsset.DefaultSpritePaletteName, transparent: true);

            loadedPalette.Updated += Asset_Updated;

            Load();
        }

        private void Asset_Updated(object sender, EventArgs e) {
            Dispose();
            Load();
            Updated?.Invoke(sender, e);
        }

        private void Load() {
            images = new Bitmap[spritesAsset.sprites.Length];
            for (int i = 0; i < images.Length; i++) {
                LoadSprite(i);
            }

            tileImages = new Bitmap[loadedGraphics.linearGraphics.Length / 4];
            for (int i = 0; i < tileImages.Length; i++) {
                Bitmap b = new Bitmap(16, 16, PixelFormat.Format8bppIndexed);
                BitmapData data = b.LockBits(new Rectangle(Point.Empty, b.Size), ImageLockMode.WriteOnly, b.PixelFormat);

                SNESGraphics.DrawTile(data, 0, 0, new LoadedTilemap.Tile(i * 4 + 0, 0, false, false), loadedGraphics.linearGraphics);
                SNESGraphics.DrawTile(data, 8, 0, new LoadedTilemap.Tile(i * 4 + 1, 0, false, false), loadedGraphics.linearGraphics);
                SNESGraphics.DrawTile(data, 0, 8, new LoadedTilemap.Tile(i * 4 + 2, 0, false, false), loadedGraphics.linearGraphics);
                SNESGraphics.DrawTile(data, 8, 8, new LoadedTilemap.Tile(i * 4 + 3, 0, false, false), loadedGraphics.linearGraphics);

                b.UnlockBits(data);
                tileImages[i] = b;
            }
        }

        public void LoadSprite(int i) {
            images[i] = spritesAsset.sprites[i].Render(loadedGraphics.linearGraphics, loadedPalette.colors, null, out int anchorX, out int anchorY);
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
