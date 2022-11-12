using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Necrofy
{
    class WrappedSprite
    {
        public const string NameProperty = "Name";

        private readonly Sprite sprite;

        public WrappedSprite(Sprite sprite) {
            this.sprite = sprite;
        }

        public void ClearBrowsableProperties() {
            browsableName = null;
        }

        private string browsableName = null;
        [DisplayName(NameProperty)]
        public string BrowsableName { get => browsableName ?? sprite.name ?? ""; set => browsableName = value; }

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
