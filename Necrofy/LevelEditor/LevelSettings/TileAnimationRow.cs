using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Necrofy
{
    partial class TileAnimationRow : LevelMonsterRow
    {
        private readonly TileAnimLevelMonster tileAnim;
        private readonly LevelSettingsDialog levelSettings;

        public TileAnimationRow(TileAnimLevelMonster tileAnim, LevelSettingsDialog levelSettings) : base(tileAnim) {
            InitializeComponent();
            this.tileAnim = tileAnim;
            this.levelSettings = levelSettings;

            if (levelSettings.presets.tileAnimations != null) {
                presetSelector.Items.AddRange(levelSettings.presets.tileAnimations.ToArray());
                presetSelector.SelectedItem = levelSettings.presets.tileAnimations.Where(p => p.value.SequenceEqual(tileAnim.entries)).FirstOrDefault();
            }
            presetSelector.SelectedIndexChanged += PresetSelector_SelectedIndexChanged;
        }

        private void PresetSelector_SelectedIndexChanged(object sender, EventArgs e) {
            if (presetSelector.SelectedIndex > -1) {
                tileAnim.entries = ((LevelSettingsPresets.Preset<List<TileAnimLevelMonster.Entry>>)presetSelector.SelectedItem).value;
            }
        }

        private void manualButton_Click(object sender, EventArgs e) {
            // TODO
        }
    }
}
