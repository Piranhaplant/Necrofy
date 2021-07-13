using System;
using System.Collections.Generic;
using System.Drawing;
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

        int collisionX = -1;
        int collisionY = -1;

        public PaintbrushTool(LevelEditor editor) : base(editor) {
            Status = "Click to paint tiles. Hold Ctrl to select a tile type from the level.";
        }

        protected override void MouseDown2(LevelMouseEventArgs e) {
#if DEBUG
            if (Control.ModifierKeys == Keys.Alt) {
                collisionX = e.X / 8;
                collisionY = e.Y / 8;

                int tileType = editor.level.Level.background[e.TileX, e.TileY];
                int tileNum = editor.level.tilemap.tiles[tileType][collisionX % 8, collisionY % 8].tileNum;
                ushort collision = editor.level.collision.tiles[tileNum];
                Status = tileNum.ToString("X3") + ": " + Convert.ToString(collision & 0xff, 2).PadLeft(8, '0') + " " + Convert.ToString(collision >> 8, 2).PadLeft(8, '0');

                editor.Repaint();
                return;
            }
#endif

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

        protected override void Paint2(Graphics g) {
            if (collisionX > -1 && collisionY > -1) {
                g.DrawRectangle(Pens.Red, collisionX * 8, collisionY * 8, 8, 8);
            }
        }
    }
}
