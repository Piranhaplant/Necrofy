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
    public partial class ObjectBrowserControl : UserControl, RegionToolTip.IClient
    {
        private static readonly Color selectedObjectColor = Color.FromArgb(209, 230, 255);
        private static readonly Brush selectionBackgroundBrush = new SolidBrush(selectedObjectColor);
        private static readonly Pen selectionBorderPen = new Pen(Color.FromArgb(132, 172, 221));

        private static readonly Brush disabledOverlayBrush = new SolidBrush(Color.FromArgb(200, SystemColors.Control));
        private static readonly Brush selectedDisabledOverlayBrush = new SolidBrush(Color.FromArgb(200, selectedObjectColor));

        private static readonly Bitmap infoImage = Properties.Resources.information_small;
        private const int padding = 8;
        
        private readonly ScrollWrapper scrollWrapper;

        private readonly List<ObjectBrowserObject> objects = new List<ObjectBrowserObject>();

        private ObjectBrowserContents contents;
        public ObjectBrowserContents Contents {
            set {
                if (contents != null) {
                    contents.ScrollPosition = vScrollBar.Value;
                    contents.ObjectsChanged -= Contents_ObjectsChanged;
                    contents.SelectedIndexChanged -= Contents_SelectedIndexChanged;
                }
                contents = value;
                if (contents != null) {
                    contents.ObjectsChanged += Contents_ObjectsChanged;
                    contents.SelectedIndexChanged += Contents_SelectedIndexChanged;
                }
                LayoutObjects();
                if (contents != null && vScrollBar.Enabled) {
                    vScrollBar.Value = Math.Min(vScrollBar.Maximum, contents.ScrollPosition);
                }
            }
        }
        
        private void Contents_ObjectsChanged(object sender, ObjectsChangedEventArgs e) {
            if (e.LayoutChanged) {
                LayoutObjects();
            } else {
                canvas.Invalidate();
            }
            if (e.ScrollToTop) {
                scrollWrapper.ScrollToPoint(0, 0);
            }
        }

        private void Contents_SelectedIndexChanged(object sender, EventArgs e) {
            canvas.Invalidate();
        }

        public ObjectBrowserControl() {
            InitializeComponent();
            scrollWrapper = new ScrollWrapper(canvas, hScrollBar, vScrollBar, autoSize: false);
            scrollWrapper.Scrolled += scrollWrapper_Scrolled;
            LayoutObjects();
            new RegionToolTip(this, toolTip, canvas);
        }
        
        public void ScrollToSelection() {
            if (contents != null && contents.SelectedIndex >= 0 && contents.SelectedIndex < objects.Count) {
                Rectangle r = objects[contents.SelectedIndex].DisplayBounds;
                scrollWrapper.ScrollToPoint(r.X + r.Width / 2, r.Y + r.Height / 2);
            }
        }

        private void LayoutObjects() {
            objects.Clear();
            if (contents == null) {
                scrollWrapper.SetClientSize(1, 1);
                return;
            }
            int x = 0;
            int y = 0;
            int rowHeight = 0;
            bool itemPlaced = false;
            objects.AddRange(contents.Objects);
            foreach (ObjectBrowserObject obj in objects) {
                int width = obj.Size.Width;
                if (x + width + padding * 2 > canvas.Width && itemPlaced) {
                    x = 0;
                    y += rowHeight + padding * 2;
                    rowHeight = 0;
                }
                obj.DisplayBounds = new Rectangle(x, y, width + padding * 2, obj.Size.Height + padding * 2);
                x += width + padding * 2;
                rowHeight = Math.Max(rowHeight, obj.Size.Height);
                itemPlaced = true;
            }
            int totalHeight = y + rowHeight + padding * 2 + 1;
            scrollWrapper.SetClientSize(canvas.Width, Math.Max(canvas.Height, totalHeight));
        }

        private void canvas_Paint(object sender, PaintEventArgs e) {
            if (contents == null) {
                return;
            }
            scrollWrapper.TransformGraphics(e.Graphics);
            for (int i = 0; i < objects.Count; i++) {
                ObjectBrowserObject obj = objects[i];
                if (i == contents.SelectedIndex) {
                    e.Graphics.FillRectangle(selectionBackgroundBrush, obj.DisplayBounds);
                    e.Graphics.DrawRectangle(selectionBorderPen, obj.DisplayBounds);
                }
                contents.PaintObject(i, e.Graphics, obj.DisplayBounds.X + padding, obj.DisplayBounds.Y + padding);
                if (!obj.Enabled) {
                    Brush b = i == contents.SelectedIndex ? selectedDisabledOverlayBrush : disabledOverlayBrush;
                    e.Graphics.FillRectangle(b, obj.DisplayBounds.X + padding, obj.DisplayBounds.Y + padding, obj.Size.Width, obj.Size.Height);
                }
                if (obj.Description != null) {
                    e.Graphics.DrawImage(infoImage, obj.DisplayBounds.Right - infoImage.Width, obj.DisplayBounds.Bottom - infoImage.Height, infoImage.Width, infoImage.Height);
                }
            }
        }

        private void canvas_MouseDown(object sender, MouseEventArgs e) {
            if (contents == null || e.Button != MouseButtons.Left) {
                return;
            }
            Point transformed = scrollWrapper.TransformPoint(e.Location);
            for (int i = 0; i < objects.Count; i++) {
                if (objects[i].DisplayBounds.Contains(transformed.X, transformed.Y)) {
                    contents.SelectedIndex = i;
                    return;
                }
            }
            contents.SelectedIndex = -1;
        }

        private void canvas_MouseDoubleClick(object sender, MouseEventArgs e) {
            if (contents == null || e.Button != MouseButtons.Left) {
                return;
            }
            if (contents.SelectedIndex >= 0) {
                contents.HandleDoubleClick();
            }
        }

        void scrollWrapper_Scrolled(object sender, EventArgs e) {
            canvas.Invalidate();
        }

        private void canvas_SizeChanged(object sender, EventArgs e) {
            LayoutObjects();
        }

        public string GetToolTipAtPoint(Point p) {
            Point transformed = scrollWrapper.TransformPoint(p);

            foreach (ObjectBrowserObject obj in objects) {
                if (obj.DisplayBounds.Contains(transformed.X, transformed.Y)) {
                    return obj.Description;
                }
            }
            return null;
        }
    }
}
