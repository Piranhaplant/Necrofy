using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Necrofy
{
    class TilemapSelectTool : TilemapTool
    {
        public TilemapSelectTool(TilemapEditor editor) : base(editor) {
            AddSubTool(new MapRectangleSelectTool(editor));
        }
    }
}
