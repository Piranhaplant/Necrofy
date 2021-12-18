using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Necrofy
{
    class ResizeLevelTool : LevelEditorTileTool
    {
        public ResizeLevelTool(LevelEditor editor) : base(editor) {
            AddSubTool(new SubTool(editor));
        }

        private class SubTool : MapTool
        {
            private const string DefaultStatus = "Drag the level border to resize. If level is expanded, the new area will be filled with the currently selected tile.";
            private const string DragStatus = "Level: {0}x{1}";

            private enum ResizeMode
            {
                None,
                Start,
                End,
            }

            private readonly LevelEditor editor;

            private bool resizing = false;
            private ResizeMode horizontalResizeMode = ResizeMode.None;
            private ResizeMode verticalResizeMode = ResizeMode.None;
            private int curStartX;
            private int curStartY;
            private int curEndX;
            private int curEndY;

            private int Width => editor.level.Level.width;
            private int Height => editor.level.Level.height;

            public SubTool(LevelEditor editor) : base(editor) {
                this.editor = editor;
                UpdateStatus();
            }

            public override void Paint(Graphics g) {
                int startX, startY, endX, endY;
                if (resizing) {
                    startX = curStartX * 64;
                    startY = curStartY * 64;
                    endX = curEndX * 64;
                    endY = curEndY * 64;
                } else {
                    startX = 0;
                    startY = 0;
                    endX = Width * 64;
                    endY = Height * 64;
                }

                int width = endX - startX;
                int height = endY - startY;
                g.DrawRectangle(Pens.Black, startX, startY, width, height);
                g.DrawRectangle(Pens.White, startX + 1, startY + 1, width - 2, height - 2);

                int midX = (startX + endX) / 2;
                int midY = (startY + endY) / 2;
                DrawHandle(g, startX, startY);
                DrawHandle(g, midX, startY);
                DrawHandle(g, endX, startY);
                DrawHandle(g, endX, midY);
                DrawHandle(g, endX, endY);
                DrawHandle(g, midX, endY);
                DrawHandle(g, startX, endY);
                DrawHandle(g, startX, midY);
            }

            private void DrawHandle(Graphics g, int x, int y) {
                g.FillRectangle(Brushes.White, x - 4, y - 4, 8, 8);
                g.DrawRectangle(Pens.Black, x - 4, y - 4, 8, 8);
            }

            public override void MouseDown(MapMouseEventArgs e) {
                if (horizontalResizeMode != ResizeMode.None || verticalResizeMode != ResizeMode.None) {
                    resizing = true;
                    curStartX = 0;
                    curStartY = 0;
                    curEndX = Width;
                    curEndY = Height;
                    editor.ScrollWrapper.ExpandingDrag = true;
                    UpdateStatus();
                }
            }

            public override void MouseMove(MapMouseEventArgs e) {
                if (resizing) {
                    int newStartX = curStartX;
                    int newStartY = curStartY;
                    int newEndX = curEndX;
                    int newEndY = curEndY;

                    if (horizontalResizeMode == ResizeMode.Start) {
                        newStartX = Math.Min((int)Math.Round(e.X / 64.0), Width - 1);
                    } else if (horizontalResizeMode == ResizeMode.End) {
                        newEndX = Math.Max((int)Math.Round(e.X / 64.0), 1);
                    }

                    if (verticalResizeMode == ResizeMode.Start) {
                        newStartY = Math.Min((int)Math.Round(e.Y / 64.0), Height - 1);
                    } else if (verticalResizeMode == ResizeMode.End) {
                        newEndY = Math.Max((int)Math.Round(e.Y / 64.0), 1);
                    }

                    if (newStartX != curStartX || newStartY != curStartY || newEndX != curEndX || newEndY != curEndY) {
                        curStartX = newStartX;
                        curStartY = newStartY;
                        curEndX = newEndX;
                        curEndY = newEndY;
                        editor.Repaint();
                        UpdateStatus();
                    }
                } else {
                    int width = Width * 64;
                    int height = Height * 64;

                    horizontalResizeMode = ResizeMode.None;
                    verticalResizeMode = ResizeMode.None;

                    if (e.Y >= -16 && e.Y < height + 16 && e.X >= -16 && e.X < width + 16) {
                        if (e.X < 16) {
                            horizontalResizeMode = ResizeMode.Start;
                        } else if (e.X >= width - 16) {
                            horizontalResizeMode = ResizeMode.End;
                        }

                        if (e.Y < 16) {
                            verticalResizeMode = ResizeMode.Start;
                        } else if (e.Y >= height - 16) {
                            verticalResizeMode = ResizeMode.End;
                        }
                    }

                    Cursor c = Cursors.Default;
                    if (horizontalResizeMode == ResizeMode.Start && verticalResizeMode == ResizeMode.Start || horizontalResizeMode == ResizeMode.End && verticalResizeMode == ResizeMode.End) {
                        c = Cursors.SizeNWSE;
                    } else if (horizontalResizeMode == ResizeMode.Start && verticalResizeMode == ResizeMode.End || horizontalResizeMode == ResizeMode.End && verticalResizeMode == ResizeMode.Start) {
                        c = Cursors.SizeNESW;
                    } else if (horizontalResizeMode != ResizeMode.None) {
                        c = Cursors.SizeWE;
                    } else if (verticalResizeMode != ResizeMode.None) {
                        c = Cursors.SizeNS;
                    }
                    editor.SetCursor(c);
                }
            }

            public override void MouseUp(MapMouseEventArgs e) {
                if (resizing && (curStartX != 0 || curStartY != 0 || curEndX != Width || curEndY != Height)) {
                    int tile = Math.Max(editor.tilesetObjectBrowserContents.SelectedTile, 0);
                    editor.undoManager.Do(new ResizeLevelAction(curStartX, curStartY, curEndX, curEndY, (ushort)tile));
                }
                EndResize();
            }

            public override void DoneBeingUsed() {
                EndResize();
            }

            private void EndResize() {
                resizing = false;
                editor.ScrollWrapper.ExpandingDrag = false;
                UpdateStatus();
            }

            private void UpdateStatus() {
                if (resizing) {
                    Status = string.Format(DragStatus, curEndX - curStartX, curEndY - curStartY);
                } else {
                    Status = DefaultStatus;
                }
            }
        }
    }
}
