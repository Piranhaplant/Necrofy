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

        public LoadedGraphics(Project project, string graphicsName, GraphicsAsset.Type type) {
            this.graphicsName = graphicsName;
            asset = GraphicsAsset.FromProject(project, graphicsName, type);
            asset.Updated += Asset_Updated;

            ReadGraphics();
        }

        public GraphicsAsset.Type GraphicsType => asset.GraphicsType;
        public bool Is2BPP => GraphicsType == GraphicsAsset.Type.TwoBPP;

        private void Asset_Updated(object sender, EventArgs e) {
            if (updating == 0) {
                ReadGraphics();
                Updated?.Invoke(sender, e);
            }
        }

        private void ReadGraphics() {
            linearGraphics = new LinearGraphics(asset.data, Is2BPP);
        }

        public void Save(Project project, GraphicsTileList tiles) {
            for (int i = 0; i < tiles.Count; i++) {
                if (Is2BPP) {
                    SNESGraphics.BitmapToPlanar2BPP(tiles.GetTemporarily(i), asset.data, i * 0x10);
                } else {
                    SNESGraphics.BitmapToPlanar(tiles.GetTemporarily(i), asset.data, i * 0x20);
                }
            }
            ReadGraphics();
            updating++;
            asset.Save(project);
            updating--;
        }

        public class LinearGraphics
        {
            private readonly byte[] data;
            private readonly bool is2BPP;
            private readonly byte[][,] linearData;

            public int Length => linearData.Length;

            public byte[,] this[int i] {
                get {
                    if (linearData[i] == null) {
                        if (is2BPP) {
                            linearData[i] = SNESGraphics.PlanarToLinear2BPP(data, i * 0x10);
                        } else {
                            linearData[i] = SNESGraphics.PlanarToLinear(data, i * 0x20);
                        }
                    }
                    return linearData[i];
                }
            }

            public LinearGraphics(byte[] data, bool is2BPP) {
                this.data = data;
                this.is2BPP = is2BPP;
                linearData = new byte[data.Length / (is2BPP ? 0x10 : 0x20)][,];
            }
        }
    }
}
