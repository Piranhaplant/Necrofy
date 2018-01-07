using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Necrofy
{
    class LoadedTilesetPalette
    {
        public readonly TilesetPaletteAsset paletteAsset;
        public Color[] colors;

        public LoadedTilesetPalette(Project project, string paletteName) {
            paletteAsset = TilesetPaletteAsset.FromProject(project, paletteName);

            colors = SNESGraphics.SNESToRGB(paletteAsset.data);
        }
    }
}
