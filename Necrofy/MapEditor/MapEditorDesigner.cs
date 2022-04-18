using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Necrofy
{
    // Version of MapEditor which can be used as a base class and still allow the Visual Studio designer to function
    class MapEditorDesigner<T> : MapEditor<T> where T : MapTool
    {
        public MapEditorDesigner() : base(1) {

        }

        public MapEditorDesigner(int tileSize) : base(tileSize) {

        }

        protected override void PaintExtras(Graphics g) {
            throw new NotImplementedException();
        }

        protected override void PaintMap(Graphics g) {
            throw new NotImplementedException();
        }

        protected override void PaintSelection(Graphics g, GraphicsPath path) {
            throw new NotImplementedException();
        }

        protected override void PaintSelectionDrawRectangle(Graphics g, Rectangle r) {
            throw new NotImplementedException();
        }

        protected override void PaintSelectionEraser(Graphics g, Rectangle r) {
            throw new NotImplementedException();
        }

        protected override void ToolChanged(T currentTool) {
            throw new NotImplementedException();
        }
    }
}
