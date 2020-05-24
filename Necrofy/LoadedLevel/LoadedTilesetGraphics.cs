using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Necrofy
{
    class LoadedTilesetGraphics : LoadedGraphics
    {
        public LoadedTilesetGraphics(Project project, string tilesetGraphicsName)
            : base(TilesetGraphicsAsset.FromProject(project, tilesetGraphicsName).data) { }
    }
}
