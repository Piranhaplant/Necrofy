using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Necrofy
{
    class TilemapSelectByPropertiesTool : TilemapTool
    {
        private SubTool subTool;

        public TilemapSelectByPropertiesTool(TilemapEditor editor) : base(editor) {
            subTool = new SubTool(editor);
            AddSubTool(subTool);
        }

        private class SubTool : MapTileSelectTool
        {
            private readonly TilemapEditor editor;
            public bool ignoreTileChange = false;

            public SubTool(TilemapEditor editor) : base(editor) {
                this.editor = editor;
                Status = "Click to select all tiles that share the same values on all locked properties.";
            }

            protected override void SelectTiles(int tileX, int tileY) {
                if (!editor.LockTileNum && !editor.LockPalette && !editor.LockFlip && !editor.LockPriority) {
                    return;
                }

                LoadedTilemap.Tile t = editor.tilemap[editor.GetLocationTileIndex(tileX, tileY)];
                editor.Selection.SetAllPoints((x, y) => {
                    LoadedTilemap.Tile t2 = editor.tilemap[editor.GetLocationTileIndex(x, y)];
                    bool passing = true;
                    passing = passing && (!editor.LockTileNum || t.tileNum == t2.tileNum);
                    passing = passing && (!editor.LockPalette || t.palette == t2.palette);
                    passing = passing && (!editor.LockFlip || (t.xFlip == t2.xFlip && t.yFlip == t2.yFlip));
                    passing = passing && (!editor.LockPriority || t.priority == t2.priority);
                    return passing;
                });

                ignoreTileChange = true;
                if (editor.LockTileNum) {
                    editor.SelectedTile = t.tileNum;
                }
                if (editor.LockPalette) {
                    editor.SelectedPalette = t.palette;
                }
                if (editor.LockFlip) {
                    editor.FlipX = t.xFlip;
                    editor.FlipY = t.yFlip;
                }
                if (editor.LockPriority) {
                    editor.Priority = t.priority;
                }
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
