using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace Necrofy
{
    public partial class SpriteTilePicker : UserControl
    {
        private IReadOnlyList<Bitmap> tiles = null;
        private Color[] colors = null;
        private readonly ScrollWrapper scrollWrapper;
        private readonly RadioButton[] paletteButtons;
        private Pen dashPen;

        private int zoom;
        private int tileSize;
        private int tilesPerRow;

        private int displayPalette = 0;
        public int DisplayPalette {
            get {
                return displayPalette;
            }
        }

        private int palette = 0;
        public int Palette {
            get {
                return palette;
            }
            set {
                if (value != palette) {
                    palette = value;
                    if (value >= 0 && value < paletteButtons.Length) {
                        displayPalette = palette;
                        UpdateUI(() => paletteButtons[palette].Checked = true);
                        Repaint();
                        PaletteChanged?.Invoke(this, EventArgs.Empty);
                    } else {
                        foreach (RadioButton button in paletteButtons) {
                            button.Checked = false;
                        }
                    }
                }
            }
        }

        private int colorsPerPalette = 16;
        public int ColorsPerPalette {
            get {
                return colorsPerPalette;
            }
            set {
                colorsPerPalette = value;
                Repaint();
            }
        }

        private bool flipX = false;
        public bool FlipX {
            get {
                return flipX;
            }
            set {
                flipX = value;
                Repaint();
            }
        }

        private bool flipY = false;
        public bool FlipY {
            get {
                return flipY;
            }
            set {
                flipY = value;
                Repaint();
            }
        }

        public delegate void SelectedTileChangedDelegate(object sender, EventArgs e);
        public event SelectedTileChangedDelegate SelectedTileChanged;

        public delegate void TileDoubleClickedDelegate(object sender, EventArgs e);
        public event TileDoubleClickedDelegate TileDoubleClicked;

        public delegate void PaletteChangedDelegate(object sender, EventArgs e);
        public event PaletteChangedDelegate PaletteChanged;

        private int selectedTile = -1;
        public int SelectedTile {
            get {
                return selectedTile;
            }
            set {
                selectedTile = value;
                SelectedTileChanged?.Invoke(this, EventArgs.Empty);
                Repaint();
            }
        }

        public SpriteTilePicker() {
            InitializeComponent();
            Disposed += SpriteTilePicker_Disposed;
            paletteButtons = new RadioButton[] { palette0Button, palette1Button, palette2Button, palette3Button, palette4Button, palette5Button, palette6Button, palette7Button };

            scrollWrapper = new ScrollWrapper(canvas, hScrollBar, vScrollBar, autoSize: false);
            scrollWrapper.Scrolled += ScrollWrapper_Scrolled;
            UpdateSelectionPen(selectionPenCancel.Token);
        }

        private void SpriteTilePicker_Disposed(object sender, EventArgs e) {
            selectionPenCancel.Cancel();
            dashPen.Dispose();
        }

        public void Repaint() {
            canvas.Repaint();
        }

        public void SetTiles(IReadOnlyList<Bitmap> tiles, Color[] colors, int zoom = 1) {
            this.tiles = tiles;
            this.colors = colors;
            this.zoom = zoom;
            dashPen = new Pen(Color.White, 1 / zoom);
            UpdateSize();
            scrollWrapper.ScrollToPoint(0, 0);
        }

        private void UpdateSize() {
            if (tiles == null) {
                return;
            }
            tileSize = tiles.Count > 0 ? tiles[0].Width : 1;
            tilesPerRow = canvas.Width / tileSize / zoom;
            int height = (int)Math.Ceiling(tiles.Count / (double)tilesPerRow);
            scrollWrapper.SetClientSize(canvas.Width / zoom, Math.Max(canvas.Height / zoom, height * tileSize));
            scrollWrapper.Zoom = zoom;
            Repaint();
        }

        private void ScrollWrapper_Scrolled(object sender, EventArgs e) {
            Repaint();
        }

        private void canvas_SizeChanged(object sender, EventArgs e) {
            UpdateSize();
        }

        private void canvas_Paint(object sender, PaintEventArgs e) {
            if (tiles == null) {
                return;
            }
            scrollWrapper.TransformGraphics(e.Graphics);
            PointF topLeft = scrollWrapper.TransformPoint(PointF.Empty);
            PointF bottomRight = scrollWrapper.TransformPoint(new PointF(canvas.Width, canvas.Height));

            for (int i = 0; i < tiles.Count; i++) {
                int x = (i % tilesPerRow) * tileSize;
                int y = (i / tilesPerRow) * tileSize;
                if (y > topLeft.Y - tileSize && y < bottomRight.Y) {
                    SNESGraphics.DrawWithPlt(e.Graphics, x, y, tiles[i], colors, displayPalette * colorsPerPalette, colorsPerPalette, flipX, flipY);
                }
            }
            if (SelectedTile >= 0) {
                using (Pen p = new Pen(Color.White, 1 / zoom)) {
                    Rectangle r = new Rectangle((SelectedTile % tilesPerRow) * tileSize, (SelectedTile / tilesPerRow) * tileSize, tileSize, tileSize);
                    e.Graphics.DrawRectangle(p, r);
                    e.Graphics.DrawRectangle(dashPen, r);
                }
            }
        }

        private void canvas_MouseDown(object sender, MouseEventArgs e) {
            if (tiles == null) {
                return;
            }
            Point transformed = scrollWrapper.TransformPoint(e.Location);
            int tile = Math.Min(tilesPerRow - 1, transformed.X / tileSize) + (transformed.Y / tileSize) * tilesPerRow;
            if (tile < tiles.Count && e.Button == MouseButtons.Left) {
                SelectedTile = tile;
                Repaint();
            }
        }

        private void canvas_MouseDoubleClick(object sender, MouseEventArgs e) {
            if (e.Button != MouseButtons.Left) {
                return;
            }
            if (selectedTile >= 0) {
                TileDoubleClicked?.Invoke(this, EventArgs.Empty);
            }
        }

        private int updatingUI = 0;
        private void UpdateUI(Action action) {
            updatingUI++;
            action();
            updatingUI--;
        }
        
        private void paletteButton_CheckedChanged(object sender, EventArgs e) {
            if (updatingUI == 0 && sender is RadioButton button && button.Checked) {
                Palette = Array.IndexOf(paletteButtons, button);
            }
        }

        public bool OnKeyDown(Keys keyCode) {
            if (Extensions.PaletteKeys.TryGetValue(keyCode, out int palette)) {
                Palette = palette;
                return true;
            }
            return false;
        }

        private CancellationTokenSource selectionPenCancel = new CancellationTokenSource();

        private async void UpdateSelectionPen(CancellationToken cancellation) {
            while (!cancellation.IsCancellationRequested) {
                if (SelectedTile >= 0 && dashPen != null) {
                    if (dashPen.Color == Color.White) {
                        dashPen.Color = Color.Black;
                    } else {
                        dashPen.Color = Color.White;
                    }
                    Repaint();
                }
                await Task.Delay(500);
            }
        }
    }
}
