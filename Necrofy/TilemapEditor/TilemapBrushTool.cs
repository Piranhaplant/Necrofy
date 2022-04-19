using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Necrofy
{
    class TilemapBrushTool : TilemapTool
    {
        public TilemapBrushTool(TilemapEditor editor) : base(editor) {
            AddSubTool(new BrushTool(editor));
        }

        private class BrushTool : MapBrushTool
        {
            private readonly TilemapEditor editor;

            public BrushTool(TilemapEditor editor) : base(editor) {
                this.editor = editor;
                Status = "Click to paint. Hold Ctrl to select a tile type. Shift + click to draw a line from the previous point.";
            }

            protected override void DrawLine(int x1, int y1, int x2, int y2) {
                editor.undoManager.Do(new PaintTilemapAction(x1, y1, x2, y2, new LoadedTilemap.Tile(editor.SelectedTile, editor.SelectedPalette, editor.FlipX, editor.FlipY)));
            }

            protected override void SelectTile(int x, int y) {
                int tileIndex = editor.GetLocationTileIndex(x, y);
                if (tileIndex >= 0) {
                    LoadedTilemap.Tile tile = editor.tilemap[tileIndex];
                    editor.SelectedTile = tile.tileNum;
                    editor.SelectedPalette = tile.palette;
                    editor.FlipX = tile.xFlip;
                    editor.FlipY = tile.yFlip;
                }
            }

            public override void MouseUp(MapMouseEventArgs e) {
                editor.undoManager.ForceNoMerge();
            }
        }
    }
}
