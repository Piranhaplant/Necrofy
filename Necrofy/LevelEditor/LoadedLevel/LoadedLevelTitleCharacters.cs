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

        private readonly LoadedGraphics loadedGraphics;
        private readonly LoadedPalette loadedPalette;
        private readonly LoadedTilemap loadedTilemap;
        private readonly DataAsset charData;
        public Bitmap[] images;

        public event EventHandler Updated;

        public LoadedLevelTitleCharacters(Project project) {
            loadedGraphics = new LoadedGraphics(project, Asset.LevelTitleFolder + Asset.FolderSeparator + GraphicsAsset.DefaultName);
            loadedPalette = new LoadedPalette(project, Asset.LevelTitleFolder + Asset.FolderSeparator + PaletteAsset.DefaultName);
            loadedTilemap = new LoadedTilemap(project, Asset.LevelTitleFolder + Asset.FolderSeparator + TilemapAsset.DefaultName);
            charData = DataAsset.FromProject(project, Asset.LevelTitleFolder, DataAsset.LevelTitleCharacterMapName);

            loadedPalette.Updated += Asset_Updated;
            loadedGraphics.Updated += Asset_Updated;
            loadedTilemap.Updated += Asset_Updated;
            charData.Updated += Asset_Updated;

            Load();
        }

        private void Asset_Updated(object sender, EventArgs e) {
            Dispose();
            Load();
            Updated?.Invoke(sender, e);
        }

        private void Load() {
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

        public int DrawChar(Graphics g, int x, int y, byte c, byte palette) {
            Bitmap image = GetImageForChar(c);
            if (image != null) {
                SNESGraphics.DrawWithPlt(g, x, y, image, loadedPalette.colors, palette * 0x10, loadedPalette.colors.Length);
                return image.Width;
            }
            return 0;
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
