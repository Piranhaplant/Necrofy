using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Necrofy
{
    class TilesetObjectBrowserContents : ObjectBrowserContents
    {
        private static readonly Font font = new Font(FontFamily.GenericMonospace, 10);
        private static Size fontSize = Size.Empty;
        private const int fontPadding = 2;

        private readonly LoadedLevel level;

        public TilesetObjectBrowserContents(LoadedLevel level) {
            this.level = level;
        }

        public override IEnumerable<Size> Objects {
            get {
                foreach (Bitmap tile in level.tiles) {
                    yield return new Size(tile.Size.Width + fontSize.Width + fontPadding, tile.Size.Height);
                }
            }
        }

        public override bool PaintObject(int i, Graphics g, int x, int y) {
            if (fontSize.Width == 0) {
                fontSize = TextRenderer.MeasureText(g, "0", font, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPadding);
                return true;
            }
            g.DrawImage(level.tiles[i], x, y);
            g.DrawString((i / 0x10).ToString("X"), font, Brushes.Black, x + level.tiles[i].Width + fontPadding, y);
            g.DrawString((i % 0x10).ToString("X"), font, Brushes.Black, x + level.tiles[i].Width + fontPadding, y + fontSize.Height);
            return false;
        }

        protected override void ObjectSelected(int i) {
            // TODO
        }
    }
}
