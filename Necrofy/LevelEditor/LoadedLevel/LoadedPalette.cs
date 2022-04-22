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
        private readonly PaletteAsset asset;
        public readonly string paletteName;
        public readonly bool transparent;
        public Color[] colors;

        public event EventHandler Updated;

        private int updating = 0;

        public LoadedPalette(Project project, string paletteName, bool transparent = false) {
            this.paletteName = paletteName;
            this.transparent = transparent;
            asset = PaletteAsset.FromProject(project, paletteName);
            asset.Updated += Asset_Updated;
            ReadColors();
        }

        private void Asset_Updated(object sender, EventArgs e) {
            if (updating == 0) {
                ReadColors();
                Updated?.Invoke(sender, e);
            }
        }

        private void ReadColors() {
            colors = SNESGraphics.SNESToRGB(asset.data, transparent);
        }

        public void Save(Project project) {
            for (int i = 0; i < colors.Length; i++) {
                asset.data.WriteInt16(i * 2, SNESGraphics.RGBToSNES(colors[i]));
            }
            updating++;
            asset.Save(project);
            updating--;
        }
    }
}
