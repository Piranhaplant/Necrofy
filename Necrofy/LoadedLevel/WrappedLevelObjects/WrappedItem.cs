using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Necrofy
{
    class WrappedItem : WrappedLevelObject<Item>
    {
        public const string TypeProperty = "Type";

        public WrappedItem(Item item, LoadedSpriteGraphics spriteGraphics) : base(item, spriteGraphics) { }

        public override SpriteDisplay.Category Category => SpriteDisplay.Category.Item;

        public override Rectangle Bounds => spriteGraphics.GetRectangle(SpriteDisplay.Key.Type.Item, wrappedObject.type, X, Y);

        public override ushort X { get => wrappedObject.x; set => wrappedObject.x = value; }
        public override ushort Y { get => wrappedObject.y; set => wrappedObject.y = value; }
        public override int Type { get => wrappedObject.type; set => wrappedObject.type = (byte)value; }

        // Properties used in the property browser
        private string browsableType = null;
        public override void ClearBrowsableProperties() {
            base.ClearBrowsableProperties();
            browsableType = null;
        }
        [DisplayName(TypeProperty)]
        public string BrowsableType { get => browsableType ?? Type.ToString(); set => browsableType = value; }

        public override void Render(Graphics g) {
            spriteGraphics.Render(SpriteDisplay.Key.Type.Item, wrappedObject.type, g, X, Y);
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
