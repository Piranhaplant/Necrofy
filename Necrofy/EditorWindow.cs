﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace Necrofy
{
    class EditorWindow : DockContent {
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

        private object[] propertyBrowserObjects = null;
        public object[] PropertyBrowserObjects {
            get {
                return propertyBrowserObjects;
            }
            protected set {
                propertyBrowserObjects = value;
                if (mainWindow != null) {
                    mainWindow.PropertyBrowser.SetObjects(propertyBrowserObjects);
                }
            }
        }

        private string title = "";
        public string Title {
            get {
                return title;
            }
            protected set {
                title = value;
                UpdateText();
            }
        }

        private string status = "";
        public string Status {
            get {
                return status;
            }
            protected set {
                if (value != status) {
                    status = value;
                    StatusChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        private string info1 = "";
        public string Info1 {
            get {
                return info1;
            }
            protected set {
                if (value != info1) {
                    info1 = value;
                    Info1Changed?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        private string info2 = "";
        public string Info2 {
            get {
                return info2;
            }
            protected set {
                if (value != info2) {
                    info2 = value;
                    Info2Changed?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public bool DockVisible {
            get {
                return mainWindow.EditorVisible(this);
            }
        }

        public bool DockActive {
            get {
                return ReferenceEquals(mainWindow.activeEditor, this);
            }
        }

        public DockAlignment? Alignment {
            get {
                NestedDockingStatus status = Pane?.NestedDockingStatus;
                if (status != null) {
                    if (status.NestedPanes.Count <= 1) {
                        return null;
                    }
                    switch (status.Alignment) {
                         case DockAlignment.Left:
                            return DockAlignment.Left;
                        case DockAlignment.Right:
                            return status.PreviousPane != null ? DockAlignment.Right : DockAlignment.Left;
                        case DockAlignment.Top:
                            return DockAlignment.Top;
                        case DockAlignment.Bottom:
                            return status.PreviousPane != null ? DockAlignment.Bottom : DockAlignment.Top;
                    }
                }
                return null;
            }
        }

        public Asset.NameInfo AssetInfo { get; private set; }

        public event EventHandler DirtyChanged;
        public event EventHandler SelectionChanged;
        public event EventHandler StatusChanged;
        public event EventHandler Info1Changed;
        public event EventHandler Info2Changed;

        protected MainWindow mainWindow { get; private set; }
        protected Project project { get; private set; }
        protected UndoManager undoManager { get; private set; }
        private bool prevDirty = false;

        private bool saveOnClose = true;

        public EditorWindow() {
            DockAreas = DockAreas.Document;
            HideOnClose = false;
            FormClosing += EditorWindow_FormClosing;
            Disposed += EditorWindow_Disposed;
        }

        public void CloseWithoutSaving() {
            saveOnClose = false;
            Close();
        }

        private void EditorWindow_FormClosing(object sender, FormClosingEventArgs e) {
            if (saveOnClose) {
                CloseChildren(e);
                bool cancel = e.Cancel;
                PromptForSave(ref cancel);
                e.Cancel = cancel;
            }
        }

        public void PromptForSave(ref bool cancel) {
            if (!cancel && Dirty && project != null) {
                // TODO: when closing the whole form, this Activate() works correctly, but not when closing an individual window
                Activate();
                DialogResult result = MessageBox.Show($"Save changes to \"{Title}\"?", "Save changes?", MessageBoxButtons.YesNoCancel);
                if (result == DialogResult.Cancel) {
                    cancel = true;
                } else if (result == DialogResult.Yes) {
                    Save();
                }
            }
        }

        private void EditorWindow_Disposed(object sender, EventArgs e) {
            undoManager?.Dispose();
        }

        public void Setup(MainWindow mainWindow, Project project, Asset.NameInfo assetInfo) {
            this.mainWindow = mainWindow;
            this.project = project;
            AssetInfo = assetInfo;
            mainWindow.ObjectBrowser.Browser.Contents = browserContents;
            undoManager = Setup();
            if (undoManager != null) {
                undoManager.DirtyChanged += (sender, e) => {
                    if (Dirty != prevDirty) {
                        prevDirty = Dirty;
                        UpdateText();
                        DirtyChanged?.Invoke(this, e);
                    }
                };
            }
        }

        public void ScrollObjectBrowserToSelection() {
            mainWindow.ObjectBrowser.Browser.ScrollToSelection();
        }

        public void RefreshPropertyBrowser() {
            mainWindow.PropertyBrowser.RefreshProperties();
        }

        private void UpdateText() {
            Text = (Dirty ? "*" : "") + title;
        }

        protected virtual UndoManager Setup() {
            return null;
        }

        public virtual void Displayed() {
            undoManager?.RefreshItems();
        }

        public virtual void Hidden() {

        }
        
        public void Save() {
            DoSave(project);
            undoManager?.Clean();
        }

        protected virtual void DoSave(Project project) { }

        public virtual void Undo() {
            undoManager?.UndoLast();
        }

        public virtual void Redo() {
            undoManager?.RedoLast();
        }

        public bool Dirty => undoManager?.Dirty ?? false;

        protected void RaiseSelectionChanged() {
            SelectionChanged?.Invoke(this, EventArgs.Empty);
        }
        
        protected virtual void CloseChildren(FormClosingEventArgs e) { }

        public virtual ToolStripGrouper.ItemSet ToolStripItemSet => ToolStripGrouper.ItemSet.None;
        public virtual void ToolStripItemClicked(ToolStripGrouper.ItemType item) { }
        public virtual void ToolStripItemCheckedChanged(ToolStripGrouper.ItemType item) { }
        public virtual void RenameAssetReferences(Asset.RenameResults results) { }

        public virtual bool CanCopy => false;
        public virtual bool CanPaste => false;
        public virtual bool CanDelete => false;
        public virtual bool HasSelection => false;

        public virtual void Copy() { }
        public virtual void Paste() { }
        public virtual void Delete() { }
        public virtual void SelectAll() { }
        public virtual void SelectNone() { }

        public virtual bool CanZoom => false;
        private float zoom = 1.0f;
        public float Zoom {
            get {
                return zoom;
            }
            set {
                if (value != zoom) {
                    zoom = value;
                    ZoomChanged();
                }
            }
        }
        protected virtual void ZoomChanged() { }

        public virtual void PropertyBrowserPropertyChanged(PropertyValueChangedEventArgs e) { }

        // Level number used for the "run from level" feature
        public virtual int? LevelNumber => null;
    }
}
