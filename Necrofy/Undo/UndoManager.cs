using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Necrofy
{
    class UndoManager
    {
        private const int MaxToolStripItems = 20;
        
        private readonly LevelEditor editor;
        
        private readonly ActionStack undoActions;
        private readonly ActionStack redoActions;
        private bool merge = true;
        private int savePos = 0;
        
        public UndoManager(ToolStripSplitButton undoButton, ToolStripSplitButton redoButton, LevelEditor editor) {
            this.editor = editor;
            undoActions = new ActionStack(this, undoButton, action => action.DoUndo());
            redoActions = new ActionStack(this, redoButton, action => action.DoRedo());
        }
        
        public void Do(Action action, bool performAction = true) {
            action.SetEditor(editor);
            if (action.cancel) {
                return;
            }
            if (performAction) {
                action.DoRedo();
            }
            if (merge && undoActions.Count > 0 && undoActions.Peek().CanMerge && undoActions.Peek().GetType() == action.GetType()) {
                undoActions.Peek().Merge(action);
            } else {
                undoActions.Push(action);
            }
            if (undoActions.Count <= savePos) {
                savePos = -1;
            }
            redoActions.Clear();
            merge = true;
        }

        public void Perform(Action action) {
            action.SetEditor(editor);
            if (action.cancel) {
                return;
            }
            action.DoRedo();
        }

        public bool Dirty {
            get {
                return savePos != undoActions.Count;
            }
        }

        public void Clean() {
            savePos = undoActions.Count;
        }

        public void ForceDirty() {
            savePos = -1;
        }

        public void ForceNoMerge() {
            merge = false;
        }

        public Action UndoLast() {
            Action action = undoActions.Pop();
            redoActions.Push(action);
            merge = false;
            return action;
        }
        
        public Action RedoLast() {
            Action action = redoActions.Pop();
            undoActions.Push(action);
            merge = false;
            return action;
        }

        private void UndoOrRedoUpTo(Action action) {
            if (undoActions.Contains(action)) {
                while (UndoLast() != action) ;
            } else if (redoActions.Contains(action)) {
                while (RedoLast() != action) ;
            } else {
                Debug.WriteLine("Unknown UndoAction: " + action);
            }
        }

        public void RefreshItems() {
            undoActions.RefreshItems();
            redoActions.RefreshItems();
        }
        
        private ToolStripMenuItem CreateToolStripItem(Action action) {
            ToolStripMenuItem item = new ToolStripMenuItem(action.ToString());
            item.Click += (sender, e) => {
                UndoOrRedoUpTo(action);
            };
            return item;
        }

        private class ActionStack
        {
            private readonly Stack<Action> actions = new Stack<Action>();
            private readonly UndoManager manager;
            private readonly ToolStripSplitButton button;
            private readonly Action<Action> performAction;

            public ActionStack(UndoManager manager, ToolStripSplitButton button, Action<Action> performAction) {
                this.manager = manager;
                this.button = button;
                this.performAction = performAction;
            }

            public int Count => actions.Count;

            public bool Contains(Action action) {
                return actions.Contains(action);
            }

            public void Push(Action action) {
                actions.Push(action);
                button.DropDownItems.Insert(0, manager.CreateToolStripItem(action));
                if (button.DropDownItems.Count > MaxToolStripItems) {
                    button.DropDownItems.RemoveAt(button.DropDownItems.Count - 1);
                }
                UpdateEnabled();
            }

            public Action Pop() {
                Action action = actions.Pop();
                performAction(action);

                button.DropDownItems.RemoveAt(0);
                if (button.DropDownItems.Count < actions.Count) {
                    // TODO make sure ElementAt works here
                    button.DropDownItems.Add(manager.CreateToolStripItem(actions.ElementAt(button.DropDownItems.Count)));
                }
                UpdateEnabled();
                return action;
            }

            public Action Peek() {
                return actions.Peek();
            }

            public void Clear() {
                actions.Clear();
                button.DropDownItems.Clear();
                UpdateEnabled();
            }

            public void RefreshItems() {
                button.DropDownItems.Clear();
                foreach (Action action in actions) {
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

        public abstract class Action
        {
            protected LevelEditor editor;
            protected Level level;
            
            public void DoUndo() {
                this.Undo();
                this.AfterAction();
                editor.Repaint();
            }
            public void DoRedo() {
                this.Redo();
                this.AfterAction();
                editor.Repaint();
            }
            protected virtual void Undo() { }
            protected virtual void Redo() { }
            protected virtual void AfterAction() { }
            protected virtual void AfterSetEditor() { }
            public virtual bool CanMerge => false;
            public virtual void Merge(Action action) { }
            public void SetEditor(LevelEditor editor) {
                this.editor = editor;
                this.level = editor.level.Level;
                this.AfterSetEditor();
            }
            public bool cancel;

            protected void UpdateSelection() {
                // TODO
                //if (EdControl.t != null)
                //    EdControl.t.UpdateSelection();
            }
        }
    }
}
