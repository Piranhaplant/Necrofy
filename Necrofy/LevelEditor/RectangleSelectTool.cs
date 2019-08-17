using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Necrofy
{
    class RectangleSelectTool : Tool
    {
        public RectangleSelectTool(LevelEditor editor) : base(editor) { }

        public override ObjectType objectType => ObjectType.Tiles;

        public override void MouseDown(LevelMouseEventArgs e) {
            bool addToSelection = true;
            if (Control.ModifierKeys == Keys.Alt) {
                addToSelection = false;
            } else if (Control.ModifierKeys != Keys.Shift) {
                editor.selection.Clear();
            }
            editor.selection.StartRect(e.TileX, e.TileY, addToSelection);
            editor.undoManager.ForceNoMerge();
        }

        public override void MouseMove(LevelMouseEventArgs e) {
            editor.selection.MoveRect(e.TileX, e.TileY);
            editor.tilesetObjectBrowserContents.SelectedIndex = -1;
        }

        public override void MouseUp(LevelMouseEventArgs e) {
            editor.selection.EndRect();
        }

        public override void TileChanged() {
            editor.FillSelection();
        }
    }
}
