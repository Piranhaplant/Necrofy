using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace Necrofy
{
    public partial class ObjectBrowserForm : DockContent
    {
        public ObjectBrowserControl Browser {
            get {
                return objectBrowser;
            }
        }

        public ObjectBrowserForm() {
            InitializeComponent();
        }

        private void grid_Click(object sender, EventArgs e) {
            SetDisplayMode(false);
        }

        private void list_Click(object sender, EventArgs e) {
            SetDisplayMode(true);
        }

        private void SetDisplayMode(bool listMode) {
            objectBrowser.ListMode = listMode;
        }

        private void contextMenu_Opening(object sender, CancelEventArgs e) {
            grid.Checked = !objectBrowser.ListMode;
            list.Checked = objectBrowser.ListMode;
            list.Enabled = objectBrowser.SupportsListMode;
        }
    }
}
