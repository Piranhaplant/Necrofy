using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Necrofy
{
    class WrappedSprite
    {
        private readonly Sprite sprite;

        public WrappedSprite(Sprite sprite) {
            this.sprite = sprite;
        }

        public string Pointer {
            get {
                if (sprite.pointer == null) {
                    return "None";
                } else {
                    return PropertyBrowser.PointerToString((int)sprite.pointer);
                }
            }
        }
    }
}
