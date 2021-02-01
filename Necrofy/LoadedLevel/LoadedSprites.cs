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

        public readonly Bitmap[] images;
        public readonly Bitmap[] tileImages;

        public LoadedSprites(Project project) {
            spritesAsset = SpritesAsset.FromProject(project);
            loadedGraphics = new LoadedGraphics(project, Asset.SpritesFolder + Asset.FolderSeparator + GraphicsAsset.DefaultName);
            loadedPalette = new LoadedPalette(project, Asset.SpritesFolder + Asset.FolderSeparator + PaletteAsset.DefaultSpritePaletteName, transparent: true);

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
