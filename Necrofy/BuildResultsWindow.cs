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
        public BuildResultsWindow() {
            InitializeComponent();
        }

        public void ShowResults(BuildResults results) {
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
    }
}
