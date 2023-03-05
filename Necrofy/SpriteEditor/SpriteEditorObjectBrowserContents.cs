using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Necrofy
{
    class SpriteEditorObjectBrowserContents : ObjectBrowserContents
    {
        private static readonly Font font = SystemFonts.DefaultFont;
        private static readonly Size fontSize = TextRenderer.MeasureText("0", font);
        private const int fontPadding = 2;

        private readonly LoadedSprites sprites;

        public SpriteEditorObjectBrowserContents(LoadedSprites sprites) {
            this.sprites = sprites;
        }

        public override bool SupportsListMode => true;

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
            Bitmap image = sprites.spriteImages[i];
            g.DrawImage(image, x, y);

            if (ListMode) {
                Sprite sprite = sprites.Sprites[i];

                string s = "";
                if (sprite.pointer != null) {
                    s += PropertyBrowser.PointerToString((int)sprite.pointer) + " ";
                }
                if (sprite.name != null) {
                    s += sprite.name;
                }

                g.DrawString(s, font, Brushes.Black, x + image.Width + fontPadding, y + (image.Height - fontSize.Height) / 2);
            }
        }
    }
}
