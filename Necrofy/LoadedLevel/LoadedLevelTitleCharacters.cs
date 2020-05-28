using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Necrofy
{
    class LoadedLevelTitleCharacters : IDisposable
    {
        private static readonly int[] charWidthForRow = new int[] { 3, 2, 6 };
        public const int height = 6;
        private const int tilemapRowWidth = 0x80;

        public readonly LoadedPalette loadedPalette;
        public readonly Bitmap[] images;

        public LoadedLevelTitleCharacters(Project project) {
            LoadedGraphics loadedGraphics = new LoadedGraphics(project, GraphicsAsset.LevelTitleName);
            loadedPalette = new LoadedPalette(project, PaletteAsset.LevelTitleName);
            LoadedTilemap loadedTilemap = new LoadedTilemap(project, TilemapAsset.LevelTitleName);
            DataAsset charData = DataAsset.FromProject(project, DataAsset.LevelTitleCharsName);

            images = new Bitmap[charData.data.Length / 2];
            for (int i = 0; i < images.Length; i++) {
                int tilemapX = charData.data[i * 2];
                int tilemapY = charData.data[i * 2 + 1];
                if (tilemapY < charWidthForRow.Length) {
                    int width = charWidthForRow[tilemapY];
                    Bitmap image = new Bitmap(width * 8, height * 8, PixelFormat.Format8bppIndexed);
                    SNESGraphics.FillPalette(image, loadedPalette.colors);
                    BitmapData imageData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);
                    for (int y = 0; y < height; y++) {
                        for (int x = 0; x < width; x++) {
                            SNESGraphics.DrawTile(imageData, x * 8, y * 8, loadedTilemap.tiles[(tilemapY * height + y) * tilemapRowWidth + tilemapX * width + x], loadedGraphics.linearGraphics);
                        }
                    }
                    image.UnlockBits(imageData);
                    images[i] = image;
                }
            }
        }

        public Bitmap GetImageForChar(byte c) {
            int imageIndex = c - 0x20;
            if (imageIndex >= 0 && imageIndex < images.Length) {
                return images[imageIndex];
            }
            return null;
        }

        public void Dispose() {
            foreach (Bitmap b in images) {
                if (b != null) {
                    b.Dispose();
                }
            }
        }
    }
}
