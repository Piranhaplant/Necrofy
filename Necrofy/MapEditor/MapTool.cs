using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Necrofy
{
    abstract class MapTool : ITool
    {
        protected readonly MapEditor mapEditor;

        private int currentSubTool = 0;
        private List<MapTool> subTools = new List<MapTool>();

        public MapTool(MapEditor mapEditor) {
            this.mapEditor = mapEditor;
        }

        protected void AddSubTool(MapTool tool) {
            subTools.Add(tool);
            tool.StatusChanged += SubTool_StatusChanged;
        }

        private void SubTool_StatusChanged(object sender, EventArgs e) {
            StatusChanged?.Invoke(this, EventArgs.Empty);
        }

        private void RunOnSubTool(Action<MapTool> action) {
            if (subTools.Count == 0) {
                PassToNextTool();
                return;
            }

            int i;
            for (i = 0; i < subTools.Count; i++) {
                MapTool subTool = subTools[i];
                subTool.passedToNextTool = false;
                action(subTool);
                if (!subTool.passedToNextTool) {
                    break;
                }
            }

            if (i < subTools.Count) {
                if (i != currentSubTool) {
                    subTools[currentSubTool].DoneBeingUsed();
                }
                currentSubTool = i;
            }
        }

        private bool passedToNextTool = false;
        protected void PassToNextTool() {
            passedToNextTool = true;
        }

        public virtual void Paint(Graphics g) {
            RunOnSubTool(t => t.Paint(g));
        }
        public virtual void MouseDown(MapMouseEventArgs e) {
            RunOnSubTool(t => t.MouseDown(e));
        }
        public virtual void MouseUp(MapMouseEventArgs e) {
            RunOnSubTool(t => t.MouseUp(e));
        }
        public virtual void MouseMove(MapMouseEventArgs e) {
            RunOnSubTool(t => t.MouseMove(e));
        }
        public virtual void KeyDown(KeyEventArgs e) {
            RunOnSubTool(t => t.KeyDown(e));
        }
        public virtual void KeyUp(KeyEventArgs e) {
            RunOnSubTool(t => t.KeyUp(e));
        }

        public virtual bool CanCopy => false;
        public virtual bool CanPaste => false;
        public virtual bool CanDelete => false;
        public virtual bool HasSelection => false;

        public virtual void Copy() {
            RunOnSubTool(t => t.Copy());
        }
        public virtual void Paste() {
            RunOnSubTool(t => t.Paste());
        }
        public virtual void Delete() {
            RunOnSubTool(t => t.Delete());
        }
        public virtual void SelectAll() {
            RunOnSubTool(t => t.SelectAll());
        }
        public virtual void SelectNone() {
            RunOnSubTool(t => t.SelectNone());
        }

        public virtual void DoneBeingUsed() {
            if (subTools.Count > 0) {
                subTools[currentSubTool].DoneBeingUsed();
            }
        }

        public event EventHandler StatusChanged;
        private string status = "";
        public string Status {
            get {
                foreach (MapTool subTool in subTools) {
                    string subStatus = subTool.Status;
                    if (!string.IsNullOrEmpty(subStatus)) {
                        return subStatus;
                    }
                }
                return status;
            }
            protected set {
                status = value;
                StatusChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
