using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Necrofy
{
    class BucketFillTool : GraphicsTool
    {
        public BucketFillTool(GraphicsEditor editor) : base(editor) {
            AddSubTool(new SubTool(editor));
        }

        private class SubTool : MapTool
        {
            private readonly GraphicsEditor editor;
            private bool selecting = false;

            public SubTool(GraphicsEditor editor) : base(editor) {
                this.editor = editor;
                Status = "Click to fill areas of the same color. Hold Ctrl to select a color.";
            }

            public override void MouseDown(MapMouseEventArgs e) {
                if (Control.ModifierKeys == Keys.Control) {
                    selecting = true;
                    MouseMove(e);
                } else {
                    editor.undoManager.Do(new BucketFillGraphicsAction(e.X, e.Y, (byte)editor.SelectedColor));
                }
            }

            public override void MouseMove(MapMouseEventArgs e) {
                if (selecting) {
                    editor.SelectColorAtPoint(e.X, e.Y);
                }
            }

            public override void MouseUp(MapMouseEventArgs e) {
                selecting = false;
            }
        }
    }
}
