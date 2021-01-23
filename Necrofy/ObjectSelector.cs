using System;
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
        private readonly int maxX;
        private readonly int maxY;
        private readonly int minX;
        private readonly int minY;

        private HashSet<T> selectedObjects = new HashSet<T>();

        private bool addToSelection;
        private bool removeFromSelection;

        private bool creating;

        private bool makingSelectionRectangle = false;
        private HashSet<T> baseSelectedObjects;
        private Rectangle selectionRectangle;

        public bool MovingObjects { get; private set; } = false;
        public int TotalMoveX { get; private set; }
        public int TotalMoveY { get; private set; }

        private int dragStartX;
        private int dragStartY;

        public ObjectSelector(IHost host, int positionStep = 1, int maxX = ushort.MaxValue, int maxY = ushort.MaxValue, int minX = 0, int minY = 0) {
            this.host = host;
            this.positionStep = positionStep;
            this.maxX = maxX;
            this.maxY = maxY;
            this.minX = minX;
            this.minY = minY;
        }

        private IEnumerable<T> SelectableObjects => host.GetObjects().Where(o => o.Selectable);

        public HashSet<T> GetSelectedObjects() {
            return selectedObjects;
        }

        public void DrawSelectionRectangle(Graphics g, float zoom = 1.0f) {
            if (makingSelectionRectangle) {
                Pen whitePen = new Pen(Color.White, 1 / zoom);
                Pen dashPen = new Pen(Color.Black, 1 / zoom) {
                    DashOffset = 0,
                    DashPattern = new float[] { 4 / zoom, 4 / zoom },
                };

                if (selectionRectangle.Width == 0 || selectionRectangle.Height == 0) {
                    g.DrawLine(whitePen, selectionRectangle.X, selectionRectangle.Y, selectionRectangle.Right, selectionRectangle.Bottom);
                    g.DrawLine(dashPen, selectionRectangle.X, selectionRectangle.Y, selectionRectangle.Right, selectionRectangle.Bottom);
                } else {
                    g.DrawRectangle(whitePen, selectionRectangle);
                    g.DrawRectangle(dashPen, selectionRectangle);
                }

                whitePen.Dispose();
                dashPen.Dispose();
            }
        }

        public void SelectAll() {
            selectedObjects = new HashSet<T>(SelectableObjects);
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
            selectedObjects = new HashSet<T>(selectedObjects.Intersect(SelectableObjects));
            host.SelectionChanged();
        }

        public void MouseDown(int x, int y) {
            addToSelection = Control.ModifierKeys == Keys.Shift;
            removeFromSelection = Control.ModifierKeys == Keys.Alt;
            creating = Control.ModifierKeys == Keys.Control;

            T hitObject = default(T);
            bool hitObjectFound = false;
            foreach (T obj in SelectableObjects.Reverse()) {
                if (obj.Bounds.Contains(x, y)) {
                    hitObject = obj;
                    hitObjectFound = true;
                    break;
                }
            }

            makingSelectionRectangle = false;
            MovingObjects = false;
            dragStartX = x;
            dragStartY = y;
            TotalMoveX = 0;
            TotalMoveY = 0;

            if (!hitObjectFound) {
                if (!addToSelection && !removeFromSelection || creating) {
                    selectedObjects.Clear();
                }
                if (creating) {
                    creating = false;
                    T newObject = host.CreateObject(x, y);
                    if (newObject != null) {
                        MovingObjects = true;
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
                    MovingObjects = true;
                }
            } else {
                if (!addToSelection && !removeFromSelection) {
                    selectedObjects.Clear();
                }
                if (!removeFromSelection) {
                    selectedObjects.Add(hitObject);
                    MovingObjects = true;
                }
            }
            host.SelectionChanged();
        }

        public void MouseMove(int x, int y) {
            if (makingSelectionRectangle) {
                selectedObjects = new HashSet<T>(baseSelectedObjects);

                selectionRectangle = new Rectangle(Math.Min(dragStartX, x), Math.Min(dragStartY, y), Math.Abs(x - dragStartX), Math.Abs(y - dragStartY));
                foreach (T obj in SelectableObjects) {
                    if (selectionRectangle.IntersectsWith(obj.Bounds)) {
                        if (removeFromSelection) {
                            selectedObjects.Remove(obj);
                        } else {
                            selectedObjects.Add(obj);
                        }
                    }
                }
                host.SelectionChanged();
            } else if (MovingObjects) {
                int newMoveX = x - dragStartX;
                int newMoveY = y - dragStartY;

                int dx = newMoveX - TotalMoveX;
                int dy = newMoveY - TotalMoveY;
                ClampObjectMove(ref dx, ref dy);

                if (dx != 0 || dy != 0) {
                    if (creating) {
                        creating = false;
                        selectedObjects = new HashSet<T>(host.CloneSelection());
                        host.SelectionChanged();
                        if (selectedObjects.Count == 0) {
                            MovingObjects = false;
                            return;
                        }
                    }

                    TotalMoveX += dx;
                    TotalMoveY += dy;
                    
                    host.MoveSelectedObjects(dx, dy);
                }
            }
        }

        public void MouseUp() {
            makingSelectionRectangle = false;
            MovingObjects = false;
            baseSelectedObjects = null;
            host.SelectionChanged();
        }

        public void KeyDown(Keys keyData) {
            if (selectedObjects.Count == 0) {
                return;
            }

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
                host.MoveSelectedObjects(dx, dy);
            }
        }

        private void ClampObjectMove(ref int dx, ref int dy) {
            int minX = selectedObjects.Min(o => o.GetX());
            int minY = selectedObjects.Min(o => o.GetY());
            int maxX = selectedObjects.Max(o => o.GetX());
            int maxY = selectedObjects.Max(o => o.GetY());

            dx = Math.Min(this.maxX - maxX, Math.Max(this.minX - minX, dx));
            dy = Math.Min(this.maxY - maxY, Math.Max(this.minY - minY, dy));
            dx = (dx / positionStep) * positionStep;
            dy = (dy / positionStep) * positionStep;
        }

        public void ParsePositionChange(string value, bool isX) {
            int intValue;
            if (int.TryParse(value, out intValue) || value.StartsWith("+") && int.TryParse(value.Substring(1), out intValue)) {
                if (value.StartsWith("+") || value.StartsWith("-") && minX >= 0) {
                    int dx = (isX ? intValue : 0) * positionStep;
                    int dy = (isX ? 0 : intValue) * positionStep;
                    ClampObjectMove(ref dx, ref dy);
                    host.MoveSelectedObjects(dx, dy);
                } else if (intValue >= minX && intValue <= maxX) {
                    if (isX) {
                        host.SetSelectedObjectsPosition(intValue, null);
                    } else {
                        host.SetSelectedObjectsPosition(null, intValue);
                    }
                }
            }
        }

        public void CenterHorizontally() {
            int min = selectedObjects.Min(o => o.Bounds.Left);
            int max = selectedObjects.Max(o => o.Bounds.Right);
            int selectionCenter = (((min + max) / 2) / positionStep) * positionStep;
            int areaCenter = (((maxX + minX + positionStep) / 2) / positionStep) * positionStep;

            int dx = areaCenter - selectionCenter;
            int dy = 0;
            ClampObjectMove(ref dx, ref dy);
            host.MoveSelectedObjects(dx, dy);
        }

        public void CenterVertically() {
            int min = selectedObjects.Min(o => o.Bounds.Top);
            int max = selectedObjects.Max(o => o.Bounds.Bottom);
            int selectionCenter = (((min + max) / 2) / positionStep) * positionStep;
            int areaCenter = (((maxY + minY + positionStep) / 2) / positionStep) * positionStep;

            int dx = 0;
            int dy = areaCenter - selectionCenter;
            ClampObjectMove(ref dx, ref dy);
            host.MoveSelectedObjects(dx, dy);
        }

        public List<int> SortAndGetZIndexes(List<T> objs) {
            List<T> allObjs = new List<T>(host.GetObjects());
            objs.Sort((a, b) => allObjs.IndexOf(a).CompareTo(allObjs.IndexOf(b)));
            return objs.Select(w => allObjs.IndexOf(w)).ToList();
        }

        public void MoveUp(out List<T> objs, out List<int> oldZIndexes, out List<int> newZIndexes) {
            List<T> allObjs = new List<T>(host.GetObjects());
            objs = new List<T>(selectedObjects);
            oldZIndexes = SortAndGetZIndexes(objs);
            newZIndexes = new List<int>(oldZIndexes);
            for (int i = objs.Count - 1; i >= 0; i--) {
                while (newZIndexes[i] < allObjs.Count - 1 && (newZIndexes[i] == oldZIndexes[i] || !allObjs[newZIndexes[i]].Selectable)) {
                    newZIndexes[i]++;
                }
                allObjs.Remove(objs[i]);
            }
        }

        public void MoveDown(out List<T> objs, out List<int> oldZIndexes, out List<int> newZIndexes) {
            List<T> allObjs = new List<T>(host.GetObjects());
            objs = new List<T>(selectedObjects);
            oldZIndexes = SortAndGetZIndexes(objs);
            newZIndexes = new List<int>(oldZIndexes);
            for (int i = 0; i < objs.Count; i++) {
                while (newZIndexes[i] > i && (newZIndexes[i] == oldZIndexes[i] || !allObjs[newZIndexes[i] - i].Selectable)) {
                    newZIndexes[i]--;
                }
                allObjs.Remove(objs[i]);
            }
        }

        public void MoveToFront(out List<T> objs, out List<int> oldZIndexes, out List<int> newZIndexes) {
            int totalObjCount = host.GetObjects().Count();
            objs = new List<T>(selectedObjects);
            oldZIndexes = SortAndGetZIndexes(objs);
            newZIndexes = new List<int>();
            for (int i = 0; i < objs.Count; i++) {
                newZIndexes.Add(totalObjCount - objs.Count + i);
            }
        }

        public void MoveToBack(out List<T> objs, out List<int> oldZIndexes, out List<int> newZIndexes) {
            objs = new List<T>(selectedObjects);
            oldZIndexes = SortAndGetZIndexes(objs);
            newZIndexes = new List<int>();
            for (int i = 0; i < objs.Count; i++) {
                newZIndexes.Add(i);
            }
        }

        public interface IHost
        {
            IEnumerable<T> GetObjects();
            void SelectionChanged();
            void MoveSelectedObjects(int dx, int dy);
            void SetSelectedObjectsPosition(int? x, int? y);
            T CreateObject(int x, int y);
            IEnumerable<T> CloneSelection();
        }
    }

    public interface ISelectableObject
    {
        Rectangle Bounds { get; }
        int GetX();
        int GetY();
        bool Selectable { get; }
    }
}
