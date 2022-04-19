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
        private bool mouseDownFirstPoint = false;
        private int prevX;
        private int prevY;
        private bool addingToSelection;

        public MapPencilSelectTool(MapEditor mapEditor) : base(mapEditor) {
            Status = "Click to select tiles. Hold alt to unselect tiles.";
        }
        
        public override void MouseDown(MapMouseEventArgs e) {
            mouseDownFirstPoint = true;
            prevX = e.TileX;
            prevY = e.TileY;
            addingToSelection = Control.ModifierKeys != Keys.Alt;
            MouseMove(e);
        }

        public override void MouseMove(MapMouseEventArgs e) {
            if (e.MouseIsDown && (e.TileX != prevX || e.TileY != prevY || mouseDownFirstPoint)) {
                mouseDownFirstPoint = false;
                MapEditor.DrawLine(prevX, prevY, e.TileX, e.TileY, (x, y) => {
                    mapEditor.Selection.SetPoint(x, y, addingToSelection);
                });
                prevX = e.TileX;
                prevY = e.TileY;
            }
        }
    }
}
