using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Necrofy
{
    class GraphicsSelectTool : GraphicsTool
    {
        public GraphicsSelectTool(GraphicsEditor editor) : base(editor) {
            AddSubTool(new RectangleSelectTool(editor));
        }

        private class RectangleSelectTool : MapRectangleSelectTool
        {
            private readonly GraphicsEditor editor;

            public RectangleSelectTool(GraphicsEditor editor) : base(editor) {
                this.editor = editor;
            }

            protected override bool SupportsSnap => true;

            protected override int GetSnapAmount() {
                return editor.tileSize * 8;
            }
        }
    }
}
