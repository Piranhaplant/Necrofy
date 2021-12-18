using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Necrofy
{
    class LevelEditorBrushTool : LevelEditorTileTool
    {
        public LevelEditorBrushTool(LevelEditor editor) : base(editor) {
            AddSubTool(new BrushTool(editor));
        }

        private class BrushTool : MapBrushTool
        {
            private readonly LevelEditor editor;

            int collisionX = -1;
            int collisionY = -1;

            public BrushTool(LevelEditor editor) : base(editor) {
                this.editor = editor;
                Status = "Click to paint tiles. Hold Ctrl to select a tile type from the level.";
            }

            public override void MouseDown(MapMouseEventArgs e) {
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
                base.MouseDown(e);
            }


            public override void MouseUp(MapMouseEventArgs e) {
                editor.undoManager.ForceNoMerge();
            }

            public override void Paint(Graphics g) {
                if (collisionX > -1 && collisionY > -1) {
                    g.DrawRectangle(Pens.Red, collisionX * 8, collisionY * 8, 8, 8);
                }
            }

            protected override void DrawLine(int x1, int y1, int x2, int y2) {
                if (editor.tilesetObjectBrowserContents.SelectedIndex >= 0) {
                    editor.undoManager.Do(new PaintTileAction(x1, y1, x2, y2, (ushort)editor.tilesetObjectBrowserContents.SelectedIndex));
                }
            }

            protected override void SelectTile(int x, int y) {
                editor.tilesetObjectBrowserContents.SelectedIndex = editor.level.Level.background[x, y];
                editor.ScrollObjectBrowserToSelection();
            }
        }
    }
}
