using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Necrofy
{
    abstract class WrappedLevelObject<T> : WrappedLevelObject
    {
        protected readonly T wrappedObject;
        protected readonly LoadedLevelSprites spriteGraphics;

        public WrappedLevelObject(T wrappedObject, LoadedLevelSprites spriteGraphics) {
            this.wrappedObject = wrappedObject;
            this.spriteGraphics = spriteGraphics;
        }

        // Properties used in the property browser
        private string browsableX = null;
        private string browsableY = null;
        public override void ClearBrowsableProperties() {
            browsableX = null;
            browsableY = null;
        }
        [DisplayName(XProperty)]
        public string BrowsableX { get => browsableX ?? X.ToString(); set => browsableX = value; }
        [DisplayName(YProperty)]
        public string BrowsableY { get => browsableY ?? Y.ToString(); set => browsableY = value; }

        public override int GetHashCode() {
            return wrappedObject.GetHashCode();
        }

        public override bool Equals(object obj) {
            if (obj is WrappedLevelObject<T> other) {
                return wrappedObject.Equals(other.wrappedObject);
            }
            return base.Equals(obj);
        }
    }

    abstract class WrappedLevelObject : ISelectableObject
    {
        public const string XProperty = "X";
        public const string YProperty = "Y";
        public const string PointerProperty = "Pointer";

        public abstract SpriteDisplay.Category Category { get; }
        [Browsable(false)]
        public abstract Rectangle Bounds { get; }
        [Browsable(false)]
        public abstract ushort X { get; set; }
        [Browsable(false)]
        public abstract ushort Y { get; set; }
        [Browsable(false)]
        public abstract int Type { get; set; }
        [Browsable(false)]
        public abstract bool Removable { get; }

        public int GetX() {
            return X;
        }

        public int GetY() {
            return Y;
        }

        public abstract void Render(Graphics g);
        public virtual void RenderExtras(Graphics g, bool showRespawnAreas) { }
        public abstract void Add(Level level);
        public abstract void Remove(Level level);
        public abstract void AddToClipboard(SpriteClipboardContents clipboard);
        public abstract void ClearBrowsableProperties();
    }
}
