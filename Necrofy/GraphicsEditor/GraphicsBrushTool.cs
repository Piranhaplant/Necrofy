using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Necrofy
{
    class GraphicsBrushTool : GraphicsTool
    {
        private bool drawing = false;
        private Point prevLocation = Point.Empty;

        public GraphicsBrushTool(GraphicsEditor editor) : base(editor) { }

        public override void MouseDown(MouseEventArgs e) {
            drawing = true;
            prevLocation = e.Location;
            MouseMove(e);
        }

        public override void MouseMove(MouseEventArgs e) {
            if (drawing) {
                editor.undoManager.Do(new PaintGraphicsAction(prevLocation.X, prevLocation.Y, e.Location.X, e.Location.Y, (byte)(editor.SelectedColor % 16)));
                prevLocation = e.Location;
            }
        }

        public override void MouseUp(MouseEventArgs e) {
            drawing = false;
            editor.undoManager.ForceNoMerge();
        }

        public override void DoneBeingUsed() {
            drawing = false;
        }
    }
}
