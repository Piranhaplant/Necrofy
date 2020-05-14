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

            levelSettings.PopulatePalettes(tilesetPaletteSelector, paletteFade.bgPal);
            levelSettings.TilesetPalettesChanged += LevelSettings_TilesetPalettesChanged;
            tilesetPaletteSelector.SelectedIndexChanged += TilesetPaletteSelector_SelectedIndexChanged;

            spritePaletteSelector.Items.AddRange(levelSettings.project.GetAssetsInCategory(AssetCategory.Palette).Select(a => a.Name).ToArray());
            if (paletteFade.spritePal == PaletteAsset.SpritesName) {
                spritePaletteSelector.SelectedIndex = 0;
            } else {
                spritePaletteSelector.SelectedItem = paletteFade.spritePal;
            }
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
                paletteFade.bgPal = levelSettings.GetFullPaletteName((string)tilesetPaletteSelector.SelectedItem);
            }
        }

        private void SpritePaletteSelector_SelectedIndexChanged(object sender, EventArgs e) {
            if (spritePaletteSelector.SelectedIndex > -1) {
                if (spritePaletteSelector.SelectedIndex == 0) {
                    paletteFade.spritePal = PaletteAsset.SpritesName;
                } else {
                    paletteFade.spritePal = (string)spritePaletteSelector.SelectedItem;
                }
            }
        }
    }
}
