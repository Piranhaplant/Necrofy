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
            
            public BrushTool(LevelEditor editor) : base(editor) {
                this.editor = editor;
                Status = "Click to paint tiles. Hold Ctrl to select a tile type from the level.";
            }

            public override void MouseDown(MapMouseEventArgs e) {
                base.MouseDown(e);
            }


            public override void MouseUp(MapMouseEventArgs e) {
                editor.undoManager.ForceNoMerge();
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
