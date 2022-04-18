using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Necrofy
{
    partial class TilemapEditor : MapEditorDesigner<TilemapTool>
    {
        private readonly LoadedTilemap loadedTilemap;
        public LoadedTilemap.Tile[] tilemap;

        public UndoManager<TilemapEditor> undoManager { get; private set; }

        private int tileWidth = 32;
        private GraphicsPath tilePath;
        private Region tileRegion;

        private bool showGrid = false;
        private bool transparency = false;

        private LoadedPalette palette;
        private Color[] Colors;

        private LoadedGraphics graphics;
        private Bitmap[] tiles;

        public bool FlipX {
            get => flipX.Checked;
            set => flipX.Checked = value;
        }
        public bool FlipY {
            get => flipY.Checked;
            set => flipY.Checked = value;
        }
        public int SelectedTile {
            get => tilePicker.SelectedTile;
            set => tilePicker.SelectedTile = value;
        }
        public int SelectedPalette {
            get => tilePicker.Palette;
            set => tilePicker.Palette = value;
        }

        public override ToolStripGrouper.ItemSet ToolStripItemSet => ToolStripGrouper.ItemSet.Tilemap;

        public TilemapEditor(LoadedTilemap loadedTilemap) : base(8) {
            InitializeComponent();
            Disposed += TilemapEditor_Disposed;

            this.loadedTilemap = loadedTilemap;
            tilemap = new LoadedTilemap.Tile[loadedTilemap.tiles.Length];
            Array.Copy(loadedTilemap.tiles, tilemap, tilemap.Length);

            Title = loadedTilemap.tilemapName;
        }

        private void TilemapEditor_Disposed(object sender, EventArgs e) {
            foreach (Bitmap tile in tiles) {
                tile.Dispose();
            }
        }

        public override void Displayed() {
            base.Displayed();
            mainWindow.GetToolStripItem(ToolStripGrouper.ItemType.ViewTransparency).Checked = transparency;
            UpdateViewOptions();
            UpdateSize(tileWidth); // Update the enabled state of the menu items
        }

        protected override void DoSave(Project project) {
            Array.Copy(tilemap, loadedTilemap.tiles, tilemap.Length);
            loadedTilemap.Save(project);
        }

        private void UpdateViewOptions() {
            showGrid = mainWindow.GetToolStripItem(ToolStripGrouper.ItemType.ViewTilemapGrid).Checked;
        }

        protected override UndoManager Setup() {
            SetupMapEditor(canvas, hScroll, vScroll);
            SetupTool(new TilemapBrushTool(this), ToolStripGrouper.ItemType.TilemapPaintBrush, Keys.P);
            SetupTool(new TilemapSelectTool(this), ToolStripGrouper.ItemType.TilemapRectangleSelect, Keys.R);

            UpdateSize(tileWidth);
            Zoom = 4.0f;
            ScrollWrapper.ScrollToPoint(0, 0);

            paletteSelector.LoadProject(project, AssetCategory.Palette, loadedTilemap.tilemapName);
            graphicsSelector.LoadProject(project, AssetCategory.Graphics, loadedTilemap.tilemapName);
            tilePicker.SelectedTile = 0;

            undoManager = new UndoManager<TilemapEditor>(mainWindow.UndoButton, mainWindow.RedoButton, this);
            return undoManager;
        }

        private void UpdateSize(int newTileWidth) {
            int minWidth = 1;
            int maxWidth = tilemap.Length;
            tileWidth = Math.Max(minWidth, Math.Min(maxWidth, newTileWidth));
            mainWindow.GetToolStripItem(ToolStripGrouper.ItemType.ViewDecreaseWidth).Enabled = tileWidth > minWidth;
            mainWindow.GetToolStripItem(ToolStripGrouper.ItemType.ViewIncreaseWidth).Enabled = tileWidth < maxWidth;
            
            int tileHeight = tilemap.Length / tileWidth;
            if (tilemap.Length % tileWidth != 0) {
                tileHeight += 1;
            }
            ResizeMap(0, 0, tileWidth, tileHeight);

            TileSelection s = new TileSelection(tileWidth, tileHeight, scale: 8);
            for (int i = 0; i < tilemap.Length; i++) {
                GetTileLocation(i, out int x, out int y);
                s.SetPoint(x, y, true);
            }
            tilePath?.Dispose();
            tilePath = s.GetGraphicsPath();
            tileRegion?.Dispose();
            tileRegion = new Region(tilePath);
        }
        
        public int GetLocationTileNum(int tileX, int tileY) {
            int tileNum = tileY * tileWidth + tileX;
            if (tileX < 0 || tileX >= tileWidth || tileX < 0 || tileNum >= tilemap.Length) {
                tileNum = -1;
            }
            return tileNum;
        }

        public void GetTileLocation(int tileNum, out int x, out int y) {
            x = tileNum % tileWidth;
            y = tileNum / tileWidth;
        }

        protected override void PaintMap(Graphics g) {
            if (tiles == null || Colors == null) {
                return;
            }
            RectangleF clipRect = GetVisibleArea();
            g.SetClip(tileRegion, CombineMode.Replace);

            for (int i = 0; i < tilemap.Length; i++) {
                LoadedTilemap.Tile t = tilemap[i];
                GetTileLocation(i, out int x, out int y);
                if (t.tileNum < tiles.Length && clipRect.IntersectsWith(new RectangleF(x * 8, y * 8, 8, 8))) {
                    SNESGraphics.DrawWithPlt(g, x * 8, y * 8, tiles[t.tileNum], Colors, t.palette * 16, 16, t.xFlip, t.yFlip);
                }
            }

            if (showGrid) {
                for (float x = (float)Math.Floor(clipRect.Left / 8 + 1) * 8; x < clipRect.Right; x += 8) {
                    g.DrawLine(WhitePen, x, clipRect.Top, x, clipRect.Bottom);
                }
                for (float y = (float)Math.Floor(clipRect.Top / 8 + 1) * 8; y < clipRect.Bottom; y += 8) {
                    g.DrawLine(WhitePen, clipRect.Left, y, clipRect.Right, y);
                }
            }

            g.ResetClip();

            if (transparency) {
                g.DrawPath(WhitePen, tilePath);
            }
        }

        protected override void PaintSelectionDrawRectangle(Graphics g, Rectangle r) {
            g.DrawRectangle(WhitePen, r);
        }

        protected override void PaintSelection(Graphics g, GraphicsPath path) {
            g.DrawPath(WhitePen, path);
            g.DrawPath(SelectionDashPen, path);
        }

        protected override void PaintSelectionEraser(Graphics g, Rectangle r) {
            // Nothing to do
        }

        protected override void PaintExtras(Graphics g) {
            // Nothing to do
        }

        protected override void ToolChanged(TilemapTool currentTool) {
            // Nothing to do
        }

        private void paletteSelector_SelectedItemChanged(object sender, EventArgs e) {
            if (paletteSelector.SelectedItem != null) {
                if (palette != null) {
                    palette.Updated -= Palette_Updated;
                }
                palette = new LoadedPalette(project, paletteSelector.SelectedItem);
                palette.Updated += Palette_Updated;
                LoadPalette();
            }
        }

        private void Palette_Updated(object sender, EventArgs e) {
            LoadPalette();
        }

        private void LoadPalette() {
            if (palette == null) {
                return;
            }
            Colors = (Color[])palette.colors.Clone();
            if (transparency) {
                for (int i = 0; i < Colors.Length; i += 16) {
                    Colors[i] = Color.Transparent;
                }
            }
            UpdateTilePicker();
            Repaint();
        }

        private void graphicsSelector_SelectedItemChanged(object sender, EventArgs e) {
            if (graphicsSelector.SelectedItem != null) {
                if (graphics != null) {
                    graphics.Updated -= Graphics_Updated;
                }
                graphics = new LoadedGraphics(project, graphicsSelector.SelectedItem);
                graphics.Updated += Graphics_Updated;
                LoadGraphics();
            }
        }

        private void Graphics_Updated(object sender, EventArgs e) {
            LoadGraphics();
        }

        private void LoadGraphics() {
            if (tiles != null) {
                for (int i = 0; i < tiles.Length; i++) {
                    tiles[i].Dispose();
                }
            }

            tiles = new Bitmap[graphics.linearGraphics.Length];
            for (int i = 0; i < tiles.Length; i++) {
                Bitmap tile = new Bitmap(8, 8, PixelFormat.Format8bppIndexed);
                BitmapData data = tile.LockBits(new Rectangle(Point.Empty, tile.Size), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);
                SNESGraphics.DrawTile(data, 0, 0, new LoadedTilemap.Tile(i, 0, false, false), graphics.linearGraphics);
                tile.UnlockBits(data);
                tiles[i] = tile;
            }
            UpdateTilePicker();
            Repaint();
        }

        private void UpdateTilePicker() {
            if (tiles != null && Colors != null) {
                tilePicker.SetTiles(tiles, Colors, zoom: 2);
            }
        }

        public override void ToolStripItemClicked(ToolStripGrouper.ItemType item) {
            base.ToolStripItemClicked(item);
            if (item == ToolStripGrouper.ItemType.ViewDecreaseWidth) {
                UpdateSize(tileWidth - 1);
            } else if (item == ToolStripGrouper.ItemType.ViewIncreaseWidth) {
                UpdateSize(tileWidth + 1);
            }
        }

        private static HashSet<ToolStripGrouper.ItemType> viewOptionsItems = new HashSet<ToolStripGrouper.ItemType>() {
            ToolStripGrouper.ItemType.ViewTilemapGrid,
        };

        public override void ToolStripItemCheckedChanged(ToolStripGrouper.ItemType item) {
            base.ToolStripItemCheckedChanged(item);
            if (item == ToolStripGrouper.ItemType.ViewTransparency && DockActive) {
                transparency = mainWindow.GetToolStripItem(ToolStripGrouper.ItemType.ViewTransparency).Checked;
                LoadPalette();
            } else if (viewOptionsItems.Contains(item)) {
                UpdateViewOptions();
                if (DockVisible) {
                    Repaint();
                }
            }
        }

        private void flipX_CheckedChanged(object sender, EventArgs e) {
            tilePicker.FlipX = flipX.Checked;
            canvas.Focus();
        }

        private void flipY_CheckedChanged(object sender, EventArgs e) {
            tilePicker.FlipY = flipY.Checked;
            canvas.Focus();
        }

        private void clearFlip_Click(object sender, EventArgs e) {
            flipX.Checked = false;
            flipY.Checked = false;
        }
    }
}
