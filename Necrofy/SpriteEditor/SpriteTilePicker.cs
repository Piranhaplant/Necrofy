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
    public partial class SpriteTilePicker : UserControl
    {
        private Bitmap[] tiles = null;
        private Color[] colors = null;
        private readonly ScrollWrapper scrollWrapper;

        private int tilesPerRow;

        private int palette = 0;
        public int Palette {
            get {
                return palette;
            }
            set {
                if (value != palette) {
                    palette = value;
                    canvas.Invalidate();
                }
            }
        }

        public delegate void SelectedTileChangedDelegate(object sender, EventArgs e);
        public event SelectedTileChangedDelegate SelectedTileChanged;

        private int selectedTile = -1;
        public int SelectedTile {
            get {
                return selectedTile;
            }
            set {
                if (value != selectedTile) {
                    selectedTile = value;
                    SelectedTileChanged?.Invoke(this, EventArgs.Empty);
                    //scrollWrapper.ScrollToPoint(0, selectedTile / tilesPerRow * 16 + 8);
                    canvas.Invalidate();
                }
            }
        }

        public SpriteTilePicker() {
            InitializeComponent();

            scrollWrapper = new ScrollWrapper(canvas, hScrollBar, vScrollBar, autoSize: false);
            scrollWrapper.Scrolled += ScrollWrapper_Scrolled;
        }

        public void SetTiles(Bitmap[] tiles, Color[] colors) {
            this.tiles = tiles;
            this.colors = colors;
            UpdateSize();
        }

        private void UpdateSize() {
            if (tiles == null) {
                return;
            }
            tilesPerRow = canvas.Width / 16;
            int height = (int)Math.Ceiling(tiles.Length / (double)tilesPerRow);
            scrollWrapper.SetClientSize(canvas.Width, Math.Max(canvas.Height, height * 16));
            canvas.Invalidate();
        }

        private void ScrollWrapper_Scrolled(object sender, EventArgs e) {
            canvas.Invalidate();
        }

        private void canvas_SizeChanged(object sender, EventArgs e) {
            UpdateSize();
        }

        private void canvas_Paint(object sender, PaintEventArgs e) {
            if (tiles == null) {
                return;
            }
            scrollWrapper.TransformGraphics(e.Graphics);

            for (int i = 0; i < tiles.Length; i++) {
                SNESGraphics.DrawWithPlt(e.Graphics, (i % tilesPerRow) * 16, (i / tilesPerRow) * 16, tiles[i], colors, palette * 16, 16);
            }
            if (SelectedTile >= 0) {
                e.Graphics.DrawRectangle(Pens.White, (SelectedTile % tilesPerRow) * 16, (SelectedTile / tilesPerRow) * 16, 16, 16);
            }
        }

        private void canvas_MouseDown(object sender, MouseEventArgs e) {
            Point transformed = scrollWrapper.TransformPoint(e.Location);
            int tile = Math.Min(tilesPerRow - 1, transformed.X / 16) + (transformed.Y / 16) * tilesPerRow;
            if (tile < tiles.Length && e.Button == MouseButtons.Left) {
                SelectedTile = tile;
                canvas.Invalidate();
            }
        }
    }
}
