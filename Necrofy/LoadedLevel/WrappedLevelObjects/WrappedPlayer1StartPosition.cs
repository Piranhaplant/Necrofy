using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Necrofy
{
    class WrappedPlayer1StartPosition : WrappedLevelObject<object>
    {
        private static readonly object obj = new object(); // Use the same object all the time so that they are computed as equal

        private readonly Level level;

        public WrappedPlayer1StartPosition(LoadedSpriteGraphics spriteGraphics, Level level) : base(obj, spriteGraphics) {
            this.level = level;
        }

        public override Rectangle Bounds => spriteGraphics.GetRectangle(SpriteDisplay.Key.Type.Player, 0, x, y);

        public override ushort x { get => level.p1startX; set => level.p1startX = value; }
        public override ushort y { get => level.p1startY; set => level.p1startY = value; }
    }
}
