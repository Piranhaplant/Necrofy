using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Necrofy
{
    class PencilSelectTool : TileTool
    {
        private int prevX;
        private int prevY;
        private bool addingToSelection;

        public PencilSelectTool(LevelEditor editor) : base(editor) {
            Status = "Click to select tiles. Hold alt to unselect tiles.";
        }

        protected override void MouseDown2(LevelMouseEventArgs e) {
            prevX = -1;
            prevY = -1;
            addingToSelection = Control.ModifierKeys != Keys.Alt;
            MouseMove(e);
        }

        protected override void MouseMove2(LevelMouseEventArgs e) {
            if (e.MouseIsDown && (e.TileX != prevX || e.TileY != prevY)) {
                editor.tileSelection.SetPoint(e.TileX, e.TileY, addingToSelection);
            }
        }

        public override void TileChanged() {
            editor.FillSelection();
        }
    }
}
