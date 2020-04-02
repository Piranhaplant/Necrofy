using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Necrofy
{
    class WrappedOneShotMonster : WrappedLevelObject<OneShotMonster>
    {
        private static readonly StringFormat victimNumberFormat = new StringFormat() {
            Alignment = StringAlignment.Center
        };

        public WrappedOneShotMonster(OneShotMonster monster, LoadedSpriteGraphics spriteGraphics) : base(monster, spriteGraphics) { }

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
            if (wrappedObject.victimNumber > 0) {
                Rectangle bounds = Bounds;
                int x = bounds.X + bounds.Width / 2;
                int y = bounds.Bottom + 4;
                g.DrawString(wrappedObject.victimNumber.ToString(), SystemFonts.DefaultFont, Brushes.Black, x + 1, y + 1, victimNumberFormat);
                g.DrawString(wrappedObject.victimNumber.ToString(), SystemFonts.DefaultFont, Brushes.White, x, y, victimNumberFormat);
            }
        }

        public override bool Removable => wrappedObject.victimNumber == 0;

        public override void Add(Level level) {
            if (Removable) {
                level.oneShotMonsters.Add(wrappedObject);
            }
        }

        public override void Remove(Level level) {
            if (Removable) {
                level.oneShotMonsters.Remove(wrappedObject);
            }
        }

        public override void AddToClipboard(SpriteClipboardContents clipboard) {
            if (Removable) {
                clipboard.oneShotMonsters.Add(wrappedObject);
            }
        }
    }
}
