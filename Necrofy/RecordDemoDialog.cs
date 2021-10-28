using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Necrofy
{
    partial class RecordDemoDialog : Form
    {
        private readonly Project project;
        private Process emulatorProcess;
        private readonly int level;
        private int character => characterBox.SelectedIndex * 2;
        private DateTime startTime;

        private RadioButton[] slotButtons;
        private Dictionary<int, DemoAsset> existingDemos = new Dictionary<int, DemoAsset>();
        private int selectedSlot = -1;

        private Demo demo;
        private bool saved = false;
        
        public RecordDemoDialog(Project project, Process emulatorProcess, int level) {
            InitializeComponent();
            slotButtons = new RadioButton[] { slot0, slot1, slot2, slot3 };

            this.project = project;
            this.emulatorProcess = emulatorProcess;
            this.level = level;

            levelBox.Text = level.ToString();
            characterBox.SelectedIndex = 0;
            
            emulatorProcess.EnableRaisingEvents = true;
            emulatorProcess.Exited += EmulatorProcess_Exited;

            HidePanel(findSRAMPanel);
            HidePanel(endedEarlyPanel);
            HidePanel(savePanel);
            startTime = DateTime.Now;
        }
        
        private void CloseButton_click(object sender, EventArgs e) {
            Close();
        }

        private void RecordDemoDialog_FormClosing(object sender, FormClosingEventArgs e) {
            if (savePanel.Visible && !saved && MessageBox.Show("Are you sure you want to cancel? The currently recorded demo will be lost.", "Discard demo", MessageBoxButtons.YesNo) != DialogResult.Yes) {
                e.Cancel = true;
            }
        }

        private void EmulatorProcess_Exited(object sender, EventArgs e) {
            if (!IsHandleCreated) return;
            Invoke((MethodInvoker)delegate {
                string saveFile = FindSaveFile();
                if (saveFile == null) {
                    ShowPanel(findSRAMPanel);
                } else {
                    OpenSaveFile(saveFile);
                }
                HidePanel(recordingPanel);
            });
        }

        private string FindSaveFile() {
            string sramName = Path.GetFileNameWithoutExtension(Project.recordDemoFilename) + ".srm";
            string sramPath = Path.Combine(project.BuildDirectory, sramName);
            if (IsSaveFileValid(sramPath)) {
                return sramPath;
            }
            string emulatorPath = Project.GetEmulatorExecutable(Path.Combine(project.BuildDirectory, Project.recordDemoFilename));
            if (emulatorPath != null) {
                sramPath = Path.Combine(Path.GetDirectoryName(emulatorPath), "Saves", sramName); // Snes9x save folder
                if (IsSaveFileValid(sramPath)) {
                    return sramPath;
                }
            }
            return null;
        }

        private bool IsSaveFileValid(string sramPath) {
            return File.Exists(sramPath) && File.GetLastWriteTime(sramPath) > startTime;
        }

        private void browseButton_Click(object sender, EventArgs e) {
            if (openSRAM.ShowDialog() == DialogResult.OK) {
                OpenSaveFile(openSRAM.FileName);
                HidePanel(findSRAMPanel);
            }
        }

        private void OpenSaveFile(string saveFile) {
            while (demo == null) {
                try {
                    using (FileStream fs = new FileStream(saveFile, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                        demo = new Demo(fs, (ushort)level, 0);
                    }
                } catch (Exception ex) {
                    if (MessageBox.Show(ex.Message, "Error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error) == DialogResult.Cancel) {
                        Close();
                        return;
                    }
                }
            }

            if (demo.inputs.Count > 0) {
                int frameLength = demo.inputs.Sum(i => i.duration);
                demoLengthBox.Text = $"{frameLength / 3600:00}:{(frameLength % 3600) / 60.0:00.00}";
                ShowPanel(savePanel);

                foreach (Asset.NameInfo nameInfo in project.Assets.Root.Enumerate()) {
                    DemoAsset asset = DemoAsset.FromNameInfo(nameInfo, project);
                    if (asset != null) {
                        existingDemos.Add(asset.slot, asset);
                        if (asset.slot < slotButtons.Length) {
                            slotButtons[asset.slot].ForeColor = Color.Red;
                        }
                    }
                }
            } else {
                ShowPanel(endedEarlyPanel);
            }
        }

        private void slot_CheckedChanged(object sender, EventArgs e) {
            RadioButton button = (RadioButton)sender;
            if (button.Checked) {
                selectedSlot = Array.IndexOf(slotButtons, button);
                if (existingDemos.TryGetValue(selectedSlot, out DemoAsset asset)) {
                    slotLabel.Text = $"Selected slot is already used on level {asset.demo.level}. Saving will overwrite the existing demo.";
                    slotLabel.ForeColor = Color.Red;
                } else {
                    slotLabel.Text = "Slot available.";
                    slotLabel.ForeColor = SystemColors.ControlText;
                }
                saveButton.Enabled = true;
            }
        }

        private void saveButton_Click(object sender, EventArgs e) {
            demo.character = (ushort)character;
            new DemoAsset(selectedSlot, demo).Save(project);
            saved = true;
            Close();
        }

        private void HidePanel(Panel panel) {
            panel.Visible = false;
            Height -= panel.Height;
        }

        private void ShowPanel(Panel panel) {
            panel.Visible = true;
            Height += panel.Height;
        }
    }
}
