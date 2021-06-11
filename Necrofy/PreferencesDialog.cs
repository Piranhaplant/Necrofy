using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Necrofy
{
    public partial class PreferencesDialog : Form
    {
        public PreferencesDialog() {
            InitializeComponent();

            systemDefaultEmulator.Checked = Properties.Settings.Default.useSystemEmulator;
            emulatorText.Text = Properties.Settings.Default.emulator;
        }

        private void systemDefaultEmulator_CheckedChanged(object sender, EventArgs e) {
            emulatorText.Enabled = !systemDefaultEmulator.Checked;
            emulatorBrowse.Enabled = !systemDefaultEmulator.Checked;
        }

        private void emulatorBrowse_Click(object sender, EventArgs e) {
            if (openEmulator.ShowDialog() == DialogResult.OK) {
                emulatorText.Text = openEmulator.FileName;
            }
        }

        private void cancelButton_Click(object sender, EventArgs e) {
            Close();
        }

        private void okButton_Click(object sender, EventArgs e) {
            Properties.Settings.Default.useSystemEmulator = systemDefaultEmulator.Checked;
            Properties.Settings.Default.emulator = emulatorText.Text;
            Properties.Settings.Default.Save();
            Close();
        }
    }
}
