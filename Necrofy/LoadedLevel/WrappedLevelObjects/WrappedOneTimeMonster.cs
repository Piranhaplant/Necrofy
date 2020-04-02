using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Necrofy
{
    class WrappedOneTimeMonster : WrappedLevelObject<OneShotMonster> {
        public WrappedOneTimeMonster(OneShotMonster monster, LoadedSpriteGraphics spriteGraphics) : base(monster, spriteGraphics) { }

        public override Rectangle Bounds {
            get {
                if (wrappedObject.type == OneShotMonster.CreditHeadType) {
                    return spriteGraphics.GetRectangle(SpriteDisplay.Key.Type.CreditHead, wrappedObject.extra, x, y);
                } else {
                    return spriteGraphics.GetRectangle(SpriteDisplay.Key.Type.Pointer, wrappedObject.type, x, y);
                }
            }
        }

        public override ushort x { get => wrappedObject.x; set => wrappedObject.x = value; }
        public override ushort y { get => wrappedObject.y; set => wrappedObject.y = value; }

        public override void Render(Graphics g) {
            if (wrappedObject.type == OneShotMonster.CreditHeadType) {
                spriteGraphics.Render(SpriteDisplay.Key.Type.CreditHead, wrappedObject.extra, g, x, y);
            } else {
                spriteGraphics.Render(SpriteDisplay.Key.Type.Pointer, wrappedObject.type, g, x, y);
            }
        }
    }
}
