using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Necrofy
{
    class LevelEditorPencilSelectTool : LevelEditorTileTool
    {
        public LevelEditorPencilSelectTool(LevelEditor editor) : base(editor) {
            AddSubTool(new MapPencilSelectTool(editor));
        }

        public override void TileChanged() {
            editor.FillSelection();
        }
    }
}
