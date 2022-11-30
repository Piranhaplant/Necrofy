using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Necrofy
{
    class SpriteEditorObjectBrowserContents : ObjectBrowserContents
    {
        private readonly LoadedSprites sprites;

        public SpriteEditorObjectBrowserContents(LoadedSprites sprites) {
            this.sprites = sprites;
        }

        public void Refresh() {
            RaiseObjectsChangedEvent(scrollToTop: false);
        }

        public override IEnumerable<ObjectBrowserObject> Objects {
            get {
                if (sprites.spriteImages == null) {
                    yield break;
                }
                foreach (Bitmap image in sprites.spriteImages) {
                    yield return new ObjectBrowserObject(image.Size);
                }
            }
        }

        public override void PaintObject(int i, Graphics g, int x, int y) {
            g.DrawImage(sprites.spriteImages[i], x, y);
        }
    }
}
