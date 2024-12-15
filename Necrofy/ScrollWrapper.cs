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
    class ScrollWrapper
    {
        private readonly Canvas control;
        
        public event EventHandler Scrolled;

        private readonly Dimension xDimension;
        private readonly Dimension yDimension;

        private Point? hiddenCursorOriginalPosition = null;
        public int HiddenCursorTotalMoveX { get; private set; }
        public int HiddenCursorTotalMoveY { get; private set; }

        private bool expandingDrag = false;
        public bool ExpandingDrag {
            get {
                return expandingDrag;
            }
            set {
                if (value != expandingDrag) {
                    expandingDrag = value;
                    if (!expandingDrag) {
                        xDimension.UpdateSize();
                        yDimension.UpdateSize();
                    }
                }
            }
        }

        private float zoom = 1.0f;
        public float Zoom {
            get {
                return zoom;
            }
            set {
                if (value != zoom) {
                    zoom = value;
                    xDimension.SetZoom(zoom);
                    yDimension.SetZoom(zoom);
                }
            }
        }

        private bool centerContents = true;
        public bool CenterContents {
            get {
                return centerContents;
            }
            set {
                if (value != centerContents) {
                    centerContents = value;
                    xDimension.SetCenterContents(centerContents);
                    yDimension.SetCenterContents(centerContents);
                }
            }
        }

        public ScrollWrapper(Canvas control, HScrollBar hscroll, VScrollBar vscroll, bool autoSize = true) {
            this.control = control;

            xDimension = new Dimension(hscroll, () => Math.Max(0, control.Width));
            yDimension = new Dimension(vscroll, () => Math.Max(0, control.Height));
            xDimension.Scrolled += Dimension_Scrolled;
            yDimension.Scrolled += Dimension_Scrolled;

            if (autoSize) {
                control.SizeChanged += Control_SizeChanged;
            }
            control.MouseDown += Control_MouseDown;
            control.MouseMove += Control_MouseMove;
            control.MouseUp += Control_MouseUp;
            control.MouseWheel += Control_MouseWheel;
        }

        private void Dimension_Scrolled(object sender, EventArgs e) {
            Scrolled?.Invoke(this, e);
            control.GenerateMouseMove();
        }

        private class Dimension
        {
            private readonly ScrollBar scrollBar;
            private readonly Func<int> controlSize;

            private int clientSize;
            private float zoom = 1.0f;
            private bool centerContents = true;

            private int scaledClientSize;
            private int clientPosition;
            private int dragStart;

            public int Position { get; private set; }
            public int Padding { get; set; }

            public event EventHandler Scrolled;

            public Dimension(ScrollBar scrollBar, Func<int> controlSize) {
                this.scrollBar = scrollBar;
                this.controlSize = controlSize;
                scrollBar.ValueChanged += ScrollBar_ValueChanged;
            }

            private void ScrollBar_ValueChanged(object sender, EventArgs e) {
                UpdatePosition();
            }

            public void SetClientSize(int size) {
                clientSize = size;
                UpdateSize();
            }

            public void SetZoom(float zoom) {
                int center = (int)((controlSize() / 2 - Position) / this.zoom);
                this.zoom = zoom;
                UpdateSize();
                SetScrollBarValue((int)(center * zoom) - controlSize() / 2);
            }

            public void SetCenterContents(bool centerContents) {
                this.centerContents = centerContents;
                UpdateSize();
            }

            public void ScrollToPoint(int point) {
                if (scrollBar.Enabled) {
                    SetScrollBarValue((int)(point * zoom) - controlSize() / 2);
                }
            }

            private void SetScrollBarValue(int value) {
                scrollBar.Value = Math.Max(scrollBar.Minimum, Math.Min(scrollBar.GetMaximumValue(), value));
            }

            public void UpdateSize() {
                scaledClientSize = (int)(clientSize * zoom - 1);
                if (centerContents) {
                    clientPosition = Math.Max(0, (controlSize() - scaledClientSize) / 2);
                } else {
                    clientPosition = 0;
                }

                scrollBar.Minimum = 0;
                scrollBar.Maximum = Math.Max(controlSize() - 1, scaledClientSize);
                scrollBar.LargeChange = controlSize();
                scrollBar.Enabled = scaledClientSize > controlSize();
                SetScrollBarValue(scrollBar.Value);

                UpdatePosition();
            }

            public void UpdatePosition() {
                Position = -scrollBar.Value + clientPosition;
                Scrolled?.Invoke(this, EventArgs.Empty);
            }

            public void MouseDown(int position, MouseButtons button) {
                if (button == MouseButtons.Middle) {
                    dragStart = scrollBar.Value + position;
                }
            }

            /// <summary>Process mouse movement</summary>
            /// <param name="position">The mouse position in this dimension</param>
            /// <param name="button">The mouse button</param>
            /// <returns>Whether this dimension requires a scrolling drag</returns>
            public bool MouseMove(int position, MouseButtons button) {
                if (button == MouseButtons.Middle) {
                    SetScrollBarValue(dragStart - position);
                } else if (button == MouseButtons.Left && (position < 0 || position > controlSize())) {
                    return true;
                }
                return false;
            }

            public void MouseUp(int position, MouseButtons button) {
                
            }

            /// <summary>Process a scrolling drag frame</summary>
            /// <param name="position">The mouse position in this dimension</param>
            /// <returns>Whether this dimension is still in scrolling drag</returns>
            public bool ScrollingDrag(int position, bool expanding) {
                int delta = 0;

                if (position < 0) {
                    delta = -1;
                } else if (position > controlSize()) {
                    delta = 1;
                }

                if (delta != 0) {
                    int newValue = scrollBar.Value + delta * 4;
                    if (expanding) {
                        if (newValue < scrollBar.Minimum) {
                            scrollBar.Minimum = newValue;
                            scrollBar.Enabled = true;
                        } else if (newValue > scrollBar.GetMaximumValue()) {
                            scrollBar.Maximum += newValue - scrollBar.GetMaximumValue();
                            scrollBar.Enabled = true;
                        }
                        UpdatePosition();
                    }
                    SetScrollBarValue(newValue);
                    return true;
                }
                return false;
            }

            /// <summary>Process a mouse wheel</summary>
            /// <param name="delta">The wheel delta</param>
            /// <returns>Whether the wheel event was used</returns>
            public bool MouseWheel(int delta) {
                if (scrollBar.Enabled) {
                    SetScrollBarValue(scrollBar.Value - 61 * Math.Sign(delta));
                    return true;
                }
                return false;
            }

            public float TransformPosition(float pos) {
                return ((pos - Position) / zoom) - Padding;
            }
            
            public int GetViewCenter() {
                return (int)Math.Round(TransformPosition(controlSize() / 2));
            }
        }

        public void SetPadding(int x, int y) {
            xDimension.Padding = x;
            yDimension.Padding = y;
        }

        public void SetClientSize(int width, int height) {
            xDimension.SetClientSize(width);
            yDimension.SetClientSize(height);
        }
        
        public void ScrollToPoint(int x, int y) {
            xDimension.ScrollToPoint(x);
            yDimension.ScrollToPoint(y);
        }

        public void TransformGraphics(Graphics g) {
            g.InterpolationMode = InterpolationMode.NearestNeighbor;
            g.PixelOffsetMode = PixelOffsetMode.Half;
            g.TranslateTransform(xDimension.Position + xDimension.Padding * zoom + 1f, yDimension.Position + yDimension.Padding * zoom + 1f);
            g.ScaleTransform(zoom, zoom);
        }

        public Point TransformPoint(Point p) {
            return new Point((int)Math.Floor(xDimension.TransformPosition(p.X)), (int)Math.Floor(yDimension.TransformPosition(p.Y)));
        }

        public PointF TransformPoint(PointF p) {
            return new PointF(xDimension.TransformPosition(p.X), yDimension.TransformPosition(p.Y));
        }
        
        public Point GetViewCenter() {
            return new Point(xDimension.GetViewCenter(), yDimension.GetViewCenter());
        }

        public void EnableHiddenCursor() {
            if (hiddenCursorOriginalPosition == null) {
                hiddenCursorOriginalPosition = Cursor.Position;
                HiddenCursorTotalMoveX = 0;
                HiddenCursorTotalMoveY = 0;
                Cursor.Hide();
                Cursor.Position = Screen.PrimaryScreen.Bounds.GetCenter();
            }
        }

        public void DisableHiddenCursor() {
            if (hiddenCursorOriginalPosition != null) {
                Cursor.Position = (Point)hiddenCursorOriginalPosition;
                Cursor.Show();
                hiddenCursorOriginalPosition = null;
            }
        }

        private void Control_SizeChanged(object sender, EventArgs e) {
            xDimension.UpdateSize();
            yDimension.UpdateSize();
        }
        
        void Control_MouseDown(object sender, MouseEventArgs e) {
            control.Focus();
            xDimension.MouseDown(e.X, e.Button);
            yDimension.MouseDown(e.Y, e.Button);
        }

        private CancellationTokenSource dragCancel = new CancellationTokenSource();

        void Control_MouseMove(object sender, MouseEventArgs e) {
            if (hiddenCursorOriginalPosition == null) {
                bool scrollingDrag = false;
                scrollingDrag |= xDimension.MouseMove(e.X, e.Button);
                scrollingDrag |= yDimension.MouseMove(e.Y, e.Button);
                if (scrollingDrag) {
                    DoScrollingDrag(dragCancel.Token);
                }
            } else {
                Point center = Screen.PrimaryScreen.Bounds.GetCenter();
                if (Cursor.Position != center) {
                    HiddenCursorTotalMoveX += Cursor.Position.X - center.X;
                    HiddenCursorTotalMoveY += Cursor.Position.Y - center.Y;
                    Cursor.Position = center;
                }
            }
        }

        private bool doingScrollingDrag = false;

        private async void DoScrollingDrag(CancellationToken cancellationToken) {
            if (doingScrollingDrag) {
                return;
            }
            doingScrollingDrag = true;
            while (true) {
                Point mousePos = control.PointToClient(Control.MousePosition);
                bool scrollingDrag = false;
                scrollingDrag |= xDimension.ScrollingDrag(mousePos.X, ExpandingDrag);
                scrollingDrag |= yDimension.ScrollingDrag(mousePos.Y, ExpandingDrag);
                if (!scrollingDrag || cancellationToken.IsCancellationRequested) {
                    break;
                }
                await Task.Delay(10);
            }
            doingScrollingDrag = false;
        }

        private void Control_MouseUp(object sender, MouseEventArgs e) {
            xDimension.MouseUp(e.X, e.Button);
            yDimension.MouseUp(e.Y, e.Button);
            dragCancel.Cancel();
            dragCancel = new CancellationTokenSource();
        }
        
        void Control_MouseWheel(object sender, MouseEventArgs e) {
            if (!yDimension.MouseWheel(e.Delta)) {
                xDimension.MouseWheel(e.Delta);
            }
        }
    }
}
