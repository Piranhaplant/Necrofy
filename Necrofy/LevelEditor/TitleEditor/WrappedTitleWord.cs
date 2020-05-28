using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Necrofy
{
    class WrappedTitleWord : ISelectableObject
    {
        private const int MoveAreaOutsideSize = 12;
        private const int MoveAreaInsideSize = 4;

        /// <summary>The visible bounds of the word</summary>
        public Rectangle VisibleBounds { get; private set; }
        /// <summary>The bounds inside of which the word can be moved</summary>
        public Rectangle MoveBounds { get; private set; }
        /// <summary>The bounds inside of which text selection will be performed</summary>
        public Rectangle TextBounds { get; private set; }

        /// <summary>X position for the left side of each character in the word. Includes an extra element at the end for the right side of the last character</summary>
        public List<int> CharXPositions { get; private set; } = new List<int>();

        ushort ISelectableObject.X => (ushort)(word.x * 8);
        ushort ISelectableObject.Y => (ushort)(word.y * 8);
        private Rectangle selectableObjectBounds;
        Rectangle ISelectableObject.Bounds => selectableObjectBounds;

        public byte X {
            get {
                return word.x;
            }
        }
        public byte Y {
            get {
                return word.y;
            }
        }
        public void SetPosition(byte x, byte y) {
            word.x = x;
            word.y = y;
            CalculateBounds();
        }

        private readonly TitlePage.Word word;
        private readonly LoadedLevelTitleCharacters loadedCharacters;

        public WrappedTitleWord(TitlePage.Word word, LoadedLevelTitleCharacters loadedCharacters) {
            this.word = word;
            this.loadedCharacters = loadedCharacters;
            CalculateBounds();
        }

        private void CalculateBounds() {
            CharXPositions.Clear();

            int x = word.x * 8;
            foreach (byte c in word.chars) {
                CharXPositions.Add(x);
                Bitmap image = loadedCharacters.GetImageForChar(c);
                if (image != null) {
                    x += image.Width;
                }
            }
            CharXPositions.Add(x);

            VisibleBounds = new Rectangle(word.x * 8, word.y * 8, x - (word.x * 8), LoadedLevelTitleCharacters.height * 8);
            MoveBounds = new Rectangle(VisibleBounds.X - MoveAreaOutsideSize, VisibleBounds.Y - MoveAreaOutsideSize, VisibleBounds.Width + MoveAreaOutsideSize * 2, VisibleBounds.Height + MoveAreaOutsideSize * 2);
            TextBounds = new Rectangle(VisibleBounds.X + MoveAreaInsideSize, VisibleBounds.Y + MoveAreaInsideSize, VisibleBounds.Width - MoveAreaInsideSize * 2, VisibleBounds.Height - MoveAreaInsideSize * 2);
            selectableObjectBounds = VisibleBounds;
        }

        public void UseExtendedBounds(bool extended) {
            selectableObjectBounds = extended ? MoveBounds : VisibleBounds;
        }

        public void Render(Graphics g) {
            for (int i = 0; i < word.chars.Count; i++) {
                Bitmap image = loadedCharacters.GetImageForChar(word.chars[i]);
                if (image != null) {
                    SNESGraphics.DrawWithPlt(g, CharXPositions[i], word.y * 8, image, loadedCharacters.loadedPalette.colors, word.palette * 0x10, 0x20);
                }
            }
        }
    }
}
