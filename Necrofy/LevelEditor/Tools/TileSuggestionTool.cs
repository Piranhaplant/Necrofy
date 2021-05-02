using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Necrofy
{
    class TileSuggestionTool : TileTool
    {
        private static readonly Pen linePen = new Pen(Color.White, 2);

        private int prevX = -1;
        private int prevY = -1;

        private bool startIsInBounds;
        private int startX;
        private int startY;
        private int endX;
        private int endY;
        private TilesetSuggestions.Direction? direction = null;
        private bool filtered = false;

        public TileSuggestionTool(LevelEditor editor) : base(editor) {
            Status = "Click a tile and drag to an adjacent tile to show all connecting tiles.";
        }

        protected override void Paint2(Graphics g) {
            if (startIsInBounds) {
                Bitmap circle = Properties.Resources.circle;
                g.DrawImage(Properties.Resources.circle, startX * 64 + 32 - circle.Width / 2, startY * 64 + 32 - circle.Height / 2, circle.Width, circle.Height);

                if (direction != null) {
                    g.DrawLine(linePen, startX * 64 + 32, startY * 64 + 32, endX * 64 + 32, endY * 64 + 32);

                    Bitmap arrow = Properties.Resources.arrow;
                    arrow.RotateFlip((RotateFlipType)direction);
                    g.DrawImage(arrow, endX * 64 + 32 - arrow.Width / 2, endY * 64 + 32 - arrow.Height / 2, arrow.Width, arrow.Height);
                }
            }
        }

        protected override void MouseDown2(LevelMouseEventArgs e) {
            prevX = -1;
            prevY = -1;
            startIsInBounds = e.InBounds;
            direction = null;
            startX = e.TileX;
            startY = e.TileY;
            MouseMove(e);
        }

        protected override void MouseMove2(LevelMouseEventArgs e) {
            if (startIsInBounds && e.MouseIsDown && (e.TileX != prevX || e.TileY != prevY)) {
                endX = e.TileX;
                endY = e.TileY;

                direction = null;
                if (e.InBounds) {
                    if (endX == startX - 1 && endY == startY) {
                        direction = TilesetSuggestions.Direction.Left;
                    } else if (endX == startX && endY == startY - 1) {
                        direction = TilesetSuggestions.Direction.Top;
                    } else if (endX == startX + 1 && endY == startY) {
                        direction = TilesetSuggestions.Direction.Right;
                    } else if (endX == startX && endY == startY + 1) {
                        direction = TilesetSuggestions.Direction.Bottom;
                    }
                }

                prevX = e.TileX;
                prevY = e.TileY;
                editor.Repaint();
            }
        }

        protected override void MouseUp2(LevelMouseEventArgs e) {
            if (direction == null) {
                startIsInBounds = false;
                editor.Repaint();
                ResetObjectBrowser();
            } else {
                editor.tilesetObjectBrowserContents.ShowTiles(editor.level.TilesetSuggestions.GetTiles(editor.level.Level.background[startX, startY], (TilesetSuggestions.Direction)direction));
                filtered = true;
            }
            editor.undoManager.ForceNoMerge();
        }

        public override void TileChanged() {
            if (direction != null && editor.tilesetObjectBrowserContents.SelectedTile > -1) {
                editor.undoManager.Do(new TileSuggestAction(endX, endY, (ushort)editor.tilesetObjectBrowserContents.SelectedTile));
            }
        }

        protected override void DoneBeingUsed2() {
            startIsInBounds = false;
            direction = null;
            ResetObjectBrowser();
        }

        private void ResetObjectBrowser() {
            if (filtered) {
                editor.tilesetObjectBrowserContents.ShowAllTiles();
                editor.ScrollObjectBrowserToSelection();
                filtered = false;
            }
        }
    }
}
