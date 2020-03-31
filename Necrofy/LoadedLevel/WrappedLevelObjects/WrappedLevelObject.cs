using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Necrofy
{
    abstract class WrappedLevelObject<T> : WrappedLevelObject
    {
        protected readonly T wrappedObject;
        protected readonly LoadedSpriteGraphics spriteGraphics;

        public WrappedLevelObject(T wrappedObject, LoadedSpriteGraphics spriteGraphics) {
            this.wrappedObject = wrappedObject;
            this.spriteGraphics = spriteGraphics;
        }
        
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
        public abstract Rectangle Bounds { get; }
        public abstract ushort x { get; set; }
        public abstract ushort y { get; set; }
    }
}
