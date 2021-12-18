using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Necrofy
{
    abstract class MapEditor<T> : MapEditor where T : MapTool
    {
        private ToolManager<T> toolManager;
        private Canvas canvas;

        private int tileSize;
        private TileSelection selection;
        private ScrollWrapper scrollWrapper;

        public override int TileSize => tileSize;
        public override TileSelection Selection => selection;
        public override ScrollWrapper ScrollWrapper => scrollWrapper;

        // Padding is in pixels, not tiles
        private int padding = 0;
        public override int MapPadding {
            get {
                return padding;
            }
            set {
                padding = value;
                UpdateScrollWrapper();
            }
        }
        // Width/height are in tiles
        private int width = 1;
        private int height = 1;

        private GraphicsPath selectionPath = null;
        private Rectangle selectionEraserRect = Rectangle.Empty;
        private Rectangle selectionDrawRect = Rectangle.Empty;
        public Pen WhitePen { get; private set; }
        public Pen SelectionDashPen { get; private set; }

        public MapEditor(int tileSize) {
            this.tileSize = tileSize;
            Disposed += MapEditor_Disposed;
        }

        protected void SetupMapEditor(Canvas canvas, HScrollBar hscroll, VScrollBar vscroll) {
            this.canvas = canvas;

            canvas.Paint += Canvas_Paint;
            canvas.MouseDown += Canvas_MouseDown;
            canvas.MouseUp += Canvas_MouseUp;
            canvas.MouseMove += Canvas_MouseMove;
            canvas.PreviewKeyDown += Canvas_PreviewKeyDown;
            canvas.KeyDown += Canvas_KeyDown;
            canvas.KeyUp += Canvas_KeyUp;

            selection = new TileSelection(1, 1, tileSize);
            selection.Changed += Selection_Changed;

            toolManager = new ToolManager<T>(mainWindow);
            toolManager.ToolChanged += ToolManager_ToolChanged;

            scrollWrapper = new ScrollWrapper(canvas, hscroll, vscroll);
            scrollWrapper.Scrolled += ScrollWrapper_Scrolled;

            ZoomChanged();
            RepaintTimer(selectionPenCancel.Token);
            UpdateSelectionPen(selectionPenCancel.Token);
        }

        private void MapEditor_Disposed(object sender, EventArgs e) {
            selectionPenCancel.Cancel();
            selectionPath?.Dispose();
            WhitePen.Dispose();
            SelectionDashPen.Dispose();
        }

        public void SetupTool(T t, ToolStripGrouper.ItemType itemType, Keys shortcutKeys) {
            toolManager.SetupTool(t, itemType, shortcutKeys);
            t.StatusChanged += Tool_StatusChanged;
        }

        private void Tool_StatusChanged(object sender, EventArgs e) {
            if (ReferenceEquals(sender, CurrentTool)) {
                Status = CurrentTool.Status;
            }
        }

        public void ChangeTool(T t) {
            toolManager.ChangeTool(t);
        }

        public T CurrentTool => toolManager.currentTool;

        public override void ResizeMap(int width, int height) {
            ResizeMap(0, 0, width, height);   
        }

        public override void ResizeMap(int startX, int startY, int endX, int endY) {
            width = endX - startX;
            height = endY - startY;
            selection.Resize(startX, startY, endX, endY);
            UpdateScrollWrapper();
        }

        private void UpdateScrollWrapper() {
            scrollWrapper.SetPadding(padding, padding);
            scrollWrapper.SetClientSize(width * tileSize + padding * 2, height * tileSize + padding * 2);
        }
        
        public override bool SelectionExists => selectionPath != null;

        public override Pen CreateSelectionBorderPen() {
            return new Pen(Color.White, 1 / Zoom);
        }

        public override Pen CreateSelectionBorderDashPen() {
            Pen dashPen = new Pen(Color.Black, 1 / Zoom);
            if (Zoom >= 1.0f) {
                dashPen.DashPattern = new float[] { 4 / Zoom, 4 / Zoom };
            } else {
                dashPen.DashPattern = new float[] { 4, 4 };
            }
            return dashPen;
        }

        public override void Displayed() {
            base.Displayed();
            toolManager.ChangeToSelectedTool();
        }

        protected override void ZoomChanged() {
            base.ZoomChanged();
            WhitePen?.Dispose();
            SelectionDashPen?.Dispose();
            WhitePen = CreateSelectionBorderPen();
            SelectionDashPen = CreateSelectionBorderDashPen();

            scrollWrapper.Zoom = Zoom;
        }

        public override void ToolStripItemClicked(ToolStripGrouper.ItemType item) {
            base.ToolStripItemClicked(item);
            toolManager.ToolStripItemClicked(item);
        }

        private void Canvas_Paint(object sender, PaintEventArgs e) {
            scrollWrapper.TransformGraphics(e.Graphics);
            PaintMap(e.Graphics);
            if (selectionDrawRect != Rectangle.Empty) {
                PaintSelectionDrawRectangle(e.Graphics, selectionDrawRect);
            }
            if (selectionPath != null) {
                PaintSelection(e.Graphics, selectionPath);
            }
            if (selectionEraserRect != Rectangle.Empty) {
                PaintSelectionEraser(e.Graphics, selectionEraserRect);
            }
            toolManager.currentTool.Paint(e.Graphics);
            PaintExtras(e.Graphics);
        }

        private void Canvas_MouseDown(object sender, MouseEventArgs e) {
            if (TransformMouseArgs(e, out MapMouseEventArgs args)) {
                toolManager.currentTool.MouseDown(args);
            }
        }

        private void Canvas_MouseUp(object sender, MouseEventArgs e) {
            if (TransformMouseArgs(e, out MapMouseEventArgs args)) {
                toolManager.currentTool.MouseUp(args);
            }
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e) {
            if (TransformMouseArgs(e, out MapMouseEventArgs args)) {
                toolManager.currentTool.MouseMove(args);
            }
        }
        
        private bool TransformMouseArgs(MouseEventArgs e, out MapMouseEventArgs ret) {
            if (e.Button == MouseButtons.Left || !canvas.IsMouseDown) {
                Point transformed = scrollWrapper.TransformPoint(e.Location);
                ret = new MapMouseEventArgs(e.Button, e.Clicks, transformed.X, transformed.Y, e.Delta, tileSize, width, height, canvas.IsMouseDown);
                return true;
            }
            ret = null;
            return false;
        }

        private void Canvas_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e) {
            if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Left || e.KeyCode == Keys.Down || e.KeyCode == Keys.Right) {
                e.IsInputKey = true;
            }
        }

        private void Canvas_KeyDown(object sender, KeyEventArgs e) {
            if (!toolManager.KeyDown(e.KeyData)) {
                toolManager.currentTool.KeyDown(e);
            }
        }

        private void Canvas_KeyUp(object sender, KeyEventArgs e) {
            toolManager.currentTool.KeyUp(e);
        }

        private void ScrollWrapper_Scrolled(object sender, EventArgs e) {
            GenerateMouseMove();
            Repaint();
        }

        private void Selection_Changed(object sender, EventArgs e) {
            selectionPath?.Dispose();
            selectionPath = selection.GetGraphicsPath();
            selectionEraserRect = selection.GetEraserRectangle();
            selectionDrawRect = selection.GetDrawRectangle();
            RaiseSelectionChanged();
            Repaint();
        }

        private void ToolManager_ToolChanged(object sender, EventArgs e) {
            canvas.IsMouseDown = false;

            Status = CurrentTool.Status;
            ToolChanged(toolManager.currentTool);

            SetCursor(Cursors.Default);
            RaiseSelectionChanged();
            Repaint();
        }

        public override void GenerateMouseMove() {
            if (toolManager?.currentTool != null) {
                canvas.GenerateMouseMove();
            }
        }

        public override void SetCursor(Cursor cursor) {
            canvas.Cursor = cursor;
        }

        private CancellationTokenSource selectionPenCancel = new CancellationTokenSource();
        private bool repaintQueued = false;

        public override void Repaint() {
            repaintQueued = true;
        }

        private async void RepaintTimer(CancellationToken cancellation) {
            while (!cancellation.IsCancellationRequested) {
                await Task.Delay(5);
                if (repaintQueued) {
                    canvas.Invalidate();
                    repaintQueued = false;
                }
            }
        }

        private async void UpdateSelectionPen(CancellationToken cancellation) {
            while (!cancellation.IsCancellationRequested) {
                if (selectionPath != null && SelectionDashPen != null) {
                    SelectionDashPen.DashOffset = (SelectionDashPen.DashOffset + 1) % 256;
                    Repaint();
                }
                await Task.Delay(100);
            }
        }

        public override bool CanCopy => CurrentTool.CanCopy;
        public override bool CanPaste => CurrentTool.CanPaste;
        public override bool CanDelete => CurrentTool.CanDelete;
        public override bool HasSelection => CurrentTool.HasSelection;
        public override bool CanZoom => true;

        public override void Copy() {
            CurrentTool.Copy();
        }

        public override void Paste() {
            CurrentTool.Paste();
        }

        public override void Delete() {
            CurrentTool.Delete();
        }

        public override void SelectAll() {
            CurrentTool.SelectAll();
        }

        public override void SelectNone() {
            CurrentTool.SelectNone();
        }

        protected abstract void ToolChanged(T currentTool);
        protected abstract void PaintMap(Graphics g);
        protected abstract void PaintSelection(Graphics g, GraphicsPath path);
        protected abstract void PaintSelectionEraser(Graphics g, Rectangle r);
        protected abstract void PaintSelectionDrawRectangle(Graphics g, Rectangle r);
        protected abstract void PaintExtras(Graphics g);
    }

    abstract class MapEditor : EditorWindow
    {
        public abstract int TileSize { get; }
        public abstract TileSelection Selection { get; }
        public abstract ScrollWrapper ScrollWrapper { get; }
        public abstract int MapPadding { get; set; }
        public abstract bool SelectionExists { get; }

        public abstract void ResizeMap(int width, int height);
        public abstract void ResizeMap(int startX, int startY, int endX, int endY);
        public abstract Pen CreateSelectionBorderPen();
        public abstract Pen CreateSelectionBorderDashPen();
        public abstract void GenerateMouseMove();
        public abstract void SetCursor(Cursor cursor);
        public abstract void Repaint();

        public static void DrawLine(int x1, int y1, int x2, int y2, Action<int, int> setPixel) {
            if (Math.Abs(x2 - x1) > Math.Abs(y2 - y1)) {
                DrawLine2(x1, y1, x2, y2, (a, b) => setPixel(a, b));
            } else {
                DrawLine2(y1, x1, y2, x2, (a, b) => setPixel(b, a));
            }
        }

        private static void DrawLine2(int a1, int b1, int a2, int b2, Action<int, int> setPixel) {
            if (a1 > a2) {
                DrawLine2(a2, b2, a1, b1, setPixel);
            } else {
                double slope = a1 == a2 ? 0 : (double)(b2 - b1) / (a2 - a1);
                for (int a = a1; a <= a2; a++) {
                    int b = (int)Math.Round(slope * (a - a1) + b1);
                    setPixel(a, b);
                }
            }
        }
    }
}
