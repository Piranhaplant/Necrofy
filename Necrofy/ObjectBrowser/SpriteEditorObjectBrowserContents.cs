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
                foreach (Bitmap image in sprites.images) {
                    yield return new ObjectBrowserObject(image.Size);
                }
            }
        }

        public override void PaintObject(int i, Graphics g, int x, int y) {
            g.DrawImage(sprites.images[i], x, y);
        }
    }
}
