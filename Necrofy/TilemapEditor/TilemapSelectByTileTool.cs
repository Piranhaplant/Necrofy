using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Necrofy
{
    class TilemapSelectByTileTool : TilemapTool
    {
        private SubTool subTool;

        public TilemapSelectByTileTool(TilemapEditor editor) : base(editor) {
            subTool = new SubTool(editor);
            AddSubTool(subTool);
        }

        private class SubTool : MapTileSelectTool
        {
            private readonly TilemapEditor editor;
            public bool ignoreTileChange = false;

            public SubTool(TilemapEditor editor) : base(editor) {
                this.editor = editor;
                Status = "Click to select all tiles of the same type.";
            }

            protected override void SelectTiles(int tileX, int tileY) {
                LoadedTilemap.Tile t = editor.tilemap[editor.GetLocationTileIndex(tileX, tileY)];
                editor.Selection.SetAllPoints((x, y) => editor.tilemap[editor.GetLocationTileIndex(x, y)].tileNum == t.tileNum);

                ignoreTileChange = true;
                editor.SelectedTile = t.tileNum;
                ignoreTileChange = false;
            }
        }

        public override void TileChanged() {
            if (!subTool.ignoreTileChange) {
                editor.FillSelection();
            }
        }
    }
}
