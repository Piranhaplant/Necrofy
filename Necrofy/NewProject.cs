using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Necrofy
{
    public partial class NewProject : Form
    {
        public string BaseROM { get; private set; }
        public string ProjectLocation { get; private set; }

        public NewProject() {
            InitializeComponent();
        }

        private void btnBaseROM_Click(object sender, EventArgs e) {
            if (ofdBaseROM.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                txtBaseROM.Text = ofdBaseROM.FileName;
            }
        }

        private void btnLocation_Click(object sender, EventArgs e) {
            if (sfdLocation.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                txtLocation.Text = sfdLocation.FileName;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e) {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void btnCreate_Click(object sender, EventArgs e) {
            // TODO: Check if file/folder exists
            BaseROM = txtBaseROM.Text;
            ProjectLocation = txtLocation.Text;
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }
    }
}
