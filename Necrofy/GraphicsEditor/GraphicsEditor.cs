using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Necrofy
{
    partial class GraphicsEditor : MapEditor<GraphicsTool>
    {
        private readonly LoadedGraphics graphics;
        public readonly GraphicsTileList tiles = new GraphicsTileList();
        public int colorsPerPalette { get; private set; }

        public UndoManager<GraphicsEditor> undoManager { get; private set; }

        private int tileWidth = 16;
        private bool largeTileMode = false;
        public int tileSize { get; private set; } = 1; // Set to one in normal mode, set to 2 in large tile (16x16 tile) mode
        private Region tileRegion;

        private bool showGrid = false;
        public bool transparency { get; private set; } = false;

        private LoadedPalette palette;
        public Color[] Colors { get; private set; }
        public int selectedPalette { get; private set; } = 0;

        public override ToolStripGrouper.ItemSet ToolStripItemSet => ToolStripGrouper.ItemSet.Graphics;

        public GraphicsEditor(LoadedGraphics graphics, Project project) : base(1) {
            InitializeComponent();
            Disposed += GraphicsEditor_Disposed;
            FormClosed += GraphicsEditor_FormClosed;
            
            Title = graphics.graphicsName;
            this.graphics = graphics;
            colorsPerPalette = graphics.Is2BPP ? 4 : 16;
            colorSelector.SquaresPerRow = colorsPerPalette;
            
            foreach (Bitmap tile in SNESGraphics.RenderAllTiles(graphics)) {
                tiles.Add(tile);
            }

            AssetOptions.GraphicsOptions options = project.settings.AssetOptions.GetOptions(AssetCategory.Graphics, graphics.graphicsName) as AssetOptions.GraphicsOptions;
            if (options != null) {
                tileWidth = options.width / 2 * 2;
                transparency = options.transparency;
                largeTileMode = options.largeTiles;
                tileSize = largeTileMode ? 2 : 1;
            }
        }

        protected override UndoManager Setup() {
            SetupMapEditor(canvas, hScroll, vScroll);
            SetupTool(new GraphicsBrushTool(this), ToolStripGrouper.ItemType.GraphicsPaintbrush, Keys.P);
            SetupTool(new GraphicsSelectTool(this), ToolStripGrouper.ItemType.GraphicsRectangleSelect, Keys.R);
            SetupTool(new BucketFillTool(this), ToolStripGrouper.ItemType.GraphicsBucketFill, Keys.B);

            UpdateSize(tileWidth);
            Zoom = 4.0f;
            ScrollWrapper.ScrollToPoint(0, 0);
            
            paletteSelector.LoadProject(project, AssetCategory.Palette, graphics.graphicsName);

            undoManager = new UndoManager<GraphicsEditor>(mainWindow.UndoButton, mainWindow.RedoButton, this);
            return undoManager;
        }

        private void GraphicsEditor_Disposed(object sender, EventArgs e) {
            tiles.Dispose();
            if (palette != null) {
                palette.Updated -= Palette_Updated;
            }
        }

        public override void Displayed() {
            base.Displayed();
            mainWindow.GetToolStripItem(ToolStripGrouper.ItemType.ViewLargeTileMode).Checked = largeTileMode;
            mainWindow.GetToolStripItem(ToolStripGrouper.ItemType.ViewTransparency).Checked = transparency;
            UpdateViewOptions();
            UpdateSize(tileWidth); // Update the enabled state of the menu items
        }

        private void GraphicsEditor_FormClosed(object sender, FormClosedEventArgs e) {
            SaveOptions();
        }

        protected override void DoSave(Project project) {
            graphics.Save(project, tiles);
            SaveOptions();
        }

        private void SaveOptions() {
            project.settings.AssetOptions.SetOptions(AssetCategory.Graphics, graphics.graphicsName, new AssetOptions.GraphicsOptions(tileWidth, transparency, largeTileMode));
        }
        
        public int SelectedColor => colorSelector.SelectionStart.X;

        private void UpdateSize(int newTileWidth) {
            int minWidth = 2;
            int maxWidth = (tiles.Count / 4) * 2;
            tileWidth = Math.Max(minWidth, Math.Min(maxWidth, newTileWidth));
            mainWindow.GetToolStripItem(ToolStripGrouper.ItemType.ViewDecreaseWidth).Enabled = tileWidth > minWidth;
            mainWindow.GetToolStripItem(ToolStripGrouper.ItemType.ViewIncreaseWidth).Enabled = tileWidth < maxWidth;
            mainWindow.GetToolStripItem(ToolStripGrouper.ItemType.WidthLabel).Text = tileWidth.ToString();
            
            int tilesPerRow = tileWidth * tileSize;
            int tileHeight = tiles.Count / tilesPerRow * tileSize;
            if (tiles.Count % tilesPerRow != 0) {
                tileHeight += tileSize;
            }
            ResizeMap(0, 0, tileWidth * 8, tileHeight * 8);

            TileSelection s = new TileSelection(tileWidth, tileHeight, scale: 8);
            for (int i = 0; i < tiles.Count; i++) {
                GetTileLocation(i, out int x, out int y);
                s.SetPoint(x, y, true);
            }
            tileRegion?.Dispose();
            tileRegion = new Region(s.GetGraphicsPath());
        }

        private void UpdateViewOptions() {
            showGrid = mainWindow.GetToolStripItem(ToolStripGrouper.ItemType.ViewGraphicsGrid).Checked;
        }
        
        public void SetSelectedColor(byte color) {
            colorSelector.SelectionStart = new Point(color, selectedPalette);
        }
        
        public int GetPixelTileNum(int x, int y) {
            int tileX = x / 8;
            int tileY = y / 8;
            int tileNum;
            if (largeTileMode) {
                tileNum = (tileX % 2) + (tileY % 2) * 2 + ((tileX / 2) + (tileY / 2) * (tileWidth / 2)) * 4;
            } else {
                tileNum = tileY * tileWidth + tileX;
            }
            if (x < 0 || tileX >= tileWidth || y < 0 || tileNum >= tiles.Count) {
                tileNum = -1;
            }
            return tileNum;
        }

        public void GetTileLocation(int tileNum, out int x, out int y) {
            if (largeTileMode) {
                x = ((tileNum / 4) * 2 + (tileNum % 2)) % tileWidth;
                y = (tileNum % 4) / 2 + (tileNum / (tileWidth * 2) * 2);
            } else {
                x = tileNum % tileWidth;
                y = tileNum / tileWidth;
            }
        }

        public void SelectColorAtPoint(int x, int y) {
            int tileNum = GetPixelTileNum(x, y);
            if (tileNum >= 0) {
                Bitmap bitmap = tiles.GetTemporarily(tileNum);
                BitmapData data = bitmap.LockBits(new Rectangle(x % 8, y % 8, 1, 1), ImageLockMode.ReadOnly, bitmap.PixelFormat);
                byte color = Marshal.ReadByte(data.Scan0);
                bitmap.UnlockBits(data);
                SetSelectedColor(color);
            }
        }

        protected override void PaintMap(Graphics g) {
            RectangleF clipRect = GetVisibleArea();
            g.SetClip(tileRegion, CombineMode.Replace);

            if (transparency) {
                SNESGraphics.DrawTransparencyGrid(g, clipRect, Zoom);
            }

            for (int i = 0; i < tiles.Count; i++) {
                GetTileLocation(i, out int x, out int y);
                if (clipRect.IntersectsWith(new RectangleF(x * 8, y * 8, 8, 8))) {
                    if (Colors != null) {
                        SNESGraphics.DrawWithPlt(g, x * 8, y * 8, tiles.GetTemporarily(i), Colors, selectedPalette * colorsPerPalette, colorsPerPalette);
                    } else {
                        g.DrawImage(tiles.GetTemporarily(i), x * 8, y * 8);
                    }
                }
            }

            if (showGrid) {
                DrawGrid(g, WhitePen, clipRect, tileSize * 8);
                if (Zoom >= 8.0f) {
                    using (Pen pen = new Pen(Color.FromArgb(80, Color.White), 1 / Zoom)) {
                        DrawGrid(g, pen, clipRect, 1);
                    }
                }
            }

            g.ResetClip();
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

        public override void ToolStripItemClicked(ToolStripGrouper.ItemType item) {
            base.ToolStripItemClicked(item);
            if (item == ToolStripGrouper.ItemType.ViewDecreaseWidth) {
                UpdateSize(tileWidth - 2);
            } else if (item == ToolStripGrouper.ItemType.ViewIncreaseWidth) {
                UpdateSize(tileWidth + 2);
            }
        }

        private static HashSet<ToolStripGrouper.ItemType> viewOptionsItems = new HashSet<ToolStripGrouper.ItemType>() {
            ToolStripGrouper.ItemType.ViewGraphicsGrid,
        };

        public override void ToolStripItemCheckedChanged(ToolStripGrouper.ItemType item) {
            base.ToolStripItemCheckedChanged(item);
            if (item == ToolStripGrouper.ItemType.ViewLargeTileMode && DockActive) {
                largeTileMode = mainWindow.GetToolStripItem(ToolStripGrouper.ItemType.ViewLargeTileMode).Checked;
                tileSize = largeTileMode ? 2 : 1;
                UpdateSize(tileWidth);
            } else if (item == ToolStripGrouper.ItemType.ViewTransparency && DockActive) {
                transparency = mainWindow.GetToolStripItem(ToolStripGrouper.ItemType.ViewTransparency).Checked;
                LoadPalette();
            } else if (viewOptionsItems.Contains(item)) {
                UpdateViewOptions();
                if (DockVisible) {
                    Repaint();
                }
            }
        }

        private void PaletteSelector_SelectedItemChanged(object sender, EventArgs e) {
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
            Colors = new Color[colorsPerPalette * 8];
            Array.Copy(palette.colors, Colors, Math.Min(palette.colors.Length, Colors.Length));
            if (transparency) {
                for (int i = 0; i < Colors.Length; i += colorsPerPalette) {
                    Colors[i] = Color.Transparent;
                }
            }
            colorSelector.Colors = Colors;
            if (!colorSelector.SelectionExists) {
                colorSelector.SelectionStart = Point.Empty;
            } else {
                Repaint();
            }
        }

        private void colorSelector_SelectionChanged(object sender, EventArgs e) {
            if (colorSelector.SelectionExists) {
                selectedPalette = colorSelector.SelectionStart.Y;
                Repaint();
            }
        }
        
        protected override void ToolChanged(GraphicsTool currentTool) {
            // Nothing to do
        }
    }
}
