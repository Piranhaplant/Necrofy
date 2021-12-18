﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Necrofy
{
    abstract class MapPasteTool : MapTool
    {
        private bool IsPasting => pasteSize != Size.Empty;
        private Size pasteSize = Size.Empty;
        private GraphicsPath pasteTilesPath;
        private int pasteX;
        private int pasteY;

        private int pasteDragX;
        private int pasteDragY;
        private int prevX;
        private int prevY;
        // Used to ignore mouse events when pasting and when finishing a paste
        private bool ignoreMouse = false;

        public MapPasteTool(MapEditor mapEditor) : base(mapEditor) { }
        
        public override sealed void Paint(Graphics g) {
            if (IsPasting) {
                RenderPaste(g, pasteX, pasteY, pasteTilesPath);

                using (Pen pen = mapEditor.CreateSelectionBorderPen())
                using (Pen dashPen = mapEditor.CreateSelectionBorderDashPen()) {
                    g.DrawPath(pen, pasteTilesPath);
                    g.DrawPath(dashPen, pasteTilesPath);
                }
            } else {
                PassToNextTool();
            }
        }

        protected abstract void RenderPaste(Graphics g, int pixelX, int pixelY, GraphicsPath path);
        
        public override sealed void MouseDown(MapMouseEventArgs e) {
            ignoreMouse = false;
            if (IsPasting) {
                if (pasteTilesPath.IsVisible(e.X, e.Y)) {
                    pasteDragX = e.X - pasteX;
                    pasteDragY = e.Y - pasteY;
                } else {
                    CommitPaste();
                    ignoreMouse = true;
                }
                prevX = e.X;
                prevY = e.Y;
            } else {
                PassToNextTool();
            }
        }

        public override sealed void MouseMove(MapMouseEventArgs e) {
            if (IsPasting) {
                if (e.MouseIsDown && !ignoreMouse) {
                    pasteX = e.X - pasteDragX;
                    pasteY = e.Y - pasteDragY;
                    TranslatePath(e.X - prevX, e.Y - prevY);
                    prevX = e.X;
                    prevY = e.Y;
                    mapEditor.Repaint();
                } else {
                    mapEditor.SetCursor(pasteTilesPath.IsVisible(e.X, e.Y) ? Cursors.SizeAll : Cursors.Default);
                }
            } else if (!ignoreMouse) {
                PassToNextTool();
            }
        }

        public override sealed void MouseUp(MapMouseEventArgs e) {
            if (ignoreMouse) {
                ignoreMouse = false;
            } else if (IsPasting) {
                int newX = RoundToTile(pasteX) * mapEditor.TileSize;
                int newY = RoundToTile(pasteY) * mapEditor.TileSize;
                TranslatePath(newX - pasteX, newY - pasteY);
                pasteX = newX;
                pasteY = newY;
                mapEditor.Repaint();
            } else {
                PassToNextTool();
            }
        }

        public override sealed void KeyDown(KeyEventArgs e) {
            if (IsPasting) {
                if (e.KeyCode == Keys.Escape) {
                    EndPaste();
                } else if (e.KeyCode == Keys.Enter) {
                    CommitPaste();
                }
            } else {
                PassToNextTool();
            }
        }

        public override sealed void DoneBeingUsed() {
            if (IsPasting) {
                CommitPaste();
            }
        }
        
        public override void Paste() {
            if (IsPasting) {
                CommitPaste();
            }
            try {
                pasteSize = ReadPaste();
                if (pasteSize == Size.Empty) {
                    return;
                }

                TileSelection selection = new TileSelection(pasteSize.Width, pasteSize.Height, scale: mapEditor.TileSize);
                selection.SetAllPoints(PointInPaste);
                pasteTilesPath = selection.GetGraphicsPath();

                if (pasteTilesPath == null) {
                    pasteSize = Size.Empty;
                    return;
                }

                ignoreMouse = true;

                Point center = mapEditor.ScrollWrapper.GetViewCenter();
                pasteX = RoundToTile(center.X - pasteSize.Width * mapEditor.TileSize / 2) * mapEditor.TileSize;
                pasteY = RoundToTile(center.Y - pasteSize.Height * mapEditor.TileSize / 2) * mapEditor.TileSize;
                TranslatePath(pasteX, pasteY);

                mapEditor.Selection.Clear();
                mapEditor.GenerateMouseMove(); // To update the cursor
            } catch (Exception e) {
                Console.WriteLine(e.StackTrace);
            }
        }

        protected abstract Size ReadPaste();
        protected abstract bool PointInPaste(int x, int y);

        public override void SelectAll() {
            mapEditor.Selection.SetAllPoints((x, y) => true);
        }

        public override void SelectNone() {
            mapEditor.Selection.Clear();
        }

        private void TranslatePath(int dx, int dy) {
            Matrix matrix = new Matrix();
            matrix.Translate(dx, dy);
            pasteTilesPath.Transform(matrix);
        }

        private void CommitPaste() {
            DoPasteAction(RoundToTile(pasteX), RoundToTile(pasteY));
            EndPaste();
        }

        protected abstract void DoPasteAction(int pasteX, int pasteY);

        private void EndPaste() {
            ClearPasteData();
            pasteSize = Size.Empty;
            pasteTilesPath.Dispose();
            pasteTilesPath = null;
            mapEditor.SetCursor(Cursors.Default);
            mapEditor.Repaint();
        }

        protected abstract void ClearPasteData();

        private int RoundToTile(int position) {
            return (int)Math.Round((double)position / mapEditor.TileSize);
        }
    }
}
