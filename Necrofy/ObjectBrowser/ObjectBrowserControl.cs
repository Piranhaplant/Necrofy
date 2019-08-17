using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Necrofy
{
    public partial class ObjectBrowserControl : UserControl
    {
        private static readonly Brush selectionBackgroundBrush = new SolidBrush(Color.FromArgb(209, 230, 255));
        private static readonly Pen selectionBorderPen = new Pen(Color.FromArgb(132, 172, 221));
        private const int padding = 8;
        
        private readonly ScrollWrapper scrollWrapper;

        private readonly List<Rectangle> objectRects = new List<Rectangle>();

        private ObjectBrowserContents contents;
        public ObjectBrowserContents Contents {
            set {
                if (contents != null) {
                    contents.Changed -= Contents_Changed;
                }
                contents = value;
                if (contents != null) {
                    contents.Changed += Contents_Changed;
                }
                LayoutObjects();
            }
        }

        private void Contents_Changed(object sender, EventArgs e) {
            LayoutObjects();
        }

        public ObjectBrowserControl() {
            InitializeComponent();
            scrollWrapper = new ScrollWrapper(canvas, hScrollBar, vScrollBar, autoSize: false);
            scrollWrapper.Scrolled += scrollWrapper_Scrolled;
            LayoutObjects();
        }

        public void ScrollToSelection() {
            if (contents != null && contents.SelectedIndex >= 0 && contents.SelectedIndex < objectRects.Count) {
                Rectangle r = objectRects[contents.SelectedIndex];
                scrollWrapper.ScrollToPoint(r.X + r.Width / 2, r.Y + r.Height / 2);
            }
        }

        private void LayoutObjects() {
            objectRects.Clear();
            if (contents == null) {
                scrollWrapper.SetClientSize(1, 1);
                return;
            }
            int x = 0;
            int y = 0;
            int rowHeight = 0;
            bool itemPlaced = false;
            foreach (Size obj in contents.Objects) {
                int width = obj.Width;
                if (contents.ListLayout) {
                    width = canvas.Width;
                }
                if (x + width + padding * 2 > canvas.Width && itemPlaced) {
                    x = 0;
                    y += rowHeight + padding * 2;
                    rowHeight = 0;
                }
                objectRects.Add(new Rectangle(x, y, width + padding * 2, obj.Height + padding * 2));
                x += width + padding * 2;
                rowHeight = Math.Max(rowHeight, obj.Height);
                itemPlaced = true;
            }
            int totalHeight = y + rowHeight + padding * 2;
            scrollWrapper.SetClientSize(canvas.Width, Math.Max(canvas.Height, totalHeight));
        }

        private void canvas_Paint(object sender, PaintEventArgs e) {
            if (contents == null) {
                return;
            }
            e.Graphics.TranslateTransform(scrollWrapper.LeftPosition, scrollWrapper.TopPosition);
            for (int i = 0; i < objectRects.Count; i++) {
                Rectangle objectRect = objectRects[i];
                if (i == contents.SelectedIndex) {
                    e.Graphics.FillRectangle(selectionBackgroundBrush, objectRect);
                    e.Graphics.DrawRectangle(selectionBorderPen, objectRect);
                }
                bool relayout = contents.PaintObject(i, e.Graphics, objectRect.X + padding, objectRect.Y + padding);
                if (relayout) {
                    LayoutObjects();
                    return;
                }
            }
        }

        private void canvas_MouseDown(object sender, MouseEventArgs e) {
            if (contents == null || e.Button != MouseButtons.Left) {
                return;
            }
            int x = e.X - scrollWrapper.LeftPosition;
            int y = e.Y - scrollWrapper.TopPosition;
            for (int i = 0; i < objectRects.Count; i++) {
                if (objectRects[i].Contains(x, y)) {
                    contents.SelectedIndex = i;
                    canvas.Invalidate();
                    return;
                }
            }
        }

        void scrollWrapper_Scrolled(object sender, EventArgs e) {
            canvas.Invalidate();
        }

        private void canvas_SizeChanged(object sender, EventArgs e) {
            LayoutObjects();
        }
    }
}
