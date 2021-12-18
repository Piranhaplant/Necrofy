using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Necrofy
{
    class LevelEditorTileSelectTool : LevelEditorTileTool
    {
        private TileSelectTool subTool;

        public LevelEditorTileSelectTool(LevelEditor editor) : base(editor) {
            subTool = new TileSelectTool(editor);
            AddSubTool(subTool);
        }

        private class TileSelectTool : MapTileSelectTool
        {
            private readonly LevelEditor editor;
            public bool ignoreTileChange = false;

            public TileSelectTool(LevelEditor editor) : base(editor) {
                this.editor = editor;
                Status = "Click to select all tiles of the same type.";
            }

            protected override void SelectTiles(int tileX, int tileY) {
                ushort tileType = editor.level.Level.background[tileX, tileY];
                editor.Selection.SetAllPoints((x, y) => editor.level.Level.background[x, y] == tileType);

                ignoreTileChange = true;
                editor.tilesetObjectBrowserContents.SelectedIndex = tileType;
                ignoreTileChange = false;
                editor.ScrollObjectBrowserToSelection();
            }
        }
        
        public override void TileChanged() {
            if (!subTool.ignoreTileChange) {
                editor.FillSelection();
            }
        }
    }
}
