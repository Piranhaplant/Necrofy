using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Necrofy
{
    class TileSelectTool : TileTool
    {
        private int prevX;
        private int prevY;
        private bool ignoreTileChange = false;

        public TileSelectTool(LevelEditor editor) : base(editor) {
            Status = "Click to select all tiles of the same type.";
        }
        
        protected override void MouseDown2(LevelMouseEventArgs e) {
            prevX = -1;
            prevY = -1;
            MouseMove(e);
        }

        protected override void MouseMove2(LevelMouseEventArgs e) {
            if (e.MouseIsDown && (e.TileX != prevX || e.TileY != prevY) && e.InBounds) {
                ushort tileType = editor.level.Level.background[e.TileX, e.TileY];
                editor.tileSelection.SetAllPoints((x, y) => editor.level.Level.background[x, y] == tileType);

                ignoreTileChange = true;
                editor.tilesetObjectBrowserContents.SelectedIndex = tileType;
                ignoreTileChange = false;
                editor.ScrollObjectBrowserToSelection();

                prevX = e.TileX;
                prevY = e.TileY;
            }
        }

        public override void TileChanged() {
            if (!ignoreTileChange) {
                editor.FillSelection();
            }
        }
    }
}
