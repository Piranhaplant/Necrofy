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
        protected override void AfterAction() {
            editor.Repaint();
        }
    }

    class PaintGraphicsAction : GraphicsEditorAction
    {
        private readonly int x1;
        private readonly int y1;
        private readonly int x2;
        private readonly int y2;
        private readonly byte color;

        private Dictionary<int, Bitmap> oldTiles = new Dictionary<int, Bitmap>();
        private Dictionary<int, Bitmap> newTiles = new Dictionary<int, Bitmap>();

        public PaintGraphicsAction(int x1, int y1, int x2, int y2, byte color) {
            this.x1 = x1;
            this.y1 = y1;
            this.x2 = x2;
            this.y2 = y2;
            this.color = color;
        }

        public override void SetEditor(GraphicsEditor editor) {
            base.SetEditor(editor);
            DrawLine(x1, y1, x2, y2, color);
            if (oldTiles.Count == 0) {
                cancel = true;
            }
        }

        protected override void Redo() {
            foreach (KeyValuePair<int, Bitmap> pair in newTiles) {
                editor.tiles[pair.Key] = pair.Value;
            }
        }

        protected override void Undo() {
            foreach (KeyValuePair<int, Bitmap> pair in oldTiles) {
                editor.tiles[pair.Key] = pair.Value;
            }
        }

        public override bool Merge(UndoAction<GraphicsEditor> action) {
            if (action is PaintGraphicsAction paintGraphicsAction) {
                foreach (int tileNum in paintGraphicsAction.newTiles.Keys) {
                    if (!oldTiles.ContainsKey(tileNum)) {
                        oldTiles[tileNum] = paintGraphicsAction.oldTiles[tileNum];
                    }
                    newTiles[tileNum] = paintGraphicsAction.newTiles[tileNum];
                }
                return true;
            }
            return false;
        }

        private void DrawLine(int x1, int y1, int x2, int y2, byte color) {
            Dictionary<int, BitmapData> modifiedTiles = new Dictionary<int, BitmapData>();
            bool selectionEmpty = editor.selection.Empty;
            void SetPixel(int x, int y) {
                int tileNum = editor.GetPixelTileNum(x, y);
                if (tileNum < 0 || (!selectionEmpty && !editor.selection.GetPoint(x, y))) {
                    return;
                }
                if (!modifiedTiles.ContainsKey(tileNum)) {
                    oldTiles[tileNum] = editor.tiles[tileNum];
                    editor.tiles[tileNum] = editor.tiles[tileNum].Clone(new Rectangle(0, 0, 8, 8), PixelFormat.Format8bppIndexed);
                    modifiedTiles[tileNum] = editor.tiles[tileNum].LockBits(new Rectangle(0, 0, 8, 8), ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);
                }
                BitmapData data = modifiedTiles[tileNum];
                Marshal.WriteByte(data.Scan0, (y % 8) * data.Stride + (x % 8), color);
            }

            if (Math.Abs(x2 - x1) > Math.Abs(y2 - y1)) {
                DrawLine2(x1, y1, x2, y2, (a, b) => SetPixel(a, b));
            } else {
                DrawLine2(y1, x1, y2, x2, (a, b) => SetPixel(b, a));
            }

            foreach (KeyValuePair<int, BitmapData> pair in modifiedTiles) {
                editor.tiles[pair.Key].UnlockBits(pair.Value);
                newTiles[pair.Key] = editor.tiles[pair.Key];
            }
        }

        private void DrawLine2(int a1, int b1, int a2, int b2, Action<int, int> setPixel) {
            if (a1 > a2) {
                DrawLine2(a2, b2, a1, b1, setPixel);
            } else {
                double slope = a1 == a2 ? 0 : (double)(b2 - b1) / (a2 - a1);
                for (int a = a1; a <= a2; a++) {
                    int b = (int)Math.Round(slope * (a - a1) + b1);
                    setPixel(a, b);
                }
            }
        }
        
        public override string ToString() {
            return "Paintbrush";
        }
    }
}
