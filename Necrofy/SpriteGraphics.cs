using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Necrofy
{
    class SpriteGraphics
    {
        public class Sprite
        {
            public int width;
            public int height;
            public int displayWidth;
            public int displayHeight;
            public int anchorX;
            public int anchorY;
            public int key;
            public Tile[,] tiles;
        }

        public class Tile
        {
            public int index;
            public bool xFlip;
            public bool yFlip;
            public int palette;
            public int xShift;
            public int yShift;
            public bool priority;
        }
    }
}
