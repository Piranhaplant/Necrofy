using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Necrofy
{
    [ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.ContextMenuStrip)]
    public class SeparateCheckToolStripMenuItem : ToolStripMenuItem
    {
        private ToolStripMenuItem checkItem;
        private bool reordering = false;
        private bool disposed = false;

        public SeparateCheckToolStripMenuItem() {
            MouseMove += SeparateCheckToolStripMenuItem_MouseMove;
            MouseEnter += SeparateCheckToolStripMenuItem_MouseEnter;
            Click += SeparateCheckToolStripMenuItem_Click;
        }

        protected override void Dispose(bool disposing) {
            // Prevents some exception from happening if we try to do the stuff in OnParentChanged after getting disposed
            disposed = true;
            base.Dispose(disposing);
        }

        protected override void OnParentChanged(ToolStrip oldParent, ToolStrip newParent) {
            base.OnParentChanged(oldParent, newParent);
            if (DesignMode || reordering || disposed) return;

            if (checkItem != null && oldParent != null) {
                oldParent.Items.Remove(checkItem);
            }
            if (checkItem == null) {
                checkItem = new ToolStripMenuItem(Image);
                checkItem.Checked = _checked;
                checkItem.CheckedChanged += CheckItem_CheckedChanged;
                checkItem.MouseEnter += CheckItem_MouseEnter;
                checkItem.MouseLeave += CheckItem_MouseLeave;
            }
            if (newParent != null) {
                newParent.Items.Insert(newParent.Items.IndexOf(this) + 1, checkItem);
            }
            checkItem.CheckOnClick = true;
            checkItem.AutoSize = false;
            checkItem.Height = Height;
            checkItem.Width = checkItem.Height + checkItem.Padding.Horizontal + 3;
            checkItem.Margin = new Padding(0, -checkItem.Height, 0, 0);
        }

        public new event EventHandler CheckedChanged;

        private void CheckItem_CheckedChanged(object sender, EventArgs e) {
            _checked = checkItem.Checked;
            CheckedChanged?.Invoke(this, EventArgs.Empty);
        }

        private bool _checked = false;
        public new bool Checked {
            get {
                if (DesignMode) {
                    return base.Checked;
                } else {
                    return _checked;
                }
            }
            set {
                if (DesignMode) {
                    base.Checked = value;
                } else if (value != _checked) {
                    _checked = value;
                    if (checkItem != null) {
                        checkItem.Checked = _checked;
                    } else {
                        CheckedChanged?.Invoke(this, EventArgs.Empty);
                    }
                }
            }
        }
        public new CheckState CheckState { get; set; }

        public override Image Image {
            get {
                return base.Image;
            }
            set {
                base.Image = value;
                if (checkItem != null) {
                    checkItem.Image = value;
                }
            }
        }

        private List<SeparateCheckToolStripMenuItem> GetSiblings() {
            List<SeparateCheckToolStripMenuItem> items = new List<SeparateCheckToolStripMenuItem>();
            foreach (ToolStripItem i in Parent.Items) {
                if (i is SeparateCheckToolStripMenuItem thisTypeItem) {
                    items.Add(thisTypeItem);
                }
            }
            return items;
        }

        private void SeparateCheckToolStripMenuItem_MouseMove(object sender, MouseEventArgs e) {
            if (DesignMode) return;

            if (checkItem.ContentRectangle.Contains(e.Location)) {
                // Move all items so the checkboxes are before the text items in the drop down list
                // This will allow the checkboxes to be selected instead of the text item when the cursor moves over them
                List<SeparateCheckToolStripMenuItem> items = GetSiblings();
                ToolStrip parent = Parent;

                parent.SuspendLayout();
                foreach (SeparateCheckToolStripMenuItem i in items) {
                    i.reordering = true;
                    parent.Items.Remove(i);
                    parent.Items.Insert(parent.Items.IndexOf(i.checkItem) + 1, i);
                    i.checkItem.Margin = new Padding(0);
                    i.Margin = new Padding(0, -i.checkItem.Height, 0, 0);
                }
                parent.ResumeLayout();
                foreach (SeparateCheckToolStripMenuItem i in items) {
                    i.reordering = false;
                }
            }
        }

        private void SeparateCheckToolStripMenuItem_MouseEnter(object sender, EventArgs e) {
            if (DesignMode) return;

            // Move all items so the checkboxes are after the text items in the drop down list
            // This will make the checkboxes be rendered second so they still display as checked with the cursor is over the text item
            List<SeparateCheckToolStripMenuItem> items = GetSiblings();
            ToolStrip parent = Parent;

            parent.SuspendLayout();
            foreach (SeparateCheckToolStripMenuItem i in items) {
                i.reordering = true;
                parent.Items.Remove(i.checkItem);
                parent.Items.Insert(parent.Items.IndexOf(i) + 1, i.checkItem);
                i.checkItem.Margin = new Padding(0, -i.checkItem.Height, 0, 0);
                i.Margin = new Padding(0);
            }
            parent.ResumeLayout();
            foreach (SeparateCheckToolStripMenuItem i in items) {
                i.reordering = false;
            }
        }

        private void SeparateCheckToolStripMenuItem_Click(object sender, EventArgs e) {
            foreach (SeparateCheckToolStripMenuItem i in GetSiblings()) {
                i.Checked = i == this;
            }
        }

        private void CheckItem_MouseEnter(object sender, EventArgs e) {
            if (Parent is ToolStripDropDown dropDown) {
                dropDown.AutoClose = false;
            }
        }

        private void CheckItem_MouseLeave(object sender, EventArgs e) {
            if (Parent is ToolStripDropDown dropDown) {
                dropDown.AutoClose = true;
            }
        }
    }
}
