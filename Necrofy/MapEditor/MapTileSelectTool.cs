using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Necrofy
{
    abstract class MapTileSelectTool : MapTool
    {
        private int prevX;
        private int prevY;

        public MapTileSelectTool(MapEditor mapEditor) : base(mapEditor) { }

        public override void MouseDown(MapMouseEventArgs e) {
            prevX = -1;
            prevY = -1;
            MouseMove(e);
        }

        public override void MouseMove(MapMouseEventArgs e) {
            if (e.MouseIsDown && (e.TileX != prevX || e.TileY != prevY) && e.InBounds) {
                SelectTiles(e.TileX, e.TileY);
                prevX = e.TileX;
                prevY = e.TileY;
            }
        }

        protected abstract void SelectTiles(int x, int y);
    }
}
