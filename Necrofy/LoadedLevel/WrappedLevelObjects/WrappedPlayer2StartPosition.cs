using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Necrofy
{
    class WrappedPlayer2StartPosition : WrappedLevelObject<object>
    {
        private readonly Level level;

        public WrappedPlayer2StartPosition(LoadedSpriteGraphics spriteGraphics, Level level) : base(new object(), spriteGraphics) {
            this.level = level;
        }

        public override Rectangle Bounds => spriteGraphics.GetRectangle(SpriteDisplay.Key.Type.Player, 1, x, y);

        public override ushort x { get => level.p2startX; set => level.p2startX = value; }
        public override ushort y { get => level.p2startY; set => level.p2startY = value; }
    }
}
