using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Necrofy
{
    partial class PaletteFadeRow : LevelMonsterRow
    {
        private readonly PaletteFadeLevelMonster paletteFade;
        private readonly LevelSettingsDialog levelSettings;

        public PaletteFadeRow(PaletteFadeLevelMonster paletteFade, LevelSettingsDialog levelSettings) : base(paletteFade) {
            InitializeComponent();
            this.paletteFade = paletteFade;
            this.levelSettings = levelSettings;

            levelSettings.PopulatePalettes(tilesetPaletteSelector);
            tilesetPaletteSelector.SelectedName = paletteFade.bgPal;
            levelSettings.TilesetPalettesChanged += LevelSettings_TilesetPalettesChanged;
            tilesetPaletteSelector.SelectedIndexChanged += TilesetPaletteSelector_SelectedIndexChanged;

            levelSettings.PopulateSpritePalettes(spritePaletteSelector);
            spritePaletteSelector.SelectedName = paletteFade.spritePal;
            spritePaletteSelector.SelectedIndexChanged += SpritePaletteSelector_SelectedIndexChanged;
        }

        public override void Remove() {
            levelSettings.TilesetPalettesChanged -= LevelSettings_TilesetPalettesChanged;
        }

        private void LevelSettings_TilesetPalettesChanged(object sender, EventArgs e) {
            levelSettings.PopulatePalettes(tilesetPaletteSelector);
            tilesetPaletteSelector.SelectedIndex = 0;
        }

        private void TilesetPaletteSelector_SelectedIndexChanged(object sender, EventArgs e) {
            if (tilesetPaletteSelector.SelectedIndex > -1) {
                paletteFade.bgPal = tilesetPaletteSelector.SelectedName;
                RaiseDataChanged();
            }
        }

        private void SpritePaletteSelector_SelectedIndexChanged(object sender, EventArgs e) {
            if (spritePaletteSelector.SelectedIndex > -1) {
                paletteFade.spritePal = spritePaletteSelector.SelectedName;
                RaiseDataChanged();
            }
        }
    }
}
