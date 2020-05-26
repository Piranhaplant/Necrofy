using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Necrofy
{
    class LevelTitleObjectBrowserContents : ObjectBrowserContents
    {
        private readonly LoadedLevelTitleCharacters characters;
        private readonly List<Bitmap> characterImages = new List<Bitmap>();
        private readonly List<int> characterNumbers = new List<int>();

        private int palette = 0;

        public LevelTitleObjectBrowserContents(LoadedLevelTitleCharacters characters) {
            this.characters = characters;
            for (int i = 0; i < characters.images.Length; i++) {
                if (characters.images[i] != null) {
                    characterImages.Add(characters.images[i]);
                    characterNumbers.Add((byte)i);
                }
            }
        }

        public void SetPalette(int palette) {
            this.palette = palette;
            RaiseObjectsChangedEvent(scrollToTop: false);
        }

        public int SelectedChar => SelectedIndex > -1 ? characterNumbers[SelectedIndex] : -1;

        public override IEnumerable<ObjectBrowserObject> Objects {
            get {
                return characterImages.Select(i => new ObjectBrowserObject(i.Size));
            }
        }

        public override void PaintObject(int i, Graphics g, int x, int y) {
            Bitmap image = characterImages[i];
            g.FillRectangle(Brushes.Black, x, y, image.Width, image.Height);
            SNESGraphics.DrawWithPlt(g, x, y, image, characters.loadedPalette.colors, palette * 0x10, 0x20);
        }
    }
}
