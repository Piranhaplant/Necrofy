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

        public PencilSelectTool(LevelEditor editor) : base(editor) { }

        protected override void MouseDown2(LevelMouseEventArgs e) {
            prevX = -1;
            prevY = -1;
            addingToSelection = Control.ModifierKeys != Keys.Alt;
            MouseMove(e);
            editor.undoManager.ForceNoMerge();
        }

        protected override void MouseMove2(LevelMouseEventArgs e) {
            if (e.TileX != prevX || e.TileY != prevY) {
                editor.tileSelection.SetPoint(e.TileX, e.TileY, addingToSelection);
                editor.tilesetObjectBrowserContents.SelectedIndex = -1;
            }
        }

        public override void TileChanged() {
            editor.FillSelection();
        }
    }
}
