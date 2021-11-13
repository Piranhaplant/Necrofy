using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Necrofy
{
    class GraphicsSelectTool : GraphicsTool
    {
        private const string DefaultStatus = "Click to create a rectangle selection. Hold ctrl to snap to grid. Hold shift to add to the selection. Hold alt to remove from the selection.";
        private const string DragStatus = "Rectangle: {0}x{1}";

        private bool selecting = false;

        public GraphicsSelectTool(GraphicsEditor editor) : base(editor) {
            UpdateStatus();
        }

        public override void MouseDown(MouseEventArgs e) {
            selecting = true;
            bool addToSelection = true;
            if (Control.ModifierKeys.HasFlag(Keys.Alt)) {
                addToSelection = false;
            } else if (!Control.ModifierKeys.HasFlag(Keys.Shift)) {
                editor.selection.Clear();
            }
            int snap = Control.ModifierKeys.HasFlag(Keys.Control) ? editor.tileSize * 8 : 1;
            editor.selection.StartRect(e.X, e.Y, addToSelection, snap);
            UpdateStatus();
        }

        public override void MouseMove(MouseEventArgs e) {
            if (selecting) {
                editor.selection.MoveRect(e.X, e.Y);
                UpdateStatus();
            }
        }

        public override void MouseUp(MouseEventArgs e) {
            editor.selection.EndRect();
            selecting = false;
            UpdateStatus();
        }

        public override void DoneBeingUsed() {
            if (selecting) {
                editor.selection.EndRect();
                selecting = false;
                UpdateStatus();
            }
        }
        
        private void UpdateStatus() {
            if (selecting) {
                Rectangle r = editor.selection.CurrentRectangle;
                if (r.Width == 0 || r.Height == 0) {
                    r = Rectangle.Empty;
                }
                Status = string.Format(DragStatus, r.Width, r.Height);
            } else {
                Status = DefaultStatus;
            }
        }
    }
}
