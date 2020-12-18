using System;
using System.Collections.Generic;
using System.Drawing;
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

        public LoadedSprites(Project project) {
            SpritesAsset spritesAsset = SpritesAsset.FromProject(project);
            LoadedGraphics loadedGraphics = new LoadedGraphics(project, Asset.SpritesFolder + Asset.FolderSeparator + GraphicsAsset.DefaultName);
            LoadedPalette loadedPalette = new LoadedPalette(project, Asset.SpritesFolder + Asset.FolderSeparator + PaletteAsset.DefaultSpritePaletteName, transparent: true);

            images = new Bitmap[spritesAsset.sprites.Length];
            for (int i = 0; i < spritesAsset.sprites.Length; i++) {
                images[i] = spritesAsset.sprites[i].Render(loadedGraphics.linearGraphics, loadedPalette.colors, null, out int anchorX, out int anchorY);
            }
        }

        public void Dispose() {
            foreach (Bitmap image in images) {
                image.Dispose();
            }
        }
    }
}
