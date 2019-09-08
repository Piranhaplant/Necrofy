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
        private List<ushort> visibleTiles = new List<ushort>();

        public TilesetObjectBrowserContents(LoadedLevel level) {
            this.level = level;
            ShowAllTiles();
        }

        public void ShowAllTiles() {
            if (visibleTiles.Count != level.tiles.Length) {
                int selectedTile = SelectedTile;
                visibleTiles.Clear();
                for (ushort i = 0; i < level.tiles.Length; i++) {
                    visibleTiles.Add(i);
                }
                RaiseObjectsChangedEvent();
                SelectedIndex = selectedTile;
            }
        }

        public void ShowTiles(List<ushort> tiles) {
            visibleTiles = tiles;
            RaiseObjectsChangedEvent();
            SelectedIndex = -1;
        }

        public int SelectedTile => SelectedIndex > -1 ? visibleTiles[SelectedIndex] : -1;

        public override IEnumerable<Size> Objects {
            get {
                foreach (ushort tile in visibleTiles) {
                    Bitmap bitmap = level.tiles[tile];
                    yield return new Size(bitmap.Size.Width + fontSize.Width + fontPadding, bitmap.Size.Height);
                }
            }
        }

        public override bool PaintObject(int i, Graphics g, int x, int y) {
            if (fontSize.Width == 0) {
                fontSize = TextRenderer.MeasureText(g, "0", font, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPadding);
                return true;
            }
            ushort tile = visibleTiles[i];
            g.DrawImage(level.tiles[tile], x, y);
            g.DrawString((tile / 0x10).ToString("X"), font, Brushes.Black, x + level.tiles[tile].Width + fontPadding, y);
            g.DrawString((tile % 0x10).ToString("X"), font, Brushes.Black, x + level.tiles[tile].Width + fontPadding, y + fontSize.Height);
            return false;
        }
    }
}
