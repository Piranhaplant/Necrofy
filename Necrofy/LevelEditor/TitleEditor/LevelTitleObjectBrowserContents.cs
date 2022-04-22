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
        private readonly List<Size> characterSizes = new List<Size>();
        private readonly List<byte> characterNumbers = new List<byte>();

        private byte palette = 0;

        public LevelTitleObjectBrowserContents(LoadedLevelTitleCharacters characters) {
            this.characters = characters;
            LoadCharacters();
        }

        private void LoadCharacters() {
            characterSizes.Clear();
            characterNumbers.Clear();
            for (int i = 0; i <= byte.MaxValue; i++) {
                Bitmap image = characters.GetImageForChar((byte)i);
                if (image != null) {
                    characterSizes.Add(image.Size);
                    characterNumbers.Add((byte)i);
                }
            }
        }

        public void CharactersUpdated() {
            LoadCharacters();
            RaiseObjectsChangedEvent(scrollToTop: false);
        }

        public void SetPalette(byte palette) {
            this.palette = palette;
            Repaint();
        }

        public byte SelectedChar => SelectedIndex > -1 ? characterNumbers[SelectedIndex] : (byte)0;

        public override IEnumerable<ObjectBrowserObject> Objects {
            get {
                return characterSizes.Select(s => new ObjectBrowserObject(s));
            }
        }

        public override void PaintObject(int i, Graphics g, int x, int y) {
            characters.DrawChar(g, x, y, characterNumbers[i], palette);
        }
    }
}
