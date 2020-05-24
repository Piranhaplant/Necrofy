using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Necrofy
{
    class LoadedTilesetPalette : LoadedPalette
    {
        public LoadedTilesetPalette(Project project, string paletteName)
            : base(TilesetPaletteAsset.FromProject(project, paletteName).data, transparent: false) { }
    }
}
