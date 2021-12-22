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
    public partial class ColorSelector : UserControl
    {
        private const int SquaresPerRow = 16;

        private Color[] colors = null;
        public Color[] Colors {
            get {
                return colors;
            }
            set {
                colors = value;
                Repaint();
            }
        }

        private float squareSize = 1;

        private bool selecting = false;

        private Point selectionStart = new Point(-1, -1);
        public Point SelectionStart {
            get {
                return selectionStart;
            }
            set {
                selectionStart = value;
                if (!MultiSelect) {
                    selectionEnd = value;
                }
                Repaint();
                SelectionChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private Point selectionEnd = new Point(-1, -1);
        public Point SelectionEnd {
            get {
                return selectionEnd;
            }
            set {
                selectionEnd = value;
                if (!MultiSelect) {
                    selectionStart = value;
                }
                Repaint();
                SelectionChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public bool MultiSelect { get; set; } = true;

        public bool SelectionExists => SelectionStart.X >= 0;
        public int SelectionStartIndex => PointToIndex(SelectionStart);
        public Point SelectionMin => new Point(Math.Min(SelectionStart.X, SelectionEnd.X), Math.Min(SelectionStart.Y, SelectionEnd.Y));
        public Point SelectionMax => new Point(Math.Max(SelectionStart.X, SelectionEnd.X), Math.Max(SelectionStart.Y, SelectionEnd.Y));

        public event EventHandler SelectionChanged;

        public ColorSelector() {
            InitializeComponent();
        }

        public void Repaint() {
            canvas.Invalidate();
        }

        public void SelectAll() {
            SelectionStart = IndexToPoint(0);
            SelectionEnd = IndexToPoint(colors.Length - 1);
        }

        public void SelectNone() {
            SelectionStart = new Point(-1, -1);
            SelectionEnd = new Point(-1, -1);
        }
        
        private void canvas_SizeChanged(object sender, EventArgs e) {
            squareSize = Width / (float)SquaresPerRow;
            Repaint();
        }

        private void canvas_Paint(object sender, PaintEventArgs e) {
            if (colors != null) {
                for (int i = 0; i < colors.Length; i++) {
                    RectangleF rect = new RectangleF(squareSize * (i % SquaresPerRow), squareSize * (i / SquaresPerRow), squareSize, squareSize);
                    if (colors[i].A < 255) {
                        e.Graphics.FillRectangle(SNESGraphics.TransparencyGridBrush1, rect);
                        e.Graphics.FillRectangle(SNESGraphics.TransparencyGridBrush2, new RectangleF(rect.X, rect.Y, rect.Width / 2, rect.Height / 2));
                        e.Graphics.FillRectangle(SNESGraphics.TransparencyGridBrush2, new RectangleF(rect.X + rect.Width / 2, rect.Y + rect.Height / 2, rect.Width / 2, rect.Height / 2));
                    }
                    using (Brush b = new SolidBrush(colors[i])) {
                        e.Graphics.FillRectangle(b, rect);
                    }
                }
                if (SelectionExists) {
                    Point selectionMin = SelectionMin;
                    Point selectionMax = SelectionMax;
                    RectangleF selectionRect = new RectangleF(selectionMin.X * squareSize, selectionMin.Y * squareSize,
                        (selectionMax.X - selectionMin.X + 1) * squareSize, (selectionMax.Y - selectionMin.Y + 1) * squareSize);
                    e.Graphics.DrawRectangle(Pens.Black, selectionRect.X, selectionRect.Y, selectionRect.Width - 1, selectionRect.Height - 1);
                    e.Graphics.DrawRectangle(Pens.White, selectionRect.X + 1, selectionRect.Y + 1, selectionRect.Width - 3, selectionRect.Height - 3);
                }
            }
        }

        public Point IndexToPoint(int i) {
            return new Point(i % SquaresPerRow, i / SquaresPerRow);
        }

        public int PointToIndex(Point p) {
            return p.Y * SquaresPerRow + p.X;
        }

        private Point MouseToPoint(int x, int y) {
            return new Point((int)Math.Floor(x / squareSize), (int)Math.Floor(y / squareSize));
        }

        private Point ClampPoint(Point p) {
            return new Point(Math.Max(0, Math.Min(SquaresPerRow - 1, p.X)),
                Math.Max(0, Math.Min(colors.Length / SquaresPerRow - 1, p.Y)));
        }

        private void canvas_MouseDown(object sender, MouseEventArgs e) {
            Point p = MouseToPoint(e.X, e.Y);
            Point clamped = ClampPoint(p);
            if (colors != null && clamped == p) {
                if (MultiSelect) {
                    selecting = true;
                }
                SelectionStart = clamped;
                SelectionEnd = clamped;
                Repaint();
            }
        }

        private void canvas_MouseMove(object sender, MouseEventArgs e) {
            if (selecting) {
                Point p = ClampPoint(MouseToPoint(e.X, e.Y));
                if (p != SelectionEnd) {
                    SelectionEnd = p;
                    Repaint();
                }
            }
        }

        private void canvas_MouseUp(object sender, MouseEventArgs e) {
            selecting = false;
        }
    }
}
