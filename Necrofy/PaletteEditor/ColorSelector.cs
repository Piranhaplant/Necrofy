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

        private int selectionStart = -1;
        public int SelectionStart {
            get {
                return selectionStart;
            }
            set {
                selectionStart = value;
                Repaint();
                SelectionChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private int selectionEnd = -1;
        public int SelectionEnd {
            get {
                return selectionEnd;
            }
            set {
                selectionEnd = value;
                Repaint();
                SelectionChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public bool MultiSelect { get; set; } = true;

        public int SelectionMin => Math.Min(SelectionStart, SelectionEnd);
        public int SelectionMax => Math.Max(SelectionStart, SelectionEnd);

        public event EventHandler SelectionChanged;

        public ColorSelector() {
            InitializeComponent();
        }

        public void Repaint() {
            canvas.Invalidate();
        }
        
        private void canvas_SizeChanged(object sender, EventArgs e) {
            squareSize = Width / (float)SquaresPerRow;
            Repaint();
        }

        private void canvas_Paint(object sender, PaintEventArgs e) {
            if (colors != null) {
                int selectionMin = SelectionMin;
                int selectionMax = SelectionMax;
                for (int i = 0; i < colors.Length; i++) {
                    RectangleF rect = new RectangleF(squareSize * (i % SquaresPerRow), squareSize * (i / SquaresPerRow), squareSize, squareSize);
                    using (Brush b = new SolidBrush(colors[i])) {
                        e.Graphics.FillRectangle(b, rect);
                    }
                    if (i >= selectionMin && i <= selectionMax) {
                        e.Graphics.DrawRectangle(Pens.Black, rect.X, rect.Y, rect.Width - 1, rect.Height - 1);
                        e.Graphics.DrawRectangle(Pens.White, rect.X + 1, rect.Y + 1, rect.Width - 3, rect.Height - 3);
                    }
                }
            }
        }

        private int GetColorIndex(int x, int y) {
            return (int)(Math.Max(-1, Math.Min(SquaresPerRow - 1, Math.Floor(x / squareSize))) + SquaresPerRow * Math.Floor(y / squareSize));
        }

        private void canvas_MouseDown(object sender, MouseEventArgs e) {
            int index = GetColorIndex(e.X, e.Y);
            if (colors != null && index < colors.Length) {
                if (MultiSelect) {
                    selecting = true;
                }
                SelectionStart = index;
                SelectionEnd = index;
                Repaint();
            }
        }

        private void canvas_MouseMove(object sender, MouseEventArgs e) {
            if (selecting) {
                int index = Math.Max(0, Math.Min(colors.Length - 1, GetColorIndex(e.X, e.Y)));
                if (index != SelectionEnd) {
                    SelectionEnd = index;
                    Repaint();
                }
            }
        }

        private void canvas_MouseUp(object sender, MouseEventArgs e) {
            selecting = false;
        }
    }
}
