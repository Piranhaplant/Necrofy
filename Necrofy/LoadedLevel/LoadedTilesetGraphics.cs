using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Necrofy
{
    class LoadedTilesetGraphics
    {
        public readonly TilesetGraphicsAsset graphicsAsset;
        public byte[][,] linearGraphics;

        public LoadedTilesetGraphics(Project project, string tilesetGraphicsName) {
            graphicsAsset = TilesetGraphicsAsset.FromProject(project, tilesetGraphicsName);

            linearGraphics = new byte[0x200][,];
            for (int i = 0; i < linearGraphics.Length; i++) {
                linearGraphics[i] = SNESGraphics.PlanarToLinear(graphicsAsset.data, i * 0x20);
            }
        }
    }
}
