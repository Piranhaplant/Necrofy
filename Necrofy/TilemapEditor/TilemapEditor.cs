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
        private Region tileRegion;

        private bool showGrid = false;
        public bool transparency { get; private set; }
        private Hinting.Type hintingType = Hinting.Type.None;
        private Hinting hinting;

        private LoadedPalette palette;
        public Color[] Colors { get; private set; }

        private LoadedGraphics graphics;
        public Bitmap[] tiles { get; private set; }
        private int colorsPerPalette;

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

        public bool LockTileNum => lockTileNum.Checked;
        public bool LockPalette => lockPalette.Checked;
        public bool LockFlip => lockFlip.Checked;

        private readonly Dictionary<Keys, CheckBox> checkboxKeys;

        public override ToolStripGrouper.ItemSet ToolStripItemSet => ToolStripGrouper.ItemSet.Tilemap;

        public TilemapEditor(LoadedTilemap loadedTilemap, Project project) : base(8) {
            InitializeComponent();
            Disposed += TilemapEditor_Disposed;
            FormClosed += TilemapEditor_FormClosed;
            checkboxKeys = new Dictionary<Keys, CheckBox>() {
                { Keys.X, flipX }, { Keys.Y, flipY }, { Keys.Q, lockTileNum }, { Keys.W, lockPalette }, { Keys.E, lockFlip }
            };

            this.loadedTilemap = loadedTilemap;
            tilemap = new LoadedTilemap.Tile[loadedTilemap.tiles.Length];
            Array.Copy(loadedTilemap.tiles, tilemap, tilemap.Length);

            Title = loadedTilemap.tilemapName;

            AssetOptions.TilemapOptions options = project.settings.AssetOptions.GetOptions(AssetCategory.Tilemap, loadedTilemap.tilemapName) as AssetOptions.TilemapOptions;
            if (options != null) {
                tileWidth = options.width;
                transparency = options.transparency;
                hintingType = options.hinting;
            }
        }

        private void TilemapEditor_Disposed(object sender, EventArgs e) {
            DisposeTiles();
        }

        public override void Displayed() {
            base.Displayed();
            mainWindow.GetToolStripItem(ToolStripGrouper.ItemType.ViewTransparency).Checked = transparency;
            UpdateViewOptions();
            UpdateToolbar();
            UpdateSize(tileWidth); // Update the enabled state of the menu items
            UpdateHinting();
        }

        private void TilemapEditor_FormClosed(object sender, FormClosedEventArgs e) {
            SaveOptions();
        }

        protected override void DoSave(Project project) {
            Array.Copy(tilemap, loadedTilemap.tiles, tilemap.Length);
            loadedTilemap.Save(project);
            SaveOptions();
        }

        private void SaveOptions() {
            project.settings.AssetOptions.SetOptions(AssetCategory.Tilemap, loadedTilemap.tilemapName, new AssetOptions.TilemapOptions(tileWidth, transparency, hintingType));
        }

        private void UpdateViewOptions() {
            showGrid = mainWindow.GetToolStripItem(ToolStripGrouper.ItemType.ViewTilemapGrid).Checked;
        }

        private static readonly ToolStripGrouper.ItemType[] tileSelectedItems = new ToolStripGrouper.ItemType[] {
            ToolStripGrouper.ItemType.FlipHorizontally, ToolStripGrouper.ItemType.FlipVertically,
        };

        public void UpdateToolbar() {
            bool enabled = CurrentTool?.CanFlip ?? false;
            foreach (ToolStripGrouper.ItemType item in tileSelectedItems) {
                mainWindow.GetToolStripItem(item).Enabled = enabled;
            }
        }

        private static readonly Dictionary<Hinting.Type, ToolStripGrouper.ItemType> hintingItems = new Dictionary<Hinting.Type, ToolStripGrouper.ItemType>() {
            { Hinting.Type.None, ToolStripGrouper.ItemType.ViewHintingNone },
            { Hinting.Type.LevelTitle, ToolStripGrouper.ItemType.ViewHintingLevelTitle },
            { Hinting.Type.Tileset, ToolStripGrouper.ItemType.ViewHintingTileset },
        };

        private void UpdateHinting() {
            hinting = Hinting.ForType(hintingType);
            foreach (ToolStripGrouper.ItemType item in hintingItems.Values) {
                mainWindow.GetToolStripItem(item).Checked = false;
            }
            mainWindow.GetToolStripItem(hintingItems[hintingType]).Checked = true;
        }

        protected override UndoManager Setup() {
            SetupMapEditor(canvas, hScroll, vScroll);
            SetupTool(new TilemapBrushTool(this), ToolStripGrouper.ItemType.TilemapPaintBrush, Keys.P);
            SetupTool(new TilemapSelectTool(this), ToolStripGrouper.ItemType.TilemapRectangleSelect, Keys.R);
            SetupTool(new TilemapPencilSelectTool(this), ToolStripGrouper.ItemType.TilemapPencilSelect, Keys.C);
            Selection.Changed += Selection_Changed;

            UpdateSize(tileWidth);
            Zoom = 4.0f;
            ScrollWrapper.ScrollToPoint(0, 0);

            paletteSelector.LoadProject(project, AssetCategory.Palette, loadedTilemap.tilemapName);
            graphicsSelector.LoadProject(project, AssetCategory.Graphics, loadedTilemap.tilemapName);
            tilePicker.SelectedTile = 0;

            undoManager = new UndoManager<TilemapEditor>(mainWindow.UndoButton, mainWindow.RedoButton, this);
            return undoManager;
        }

        private void Selection_Changed(object sender, EventArgs e) {
            undoManager?.ForceNoMerge();
            UpdateToolbar();
        }

        private void UpdateSize(int newTileWidth) {
            int minWidth = 1;
            int maxWidth = tilemap.Length;
            tileWidth = Math.Max(minWidth, Math.Min(maxWidth, newTileWidth));
            mainWindow.GetToolStripItem(ToolStripGrouper.ItemType.ViewDecreaseWidth).Enabled = tileWidth > minWidth;
            mainWindow.GetToolStripItem(ToolStripGrouper.ItemType.ViewIncreaseWidth).Enabled = tileWidth < maxWidth;
            mainWindow.GetToolStripItem(ToolStripGrouper.ItemType.WidthLabel).Text = tileWidth.ToString();

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
            tileRegion?.Dispose();
            tileRegion = new Region(s.GetGraphicsPath());
        }
        
        public int GetLocationTileIndex(int tileX, int tileY) {
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

        public void FillSelection() {
            if (SelectionExists) {
                undoManager.Do(new FillTilemapSelectionAction(new LoadedTilemap.Tile(SelectedTile, SelectedPalette, FlipX, FlipY)));
            }
        }

        protected override void PaintMap(Graphics g) {
            if (tiles == null || Colors == null) {
                return;
            }
            RectangleF clipRect = GetVisibleArea();
            g.SetClip(tileRegion, CombineMode.Replace);

            if (transparency) {
                SNESGraphics.DrawTransparencyGrid(g, clipRect, Zoom);
            }

            for (int i = 0; i < tilemap.Length; i++) {
                LoadedTilemap.Tile t = tilemap[i];
                GetTileLocation(i, out int x, out int y);
                if (t.tileNum < tiles.Length && clipRect.IntersectsWith(new RectangleF(x * 8, y * 8, 8, 8))) {
                    SNESGraphics.DrawWithPlt(g, x * 8, y * 8, tiles[t.tileNum], Colors, t.palette * colorsPerPalette, colorsPerPalette, t.xFlip, t.yFlip);
                }
            }

            if (showGrid) {
                using (Pen pen = new Pen(Color.FromArgb(150, Color.White), 1 / Zoom)) {
                    DrawGrid(g, pen, clipRect, 8);
                }
            }

            g.ResetClip();

            hinting.Render(g, clipRect, Zoom);
        }

        protected override void PaintSelectionDrawRectangle(Graphics g, Rectangle r) {
            // Nothing to do
        }

        protected override void PaintSelection(Graphics g, GraphicsPath path) {
            g.FillPath(LevelEditor.selectionFillBrush, path);
            g.DrawPath(WhitePen, path);
            g.DrawPath(SelectionDashPen, path);
        }

        protected override void PaintSelectionEraser(Graphics g, Rectangle r) {
            g.FillRectangle(LevelEditor.eraserFillBrush, r);
            g.DrawRectangle(WhitePen, r);
            g.DrawRectangle(SelectionDashPen, r);
        }

        protected override void PaintExtras(Graphics g) {
            // Nothing to do
        }

        protected override void ToolChanged(TilemapTool currentTool) {
            // Nothing to do
        }

        private void canvas_KeyDown(object sender, KeyEventArgs e) {
            tilePicker.OnKeyDown(e.KeyCode);
            if (checkboxKeys.TryGetValue(e.KeyCode, out CheckBox checkbox)) {
                checkbox.Checked = !checkbox.Checked;
            }
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
            GraphicsAsset.Type? graphicsType = GraphicsAsset.GetGraphicsType(graphicsSelector.SelectedNameInfo);
            if (graphicsType != null) {
                if (graphics != null) {
                    graphics.Updated -= Graphics_Updated;
                }
                graphics = new LoadedGraphics(project, graphicsSelector.SelectedItem, (GraphicsAsset.Type)graphicsType);
                graphics.Updated += Graphics_Updated;
                LoadGraphics();
            }
        }

        private void Graphics_Updated(object sender, EventArgs e) {
            LoadGraphics();
        }

        private void DisposeTiles() {
            if (tiles != null) {
                foreach (Bitmap tile in tiles) {
                    tile.Dispose();
                }
            }
        }

        private void LoadGraphics() {
            DisposeTiles();
            tiles = SNESGraphics.RenderAllTiles(graphics);
            colorsPerPalette = graphics.Is2BPP ? 4 : 16;
            UpdateTilePicker();
            Repaint();
        }

        private void UpdateTilePicker() {
            if (tiles != null && Colors != null) {
                tilePicker.SetTiles(tiles, Colors, zoom: 2);
                tilePicker.ColorsPerPalette = colorsPerPalette;
            }
        }

        public override void ToolStripItemClicked(ToolStripGrouper.ItemType item) {
            base.ToolStripItemClicked(item);
            if (item == ToolStripGrouper.ItemType.ViewDecreaseWidth) {
                UpdateSize(tileWidth - 1);
            } else if (item == ToolStripGrouper.ItemType.ViewIncreaseWidth) {
                UpdateSize(tileWidth + 1);
            } else if (item == ToolStripGrouper.ItemType.FlipHorizontally) {
                CurrentTool?.Flip(true);
            } else if (item == ToolStripGrouper.ItemType.FlipVertically) {
                CurrentTool?.Flip(false);
            } else if (hintingItems.ContainsValue(item)) {
                hintingType = hintingItems.Where(pair => pair.Value == item).Select(pair => pair.Key).FirstOrDefault();
                UpdateHinting();
                Repaint();
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
            SelectedTileChanged();
        }

        private void flipY_CheckedChanged(object sender, EventArgs e) {
            tilePicker.FlipY = flipY.Checked;
            SelectedTileChanged();
        }
        
        private void tilePicker_SelectedTileChanged(object sender, EventArgs e) {
            SelectedTileChanged();
        }

        private void tilePicker_PaletteChanged(object sender, EventArgs e) {
            SelectedTileChanged();
        }

        private void SelectedTileChanged() {
            canvas.Focus();
            CurrentTool?.TileChanged();
        }

        private void lock_CheckedChanged(object sender, EventArgs e) {
            canvas.Focus();
        }
    }
}
