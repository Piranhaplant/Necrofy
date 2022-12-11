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
            tool.Info1Changed += SubTool_Info1Changed;
            tool.Info2Changed += SubTool_Info2Changed;
        }

        private void SubTool_StatusChanged(object sender, EventArgs e) {
            StatusChanged?.Invoke(this, EventArgs.Empty);
        }

        private void SubTool_Info1Changed(object sender, EventArgs e) {
            Info1Changed?.Invoke(this, EventArgs.Empty);
        }

        private void SubTool_Info2Changed(object sender, EventArgs e) {
            Info2Changed?.Invoke(this, EventArgs.Empty);
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
        public virtual void MouseLeave() {
            RunOnSubTool(t => t.MouseLeave());
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

        private string GetSubToolValue(Func<MapTool, string> getter, string baseValue) {
            foreach (MapTool subTool in subTools) {
                string subValue = getter(subTool);
                if (!string.IsNullOrEmpty(subValue)) {
                    return subValue;
                }
            }
            return baseValue;
        }

        public event EventHandler StatusChanged;
        private string status = "";
        public string Status {
            get {
                return GetSubToolValue(t => t.Status, status);
            }
            protected set {
                status = value;
                StatusChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public event EventHandler Info1Changed;
        private string info1 = "";
        public string Info1 {
            get {
                return GetSubToolValue(t => t.Info1, info1);
            }
            protected set {
                info1 = value;
                Info1Changed?.Invoke(this, EventArgs.Empty);
            }
        }

        public event EventHandler Info2Changed;
        private string info2 = "";
        public string Info2 {
            get {
                return GetSubToolValue(t => t.Info2, info2);
            }
            protected set {
                info2 = value;
                Info2Changed?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
