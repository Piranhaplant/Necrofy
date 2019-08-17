using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Necrofy
{
    class PaintbrushTool : Tool
    {
        private int prevX = -1;
        private int prevY = -1;
        private bool selecting = false;

        public PaintbrushTool(LevelEditor editor) : base(editor) { }

        public override ObjectType objectType => ObjectType.Tiles;

        public override void MouseDown(LevelMouseEventArgs e) {
            selecting = (Control.ModifierKeys == Keys.Control);
            MouseMove(e);
        }

        public override void MouseMove(LevelMouseEventArgs e) {
            if ((e.TileX != prevX || e.TileY != prevY) && e.InBounds) {
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

        public override void MouseUp(LevelMouseEventArgs e) {
            prevX = -1;
            prevY = -1;
            editor.undoManager.ForceNoMerge();
        }
    }
}
