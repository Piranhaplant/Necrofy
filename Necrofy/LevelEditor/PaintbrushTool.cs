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

        public override void MouseDown(MouseEventArgs e) {
            selecting = (Control.ModifierKeys == Keys.Control);
            MouseMove(e);
        }

        public override void MouseMove(MouseEventArgs e) {
            int tileX = e.X / 64;
            int tileY = e.Y / 64;
            if ((tileX != prevX || tileY != prevY) && tileX >= 0 && tileY >= 0 && tileX < editor.level.Level.width && tileY < editor.level.Level.height) {
                if (selecting) {
                    editor.tilesetObjectBrowserContents.SelectedIndex = editor.level.Level.background[tileX, tileY];
                } else if (editor.tilesetObjectBrowserContents.SelectedIndex > -1) {
                    editor.undoManager.Do(new PaintTileAction(tileX, tileY, (ushort)editor.tilesetObjectBrowserContents.SelectedIndex));
                    editor.Repaint();
                }
                prevX = tileX;
                prevY = tileY;
            }
        }

        public override void MouseUp(MouseEventArgs e) {
            prevX = -1;
            prevY = -1;
            editor.undoManager.ForceNoMerge();
        }
    }
}
