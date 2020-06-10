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
        private static readonly Size fontSize = TextRenderer.MeasureText("0", font);
        private const int fontPadding = 2;

        private readonly LoadedLevel level;
        private List<ushort> visibleTiles = new List<ushort>();

        public TilesetObjectBrowserContents(LoadedLevel level) {
            this.level = level;
            level.TilesChanged += Level_TilesChanged;
            ShowAllTiles();
        }

        private void Level_TilesChanged(object sender, EventArgs e) {
            Repaint();
        }

        private bool solidOnly;
        public bool SolidOnly {
            get {
                return solidOnly;
            }
            set {
                if (value != solidOnly) {
                    solidOnly = value;
                    Repaint();
                }
            }
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

        public int SelectedTile => SelectedIndex < 0 ? -1 : visibleTiles[SelectedIndex];

        public override IEnumerable<ObjectBrowserObject> Objects {
            get {
                foreach (ushort tile in visibleTiles) {
                    Bitmap bitmap = level.tiles[tile];
                    yield return new ObjectBrowserObject(new Size(bitmap.Size.Width + fontSize.Width + fontPadding, bitmap.Size.Height));
                }
            }
        }

        public override void PaintObject(int i, Graphics g, int x, int y) {
            ushort tile = visibleTiles[i];
            if (solidOnly) {
                g.DrawImage(level.solidOnlyTiles[tile], x, y);
            } else {
                g.DrawImage(level.tiles[tile], x, y);
            }
            g.DrawString((tile / 0x10).ToString("X"), font, Brushes.Black, x + level.tiles[tile].Width + fontPadding, y);
            g.DrawString((tile % 0x10).ToString("X"), font, Brushes.Black, x + level.tiles[tile].Width + fontPadding, y + fontSize.Height);
        }
    }
}
