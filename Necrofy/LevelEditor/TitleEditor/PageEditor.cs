using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Necrofy
{
    partial class PageEditor : UserControl
    {
        private TitlePage page;
        private LoadedLevelTitleCharacters characters;

        public PageEditor() {
            InitializeComponent();
        }

        public void LoadPage(TitlePage page, LoadedLevelTitleCharacters characters) {
            this.page = page.JsonClone();
            this.characters = characters;
            Invalidate();
        }

        private void PageEditor_Paint(object sender, PaintEventArgs e) {
            if (page != null) {
                foreach (TitlePage.Word word in page.words) {
                    int x = word.x * 8;
                    int y = word.y * 8;
                    foreach (byte c in word.chars) {
                        int charIndex = c - 0x20;
                        if (charIndex >= 0 && charIndex < characters.images.Length) {
                            Bitmap b = characters.images[charIndex];
                            if (b != null) {
                                SNESGraphics.DrawWithPlt(e.Graphics, x, y, b, characters.loadedPalette.colors, word.palette * 0x10, 0x20);
                                x += b.Width;
                            }
                        }
                    }
                }
            }
        }
    }
}
