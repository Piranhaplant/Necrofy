using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Necrofy
{
    abstract class MapBrushTool : MapTool
    {
        private bool mouseDownFirstPoint = false;
        private bool selecting = false;
        private int prevX = int.MinValue;
        private int prevY = int.MinValue;

        public MapBrushTool(MapEditor mapEditor) : base(mapEditor) { }

        public override void MouseDown(MapMouseEventArgs e) {
            if (Control.ModifierKeys == Keys.Shift && prevX != int.MinValue) {
                selecting = false;
                DrawLine(prevX, prevY, e.TileX, e.TileY);
                prevX = e.TileX;
                prevY = e.TileY;
            } else {
                mouseDownFirstPoint = true;
                prevX = e.TileX;
                prevY = e.TileY;
                selecting = Control.ModifierKeys == Keys.Control;
                MouseMove(e);
            }
        }

        public override void MouseMove(MapMouseEventArgs e) {
            if (e.MouseIsDown && (e.TileX != prevX || e.TileY != prevY || mouseDownFirstPoint)) {
                mouseDownFirstPoint = false;
                if (selecting) {
                    SelectTile(e.TileX, e.TileY);
                } else {
                    DrawLine(prevX, prevY, e.TileX, e.TileY);
                }
                prevX = e.TileX;
                prevY = e.TileY;
            }
        }
        
        protected abstract void SelectTile(int x, int y);
        protected abstract void DrawLine(int x1, int y1, int x2, int y2);
    }
}
