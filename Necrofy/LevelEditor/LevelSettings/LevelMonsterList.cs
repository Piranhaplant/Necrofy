using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Necrofy
{
    class LevelMonsterList : Panel
    {
        private readonly List<LevelMonsterRow> rows = new List<LevelMonsterRow>();

        public event EventHandler SelectedRowChanged;
        public event EventHandler DataChanged;

        private LevelMonsterRow selectedRow = null;
        public LevelMonsterRow SelectedRow {
            get {
                return selectedRow;
            }
            private set {
                selectedRow = value;
                SelectedRowChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public LevelMonsterList() {
            AutoScroll = true;
        }

        public void AddRow(LevelMonsterRow row) {
            Controls.Add(row);
            if (rows.Count == 0) {
                row.Location = Point.Empty;
            } else {
                row.Location = new Point(0, rows[rows.Count - 1].Bounds.Bottom);
            }
            rows.Add(row);
            row.WasSelected += Row_WasSelected;
            row.DataChanged += Row_DataChanged;
        }

        public void RemoveRow(LevelMonsterRow row) {
            if (rows.Remove(row)) {
                Controls.Remove(row);
                row.Remove();
                for (int i = 0; i < rows.Count; i++) {
                    if (i == 0) {
                        rows[i].Location = Point.Empty;
                    } else {
                        rows[i].Location = new Point(0, rows[i - 1].Bounds.Bottom);
                    }
                }
                if (row == selectedRow) {
                    SelectedRow = null;
                }
            }
        }

        private void Row_WasSelected(object sender, EventArgs e) {
            SelectedRow = sender as LevelMonsterRow;
        }

        private void Row_DataChanged(object sender, EventArgs e) {
            DataChanged?.Invoke(sender, e);
        }

        public void ScrollToBottom() {
            if (rows.Count > 0) {
                ScrollControlIntoView(rows[rows.Count - 1]);
            }
        }
    }
}
