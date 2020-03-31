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
        private readonly IHost host;
        private HashSet<T> selectedObjects = new HashSet<T>();

        private bool addToSelection;
        private bool removeFromSelection;

        private bool makingSelectionRectangle = false;
        private HashSet<T> baseSelectedObjects;
        private Rectangle selectionRectangle;

        private bool movingObjects = false;

        private int dragStartX;
        private int dragStartY;

        public ObjectSelector(IHost host) {
            this.host = host;
        }

        public HashSet<T> GetSelectedObjects() {
            return selectedObjects;
        }

        public Rectangle GetSelectionRectangle() {
            if (makingSelectionRectangle) {
                return selectionRectangle;
            }
            return Rectangle.Empty;
        }

        public void SelectAll() {
            selectedObjects = new HashSet<T>(host.GetObjects());
            host.SelectionChanged();
        }

        public void SelectNone() {
            selectedObjects.Clear();
            host.SelectionChanged();
        }

        public void MouseDown(int x, int y) {
            addToSelection = Control.ModifierKeys == Keys.Shift;
            removeFromSelection = Control.ModifierKeys == Keys.Alt;

            T hitObject = default(T);
            bool hitObjectFound = false;
            foreach (T obj in host.GetObjects()) {
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

            if (!hitObjectFound) {
                if (!addToSelection && !removeFromSelection) {
                    selectedObjects.Clear();
                }
                makingSelectionRectangle = true;
                baseSelectedObjects = new HashSet<T>(selectedObjects);
                selectionRectangle = Rectangle.Empty;
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
                // TODO
                host.SelectionChanged();
            }
        }

        public void MouseUp() {
            makingSelectionRectangle = false;
            movingObjects = false;
            baseSelectedObjects = null;
            host.SelectionChanged();
        }
        
        public interface IHost
        {
            IEnumerable<T> GetObjects();
            void SelectionChanged();
        }
    }

    public interface ISelectableObject
    {
        Rectangle Bounds { get; }
    }
}
