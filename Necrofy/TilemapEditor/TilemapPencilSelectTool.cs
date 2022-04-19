using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Necrofy
{
    class TilemapPencilSelectTool : TilemapTool
    {
        public TilemapPencilSelectTool(TilemapEditor editor) : base(editor) {
            AddSubTool(new MapPencilSelectTool(editor));
        }

        public override void TileChanged() {
            editor.FillSelection();
        }
    }
}
