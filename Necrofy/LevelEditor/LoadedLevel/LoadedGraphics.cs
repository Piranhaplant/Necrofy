using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Necrofy
{
    class LoadedGraphics
    {
        public readonly GraphicsAsset asset;
        public readonly string graphicsName;
        public LinearGraphics linearGraphics { get; private set; }

        public event EventHandler Updated;

        private int updating = 0;

        public LoadedGraphics(Project project, string graphicsName) {
            this.graphicsName = graphicsName;
            asset = GraphicsAsset.FromProject(project, graphicsName);
            asset.Updated += Asset_Updated;
            ReadGraphics();
        }

        private void Asset_Updated(object sender, EventArgs e) {
            if (updating == 0) {
                ReadGraphics();
                Updated?.Invoke(sender, e);
            }
        }

        private void ReadGraphics() {
            linearGraphics = new LinearGraphics(asset.data);
        }

        public void Save(Project project, GraphicsTileList tiles) {
            for (int i = 0; i < tiles.Count; i++) {
                SNESGraphics.BitmapToPlanar(tiles.GetTemporarily(i), asset.data, i * 0x20);
            }
            ReadGraphics();
            updating++;
            asset.Save(project);
            updating--;
        }

        public class LinearGraphics
        {
            private readonly byte[] data;
            private readonly byte[][,] linearData;

            public int Length => linearData.Length;

            public byte[,] this[int i] {
                get {
                    if (linearData[i] == null) {
                        linearData[i] = SNESGraphics.PlanarToLinear(data, i * 0x20);
                    }
                    return linearData[i];
                }
            }

            public LinearGraphics(byte[] data) {
                this.data = data;
                linearData = new byte[data.Length / 0x20][,];
            }
        }
    }
}
