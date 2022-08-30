using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Necrofy
{
    abstract class TilemapEditorAction : UndoAction<TilemapEditor>
    {
        protected Dictionary<int, LoadedTilemap.Tile> oldTiles = new Dictionary<int, LoadedTilemap.Tile>();
        protected Dictionary<int, LoadedTilemap.Tile> newTiles = new Dictionary<int, LoadedTilemap.Tile>();

        protected void SetTile(int x, int y, LoadedTilemap.Tile tile, bool lockTileNum, bool lockPalette, bool lockFlip, bool lockPriority) {
            int tileIndex = editor.GetLocationTileIndex(x, y);
            if (tileIndex >= 0) {
                LoadedTilemap.Tile oldTile = editor.tilemap[tileIndex];
                LoadedTilemap.Tile newTile;
                
                if (!lockTileNum && !lockPalette && !lockFlip && !lockPriority) {
                    newTile = tile;
                } else {
                    int tileNum = lockTileNum ? oldTile.tileNum : tile.tileNum;
                    int palette = lockPalette ? oldTile.palette : tile.palette;
                    bool priority = lockPriority ? oldTile.priority : tile.priority;
                    bool xFlip = lockFlip ? oldTile.xFlip : tile.xFlip;
                    bool yFlip = lockFlip ? oldTile.yFlip : tile.yFlip;
                    newTile = new LoadedTilemap.Tile(tileNum, palette, priority, xFlip, yFlip);
                }
                if (!newTile.Equals(oldTile)) {
                    oldTiles[tileIndex] = editor.tilemap[tileIndex];
                    newTiles[tileIndex] = newTile;
                }
            }
        }

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
                SetTile(x, y, tile, editor.LockTileNum, editor.LockPalette, editor.LockFlip, editor.LockPriority);
            });
            cancel = oldTiles.Count == 0;
        }

        public override string ToString() {
            return "Paintbrush";
        }
    }

    class PasteTilemapAction : TilemapEditorAction
    {
        private LoadedTilemap.Tile?[,] tiles;
        private readonly int pasteX;
        private readonly int pasteY;

        public PasteTilemapAction(LoadedTilemap.Tile?[,] tiles, int pasteX, int pasteY) {
            this.tiles = tiles;
            this.pasteX = pasteX;
            this.pasteY = pasteY;
        }

        public override void SetEditor(TilemapEditor editor) {
            base.SetEditor(editor);
            for (int y = 0; y < tiles.GetHeight(); y++) {
                for (int x = 0; x < tiles.GetWidth(); x++) {
                    if (tiles[x, y] != null) {
                        SetTile(pasteX + x, pasteY + y, (LoadedTilemap.Tile)tiles[x, y], false, false, false, false);
                    }
                }
            }
            tiles = null; // Allow this to be garbage collected
            cancel = oldTiles.Count == 0;
        }

        public override bool Merge(UndoAction<TilemapEditor> action) {
            return false;
        }

        public override string ToString() {
            return "Paste tiles";
        }
    }

    class FillTilemapSelectionAction : TilemapEditorAction
    {
        private readonly LoadedTilemap.Tile tile;

        public FillTilemapSelectionAction(LoadedTilemap.Tile tile) {
            this.tile = tile;
        }

        public override void SetEditor(TilemapEditor editor) {
            base.SetEditor(editor);
            for (int y = 0; y < editor.Selection.height; y++) {
                for (int x = 0; x < editor.Selection.width; x++) {
                    if (editor.Selection.GetPoint(x, y)) {
                        SetTile(x, y, tile, editor.LockTileNum, editor.LockPalette, editor.LockFlip, editor.LockPriority);
                    }
                }
            }
            cancel = oldTiles.Count == 0;
        }
        
        public override string ToString() {
            return "Fill selection";
        }
    }

    class DeleteTilemapAction : TilemapEditorAction
    {
        public override void SetEditor(TilemapEditor editor) {
            base.SetEditor(editor);
            for (int y = 0; y < editor.Selection.height; y++) {
                for (int x = 0; x < editor.Selection.width; x++) {
                    if (editor.Selection.GetPoint(x, y)) {
                        SetTile(x, y, LoadedTilemap.Tile.Empty, false, false, editor.LockFlip, editor.LockPriority);
                    }
                }
            }
            cancel = oldTiles.Count == 0;
        }

        public override string ToString() {
            return "Delete tiles";
        }
    }

    class FlipTilemapAction : TilemapEditorAction
    {
        private readonly bool flipHorizontally;

        public FlipTilemapAction(bool flipHorizontally) {
            this.flipHorizontally = flipHorizontally;
        }

        public override void SetEditor(TilemapEditor editor) {
            base.SetEditor(editor);
            Rectangle bounds = editor.Selection.GetSelectedAreaBounds();
            Extensions.Flip(flipHorizontally, bounds.X, bounds.Y, bounds.Width, bounds.Height, (x1, y1, x2, y2) => {
                LoadedTilemap.Tile? tile1 = editor.Selection.GetPoint(x1, y1) ? editor.tilemap[editor.GetLocationTileIndex(x1, y1)] : (LoadedTilemap.Tile?)null;
                LoadedTilemap.Tile? tile2 = editor.Selection.GetPoint(x2, y2) ? editor.tilemap[editor.GetLocationTileIndex(x2, y2)] : (LoadedTilemap.Tile?)null;
                if (tile1 != null || tile2 != null) {
                    SetTile(x1, y1, LoadedTilemap.Tile.Flip(tile2, flipHorizontally) ?? LoadedTilemap.Tile.Empty, false, false, false, false);
                    SetTile(x2, y2, LoadedTilemap.Tile.Flip(tile1, flipHorizontally) ?? LoadedTilemap.Tile.Empty, false, false, false, false);
                }
            });
            cancel = oldTiles.Count == 0;
        }

        public override string ToString() {
            return "Flip tiles";
        }
    }
}
