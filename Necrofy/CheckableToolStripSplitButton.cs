using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Necrofy
{
    class CheckableToolStripSplitButton : ToolStripSplitButton
    {
        private static readonly ProfessionalColorTable colorTable = new ProfessionalColorTable();
        
        public event EventHandler CheckedChanged;

        private bool _checked;
        public bool Checked {
            get {
                return _checked;
            }
            set {
                _checked = value;
                Invalidate();
                CheckedChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        
        protected override void OnPaint(PaintEventArgs e) {
            if (_checked) {
                using (Brush b = new LinearGradientBrush(ButtonBounds, colorTable.ButtonCheckedGradientBegin, colorTable.ButtonCheckedGradientEnd, LinearGradientMode.Vertical)) {
                    e.Graphics.FillRectangle(b, ButtonBounds);
                }

                using (Pen p = new Pen(colorTable.ButtonSelectedBorder)) {
                    e.Graphics.DrawRectangle(p, ButtonBounds.X, ButtonBounds.Y, ButtonBounds.Width - 1, ButtonBounds.Height - 1);
                }
            }
            base.OnPaint(e);
        }
    }
}
