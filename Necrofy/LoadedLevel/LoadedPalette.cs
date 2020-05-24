using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Necrofy
{
    class LoadedPalette
    {
        public readonly Color[] colors;

        public LoadedPalette(Project project, string paletteName)
            : this(PaletteAsset.FromProject(project, paletteName).data, transparent: true) { }

        protected LoadedPalette(byte[] data, bool transparent) {
            colors = SNESGraphics.SNESToRGB(data, transparent);
        }
    }
}
