﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Necrofy
{
    class ObjectSelector<T> where T : ISelectableObject
    {
        private static readonly Pen selectionBorderDashPen = new Pen(Color.Black) {
            DashOffset = 0,
            DashPattern = new float[] { 4f, 4f },
        };

        private readonly IHost host;
        private readonly int positionStep;
        private readonly int width;
        private readonly int height;

        private HashSet<T> selectedObjects = new HashSet<T>();

        private bool addToSelection;
        private bool removeFromSelection;

        private bool creating;

        private bool makingSelectionRectangle = false;
        private HashSet<T> baseSelectedObjects;
        private Rectangle selectionRectangle;

        private bool movingObjects = false;
        private int totalMoveX;
        private int totalMoveY;

        private int dragStartX;
        private int dragStartY;

        public ObjectSelector(IHost host, int positionStep = 1, int width = ushort.MaxValue, int height = ushort.MaxValue) {
            this.host = host;
            this.positionStep = positionStep;
            this.width = width;
            this.height = height;
        }

        public HashSet<T> GetSelectedObjects() {
            return selectedObjects;
        }

        public void DrawSelectionRectangle(Graphics g) {
            if (makingSelectionRectangle) {
                if (selectionRectangle.Width == 0 || selectionRectangle.Height == 0) {
                    g.DrawLine(Pens.White, selectionRectangle.X, selectionRectangle.Y, selectionRectangle.Right, selectionRectangle.Bottom);
                    g.DrawLine(selectionBorderDashPen, selectionRectangle.X, selectionRectangle.Y, selectionRectangle.Right, selectionRectangle.Bottom);
                } else {
                    g.DrawRectangle(Pens.White, selectionRectangle);
                    g.DrawRectangle(selectionBorderDashPen, selectionRectangle);
                }
            }
        }

        public void SelectAll() {
            selectedObjects = new HashSet<T>(host.GetObjects());
            host.SelectionChanged();
        }

        public void SelectNone() {
            selectedObjects.Clear();
            host.SelectionChanged();
        }

        public void SelectObjects(IEnumerable<T> objs) {
            selectedObjects = new HashSet<T>(objs);
            host.SelectionChanged();
        }

        public void UpdateSelection() {
            selectedObjects = new HashSet<T>(selectedObjects.Intersect(host.GetObjects()));
            host.SelectionChanged();
        }

        public void MouseDown(int x, int y) {
            addToSelection = Control.ModifierKeys == Keys.Shift;
            removeFromSelection = Control.ModifierKeys == Keys.Alt;
            creating = Control.ModifierKeys == Keys.Control;

            T hitObject = default(T);
            bool hitObjectFound = false;
            foreach (T obj in host.GetObjects().Reverse()) {
                if (obj.Bounds.Contains(x, y)) {
                    hitObject = obj;
                    hitObjectFound = true;
                    break;
                }
            }

            makingSelectionRectangle = false;
            movingObjects = false;
            dragStartX = x;
            dragStartY = y;
            totalMoveX = 0;
            totalMoveY = 0;

            if (!hitObjectFound) {
                if (!addToSelection && !removeFromSelection || creating) {
                    selectedObjects.Clear();
                }
                if (creating) {
                    creating = false;
                    T newObject = host.CreateObject(x, y);
                    if (newObject != null) {
                        movingObjects = true;
                        selectedObjects.Add(newObject);
                    }
                } else {
                    makingSelectionRectangle = true;
                    baseSelectedObjects = new HashSet<T>(selectedObjects);
                    selectionRectangle = Rectangle.Empty;
                }
            } else if (selectedObjects.Contains(hitObject)) {
                if (removeFromSelection) {
                    selectedObjects.Remove(hitObject);
                } else {
                    movingObjects = true;
                }
            } else {
                if (!addToSelection && !removeFromSelection) {
                    selectedObjects.Clear();
                }
                if (!removeFromSelection) {
                    selectedObjects.Add(hitObject);
                    movingObjects = true;
                }
            }
            host.SelectionChanged();
        }

        public void MouseMove(int x, int y) {
            if (makingSelectionRectangle) {
                selectedObjects = new HashSet<T>(baseSelectedObjects);

                selectionRectangle = new Rectangle(Math.Min(dragStartX, x), Math.Min(dragStartY, y), Math.Abs(x - dragStartX), Math.Abs(y - dragStartY));
                foreach (T obj in host.GetObjects()) {
                    if (selectionRectangle.IntersectsWith(obj.Bounds)) {
                        if (removeFromSelection) {
                            selectedObjects.Remove(obj);
                        } else {
                            selectedObjects.Add(obj);
                        }
                    }
                }
                host.SelectionChanged();
            } else if (movingObjects) {
                int newMoveX = x - dragStartX;
                int newMoveY = y - dragStartY;

                int dx = newMoveX - totalMoveX;
                int dy = newMoveY - totalMoveY;
                ClampObjectMove(ref dx, ref dy);

                if (dx != 0 || dy != 0) {
                    if (creating) {
                        creating = false;
                        selectedObjects = new HashSet<T>(host.CloneSelection());
                        host.SelectionChanged();
                        if (selectedObjects.Count == 0) {
                            movingObjects = false;
                            return;
                        }
                    }

                    totalMoveX += dx;
                    totalMoveY += dy;

                    // TODO add snap
                    host.MoveSelectedObjects(dx, dy, 1);
                }
            }
        }

        public void MouseUp() {
            makingSelectionRectangle = false;
            movingObjects = false;
            baseSelectedObjects = null;
            host.SelectionChanged();
        }

        public void KeyDown(Keys keyData) {
            int dx = 0;
            int dy = 0;
            switch (keyData) {
                case Keys.Up:
                    dy = -1;
                    break;
                case Keys.Down:
                    dy = 1;
                    break;
                case Keys.Left:
                    dx = -1;
                    break;
                case Keys.Right:
                    dx = 1;
                    break;
            }
            dx *= positionStep;
            dy *= positionStep;
            ClampObjectMove(ref dx, ref dy);

            if (dx != 0 || dy != 0) {
                // TODO add snap?
                host.MoveSelectedObjects(dx, dy, 1);
            }
        }

        private void ClampObjectMove(ref int dx, ref int dy) {
            int minX = selectedObjects.Min(o => o.X);
            int minY = selectedObjects.Min(o => o.Y);
            int maxX = selectedObjects.Max(o => o.X);
            int maxY = selectedObjects.Max(o => o.Y);

            dx = Math.Min(width - maxX, Math.Max(-minX, dx));
            dy = Math.Min(height - maxY, Math.Max(-minY, dy));
            dx = (dx / positionStep) * positionStep;
            dy = (dy / positionStep) * positionStep;
        }

        public interface IHost
        {
            IEnumerable<T> GetObjects();
            void SelectionChanged();
            void MoveSelectedObjects(int dx, int dy, int snap);
            T CreateObject(int x, int y);
            IEnumerable<T> CloneSelection();
        }
    }

    public interface ISelectableObject
    {
        Rectangle Bounds { get; }
        ushort X { get; }
        ushort Y { get; }
    }
}
