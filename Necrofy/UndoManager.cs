using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Necrofy
{
    class UndoManager<T> : UndoManager
    {
        private readonly T editor;

        private readonly ActionStack undoActions;
        private readonly ActionStack redoActions;
        private bool merge = true;
        private int savePos = 0;

        public UndoManager(ToolStripSplitButton undoButton, ToolStripSplitButton redoButton, T editor) {
            this.editor = editor;
            undoActions = new ActionStack(this, undoButton, action => action.DoUndo());
            redoActions = new ActionStack(this, redoButton, action => action.DoRedo());
        }

        public override void Dispose() {
            undoActions.Clear();
            redoActions.Clear();
        }

        public void Do(UndoAction<T> action) {
            action.SetEditor(editor);
            if (action.cancel) {
                action.Dispose();
                return;
            }
            action.DoRedo();
            if (!merge || undoActions.Count == 0 || !undoActions.Peek().Merge(action)) {
                undoActions.Push(action);
            } else {
                action.Dispose();
            }
            if (undoActions.Count <= savePos) {
                savePos = -1;
            }
            redoActions.Clear();
            merge = true;
            InvokeDirtyChanged();
        }

        public void Revert(UndoAction<T> action) {
            if (action != null && undoActions.Count > 0) {
                if (undoActions.Peek() == action) {
                    undoActions.Pop();
                } else if (undoActions.Peek().Unmerge(action)) {
                    action.DoUndo();
                }
            }
        }
        
        public override bool Dirty {
            get {
                return savePos != undoActions.Count;
            }
        }

        public override void Clean() {
            savePos = undoActions.Count;
            InvokeDirtyChanged();
        }

        public override void ForceDirty() {
            savePos = -1;
            InvokeDirtyChanged();
        }

        public override void ForceNoMerge() {
            merge = false;
        }

        public override void UndoLast() {
            UndoLastAndGet();
            InvokeDirtyChanged();
        }

        private UndoAction<T> UndoLastAndGet() {
            UndoAction<T> action = undoActions.Pop();
            redoActions.Push(action);
            merge = false;
            return action;
        }

        public override void RedoLast() {
            RedoLastAndGet();
            InvokeDirtyChanged();
        }

        private UndoAction<T> RedoLastAndGet() {
            UndoAction<T> action = redoActions.Pop();
            undoActions.Push(action);
            merge = false;
            return action;
        }

        private void UndoOrRedoUpTo(UndoAction<T> action) {
            if (undoActions.Contains(action)) {
                while (UndoLastAndGet() != action) ;
            } else if (redoActions.Contains(action)) {
                while (RedoLastAndGet() != action) ;
            } else {
                throw new Exception("Unknown UndoAction: " + action);
            }
            InvokeDirtyChanged();
        }

        public override void RefreshItems() {
            undoActions.RefreshItems();
            redoActions.RefreshItems();
        }

        private ToolStripMenuItem CreateToolStripItem(UndoAction<T> action) {
            ToolStripMenuItem item = new ToolStripMenuItem(action.ToString());
            item.Click += (sender, e) => {
                UndoOrRedoUpTo(action);
            };
            return item;
        }

        private class ActionStack
        {
            private const int MaxToolStripItems = 20;

            private readonly Stack<UndoAction<T>> actions = new Stack<UndoAction<T>>();
            private readonly UndoManager<T> manager;
            private readonly ToolStripSplitButton button;
            private readonly Action<UndoAction<T>> performAction;

            public ActionStack(UndoManager<T> manager, ToolStripSplitButton button, Action<UndoAction<T>> performAction) {
                this.manager = manager;
                this.button = button;
                this.performAction = performAction;
            }

            public int Count => actions.Count;

            public bool Contains(UndoAction<T> action) {
                return actions.Contains(action);
            }

            public void Push(UndoAction<T> action) {
                actions.Push(action);
                button.DropDownItems.Insert(0, manager.CreateToolStripItem(action));
                if (button.DropDownItems.Count > MaxToolStripItems) {
                    button.DropDownItems.RemoveAt(button.DropDownItems.Count - 1);
                }
                UpdateEnabled();
            }

            public UndoAction<T> Pop() {
                UndoAction<T> action = actions.Pop();
                performAction(action);

                button.DropDownItems.RemoveAt(0);
                if (button.DropDownItems.Count < actions.Count) {
                    button.DropDownItems.Add(manager.CreateToolStripItem(actions.ElementAt(button.DropDownItems.Count)));
                }
                UpdateEnabled();
                return action;
            }

            public UndoAction<T> Peek() {
                return actions.Peek();
            }

            public void Clear() {
                foreach (UndoAction<T> action in actions) {
                    action.Dispose();
                }
                actions.Clear();
                button.DropDownItems.Clear();
                UpdateEnabled();
            }

            public void RefreshItems() {
                button.DropDownItems.Clear();
                foreach (UndoAction<T> action in actions) {
                    button.DropDownItems.Add(manager.CreateToolStripItem(action));
                    if (button.DropDownItems.Count == MaxToolStripItems) {
                        break;
                    }
                }
                UpdateEnabled();
            }

            private void UpdateEnabled() {
                button.Enabled = actions.Count > 0;
            }
        }
    }

    abstract class UndoManager
    {
        public event EventHandler DirtyChanged;

        protected void InvokeDirtyChanged() {
            DirtyChanged?.Invoke(this, EventArgs.Empty);
        }

        public abstract void Dispose();
        public abstract bool Dirty { get; }
        public abstract void Clean();
        public abstract void ForceDirty();
        public abstract void ForceNoMerge();
        public abstract void UndoLast();
        public abstract void RedoLast();
        public abstract void RefreshItems();
    }

    public abstract class UndoAction<T>
    {
        protected T editor;

        public void DoUndo() {
            this.BeforeAction();
            this.Undo();
            this.AfterAction();
        }
        public void DoRedo() {
            this.BeforeAction();
            this.Redo();
            this.AfterAction();
        }
        protected abstract void Undo();
        protected abstract void Redo();
        protected virtual void BeforeAction() { }
        protected virtual void AfterAction() { }
        public virtual bool Merge(UndoAction<T> action) { return false; }
        public virtual bool Unmerge(UndoAction<T> action) { return false; }
        public virtual void SetEditor(T editor) {
            this.editor = editor;
        }
        public virtual void Dispose() { }
        public bool cancel { get; protected set; }
    }
}
