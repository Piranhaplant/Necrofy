using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Necrofy
{
    class WrappedItem : WrappedLevelObject<Item>
    {
        public WrappedItem(Item item, LoadedSpriteGraphics spriteGraphics) : base(item, spriteGraphics) { }

        public override SpriteDisplay.Category Category => SpriteDisplay.Category.Item;

        public override Rectangle Bounds => spriteGraphics.GetRectangle(SpriteDisplay.Key.Type.Item, wrappedObject.type, x, y);

        public override ushort x { get => wrappedObject.x; set => wrappedObject.x = value; }
        public override ushort y { get => wrappedObject.y; set => wrappedObject.y = value; }
        public override int type { get => wrappedObject.type; set => wrappedObject.type = (byte)value; }

        public override void Render(Graphics g) {
            spriteGraphics.Render(SpriteDisplay.Key.Type.Item, wrappedObject.type, g, x, y);
        }

        public override bool Removable => true;

        public override void Add(Level level) {
            level.items.Add(wrappedObject);
        }

        public override void Remove(Level level) {
            level.items.Remove(wrappedObject);
        }

        public override void AddToClipboard(SpriteClipboardContents clipboard) {
            clipboard.items.Add(wrappedObject);
        }
    }
}
