using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Necrofy
{
    class MapMouseEventArgs : MouseEventArgs
    {
        public int TileX { get; private set; }
        public int TileY { get; private set; }
        public bool InBounds { get; private set; }
        public bool MouseIsDown { get; private set; }

        public MapMouseEventArgs(MouseButtons button, int clicks, int x, int y, int delta, int tileSize, int boundsWidth, int boundsHeight, bool mouseIsDown) : base(button, clicks, x, y, delta) {
            TileX = (int)Math.Floor((double)x / tileSize);
            TileY = (int)Math.Floor((double)y / tileSize);
            InBounds = TileX >= 0 && TileY >= 0 && TileX < boundsWidth && TileY < boundsHeight;
            MouseIsDown = mouseIsDown;
        }
    }
}
