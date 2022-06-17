using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Necrofy
{
    abstract class LevelEditorTileTool : LevelEditorTool
    {
        public override ObjectType objectType => ObjectType.Tiles;

        public LevelEditorTileTool(LevelEditor editor) : base(editor) {
            AddSubTool(new PasteTool(editor));
        }

        public override bool CanCopy => editor.SelectionExists;
        public override bool CanPaste => true;
        public override bool CanDelete => false;
        public override bool HasSelection => true;

        private class PasteTool : MapPasteTool
        {
            private readonly LevelEditor editor;
            private ushort?[,] pasteTiles;

            public PasteTool(LevelEditor editor) : base(editor) {
                this.editor = editor;
            }

            public override void Copy() {
                Rectangle bounds = editor.Selection.GetSelectedAreaBounds();

                ushort?[,] tiles = new ushort?[bounds.Width, bounds.Height];
                for (int y = bounds.Top; y < bounds.Bottom; y++) {
                    for (int x = bounds.Left; x < bounds.Right; x++) {
                        if (editor.Selection.GetPoint(x, y)) {
                            tiles[x - bounds.Left, y - bounds.Top] = editor.level.Level.background[x, y];
                        }
                    }
                }

                Clipboard.SetText(JsonConvert.SerializeObject(tiles));
            }

            protected override void RenderPaste(Graphics g, int pixelX, int pixelY, GraphicsPath path) {
                for (int y = 0; y < pasteTiles.GetHeight(); y++) {
                    for (int x = 0; x < pasteTiles.GetWidth(); x++) {
                        if (pasteTiles[x, y] != null) {
                            g.DrawImage(editor.level.tileset.tiles[(int)pasteTiles[x, y]], pixelX + x * 64, pixelY + y * 64);
                        }
                    }
                }

                g.FillPath(LevelEditor.selectionFillBrush, path);
            }

            protected override Size ReadPaste() {
                pasteTiles = JsonConvert.DeserializeObject<ushort?[,]>(Clipboard.GetText());
                return new Size(pasteTiles.GetWidth(), pasteTiles.GetHeight());
            }

            protected override bool PointInPaste(int x, int y) {
                return pasteTiles[x, y] != null;
            }

            protected override void DoPasteAction(int pasteX, int pasteY) {
                editor.undoManager.Do(new PasteTilesAction(pasteX, pasteY, pasteTiles));
            }

            protected override void ClearPasteData() {
                pasteTiles = null;
            }
        }
    }
}
