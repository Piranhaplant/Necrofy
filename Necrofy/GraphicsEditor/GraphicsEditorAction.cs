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
                DoMerge(graphicsEditorAction);
                return true;
            }
            return false;
        }

        protected void DoMerge(GraphicsEditorAction graphicsEditorAction) {
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
        }

        protected delegate void SetPixelDelegate(int x, int y, byte color);
        protected delegate byte? GetPixelDelegate(int x, int y);

        protected void ModifyTiles(Action<SetPixelDelegate, GetPixelDelegate> action) {
            Dictionary<int, BitmapData> modifiedTiles = new Dictionary<int, BitmapData>();
            bool selectionExists = editor.SelectionExists;
            BitmapData LoadTile(int x, int y) {
                int tileNum = editor.GetPixelTileNum(x, y);
                if (tileNum < 0 || (selectionExists && !editor.Selection.GetPoint(x, y))) {
                    return null;
                }
                if (!modifiedTiles.ContainsKey(tileNum)) {
                    oldTiles[tileNum] = editor.tiles.Get(tileNum);
                    newTiles[tileNum] = editor.tiles.Clone(tileNum);
                    modifiedTiles[tileNum] = newTiles[tileNum].LockBits(new Rectangle(0, 0, 8, 8), ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);
                }
                return modifiedTiles[tileNum];
            }
            void SetPixel(int x, int y, byte color) {
                BitmapData data = LoadTile(x, y);
                if (data != null) {
                    Marshal.WriteByte(data.Scan0, (y % 8) * data.Stride + (x % 8), color);
                }
            }
            byte? GetPixel(int x, int y) {
                BitmapData data = LoadTile(x, y);
                if (data != null) {
                    return Marshal.ReadByte(data.Scan0, (y % 8) * data.Stride + (x % 8));
                } else {
                    return null;
                }
            }

            action(SetPixel, GetPixel);

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
            ModifyTiles((setPixel, getPixel) => {
                MapEditor.DrawLine(x1, y1, x2, y2, (x, y) => setPixel(x, y, color));
            });
        }
        
        public override string ToString() {
            return "Paintbrush";
        }
    }

    class DeleteGraphicsAction : GraphicsEditorAction
    {
        private string text = "Delete";

        public override void SetEditor(GraphicsEditor editor) {
            base.SetEditor(editor);
            ModifyTiles((setPixel, getPixel) => {
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
            if (action is PasteGraphicsAction pasteGraphicsAction) {
                DoMerge(pasteGraphicsAction);
                text = "Move selection";
                return true;
            }
            return false;
        }

        public override string ToString() {
            return text;
        }
    }

    class PasteGraphicsAction : GraphicsEditorAction
    {
        private sbyte[,] pasteData;
        private readonly int x;
        private readonly int y;
        private readonly bool transparent;

        public PasteGraphicsAction(sbyte[,] pasteData, int x, int y, bool transparent) {
            this.pasteData = pasteData;
            this.x = x;
            this.y = y;
            this.transparent = transparent;
        }

        public override void SetEditor(GraphicsEditor editor) {
            base.SetEditor(editor);
            int cutoff = transparent ? 1 : 0;
            ModifyTiles((setPixel, getPixel) => {
                for (int y = 0; y < pasteData.GetHeight(); y++) {
                    for (int x = 0; x < pasteData.GetWidth(); x++) {
                        if (pasteData[x, y] >= cutoff) {
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

    class BucketFillGraphicsAction : GraphicsEditorAction
    {
        private readonly int x;
        private readonly int y;
        private readonly byte color;

        public BucketFillGraphicsAction(int x, int y, byte color) {
            this.x = x;
            this.y = y;
            this.color = color;
        }

        public override void SetEditor(GraphicsEditor editor) {
            base.SetEditor(editor);
            ModifyTiles((setPixel, getPixel) => {
                byte? startColor = getPixel(x, y);
                if (startColor != null && startColor != color) {
                    Stack<Point> points = new Stack<Point>();
                    points.Push(new Point(x, y));
                    while (points.Count > 0) {
                        Point p = points.Pop();
                        if (getPixel(p.X, p.Y) == startColor) {
                            setPixel(p.X, p.Y, color);
                            points.Push(new Point(p.X - 1, p.Y));
                            points.Push(new Point(p.X + 1, p.Y));
                            points.Push(new Point(p.X, p.Y - 1));
                            points.Push(new Point(p.X, p.Y + 1));
                        }
                    }
                } else {
                    cancel = true;
                }
            });
        }

        public override bool Merge(UndoAction<GraphicsEditor> action) {
            return false;
        }

        public override string ToString() {
            return "Bucket fill";
        }
    }
}
