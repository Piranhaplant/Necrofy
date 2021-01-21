using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Necrofy
{
    class WrappedTitleWord : ISelectableObject
    {
        public const string XProperty = "X";
        public const string YProperty = "Y";
        public const string PaletteProperty = "Palette";

        private const int MaxX = 0x80;
        private const int MoveAreaOutsideSize = 12;
        private const int MoveAreaInsideSize = 4;
        private static readonly Brush WrapAroundBrush = new SolidBrush(Color.FromArgb(128, Color.Red));

        /// <summary>The visible bounds of the word</summary>
        [Browsable(false)]
        public Rectangle VisibleBounds { get; private set; }
        /// <summary>The bounds inside of which the word can be moved</summary>
        [Browsable(false)]
        public Rectangle MoveBounds { get; private set; }
        /// <summary>The bounds inside of which text selection will be performed</summary>
        [Browsable(false)]
        public Rectangle TextBounds { get; private set; }

        /// <summary>X position for the left side of each character in the word. Includes an extra element at the end for the right side of the last character</summary>
        [Browsable(false)]
        public List<int> CharXPositions { get; private set; } = new List<int>();
        
        int ISelectableObject.GetX() {
            return word.x * 8;
        }
        int ISelectableObject.GetY() {
            return word.y * 8;
        }
        public bool Selectable => word.chars.Count > 0;

        private Rectangle selectableObjectBounds;
        Rectangle ISelectableObject.Bounds => selectableObjectBounds;

        [Browsable(false)]
        public byte X {
            get {
                return word.x;
            }
        }
        [Browsable(false)]
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

        [Browsable(false)]
        public byte Palette {
            get {
                return word.palette;
            }
            set {
                word.palette = value;
            }
        }

        [Browsable(false)]
        public IList<byte> Chars {
            get {
                return word.chars.AsReadOnly();
            }
            set {
                word.chars = new List<byte>(value);
                CalculateBounds();
            }
        }
        
        public readonly TitlePage.Word word;
        private readonly LoadedLevelTitleCharacters loadedCharacters;

        public WrappedTitleWord(TitlePage.Word word, LoadedLevelTitleCharacters loadedCharacters) {
            this.word = word;
            this.loadedCharacters = loadedCharacters;
            CalculateBounds();
        }

        private void CalculateBounds() {
            CharXPositions.Clear();

            int x = (word.x % MaxX) * 8;
            int startX = x;
            foreach (byte c in word.chars) {
                CharXPositions.Add(x);
                Bitmap image = loadedCharacters.GetImageForChar(c);
                if (image != null) {
                    x += image.Width;
                }
            }
            CharXPositions.Add(x);

            VisibleBounds = new Rectangle(word.x * 8, word.y * 8, x - startX, LoadedLevelTitleCharacters.height * 8);
            MoveBounds = new Rectangle(VisibleBounds.X - MoveAreaOutsideSize, VisibleBounds.Y - MoveAreaOutsideSize, VisibleBounds.Width + MoveAreaOutsideSize * 2, VisibleBounds.Height + MoveAreaOutsideSize * 2);
            TextBounds = new Rectangle(VisibleBounds.X + MoveAreaInsideSize, VisibleBounds.Y + MoveAreaInsideSize, VisibleBounds.Width - MoveAreaInsideSize * 2, VisibleBounds.Height - MoveAreaInsideSize * 2);
            selectableObjectBounds = VisibleBounds;
        }

        public void UseExtendedBounds(bool extended) {
            selectableObjectBounds = extended ? MoveBounds : VisibleBounds;
        }

        public void Render(Graphics g) {
            void DrawImage(int i, Bitmap image, bool wrapped, int xOffset = 0, int yOffset = 0) {
                int x = CharXPositions[i] % SNESGraphics.ScreenWidth + xOffset;
                int y = (word.y + CharXPositions[i] / SNESGraphics.ScreenWidth) * 8 + yOffset;
                loadedCharacters.DrawChar(g, x, y, word.chars[i], word.palette);
                if (wrapped) {
                    g.FillRectangle(WrapAroundBrush, x, y, image.Width, image.Height);
                }
            }

            for (int i = 0; i < word.chars.Count; i++) {
                Bitmap image = loadedCharacters.GetImageForChar(word.chars[i]);
                if (image != null) {
                    DrawImage(i, image, wrapped: X >= MaxX || CharXPositions[i] >= SNESGraphics.ScreenWidth);
                    if (CharXPositions[i] / SNESGraphics.ScreenWidth != CharXPositions[i + 1] / SNESGraphics.ScreenWidth) {
                        DrawImage(i, image, wrapped: true, xOffset: -SNESGraphics.ScreenWidth, yOffset: 8);
                    }
                }
            }
        }

        // Properties used in the property browser
        private string browsableX = null;
        private string browsableY = null;
        private string browsablePalette = null;
        public void ClearBrowsableProperties() {
            browsableX = null;
            browsableY = null;
            browsablePalette = null;
        }

        [DisplayName(XProperty)]
        public string BrowsableX { get => browsableX ?? X.ToString(); set => browsableX = value; }
        [DisplayName(YProperty)]
        public string BrowsableY { get => browsableY ?? Y.ToString(); set => browsableY = value; }
        [DisplayName(PaletteProperty)]
        public string BrowsablePalette { get => browsablePalette ?? Palette.ToString(); set => browsablePalette = value; }
    }
}
