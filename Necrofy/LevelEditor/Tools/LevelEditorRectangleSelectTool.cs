using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Necrofy
{
    class LevelEditorRectangleSelectTool : LevelEditorTileTool
    {
        public LevelEditorRectangleSelectTool(LevelEditor editor) : base(editor) {
            AddSubTool(new MapRectangleSelectTool(editor));
        }
        
        public override void TileChanged() {
            editor.FillSelection();
        }
    }
}
