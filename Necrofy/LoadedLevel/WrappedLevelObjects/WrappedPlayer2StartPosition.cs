using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Necrofy
{
    class WrappedPlayer2StartPosition : WrappedLevelObject<object>
    {
        private static readonly object obj = new object(); // Use the same object all the time so that they are computed as equal

        private readonly Level level;

        public WrappedPlayer2StartPosition(LoadedSpriteGraphics spriteGraphics, Level level) : base(obj, spriteGraphics) {
            this.level = level;
        }

        public override Rectangle Bounds => spriteGraphics.GetRectangle(SpriteDisplay.Key.Type.Player, 1, x, y);

        public override ushort x { get => level.p2startX; set => level.p2startX = value; }
        public override ushort y { get => level.p2startY; set => level.p2startY = value; }
    }
}
