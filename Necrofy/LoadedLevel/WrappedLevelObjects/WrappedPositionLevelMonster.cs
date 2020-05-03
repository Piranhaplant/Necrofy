using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Necrofy
{
    class WrappedPositionLevelMonster : WrappedLevelObject<PositionLevelMonster>
    {
        public WrappedPositionLevelMonster(PositionLevelMonster monster, LoadedSpriteGraphics spriteGraphics) : base(monster, spriteGraphics) { }

        public override SpriteDisplay.Category Category => SpriteDisplay.Category.LevelMonster;

        public override Rectangle Bounds => spriteGraphics.GetRectangle(SpriteDisplay.Key.Type.Pointer, wrappedObject.type, X, Y);

        public override ushort X { get => wrappedObject.x; set => wrappedObject.x = value; }
        public override ushort Y { get => wrappedObject.y; set => wrappedObject.y = value; }
        public override int Type { get => wrappedObject.type; set => wrappedObject.type = value; }

        // Properties used in the property browser
        private string browsablePointer = null;
        public override void ClearBrowsableProperties() {
            base.ClearBrowsableProperties();
            browsablePointer = null;
        }
        [DisplayName(PointerProperty)]
        public string BrowsablePointer { get => browsablePointer ?? PropertyBrowser.PointerToString(Type); set => browsablePointer = value; }

        public override void Render(Graphics g) {
            spriteGraphics.Render(SpriteDisplay.Key.Type.Pointer, wrappedObject.type, g, X, Y);
        }

        public override bool Removable => true;

        public override void Add(Level level) {
            level.levelMonsters.Add(wrappedObject);
        }

        public override void Remove(Level level) {
            level.levelMonsters.Remove(wrappedObject);
        }

        public override void AddToClipboard(SpriteClipboardContents clipboard) {
            clipboard.bossMonsters.Add(wrappedObject);
        }
    }
}
