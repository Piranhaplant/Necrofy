using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Necrofy
{
    class MapRectangleSelectTool : MapTool
    {
        private const string DefaultStatus = "Click to create a rectangle selection. Hold shift to add to the selection. Hold alt to remove from the selection.";
        private const string DefaultStatusSnap = "Click to create a rectangle selection. Hold ctrl to snap to grid. Hold shift to add to the selection. Hold alt to remove from the selection.";
        private const string DragStatus = "Rectangle: {0}x{1}";

        private bool selecting = false;

        public MapRectangleSelectTool(MapEditor mapEditor) : base(mapEditor) {
            UpdateStatus();
        }

        protected virtual bool SupportsSnap => false;
        
        protected virtual int GetSnapAmount() {
            return 1;
        }

        public override void MouseDown(MapMouseEventArgs e) {
            selecting = true;
            bool addToSelection = true;
            if (Control.ModifierKeys.HasFlag(Keys.Alt)) {
                addToSelection = false;
            } else if (!Control.ModifierKeys.HasFlag(Keys.Shift)) {
                mapEditor.Selection.Clear();
            }
            int snap = Control.ModifierKeys.HasFlag(Keys.Control) ? GetSnapAmount() : 1;
            mapEditor.Selection.StartRect(e.TileX, e.TileY, addToSelection, snap);
            UpdateStatus();
        }

        public override void MouseMove(MapMouseEventArgs e) {
            if (selecting) {
                mapEditor.Selection.MoveRect(e.TileX, e.TileY);
                UpdateStatus();
            }
        }

        public override void MouseUp(MapMouseEventArgs e) {
            EndSelection();
        }

        public override void DoneBeingUsed() {
            if (selecting) {
                EndSelection();
            }
        }

        private void EndSelection() {
            mapEditor.Selection.EndRect();
            selecting = false;
            UpdateStatus();
        }
        
        private void UpdateStatus() {
            if (selecting) {
                Rectangle r = mapEditor.Selection.CurrentRectangle;
                if (r.Width == 0 || r.Height == 0) {
                    r = Rectangle.Empty;
                }
                Status = string.Format(DragStatus, r.Width, r.Height);
            } else {
                Status = SupportsSnap ? DefaultStatusSnap : DefaultStatus;
            }
        }
    }
}
