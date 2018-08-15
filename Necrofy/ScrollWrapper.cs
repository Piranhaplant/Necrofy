using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Necrofy
{
    class ScrollWrapper
    {
        private readonly Control control;
        private readonly HScrollBar hscroll;
        private readonly VScrollBar vscroll;

        public delegate void ScrollDelegate();
        public event ScrollDelegate Scrolled;

        public int LeftPosition { get; private set; }
        public int TopPosition { get; private set; }

        private int clientWidth;
        private int clientHeight;

        public ScrollWrapper(Control control, HScrollBar hscroll, VScrollBar vscroll) {
            this.control = control;
            this.hscroll = hscroll;
            this.vscroll = vscroll;

            control.SizeChanged += new EventHandler(UpdateSize);
            hscroll.ValueChanged += new EventHandler(UpdatePosition);
            vscroll.ValueChanged += new EventHandler(UpdatePosition);
        }

        public void SetClientSize(int width, int height) {
            clientWidth = width;
            clientHeight = height;
            TopPosition = 0;
            LeftPosition = 0;
            UpdateSize();
        }

        private void UpdateSize(object sender, EventArgs e) {
            UpdateSize();
        }

        private void UpdateSize() {
            if (clientWidth <= control.Width) {
                hscroll.Enabled = false;
                LeftPosition = (control.Width - clientWidth) / 2;
            } else {
                hscroll.Enabled = true;
                hscroll.Minimum = 0;
                hscroll.Maximum = clientWidth;
                hscroll.LargeChange = control.Width;
                hscroll.Value = Math.Max(hscroll.Minimum, Math.Min(hscroll.Maximum - hscroll.LargeChange, hscroll.Value));
                LeftPosition = -hscroll.Value;
            }

            if (clientHeight <= control.Height) {
                vscroll.Enabled = false;
                TopPosition = (control.Height - clientHeight) / 2;
            } else {
                vscroll.Enabled = true;
                vscroll.Minimum = 0;
                vscroll.Maximum = clientHeight;
                vscroll.LargeChange = control.Height;
                vscroll.Value = Math.Max(vscroll.Minimum, Math.Min(vscroll.Maximum - vscroll.LargeChange, vscroll.Value));
                TopPosition = -vscroll.Value;
            }

            if (Scrolled != null) {
                Scrolled.Invoke();
            }
        }

        void UpdatePosition(object sender, EventArgs e) {
            UpdatePosition();
        }

        private void UpdatePosition() {
            if (vscroll.Enabled) {
                TopPosition = -vscroll.Value;
            }
            if (hscroll.Enabled) {
                LeftPosition = -hscroll.Value;
            }

            if (Scrolled != null) {
                Scrolled.Invoke();
            }
        }
    }
}
