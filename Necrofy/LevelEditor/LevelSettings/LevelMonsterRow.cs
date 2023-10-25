using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Collections;
using System.Windows.Forms.Design.Behavior;

namespace Necrofy
{
    partial class LevelMonsterRow : UserControl
    {
        public LevelMonster Monster { get; private set; }

        public event EventHandler WasSelected;
        public event EventHandler DataChanged;

        private bool selected = false;
        public bool Selected {
            get {
                return selected;
            }
            private set {
                selected = value;
                if (selected) {
                    BackColor = SystemColors.Highlight;
                    SetForeColor(SystemColors.HighlightText);
                    WasSelected?.Invoke(this, EventArgs.Empty);
                } else {
                    BackColor = SystemColors.Window;
                    SetForeColor(SystemColors.ControlText);
                }
            }
        }

        private void SetForeColor(Color color) {
            foreach (Control control in Controls) {
                if (control is Label) {
                    control.ForeColor = color;
                }
            }
        }

        public LevelMonsterRow(LevelMonster monster) {
            InitializeComponent();
            Monster = monster;
            ControlAdded += Control_ControlAdded;
            Click += Control_Click;
        }

        public virtual void Remove() { }

        private void Control_ControlAdded(object sender, ControlEventArgs e) {
            e.Control.Click += Control_Click;
            e.Control.ControlAdded += Control_ControlAdded;
        }

        private void Control_Click(object sender, EventArgs e) {
            Selected = true;
            if (sender is Label || sender is LevelMonsterRow) {
                Focus();
            }
            foreach (Control control in Parent.Controls) {
                if (control != this && control is LevelMonsterRow row) {
                    row.Selected = false;
                }
            }
        }

        protected void RaiseDataChanged() {
            DataChanged?.Invoke(this, EventArgs.Empty);
        }

        // Included so sub-classes will work in the designer
        public LevelMonsterRow() : this(null) {
            if (LicenseManager.UsageMode != LicenseUsageMode.Designtime) {
                throw new Exception("LevelMonsterRow no parameter constructor called");
            }
        }

        private void LevelMonsterRow_Load(object sender, EventArgs e) {
            if (!DesignMode) {
                foreach (Control control in new Control[] { alignment1, alignment2, alignment3, alignment4 }) {
                    control.Visible = false;
                }
            }
        }
    }
}
