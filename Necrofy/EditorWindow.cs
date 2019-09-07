using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace Necrofy
{
    class EditorWindow : DockContent
    {
        public MenuStrip EditorMenuStrip { get; set; }
        public ToolStrip EditorToolStrip { get; set; }

        public event EventHandler DirtyChanged;
        public event EventHandler SelectionChanged;
        
        protected MainWindow mainWindow;
        private UndoManager undoManager;

        public EditorWindow() {
            DockAreas = DockAreas.Document;
            HideOnClose = false;
        }

        public void Setup(MainWindow mainWindow) {
            this.mainWindow = mainWindow;
            undoManager = Setup();
            if (undoManager != null) {
                undoManager.DirtyChanged += (sender, e) => DirtyChanged?.Invoke(this, e);
            }
        }

        protected virtual UndoManager Setup() {
            return null;
        }

        public virtual void Displayed() {
            undoManager?.RefreshItems();
        }
        
        public void Save(Project project) {
            DoSave(project);
            undoManager?.Clean();
        }

        protected virtual void DoSave(Project project) { }

        public void Undo() {
            undoManager?.UndoLast();
        }

        public void Redo() {
            undoManager?.RedoLast();
        }

        public bool Dirty => undoManager?.Dirty ?? false;

        protected void RaiseSelectionChanged() {
            SelectionChanged?.Invoke(this, EventArgs.Empty);
        }

        public virtual bool CanCopy => false;
        public virtual bool CanPaste => false;
        public virtual bool CanDelete => false;
        public virtual bool HasSelection => false;

        public virtual void Copy() { }
        public virtual void Paste() { }
        public virtual void Delete() { }
        public virtual void SelectAll() { }
        public virtual void SelectNone() { }
    }
}
