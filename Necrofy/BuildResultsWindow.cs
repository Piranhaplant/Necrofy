using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace Necrofy
{
    public partial class BuildResultsWindow : DockContent
    {
        private BuildResults results;

        public BuildResultsWindow() {
            InitializeComponent();
        }

        public void ShowResults(BuildResults results) {
            this.results = results;
            dataGrid.Rows.Clear();

            foreach (BuildResults.Entry entry in results.Entries) {
                dataGrid.Rows.Add(GetLevelBitmap(entry.level), entry.file, entry.description);
            }
        }

        private Bitmap GetLevelBitmap(BuildResults.Entry.Level level) {
            switch (level) {
                case BuildResults.Entry.Level.ERROR:
                    return Properties.Resources.cross_circle;
                case BuildResults.Entry.Level.WARNING:
                    return Properties.Resources.exclamation;
                case BuildResults.Entry.Level.INFO:
                default:
                    return Properties.Resources.information;
            }
        }

        private void resultsShowDetails_Click(object sender, EventArgs e) {
            if (dataGrid.SelectedCells.Count > 0) {
                BuildResults.Entry entry = results.Entries[dataGrid.SelectedCells[0].RowIndex];
                new TextDisplayForm("Error Details", entry.description + Environment.NewLine + entry.stackTrace).ShowDialog();
            }
        }

        private void dataGrid_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e) {
            if (e.ColumnIndex > -1 && e.RowIndex > -1) {
                DataGridViewCell cell = dataGrid[e.ColumnIndex, e.RowIndex];
                dataGrid.CurrentCell = cell;
                cell.Selected = true;
            }
        }
    }
}
