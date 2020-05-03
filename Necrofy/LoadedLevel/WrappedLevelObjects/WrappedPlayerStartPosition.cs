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

        public override SpriteDisplay.Category Category => SpriteDisplay.Category.Player;

        public override Rectangle Bounds => spriteGraphics.GetRectangle(SpriteDisplay.Key.Type.Player, wrappedObject, X, Y);
        public override int Type { get => wrappedObject; set { } }

        public override void Render(Graphics g) {
            spriteGraphics.Render(SpriteDisplay.Key.Type.Player, wrappedObject, g, X, Y);
        }

        public override bool Removable => false;

        public override void Add(Level level) {
            // Can't be added or removed
        }

        public override void Remove(Level level) {
            // Can't be added or removed
        }

        public override void AddToClipboard(SpriteClipboardContents clipboard) {
            // Can't be added or removed
        }
    }

    class WrappedPlayer1StartPosition : WrappedPlayerStartPosition
    {
        public WrappedPlayer1StartPosition(LoadedSpriteGraphics spriteGraphics, Level level) : base(0, spriteGraphics, level) { }
        
        public override ushort X { get => level.p1startX; set => level.p1startX = value; }
        public override ushort Y { get => level.p1startY; set => level.p1startY = value; }
    }

    class WrappedPlayer2StartPosition : WrappedPlayerStartPosition
    {
        public WrappedPlayer2StartPosition(LoadedSpriteGraphics spriteGraphics, Level level) : base(1, spriteGraphics, level) { }

        public override ushort X { get => level.p2startX; set => level.p2startX = value; }
        public override ushort Y { get => level.p2startY; set => level.p2startY = value; }
    }
}
