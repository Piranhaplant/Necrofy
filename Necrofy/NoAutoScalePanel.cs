using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Necrofy
{
    class NoAutoScalePanel : Panel
    {
        public NoAutoScalePanel() {
            ControlAdded += NoAutoScalePanel_ControlAdded;
            ControlRemoved += NoAutoScalePanel_ControlRemoved;
            FontChanged += NoAutoScalePanel_FontChanged;
        }

        private void NoAutoScalePanel_ControlAdded(object sender, ControlEventArgs e) {
            e.Control.ControlAdded += NoAutoScalePanel_ControlAdded;
            e.Control.ControlRemoved += NoAutoScalePanel_ControlRemoved;
            e.Control.FontChanged += NoAutoScalePanel_FontChanged;
            NoAutoScalePanel_FontChanged(e.Control, e);
        }

        private void NoAutoScalePanel_ControlRemoved(object sender, ControlEventArgs e) {
            e.Control.ControlAdded -= NoAutoScalePanel_ControlAdded;
            e.Control.ControlRemoved -= NoAutoScalePanel_ControlRemoved;
            e.Control.FontChanged -= NoAutoScalePanel_FontChanged;
        }

        private void NoAutoScalePanel_FontChanged(object sender, EventArgs e) {
            Control control = (Control)sender;
            if (control.Font.Unit != GraphicsUnit.Pixel) {
                control.Font = new Font(control.Font.FontFamily, control.Font.SizeInPoints * 96f / 72f, GraphicsUnit.Pixel);
            }
        }

        protected override void ScaleControl(SizeF factor, BoundsSpecified specified) {
            // Disable auto-size
        }

        protected override bool ScaleChildren => false;
    }
}
