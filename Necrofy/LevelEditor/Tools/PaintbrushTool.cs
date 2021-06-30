using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Necrofy
{
    class PaintbrushTool : TileTool
    {
        private int prevX = -1;
        private int prevY = -1;
        private bool selecting = false;

        public PaintbrushTool(LevelEditor editor) : base(editor) {
            Status = "Click to paint tiles. Hold Ctrl to select a tile type from the level.";
        }

        protected override void MouseDown2(LevelMouseEventArgs e) {
            prevX = -1;
            prevY = -1;
            selecting = (Control.ModifierKeys == Keys.Control);
            MouseMove(e);
        }

        protected override void MouseMove2(LevelMouseEventArgs e) {
            if (e.MouseIsDown && (e.TileX != prevX || e.TileY != prevY) && e.InBounds) {
                if (selecting) {
                    editor.tilesetObjectBrowserContents.SelectedIndex = editor.level.Level.background[e.TileX, e.TileY];
                    editor.ScrollObjectBrowserToSelection();
                } else if (editor.tilesetObjectBrowserContents.SelectedIndex > -1) {
                    editor.undoManager.Do(new PaintTileAction(e.TileX, e.TileY, (ushort)editor.tilesetObjectBrowserContents.SelectedIndex));
                    editor.Repaint();
                }
                prevX = e.TileX;
                prevY = e.TileY;
            }
        }

        protected override void MouseUp2(LevelMouseEventArgs e) {
            editor.undoManager.ForceNoMerge();
        }
    }
}
