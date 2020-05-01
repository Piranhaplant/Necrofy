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

        private ObjectBrowserContents browserContents = null;
        public ObjectBrowserContents BrowserContents {
            get {
                return browserContents;
            }
            protected set {
                browserContents = value;
                if (mainWindow != null) {
                    mainWindow.ObjectBrowser.Browser.Contents = browserContents;
                }
            }
        }

        public event EventHandler DirtyChanged;
        public event EventHandler SelectionChanged;
        
        protected MainWindow mainWindow;
        private Project project;
        private UndoManager undoManager;

        public EditorWindow() {
            DockAreas = DockAreas.Document;
            HideOnClose = false;
            FormClosing += EditorWindow_FormClosing;
        }

        private void EditorWindow_FormClosing(object sender, FormClosingEventArgs e) {
            if (Dirty && project != null) {
                Activate();
                // TODO: Fix the "*" appearing after the editor title
                DialogResult result = MessageBox.Show("Save changes to \"" + Text + "\"?", "Save changes?", MessageBoxButtons.YesNoCancel);
                if (result == DialogResult.Cancel) {
                    e.Cancel = true;
                } else if (result == DialogResult.Yes) {
                    Save();
                }
            }
        }

        public void Setup(MainWindow mainWindow, Project project) {
            this.mainWindow = mainWindow;
            this.project = project;
            mainWindow.ObjectBrowser.Browser.Contents = browserContents;
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
        
        public void Save() {
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

        // Level number used for the "run from level" feature
        public virtual int? LevelNumber => null;
    }
}
