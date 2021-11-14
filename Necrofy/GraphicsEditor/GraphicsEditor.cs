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
    partial class GraphicsEditor : EditorWindow
    {
        private readonly LoadedGraphics graphics;
        public readonly GraphicsTileList tiles = new GraphicsTileList();

        private readonly ScrollWrapper scrollWrapper;
        public UndoManager<GraphicsEditor> undoManager;

        private int tileWidth = 16;
        private bool largeTileMode = false;
        public int tileSize { get; private set; } = 1; // Set to one for normal mode, set to 2 for large tile (16x16 tile) mode
        private bool showGrid = false;

        private LoadedPalette palette;
        private int selectedPalette = 0;

        public readonly TileSelection selection = new TileSelection(1, 1, scale: 1);
        private GraphicsPath selectionPath = null;
        private Rectangle selectionDrawRect = Rectangle.Empty;
        private Pen selectionBorderDashPen;

        public override ToolStripGrouper.ItemSet ToolStripItemSet => ToolStripGrouper.ItemSet.Graphics;

        private ToolManager<GraphicsTool> toolManager;

        public GraphicsEditor(LoadedGraphics graphics) {
            InitializeComponent();
            Disposed += GraphicsEditor_Disposed;
            Title = graphics.graphicsName;
            this.graphics = graphics;

            scrollWrapper = new ScrollWrapper(canvas, hScroll, vScroll);
            scrollWrapper.Scrolled += ScrollWrapper_Scrolled;
            selection.Changed += Selection_Changed;
            
            for (int i = 0; i < graphics.linearGraphics.Length; i++) {
                Bitmap tile = new Bitmap(8, 8, PixelFormat.Format8bppIndexed);
                BitmapData data = tile.LockBits(new Rectangle(Point.Empty, tile.Size), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);
                SNESGraphics.DrawTile(data, 0, 0, new LoadedTilemap.Tile(i, 0, false, false), graphics.linearGraphics);
                tile.UnlockBits(data);
                tiles.Add(tile);
            }
        }

        protected override UndoManager Setup() {
            UpdateSize(tileWidth);
            Zoom = 4.0f;
            scrollWrapper.ScrollToPoint(0, 0);

            toolManager = new ToolManager<GraphicsTool>(mainWindow);
            toolManager.ToolChanged += ToolManager_ToolChanged;
            toolManager.SetupTool(new GraphicsBrushTool(this), ToolStripGrouper.ItemType.GraphicsPaintbrush, Keys.P);
            toolManager.SetupTool(new GraphicsSelectTool(this), ToolStripGrouper.ItemType.GraphicsRectangleSelect, Keys.R);

            if (project.Assets.Root.FindFolder(Path.GetDirectoryName(graphics.graphicsName), out AssetTree.Folder folder)) {
                foreach (AssetTree.AssetEntry sibling in folder.Assets) {
                    if (sibling.Asset.Category == AssetCategory.Palette) {
                        paletteSelector.Items.Add(sibling.Asset.Name);
                    }
                }

                if (paletteSelector.Items.Count > 0) {
                    paletteSelector.SelectedIndex = 0;
                }
            }

            UpdateSelectionPen(selectionPenCancel.Token);

            undoManager = new UndoManager<GraphicsEditor>(mainWindow.UndoButton, mainWindow.RedoButton, this);
            return undoManager;
        }

        private void GraphicsEditor_Disposed(object sender, EventArgs e) {
            tiles.Dispose();
            selectionPenCancel.Cancel();
        }

        private CancellationTokenSource selectionPenCancel = new CancellationTokenSource();

        private async void UpdateSelectionPen(CancellationToken cancellation) {
            while (!cancellation.IsCancellationRequested) {
                if (selectionPath != null && selectionBorderDashPen != null) {
                    selectionBorderDashPen.DashOffset = (selectionBorderDashPen.DashOffset + 1) % 256;
                    Repaint();
                }
                await Task.Delay(100);
            }
        }

        public override void Displayed() {
            base.Displayed();
            mainWindow.GetToolStripItem(ToolStripGrouper.ItemType.ViewLargeTileMode).Checked = largeTileMode;
            UpdateViewOptions();
            UpdateSize(tileWidth); // Update the enabled state of the menu items
            toolManager.ChangeToSelectedTool();
        }

        private void ToolManager_ToolChanged(object sender, ToolManager<GraphicsTool>.ToolChangedEventArgs e) {
            canvas.IsMouseDown = false;
            e.PreviousTool?.DoneBeingUsed();
            Status = toolManager.currentTool.Status;
        }

        private void ScrollWrapper_Scrolled(object sender, EventArgs e) {
            GenerateMouseMove();
            Repaint();
        }

        public override bool CanCopy => SelectionExists;
        public override bool CanPaste => true;
        public override bool CanDelete => SelectionExists;
        public override bool HasSelection => true;
        public override bool CanZoom => true;

        public override void Copy() {
            
        }

        public override void Paste() {
            
        }

        public override void Delete() {
            undoManager.Do(new DeleteGraphicsAction());
        }

        public override void SelectAll() {
            selection.SetAllPoints((x, y) => true);
        }

        public override void SelectNone() {
            selection.Clear();
        }

        protected override void ZoomChanged() {
            selectionBorderDashPen?.Dispose();
            selectionBorderDashPen = CreateSelectionBorderDashPen();
            scrollWrapper.Zoom = Zoom;
        }

        private void Selection_Changed(object sender, EventArgs e) {
            selectionPath?.Dispose();
            selectionPath = selection.GetGraphicsPath();
            selectionDrawRect = selection.GetDrawRectangle();
            RaiseSelectionChanged();
            Repaint();
            undoManager?.ForceNoMerge();
        }

        public bool SelectionExists => selectionPath != null;

        public int SelectedColor => colorSelector.SelectionStart;

        private void UpdateSize(int newTileWidth) {
            int minWidth = 2;
            int maxWidth = (tiles.Count / 4) * 2;
            tileWidth = Math.Max(minWidth, Math.Min(maxWidth, newTileWidth));
            mainWindow.GetToolStripItem(ToolStripGrouper.ItemType.ViewDecreaseWidth).Enabled = tileWidth > minWidth;
            mainWindow.GetToolStripItem(ToolStripGrouper.ItemType.ViewIncreaseWidth).Enabled = tileWidth < maxWidth;
            
            int tilesPerRow = tileWidth * tileSize;
            int tileHeight = tiles.Count / tilesPerRow * tileSize;
            if (tiles.Count % tilesPerRow != 0) {
                tileHeight += tileSize;
            }
            scrollWrapper.SetClientSize(tileWidth * 8, tileHeight * 8);
            selection.Resize(0, 0, tileWidth * 8, tileHeight * 8);
        }

        private void UpdateViewOptions() {
            showGrid = mainWindow.GetToolStripItem(ToolStripGrouper.ItemType.ViewGraphicsGrid).Checked;
        }

        public void Repaint() {
            canvas.Invalidate();
        }

        public void SetStatus(string status) {
            Status = status;
        }

        public void GenerateMouseMove() {
            if (toolManager?.currentTool != null) {
                canvas.GenerateMouseMove();
            }
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

        private void canvas_Paint(object sender, PaintEventArgs e) {
            scrollWrapper.TransformGraphics(e.Graphics);
            Point topLeft = scrollWrapper.TransformPoint(Point.Empty);
            Point bottomRight = scrollWrapper.TransformPoint(new Point(canvas.Size));
            Rectangle clipRect = new Rectangle(topLeft, new Size(bottomRight.X - topLeft.X, bottomRight.Y - topLeft.Y));

            Dictionary<int, int> maxHeightForColumn = new Dictionary<int, int>();
            Dictionary<int, int> maxWidthForRow = new Dictionary<int, int>();

            for (int i = 0; i < tiles.Count; i++) {
                GetTileLocation(i, out int x, out int y);
                Rectangle tileRect = new Rectangle(x * 8, y * 8, 8, 8);
                if (tileRect.IntersectsWith(clipRect)) {
                    if (palette != null) {
                        SNESGraphics.DrawWithPlt(e.Graphics, tileRect.X, tileRect.Y, tiles.GetTemporarily(i), palette.colors, selectedPalette * 16, 16);
                    } else {
                        e.Graphics.DrawImage(tiles.GetTemporarily(i), tileRect.X, tileRect.Y);
                    }
                }
                maxHeightForColumn[x] = y + 1;
                maxWidthForRow[y] = x + 1;
            }

            if (showGrid) {
                int maxY = maxWidthForRow.Keys.Max();
                using (Pen pen = new Pen(Color.White, 1 / Zoom)) {
                    for (int x = tileSize; x < tileWidth; x += tileSize) {
                        e.Graphics.DrawLine(pen, x * 8, 0, x * 8, maxHeightForColumn[x] * 8);
                    }
                    for (int y = tileSize; y <= maxY; y += tileSize) {
                        e.Graphics.DrawLine(pen, 0, y * 8, maxWidthForRow[y] * 8, y * 8);
                    }
                }
                if (Zoom >= 8.0f) {
                    using (Pen pen = new Pen(Color.FromArgb(80, Color.White), 1 / Zoom)) {
                        for (int x = 1; x < tileWidth * 8; x++) {
                            e.Graphics.DrawLine(pen, x, 0, x, maxHeightForColumn[x / 8] * 8);
                        }
                        for (int y = 1; y < (maxY + 1) * 8; y++) {
                            e.Graphics.DrawLine(pen, 0, y, maxWidthForRow[y / 8] * 8, y);
                        }
                    }
                }
            }
            
            using (Pen selectionBorderPen = CreateSelectionBorderPen()) {
                if (selectionDrawRect != Rectangle.Empty) {
                    e.Graphics.DrawRectangle(selectionBorderPen, selectionDrawRect);
                }
                if (selectionPath != null) {
                    e.Graphics.DrawPath(selectionBorderPen, selectionPath);
                    e.Graphics.DrawPath(selectionBorderDashPen, selectionPath);
                }
            }
        }

        private void canvas_MouseDown(object sender, MouseEventArgs e) {
            if (TransformMouseArgs(e, out MouseEventArgs args)) {
                toolManager.currentTool.MouseDown(args);
            }
        }

        private void canvas_MouseUp(object sender, MouseEventArgs e) {
            if (TransformMouseArgs(e, out MouseEventArgs args)) {
                toolManager.currentTool.MouseUp(args);
            }
        }

        private void canvas_MouseMove(object sender, MouseEventArgs e) {
            if (TransformMouseArgs(e, out MouseEventArgs args)) {
                toolManager.currentTool.MouseMove(args);
            }
        }

        private bool TransformMouseArgs(MouseEventArgs e, out MouseEventArgs ret) {
            if (e.Button == MouseButtons.Left || !canvas.IsMouseDown) {
                Point transformed = scrollWrapper.TransformPoint(e.Location);
                ret = new MouseEventArgs(e.Button, e.Clicks, transformed.X, transformed.Y, e.Delta);
                return true;
            }
            ret = null;
            return false;
        }

        private void canvas_KeyDown(object sender, KeyEventArgs e) {
            toolManager.KeyDown(e.KeyData);
        }

        public override void ToolStripItemClicked(ToolStripGrouper.ItemType item) {
            if (toolManager.ToolStripItemClicked(item)) {
                return;
            } else if (item == ToolStripGrouper.ItemType.ViewDecreaseWidth) {
                UpdateSize(tileWidth - 2);
            } else if (item == ToolStripGrouper.ItemType.ViewIncreaseWidth) {
                UpdateSize(tileWidth + 2);
            }
        }

        private static HashSet<ToolStripGrouper.ItemType> viewOptionsItems = new HashSet<ToolStripGrouper.ItemType>() {
            ToolStripGrouper.ItemType.ViewGraphicsGrid,
        };

        public override void ToolStripItemCheckedChanged(ToolStripGrouper.ItemType item) {
            if (item == ToolStripGrouper.ItemType.ViewLargeTileMode && DockActive) {
                largeTileMode = mainWindow.GetToolStripItem(ToolStripGrouper.ItemType.ViewLargeTileMode).Checked;
                tileSize = largeTileMode ? 2 : 1;
                UpdateSize(tileWidth);
            } else if (viewOptionsItems.Contains(item)) {
                UpdateViewOptions();
                if (DockVisible) {
                    Repaint();
                }
            }
        }

        private void paletteSelector_SelectedIndexChanged(object sender, EventArgs e) {
            palette = new LoadedPalette(project, (string)paletteSelector.SelectedItem);
            colorSelector.Colors = palette.colors;
            if (colorSelector.SelectionStart < 0) {
                colorSelector.SelectionStart = 0;
                colorSelector.SelectionEnd = 0;
            } else {
                Repaint();
            }
        }

        private void colorSelector_SelectionChanged(object sender, EventArgs e) {
            if (colorSelector.SelectionStart >= 0) {
                selectedPalette = colorSelector.SelectionStart / 16;
                Repaint();
            }
        }
    }
}
