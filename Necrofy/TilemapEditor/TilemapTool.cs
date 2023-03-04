using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Necrofy
{
    abstract class TilemapTool : MapTool
    {
        protected readonly TilemapEditor editor;
        private readonly PasteTool pasteTool;

        public TilemapTool(TilemapEditor editor) : base(editor) {
            this.editor = editor;
            pasteTool = new PasteTool(editor);
            AddSubTool(pasteTool);
        }

        public override bool CanCopy => mapEditor.SelectionExists;
        public override bool CanPaste => true;
        public override bool CanDelete => mapEditor.SelectionExists;
        public override bool HasSelection => true;

        public virtual void TileChanged() { }

        public void FloatSelection() {
            pasteTool.FloatSelection();
        }

        public void Flip(bool horizontal) {
            if (pasteTool.IsPasting) {
                pasteTool.Flip(horizontal);
            } else {
                editor.undoManager.Do(new FlipTilemapAction(horizontal));
            }
        }

        public bool CanFlip => editor.SelectionExists || pasteTool.IsPasting;

        public override void MouseMove(MapMouseEventArgs e) {
            base.MouseMove(e);
            Info1 = $"Cursor: ({e.TileX}, {e.TileY})";
        }

        public override void MouseLeave() {
            base.MouseLeave();
            Info1 = "Cursor: N/A";
        }

        private class PasteTool : MapPasteTool<ClipboardContents>
        {
            private readonly TilemapEditor editor;
            private LoadedTilemap.Tile?[,] pasteTiles;

            public PasteTool(TilemapEditor editor) : base(editor) {
                this.editor = editor;
            }

            public override void Paste() {
                base.Paste();
                editor.UpdateToolbar();
            }

            protected override ClipboardContents GetCopyData() {
                Rectangle bounds = editor.Selection.GetSelectedAreaBounds();

                ushort?[,] tiles = new ushort?[bounds.Width, bounds.Height];
                for (int y = bounds.Top; y < bounds.Bottom; y++) {
                    for (int x = bounds.Left; x < bounds.Right; x++) {
                        if (editor.Selection.GetPoint(x, y)) {
                            tiles[x - bounds.Left, y - bounds.Top] = editor.tilemap[editor.GetLocationTileIndex(x, y)].ToUshort();
                        }
                    }
                }

                return new ClipboardContents(tiles);
            }

            protected override Size ReadPaste(ClipboardContents data) {
                ushort?[,] tilemap = data.tilemap;
                if (tilemap == null) {
                    return Size.Empty;
                }

                pasteTiles = new LoadedTilemap.Tile?[tilemap.GetWidth(), tilemap.GetHeight()];
                for (int y = 0; y < tilemap.GetHeight(); y++) {
                    for (int x = 0; x < tilemap.GetWidth(); x++) {
                        if (tilemap[x, y] != null) {
                            pasteTiles[x, y] = new LoadedTilemap.Tile((ushort)tilemap[x, y]);
                        }
                    }
                }
                return new Size(pasteTiles.GetWidth(), pasteTiles.GetHeight());
            }

            protected override bool PointInPaste(int x, int y) {
                return pasteTiles[x, y] != null;
            }

            protected override void DoPasteAction(int pasteX, int pasteY) {
                editor.undoManager.Do(new PasteTilemapAction(pasteTiles, pasteX, pasteY));
            }

            protected override void RenderPaste(Graphics g, int pixelX, int pixelY, GraphicsPath path) {
                if (editor.transparency) {
                    g.SetClip(path);
                    SNESGraphics.DrawTransparencyGrid(g, new RectangleF(pixelX, pixelY, editor.TileSize * pasteTiles.GetWidth(), editor.TileSize * pasteTiles.GetHeight()), editor.Zoom);
                    g.ResetClip();
                }
                for (int y = 0; y < pasteTiles.GetHeight(); y++) {
                    for (int x = 0; x < pasteTiles.GetWidth(); x++) {
                        if (pasteTiles[x, y] != null) {
                            LoadedTilemap.Tile tile = (LoadedTilemap.Tile)pasteTiles[x, y];
                            editor.RenderTile(g, tile, pixelX + x * editor.TileSize, pixelY + y * editor.TileSize);
                        }
                    }
                }
            }

            protected override void ClearPasteData() {
                pasteTiles = null;
                editor.UpdateToolbar();
            }

            protected override void FlipSelectionData(bool horizontalFlip) {
                pasteTiles.Flip(horizontalFlip, 0, 0, pasteTiles.GetWidth(), pasteTiles.GetHeight(), t => LoadedTilemap.Tile.Flip(t, horizontalFlip));
            }

            public override void Delete() {
                editor.undoManager.Do(new DeleteTilemapAction());
            }
        }

        private class ClipboardContents
        {
            public readonly ushort?[,] tilemap;

            public ClipboardContents(ushort?[,] tilemap) {
                this.tilemap = tilemap;
            }
        }
    }
}
