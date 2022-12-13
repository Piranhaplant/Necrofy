using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Necrofy
{
    class GraphicsSelectByColorTool : GraphicsTool
    {
        public GraphicsSelectByColorTool(GraphicsEditor editor) : base(editor) {
            AddSubTool(new SelectByColorTool(editor));
        }

        private class SelectByColorTool : MapTileSelectTool
        {
            private readonly GraphicsEditor editor;

            public SelectByColorTool(GraphicsEditor editor) : base(editor) {
                this.editor = editor;
                Status = "Click to select all pixels of the same color.";
            }

            protected override void SelectTiles(int tileX, int tileY) {
                ReadPixelData(editor, getPixel => {
                    if (getPixel(tileX, tileY, out byte color)) {
                        editor.Selection.SetAllPoints((x, y) => getPixel(x, y, out byte otherColor) && otherColor == color);
                    }
                });
            }
        }
    }
}
