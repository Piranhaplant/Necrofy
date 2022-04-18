using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Necrofy
{
    abstract class TilemapEditorAction : UndoAction<TilemapEditor>
    {
        protected Dictionary<int, LoadedTilemap.Tile> oldTiles = new Dictionary<int, LoadedTilemap.Tile>();
        protected Dictionary<int, LoadedTilemap.Tile> newTiles = new Dictionary<int, LoadedTilemap.Tile>();

        protected override void Undo() {
            foreach (KeyValuePair<int, LoadedTilemap.Tile> tile in oldTiles) {
                editor.tilemap[tile.Key] = tile.Value;
            }
        }

        protected override void Redo() {
            foreach (KeyValuePair<int, LoadedTilemap.Tile> tile in newTiles) {
                editor.tilemap[tile.Key] = tile.Value;
            }
        }

        protected override void AfterAction() {
            editor.Repaint();
        }

        public override bool Merge(UndoAction<TilemapEditor> action) {
            if (action is TilemapEditorAction tilemapEditorAction && action.GetType() == this.GetType()) {
                foreach (KeyValuePair<int, LoadedTilemap.Tile> newTile in tilemapEditorAction.newTiles) {
                    if (!oldTiles.ContainsKey(newTile.Key)) {
                        oldTiles[newTile.Key] = tilemapEditorAction.oldTiles[newTile.Key];
                    }
                    newTiles[newTile.Key] = newTile.Value;
                }
                return true;
            }
            return false;
        }
    }

    class PaintTilemapAction : TilemapEditorAction
    {
        private readonly int x1;
        private readonly int y1;
        private readonly int x2;
        private readonly int y2;
        private readonly LoadedTilemap.Tile tile;

        public PaintTilemapAction(int x1, int y1, int x2, int y2, LoadedTilemap.Tile tile) {
            this.x1 = x1;
            this.y1 = y1;
            this.x2 = x2;
            this.y2 = y2;
            this.tile = tile;
        }

        public override void SetEditor(TilemapEditor editor) {
            base.SetEditor(editor);
            MapEditor.DrawLine(x1, y1, x2, y2, (x, y) => {
                int tileNum = editor.GetLocationTileNum(x, y);
                if (tileNum >= 0) {
                    oldTiles[tileNum] = editor.tilemap[tileNum];
                    newTiles[tileNum] = tile;
                }
            });
        }

        public override string ToString() {
            return "Paintbrush";
        }
    }
}
