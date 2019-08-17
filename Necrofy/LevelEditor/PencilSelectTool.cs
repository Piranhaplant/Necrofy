using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Necrofy
{
    class PencilSelectTool : Tool
    {
        private int prevX;
        private int prevY;
        private bool addingToSelection;

        public PencilSelectTool(LevelEditor editor) : base(editor) { }

        public override ObjectType objectType => ObjectType.Tiles;

        public override void MouseDown(LevelMouseEventArgs e) {
            prevX = -1;
            prevY = -1;
            addingToSelection = Control.ModifierKeys != Keys.Alt;
            MouseMove(e);
            editor.undoManager.ForceNoMerge();
        }

        public override void MouseMove(LevelMouseEventArgs e) {
            if (e.TileX != prevX || e.TileY != prevY) {
                editor.selection.SetPoint(e.TileX, e.TileY, addingToSelection);
                editor.tilesetObjectBrowserContents.SelectedIndex = -1;
            }
        }

        public override void TileChanged() {
            editor.FillSelection();
        }
    }
}
