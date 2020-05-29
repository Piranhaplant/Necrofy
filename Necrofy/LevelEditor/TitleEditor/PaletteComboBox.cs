using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Necrofy
{
    class PaletteComboBox : ComboBox
    {
        private static readonly Brush highlightedItemBrush = new SolidBrush(Color.FromArgb(128, SystemColors.Highlight));

        public PaletteComboBox() {
            DrawMode = DrawMode.OwnerDrawFixed;
            DropDownStyle = ComboBoxStyle.DropDownList;
            ItemHeight = LoadedLevelTitleCharacters.height * 8;
        }

        public void AddPalette(LoadedLevelTitleCharacters loadedCharacters, byte palette) {
            Items.Add(new Item(loadedCharacters, palette));
        }

        public byte SelectedPalette {
            get {
                return (SelectedItem as Item)?.palette ?? 255;
            }
            set {
                foreach (object obj in Items) {
                    if (obj is Item item && item.palette == value) {
                        SelectedItem = obj;
                        return;
                    }
                }
                SelectedIndex = -1;
            }
        }

        protected override void OnDrawItem(DrawItemEventArgs e) {
            if (e.Index >= 0 && e.Index < Items.Count) {
                if (Items[e.Index] is Item item) {
                    item.Draw(e);
                }
                if (e.State.HasFlag(DrawItemState.Focus)) {
                    e.Graphics.FillRectangle(highlightedItemBrush, e.Bounds);
                }
                if (e.State.HasFlag(DrawItemState.Disabled)) {
                    using (Brush b = new SolidBrush(Color.FromArgb(128, e.BackColor))) {
                        e.Graphics.FillRectangle(b, e.Bounds);
                    }
                }
            }
            base.OnDrawItem(e);
        }

        private class Item
        {
            private static readonly byte[] displayChars = Encoding.ASCII.GetBytes("ABC");

            private readonly LoadedLevelTitleCharacters loadedCharacters;
            public readonly byte palette;

            public Item(LoadedLevelTitleCharacters loadedCharacters, byte palette) {
                this.loadedCharacters = loadedCharacters;
                this.palette = palette;
            }

            public void Draw(DrawItemEventArgs e) {
                int x = e.Bounds.X;
                foreach (byte c in displayChars) {
                    x += loadedCharacters.DrawChar(e.Graphics, x, e.Bounds.Y, c, palette);
                }
            }
        }
    }
}
