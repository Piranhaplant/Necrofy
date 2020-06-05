using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Necrofy
{
    class WrappedMonster : WrappedLevelObject<Monster>
    {
        public const string RadiusProperty = "Radius";
        public const string DelayProperty = "Delay";

        public WrappedMonster(Monster monster, LoadedSpriteGraphics spriteGraphics) : base(monster, spriteGraphics) { }

        public override SpriteDisplay.Category Category => SpriteDisplay.Category.Monster;

        public override Rectangle Bounds => spriteGraphics.GetRectangle(SpriteDisplay.Key.Type.Pointer, wrappedObject.type, X, Y);

        public override ushort X { get => wrappedObject.x; set => wrappedObject.x = value; }
        public override ushort Y { get => wrappedObject.y; set => wrappedObject.y = value; }
        public override int Type { get => wrappedObject.type; set => wrappedObject.type = value; }
        [Browsable(false)]
        public byte Radius { get => wrappedObject.radius; set => wrappedObject.radius = value; }
        [Browsable(false)]
        public byte Delay { get => wrappedObject.delay; set => wrappedObject.delay = value; }

        // Properties used in the property browser
        private string browsableRadius = null;
        private string browsableDelay = null;
        private string browsablePointer = null;
        public override void ClearBrowsableProperties() {
            base.ClearBrowsableProperties();
            browsableRadius = null;
            browsableDelay = null;
            browsablePointer = null;
        }
        [DisplayName(RadiusProperty)]
        public string BrowsableRadius { get => browsableRadius ?? wrappedObject.radius.ToString(); set => browsableRadius = value; }
        [DisplayName(DelayProperty)]
        public string BrowsableDelay { get => browsableDelay ?? wrappedObject.delay.ToString(); set => browsableDelay = value; }
        [DisplayName(PointerProperty)]
        public string BrowsablePointer { get => browsablePointer ?? PropertyBrowser.PointerToString(Type); set => browsablePointer = value; }
        
        public override void Render(Graphics g) {
            spriteGraphics.Render(SpriteDisplay.Key.Type.Pointer, wrappedObject.type, g, X, Y);
        }

        public override void RenderExtras(Graphics g, bool showRespawnAreas) {
            if (showRespawnAreas) {
                Rectangle bounds = Bounds;
                g.DrawRectangle(Pens.LightSteelBlue,
                    bounds.X - wrappedObject.radius / 2, bounds.Y - wrappedObject.radius / 2,
                    bounds.Width + wrappedObject.radius, bounds.Height + wrappedObject.radius);
            }
        }

        public override bool Removable => true;

        public override void Add(Level level) {
            level.monsters.Add(wrappedObject);
        }

        public override void Remove(Level level) {
            level.monsters.Remove(wrappedObject);
        }

        public override void AddToClipboard(SpriteClipboardContents clipboard) {
            clipboard.monsters.Add(wrappedObject);
        }
    }
}
