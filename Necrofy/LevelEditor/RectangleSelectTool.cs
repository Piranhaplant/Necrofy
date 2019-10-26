using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Necrofy
{
    class RectangleSelectTool : TileTool
    {
        bool selecting = false;

        public RectangleSelectTool(LevelEditor editor) : base(editor) { }

        protected override void MouseDown2(LevelMouseEventArgs e) {
            selecting = true;
            bool addToSelection = true;
            if (Control.ModifierKeys == Keys.Alt) {
                addToSelection = false;
            } else if (Control.ModifierKeys != Keys.Shift) {
                editor.tileSelection.Clear();
            }
            editor.tileSelection.StartRect(e.TileX, e.TileY, addToSelection);
            editor.undoManager.ForceNoMerge();
        }

        protected override void MouseMove2(LevelMouseEventArgs e) {
            if (selecting) {
                editor.tileSelection.MoveRect(e.TileX, e.TileY);
                editor.tilesetObjectBrowserContents.SelectedIndex = -1;
            }
        }

        protected override void MouseUp2(LevelMouseEventArgs e) {
            editor.tileSelection.EndRect();
            selecting = false;
        }

        protected override void DoneBeingUsed2() {
            if (selecting) {
                editor.tileSelection.EndRect();
                selecting = false;
            }
        }

        public override void TileChanged() {
            editor.FillSelection();
        }
    }
}
