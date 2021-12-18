using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Necrofy
{
    abstract class GraphicsEditorAction : UndoAction<GraphicsEditor>
    {
        protected Dictionary<int, Bitmap> oldTiles = new Dictionary<int, Bitmap>();
        protected Dictionary<int, Bitmap> newTiles = new Dictionary<int, Bitmap>();

        public override void Dispose() {
            foreach (Bitmap tile in oldTiles.Values) {
                editor.tiles.DoneUsing(tile);
            }
            foreach (Bitmap tile in newTiles.Values) {
                editor.tiles.DoneUsing(tile);
            }
        }

        protected override void Redo() {
            foreach (KeyValuePair<int, Bitmap> pair in newTiles) {
                editor.tiles.Set(pair.Key, pair.Value);
            }
        }

        protected override void Undo() {
            foreach (KeyValuePair<int, Bitmap> pair in oldTiles) {
                editor.tiles.Set(pair.Key, pair.Value);
            }
        }

        protected override void AfterAction() {
            editor.Repaint();
        }

        public override bool Merge(UndoAction<GraphicsEditor> action) {
            if (action is GraphicsEditorAction graphicsEditorAction && action.GetType() == this.GetType()) {
                foreach (int tileNum in graphicsEditorAction.newTiles.Keys) {
                    if (!oldTiles.ContainsKey(tileNum)) {
                        oldTiles[tileNum] = graphicsEditorAction.oldTiles[tileNum];
                        editor.tiles.Use(oldTiles[tileNum]);
                    } else {
                        editor.tiles.DoneUsing(newTiles[tileNum]);
                    }
                    newTiles[tileNum] = graphicsEditorAction.newTiles[tileNum];
                    editor.tiles.Use(newTiles[tileNum]);
                }
                return true;
            }
            return false;
        }

        protected delegate void SetPixelDelegate(int x, int y, byte color);

        protected void ModifyTiles(Action<SetPixelDelegate> action) {
            Dictionary<int, BitmapData> modifiedTiles = new Dictionary<int, BitmapData>();
            bool selectionExists = editor.SelectionExists;
            void SetPixel(int x, int y, byte color) {
                int tileNum = editor.GetPixelTileNum(x, y);
                if (tileNum < 0 || (selectionExists && !editor.Selection.GetPoint(x, y))) {
                    return;
                }
                if (!modifiedTiles.ContainsKey(tileNum)) {
                    oldTiles[tileNum] = editor.tiles.Get(tileNum);
                    newTiles[tileNum] = editor.tiles.Clone(tileNum);
                    modifiedTiles[tileNum] = newTiles[tileNum].LockBits(new Rectangle(0, 0, 8, 8), ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);
                }
                BitmapData data = modifiedTiles[tileNum];
                Marshal.WriteByte(data.Scan0, (y % 8) * data.Stride + (x % 8), color);
            }

            action(SetPixel);

            foreach (KeyValuePair<int, BitmapData> pair in modifiedTiles) {
                newTiles[pair.Key].UnlockBits(pair.Value);
            }

            if (oldTiles.Count == 0) {
                cancel = true;
            }
        }
    }

    class PaintGraphicsAction : GraphicsEditorAction
    {
        private readonly int x1;
        private readonly int y1;
        private readonly int x2;
        private readonly int y2;
        private readonly byte color;

        public PaintGraphicsAction(int x1, int y1, int x2, int y2, byte color) {
            this.x1 = x1;
            this.y1 = y1;
            this.x2 = x2;
            this.y2 = y2;
            this.color = color;
        }

        public override void SetEditor(GraphicsEditor editor) {
            base.SetEditor(editor);
            ModifyTiles(setPixel => {
                MapEditor.DrawLine(x1, y1, x2, y2, (x, y) => setPixel(x, y, color));
            });
        }
        
        public override string ToString() {
            return "Paintbrush";
        }
    }

    class DeleteGraphicsAction : GraphicsEditorAction
    {
        public override void SetEditor(GraphicsEditor editor) {
            base.SetEditor(editor);
            ModifyTiles(setPixel => {
                for (int y = 0; y < editor.Selection.height; y++) {
                    for (int x = 0; x < editor.Selection.width; x++) {
                        if (editor.Selection.GetPoint(x, y)) {
                            setPixel(x, y, 0);
                        }
                    }
                }
            });
        }

        public override bool Merge(UndoAction<GraphicsEditor> action) {
            return false;
        }

        public override string ToString() {
            return "Delete";
        }
    }

    class PasteGraphicsAction : GraphicsEditorAction
    {
        private sbyte[,] pasteData;
        private readonly int x;
        private readonly int y;

        public PasteGraphicsAction(sbyte[,] pasteData, int x, int y) {
            this.pasteData = pasteData;
            this.x = x;
            this.y = y;
        }

        public override void SetEditor(GraphicsEditor editor) {
            base.SetEditor(editor);
            ModifyTiles(setPixel => {
                for (int y = 0; y < pasteData.GetHeight(); y++) {
                    for (int x = 0; x < pasteData.GetWidth(); x++) {
                        if (pasteData[x, y] >= 0) {
                            setPixel(x + this.x, y + this.y, (byte)pasteData[x, y]);
                        }
                    }
                }
            });
            pasteData = null; // Allow data to be garbage collected
        }

        public override bool Merge(UndoAction<GraphicsEditor> action) {
            return false;
        }

        public override string ToString() {
            return "Paste";
        }
    }
}
