using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Necrofy
{
    class MapPencilSelectTool : MapTool
    {
        private int prevX;
        private int prevY;
        private bool addingToSelection;

        public MapPencilSelectTool(MapEditor mapEditor) : base(mapEditor) {
            Status = "Click to select tiles. Hold alt to unselect tiles.";
        }
        
        public override void MouseDown(MapMouseEventArgs e) {
            prevX = -1;
            prevY = -1;
            addingToSelection = Control.ModifierKeys != Keys.Alt;
            MouseMove(e);
        }

        public override void MouseMove(MapMouseEventArgs e) {
            // TODO: use lines for selecting
            if (e.MouseIsDown && (e.TileX != prevX || e.TileY != prevY)) {
                mapEditor.Selection.SetPoint(e.TileX, e.TileY, addingToSelection);
            }
        }
    }
}
