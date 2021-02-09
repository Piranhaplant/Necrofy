using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Necrofy
{
    public partial class ProjectSettingsDialog : Form
    {
        private static readonly HashSet<string> hiddenPatches = new HashSet<string>() {
            Project.ROMExpandPatchName, Project.OtherExpandPatchName, Project.RunFromLevelPatchName
        };

        private readonly ProjectSettings settings;

        private readonly List<string> patchDescriptions = new List<string>();
        private readonly List<ProjectSettings.Patch> hiddenEnabledPatches = new List<ProjectSettings.Patch>();

        public ProjectSettingsDialog(ProjectSettings settings) {
            InitializeComponent();
            this.settings = settings;

            foreach (string path in Directory.GetFiles(Project.internalPatchesPath)) {
                string fileName = Path.GetFileName(path);
                ProjectSettings.Patch existingPatch = settings.EnabledPatches.FirstOrDefault(p => p.Name == fileName);

                if (!hiddenPatches.Contains(fileName)) {
                    if (existingPatch != null) {
                        patchesList.Items.Add(existingPatch, true);
                    } else {
                        patchesList.Items.Add(new ProjectSettings.Patch(fileName));
                    }
                    patchDescriptions.Add(ReadDescription(path));
                } else if (existingPatch != null) {
                    hiddenEnabledPatches.Add(existingPatch);
                }
            }
        }

        private string ReadDescription(string filename) {
            StringBuilder s = new StringBuilder();
            try {
                foreach (string line in File.ReadLines(filename)) {
                    if (line.StartsWith(";")) {
                        s.AppendLine(line.Substring(1).Trim());
                    } else {
                        break;
                    }
                }
            } catch (Exception ex) {
                Console.WriteLine(ex);
            }
            return s.ToString();
        }

        private void patchesList_SelectedIndexChanged(object sender, EventArgs e) {
            if (patchesList.SelectedIndex > -1) {
                patchDescriptionText.Text = patchDescriptions[patchesList.SelectedIndex];
            } else {
                patchDescriptionText.Text = "";
            }
        }

        private void okButton_Click(object sender, EventArgs e) {
            settings.EnabledPatches = new List<ProjectSettings.Patch>(hiddenEnabledPatches);
            for (int i = 0; i < patchesList.Items.Count; i++) {
                if (patchesList.GetItemChecked(i)) {
                    settings.EnabledPatches.Add((ProjectSettings.Patch)patchesList.Items[i]);
                }
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e) {
            Close();
        }
    }
}
