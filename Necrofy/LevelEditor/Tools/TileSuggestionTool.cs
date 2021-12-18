using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Necrofy
{
    class TileSuggestionTool : LevelEditorTileTool
    {
        private SubTool subTool;

        public TileSuggestionTool(LevelEditor editor) : base(editor) {
            subTool = new SubTool(editor);
            AddSubTool(subTool);
        }

        private class SubTool : MapTool
        {
            private static readonly Pen linePen = new Pen(Color.White, 2);
            private static readonly Bitmap circle = Properties.Resources.circle;

            private readonly LevelEditor editor;

            private int prevX = -1;
            private int prevY = -1;

            private bool startIsInBounds;
            private int startX;
            private int startY;
            public int endX;
            public int endY;
            public TilesetSuggestions.Direction? direction = null;
            private bool filtered = false;

            public SubTool(LevelEditor editor) : base(editor) {
                this.editor = editor;
                Status = "Click a tile and drag to an adjacent tile to show all connecting tiles.";
            }

            public override void Paint(Graphics g) {
                if (startIsInBounds) {
                    g.DrawImage(circle, startX * 64 + 32 - circle.Width / 2, startY * 64 + 32 - circle.Height / 2, circle.Width, circle.Height);

                    if (direction != null) {
                        g.DrawLine(linePen, startX * 64 + 32, startY * 64 + 32, endX * 64 + 32, endY * 64 + 32);

                        Bitmap arrow = Properties.Resources.arrow;
                        arrow.RotateFlip((RotateFlipType)direction);
                        g.DrawImage(arrow, endX * 64 + 32 - arrow.Width / 2, endY * 64 + 32 - arrow.Height / 2, arrow.Width, arrow.Height);
                        arrow.Dispose();
                    }
                }
            }

            public override void MouseDown(MapMouseEventArgs e) {
                prevX = -1;
                prevY = -1;
                startIsInBounds = e.InBounds;
                direction = null;
                startX = e.TileX;
                startY = e.TileY;
                MouseMove(e);
            }

            public override void MouseMove(MapMouseEventArgs e) {
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

            public override void MouseUp(MapMouseEventArgs e) {
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

            public override void DoneBeingUsed() {
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

        public override void TileChanged() {
            if (subTool.direction != null && editor.tilesetObjectBrowserContents.SelectedTile > -1) {
                editor.undoManager.Do(new TileSuggestAction(subTool.endX, subTool.endY, (ushort)editor.tilesetObjectBrowserContents.SelectedTile));
            }
        }
    }
}
