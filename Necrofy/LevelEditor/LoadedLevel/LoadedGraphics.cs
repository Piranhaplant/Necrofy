using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Necrofy
{
    class LoadedGraphics
    {
        public readonly GraphicsAsset asset;
        public readonly string graphicsName;
        public readonly LinearGraphics linearGraphics;

        public LoadedGraphics(Project project, string graphicsName) {
            this.graphicsName = graphicsName;
            asset = GraphicsAsset.FromProject(project, graphicsName);
            linearGraphics = new LinearGraphics(asset.data);
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

            public LinearGraphics(byte[][,] linearData) {
                data = null;
                this.linearData = linearData;
            }
        }
    }
}
