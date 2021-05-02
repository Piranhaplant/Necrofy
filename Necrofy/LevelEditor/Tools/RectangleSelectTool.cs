using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Necrofy
{
    class RectangleSelectTool : TileTool
    {
        private const string DefaultStatus = "Click to create a rectangle selection. Hold shift to add to the selection. Hold alt to remove from the selection.";
        private const string DragStatus = "Rectangle: {0}x{1}";

        private bool selecting = false;

        public RectangleSelectTool(LevelEditor editor) : base(editor) {
            UpdateStatus();
        }

        protected override void MouseDown2(LevelMouseEventArgs e) {
            selecting = true;
            bool addToSelection = true;
            if (Control.ModifierKeys == Keys.Alt) {
                addToSelection = false;
            } else if (Control.ModifierKeys != Keys.Shift) {
                editor.tileSelection.Clear();
            }
            editor.tileSelection.StartRect(e.TileX, e.TileY, addToSelection);
            UpdateStatus();
        }

        protected override void MouseMove2(LevelMouseEventArgs e) {
            if (selecting) {
                editor.tileSelection.MoveRect(e.TileX, e.TileY);
                UpdateStatus();
            }
        }

        protected override void MouseUp2(LevelMouseEventArgs e) {
            editor.tileSelection.EndRect();
            selecting = false;
            UpdateStatus();
        }

        protected override void DoneBeingUsed2() {
            if (selecting) {
                editor.tileSelection.EndRect();
                selecting = false;
                UpdateStatus();
            }
        }

        public override void TileChanged() {
            editor.FillSelection();
        }

        private void UpdateStatus() {
            if (selecting) {
                Rectangle r = editor.tileSelection.CurrentRectangle;
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
