using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Necrofy
{
    abstract class WrappedPlayerStartPosition : WrappedLevelObject<int>
    {
        protected readonly Level level;

        public WrappedPlayerStartPosition(int playerNum, LoadedSpriteGraphics spriteGraphics, Level level) : base(playerNum, spriteGraphics) {
            this.level = level;
        }

        public override Rectangle Bounds => spriteGraphics.GetRectangle(SpriteDisplay.Key.Type.Player, wrappedObject, x, y);

        public override void Render(Graphics g) {
            spriteGraphics.Render(SpriteDisplay.Key.Type.Player, wrappedObject, g, x, y);
        }
    }

    class WrappedPlayer1StartPosition : WrappedPlayerStartPosition
    {
        public WrappedPlayer1StartPosition(LoadedSpriteGraphics spriteGraphics, Level level) : base(0, spriteGraphics, level) { }
        
        public override ushort x { get => level.p1startX; set => level.p1startX = value; }
        public override ushort y { get => level.p1startY; set => level.p1startY = value; }
    }

    class WrappedPlayer2StartPosition : WrappedPlayerStartPosition
    {
        public WrappedPlayer2StartPosition(LoadedSpriteGraphics spriteGraphics, Level level) : base(1, spriteGraphics, level) { }

        public override ushort x { get => level.p2startX; set => level.p2startX = value; }
        public override ushort y { get => level.p2startY; set => level.p2startY = value; }
    }
}
