using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Necrofy
{
    partial class AssetExtractorDialog : Form
    {
        private readonly Project project;
        private readonly List<ExtractionPreset> presets;

        public AssetExtractorDialog(Project project) {
            InitializeComponent();
            dataGrid.AutoGenerateColumns = false;

            this.project = project;
            presets = EditorAsset<ExtractionFile>.FromProject(project, "AssetExtractionPresets", new AssetOptions.AssetExtractionJsonConverter()).data.Presets;

            for (int i = 0; i < presets.Count; i++) {
                ExtractionPreset preset = presets[i];
                dataGrid.Rows.Add(i, preset.Description, preset.Category, preset.Type.ToString(), "0x" + preset.Address.ToString("X6"), "0x" + preset.Length.ToString("X4"));
            }

            project.Assets.AssetAdded += AssetsChanged;
            project.Assets.AssetChanged += AssetsChanged;
            project.Assets.AssetRemoved += AssetsChanged;
            UpdateHiddenRows();
        }

        private void AssetExtractorDialog_Load(object sender, EventArgs e) {
            dataGrid.ClearSelection();
        }

        private void AssetExtractorDialog_FormClosed(object sender, FormClosedEventArgs e) {
            project.Assets.AssetAdded -= AssetsChanged;
            project.Assets.AssetChanged -= AssetsChanged;
            project.Assets.AssetRemoved -= AssetsChanged;
        }

        private void AssetsChanged(object sender, AssetEventArgs e) {
            UpdateHiddenRows();
        }

        private void UpdateHiddenRows() {
            HashSet<int> existingAssetPointers = new HashSet<int>(project.Assets.Root.Enumerate().Select(a => a.Parts.pointer).Where(p => p != null).Cast<int>());

            for (int i = 0; i < dataGrid.Rows.Count; i++) {
                int dataIndex = (int)dataGrid.Rows[i].Cells[0].Value;
                bool existing = existingAssetPointers.Contains(presets[dataIndex].Address);
                if (showExisting.Checked) {
                    dataGrid.Rows[i].Visible = true;
                } else {
                    dataGrid.Rows[i].Visible = !existing;
                }
                dataGrid.Rows[i].DefaultCellStyle.BackColor = existing ? SystemColors.ControlLight : SystemColors.Window;
            }
            UpdateSelectedRowCount();
        }

        private void dataGrid_SelectionChanged(object sender, EventArgs e) {
            UpdateSelectedRowCount();
        }

        private void UpdateSelectedRowCount() {
            int selectedRowCount = dataGrid.SelectedRows.Cast<DataGridViewRow>().Count(r => r.Visible);
            if (selectedRowCount > 0) {
                extractButton.Enabled = true;
                if (selectedRowCount == 1) {
                    extractButton.Text = "Extract 1 asset";
                } else {
                    extractButton.Text = $"Extract {selectedRowCount} assets";
                }
            } else {
                extractButton.Enabled = false;
                extractButton.Text = "Extract";
            }
        }

        private void extractButton_Click(object sender, EventArgs e) {
            ROMInfo romInfo = new ROMInfo();
            using (NStream s = new NStream(new FileStream(Path.Combine(project.path, Project.baseROMFilename), FileMode.Open, FileAccess.Read, FileShare.Read))) {
                for (int i = 0; i < dataGrid.SelectedRows.Count; i++) {
                    if (dataGrid.SelectedRows[i].Visible) {
                        ExtractionPreset preset = presets[(int)dataGrid.SelectedRows[i].Cells[0].Value];
                        Asset.Extract(s, romInfo, preset);
                    }
                }
            }

            foreach (Asset asset in romInfo.assets) {
                asset.Save(project);
            }
            project.settings.AssetOptions.Merge(romInfo.assetOptions);

            dataGrid.ClearSelection();
        }

        private void showExisting_CheckedChanged(object sender, EventArgs e) {
            UpdateHiddenRows();
        }
    }
}
