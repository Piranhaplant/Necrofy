using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Necrofy
{
    public partial class NewProjectDialog : Form
    {
        public string BaseROM { get; private set; }
        public string ProjectLocation { get; private set; }

        public NewProjectDialog() {
            InitializeComponent();
        }

        private void btnBaseROM_Click(object sender, EventArgs e) {
            if (ofdBaseROM.ShowDialog() == DialogResult.OK) {
                txtBaseROM.Text = ofdBaseROM.FileName;
            }
        }

        private void btnLocation_Click(object sender, EventArgs e) {
            if (sfdLocation.ShowDialog() == DialogResult.OK) {
                txtLocation.Text = sfdLocation.FileName;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e) {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void btnCreate_Click(object sender, EventArgs e) {
            if (Directory.Exists(txtLocation.Text)) {
                if (MessageBox.Show($"The folder {txtLocation.Text} already exists and will be deleted. Is this okay?", "Project location already exists", MessageBoxButtons.OKCancel) == DialogResult.Cancel) {
                    return;
                }
                Directory.Delete(txtLocation.Text, true);
            }
            BaseROM = txtBaseROM.Text;
            ProjectLocation = txtLocation.Text;
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
