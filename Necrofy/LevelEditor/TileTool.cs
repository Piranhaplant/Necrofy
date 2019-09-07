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
    abstract class TileTool : Tool
    {
        private bool IsPasting => pasteTiles != null;
        private ushort?[,] pasteTiles;
        private GraphicsPath pasteTilesPath;
        private int pasteX;
        private int pasteY;

        private int pasteDragX;
        private int pasteDragY;
        private int prevX;
        private int prevY;
        // Used to avoid sending mouse events to the actual tool after completing a paste until the mouse up event happens
        private bool pasteFinished = false;

        public TileTool(LevelEditor editor) : base(editor) { }

        public override ObjectType objectType => ObjectType.Tiles;

        public override sealed void Paint(Graphics g) {
            if (IsPasting) {
                for (int y = 0; y < pasteTiles.GetHeight(); y++) {
                    for (int x = 0; x < pasteTiles.GetWidth(); x++) {
                        if (pasteTiles[x, y] != null) {
                            g.DrawImage(editor.level.tiles[(int)pasteTiles[x, y]], pasteX + x * 64, pasteY + y * 64);
                        }
                    }
                }

                g.FillPath(LevelEditor.selectionFillBrush, pasteTilesPath);
                g.DrawPath(Pens.White, pasteTilesPath);
                g.DrawPath(LevelEditor.selectionBorderDashPen, pasteTilesPath);
            } else {
                Paint2(g);
            }
        }

        public override sealed void MouseDown(LevelMouseEventArgs e) {
            if (IsPasting) {
                int relativeX = e.TileX - pasteX / 64;
                int relativeY = e.TileY - pasteY / 64;
                if (relativeX >= 0 && relativeX < pasteTiles.GetWidth() && relativeY >= 0 && relativeY < pasteTiles.GetHeight() && pasteTiles[relativeX, relativeY] != null) {
                    pasteDragX = e.X - pasteX;
                    pasteDragY = e.Y - pasteY;
                } else {
                    CommitPaste();
                    pasteFinished = true;
                    editor.Repaint();
                }
                prevX = e.X;
                prevY = e.Y;
            } else {
                MouseDown2(e);
            }
        }

        public override sealed void MouseMove(LevelMouseEventArgs e) {
            if (IsPasting) {
                pasteX = e.X - pasteDragX;
                pasteY = e.Y - pasteDragY;
                TranslatePath(e.X - prevX, e.Y - prevY);
                prevX = e.X;
                prevY = e.Y;
                editor.Repaint();
            } else if (!pasteFinished) {
                MouseMove2(e);
            }
        }

        public override sealed void MouseUp(LevelMouseEventArgs e) {
            if (IsPasting) {
                int newX = RoundToTile(pasteX) * 64;
                int newY = RoundToTile(pasteY) * 64;
                TranslatePath(newX - pasteX, newY - pasteY);
                pasteX = newX;
                pasteY = newY;
                // TODO animation?
                editor.Repaint();
            } else if (!pasteFinished) {
                MouseUp2(e);
            } else {
                pasteFinished = false;
            }
        }

        protected virtual void Paint2(Graphics g) { }
        protected virtual void MouseDown2(LevelMouseEventArgs e) { }
        protected virtual void MouseUp2(LevelMouseEventArgs e) { }
        protected virtual void MouseMove2(LevelMouseEventArgs e) { }

        public override bool CanCopy => editor.TileSelectionExists;
        public override bool CanPaste => true;
        public override bool CanDelete => false;
        public override bool HasSelection => true;

        public override void Copy() {
            int minX = int.MaxValue;
            int maxX = -1;
            int minY = int.MaxValue;
            int maxY = -1;

            for (int y = 0; y < editor.level.Level.height; y++) {
                for (int x = 0; x < editor.level.Level.width; x++) {
                    if (editor.tileSelection.GetPoint(x, y)) {
                        minX = Math.Min(minX, x);
                        maxX = Math.Max(maxX, x);
                        minY = Math.Min(minY, y);
                        maxY = Math.Max(maxY, y);
                    }
                }
            }

            ushort?[,] tiles = new ushort?[maxX - minX + 1, maxY - minY + 1];
            for (int y = minY; y <= maxY; y++) {
                for (int x = minX; x <= maxX; x++) {
                    if (editor.tileSelection.GetPoint(x, y)) {
                        tiles[x - minX, y - minY] = editor.level.Level.background[x, y];
                    }
                }
            }

            Clipboard.SetText(JsonConvert.SerializeObject(tiles));
        }

        public override void Paste() {
            if (IsPasting) {
                CommitPaste();
            }
            try {
                pasteTiles = JsonConvert.DeserializeObject<ushort?[,]>(Clipboard.GetText());

                TileSelection selection = new TileSelection(pasteTiles.GetWidth(), pasteTiles.GetHeight());
                selection.SetAllPoints((x, y) => pasteTiles[x, y] != null);
                pasteTilesPath = selection.GetGraphicsPath();

                if (pasteTilesPath == null) {
                    pasteTiles = null;
                    return;
                }

                Point center = editor.GetViewCenter();
                pasteX = RoundToTile(center.X - pasteTiles.GetWidth() * 64 / 2) * 64;
                pasteY = RoundToTile(center.Y - pasteTiles.GetHeight() * 64 / 2) * 64;
                TranslatePath(pasteX, pasteY);

                editor.tileSelection.Clear();
            } catch (Exception) { }
        }

        public override void SelectAll() {
            editor.tileSelection.SetAllPoints((x, y) => true);
        }

        public override void SelectNone() {
            editor.tileSelection.Clear();
        }

        private void TranslatePath(int dx, int dy) {
            Matrix scaleMatrix = new Matrix();
            scaleMatrix.Translate(dx, dy);
            pasteTilesPath.Transform(scaleMatrix);
        }

        private void CommitPaste() {
            editor.undoManager.Do(new PasteTilesAction(RoundToTile(pasteX), RoundToTile(pasteY), pasteTiles));
            pasteTiles = null;
            pasteTilesPath.Dispose();
            pasteTilesPath = null;
        }

        private int RoundToTile(int position) {
            return (int)Math.Round(position / 64.0);
        }
    }
}
