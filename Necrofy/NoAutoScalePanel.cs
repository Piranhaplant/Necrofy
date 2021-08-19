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
        protected override void ScaleControl(SizeF factor, BoundsSpecified specified) {
            // Disable auto-size
        }

        protected override bool ScaleChildren => false;
    }
}
