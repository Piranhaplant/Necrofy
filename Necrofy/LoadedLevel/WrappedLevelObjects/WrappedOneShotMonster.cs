using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Necrofy
{
    class WrappedOneShotMonster : WrappedLevelObject<OneShotMonster>
    {
        public const string ExtraProperty = "Extra";

        private static readonly Font victimNumberFont = SystemFonts.DefaultFont;

        private static readonly StringFormat victimNumberFormat = new StringFormat() {
            Alignment = StringAlignment.Center
        };

        public WrappedOneShotMonster(OneShotMonster monster, LoadedLevelSprites spriteGraphics) : base(monster, spriteGraphics) { }

        public override SpriteDisplay.Category Category {
            get {
                if (Removable) {
                    return SpriteDisplay.Category.OneShotMonster;
                } else {
                    return SpriteDisplay.Category.Victim;
                }
            }
        }

        public override Rectangle Bounds {
            get {
                if (wrappedObject.type == OneShotMonster.CreditHeadType) {
                    return spriteGraphics.GetRectangle(SpriteDisplay.Key.Type.CreditHead, wrappedObject.extra, X, Y);
                } else {
                    return spriteGraphics.GetRectangle(SpriteDisplay.Key.Type.Pointer, wrappedObject.type, X, Y);
                }
            }
        }

        public override ushort X { get => wrappedObject.x; set => wrappedObject.x = value; }
        public override ushort Y { get => wrappedObject.y; set => wrappedObject.y = value; }
        public override int Type { get => wrappedObject.type; set => wrappedObject.type = value; }
        [Browsable(false)]
        public ushort Extra { get => wrappedObject.extra; set => wrappedObject.extra = value; }

        // Properties used in the property browser
        private string browsableExtra = null;
        private string browsablePointer = null;
        public override void ClearBrowsableProperties() {
            base.ClearBrowsableProperties();
            browsableExtra = null;
            browsablePointer = null;
        }
        [DisplayName(ExtraProperty)]
        public string BrowsableExtra { get => browsableExtra ?? wrappedObject.extra.ToString(); set => browsableExtra = value; }
        [DisplayName(PointerProperty)]
        public string BrowsablePointer { get => browsablePointer ?? PropertyBrowser.PointerToString(Type); set => browsablePointer = value; }

        public override void Render(Graphics g) {
            if (wrappedObject.type == OneShotMonster.CreditHeadType) {
                spriteGraphics.Render(SpriteDisplay.Key.Type.CreditHead, wrappedObject.extra, g, X, Y);
            } else {
                spriteGraphics.Render(SpriteDisplay.Key.Type.Pointer, wrappedObject.type, g, X, Y);
            }
        }

        public override void RenderExtras(Graphics g, bool showRespawnAreas, float zoom) {
            if (wrappedObject.victimNumber > 0) {
                Rectangle bounds = Bounds;
                int x = bounds.X + bounds.Width / 2;
                int y = bounds.Bottom + 4;
                g.DrawString(wrappedObject.victimNumber.ToString(), victimNumberFont, Brushes.Black, x + 1, y + 1, victimNumberFormat);
                g.DrawString(wrappedObject.victimNumber.ToString(), victimNumberFont, Brushes.White, x, y, victimNumberFormat);
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
