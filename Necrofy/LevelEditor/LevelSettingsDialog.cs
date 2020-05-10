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
    partial class LevelSettingsDialog : Form
    {
        private const ushort DefaultVisibleTilesEnd = 0x1ff;

        private readonly Project project;
        private readonly LevelEditor levelEditor;

        public LevelSettingsDialog(Project project, LevelEditor levelEditor) {
            InitializeComponent();
            this.project = project;
            this.levelEditor = levelEditor;
            LoadedLevel level = levelEditor.level;

            // TODO Error handling
            LevelSettingsPresets presets = EditorAsset<LevelSettingsPresets>.FromProject(project, "LevelSettingsPresets").data;

            tilesSelector.Items.AddRange(project.GetAssetsInCategory(AssetCategory.TilesetTilemap).Select(a => a.Name).ToArray());
            tilesSelector.SelectedItem = level.Level.tilesetTilemapName;

            PopulatePalettes(level.Level.tilesetTilemapName, selectedItem: level.Level.paletteName);

            if (presets.paletteAnimation != null) {
                paletteAnimationSelector.Items.AddRange(presets.paletteAnimation.ToArray());
                paletteAnimationSelector.SelectedItem = presets.paletteAnimation.Where(p => p.value == level.Level.paletteAnimationPtr).FirstOrDefault();
            }

            graphicsSelector.Items.AddRange(project.GetAssetsInCategory(AssetCategory.TilesetGraphics).Select(a => a.Name).ToArray());
            graphicsSelector.SelectedItem = level.Level.tilesetGraphicsName;
            SetupAutoCheckBox(graphicsAuto, graphicsSelector, level.Level.tilesetGraphicsName == level.Level.tilesetTilemapName);

            collisionSelector.Items.AddRange(project.GetAssetsInCategory(AssetCategory.Collision).Select(a => a.Name).ToArray());
            collisionSelector.SelectedItem = level.Level.tilesetCollisionName;
            SetupAutoCheckBox(collisionAuto, collisionSelector, level.Level.tilesetCollisionName == level.Level.tilesetTilemapName);

            prioritySelector.Value = level.Level.priorityTileCount;
            SetupAutoCheckBox(priorityAuto, prioritySelector, level.Level.priorityTileCount == level.TilesetSuggestions.PriorityTileCount);

            visibleEndSelector.Value = level.Level.visibleTilesEnd;
            SetupAutoCheckBox(visibleEndAuto, visibleEndSelector, level.Level.visibleTilesEnd == DefaultVisibleTilesEnd);

            spritePaletteSelector.Items.AddRange(project.GetAssetsInCategory(AssetCategory.Palette).Select(a => a.Name).ToArray());
            spritePaletteSelector.SelectedItem = level.Level.spritePaletteName;
            SetupAutoCheckBox(spritePaletteAuto, spritePaletteSelector, level.Level.spritePaletteName == PaletteAsset.SpritePaletteName);

            if (presets.music != null) {
                musicSelector.Items.AddRange(presets.music.ToArray());
                musicSelector.SelectedItem = presets.music.Where(p => p.value == level.Level.music).FirstOrDefault();
            }

            if (presets.sounds != null) {
                soundsSelector.Items.AddRange(presets.sounds.ToArray());
                soundsSelector.SelectedItem = presets.sounds.Where(p => p.value == level.Level.sounds).FirstOrDefault();
            }
        }

        private void SetupAutoCheckBox(CheckBox autoCheckBox, Control manualControl, bool enabled) {
            autoCheckBox.CheckedChanged += (sender, e) => {
                manualControl.Visible = !autoCheckBox.Checked;
            };
            autoCheckBox.Checked = enabled;
        }

        private int prevTilesSelectorSelectedIndex = -1;
        private void tilesSelector_SelectedIndexChanged(object sender, EventArgs e) {
            if (tilesSelector.SelectedIndex != prevTilesSelectorSelectedIndex) {
                PopulatePalettes((string)tilesSelector.SelectedItem);
                tilesetPaletteSelector.SelectedIndex = 0;
                prevTilesSelectorSelectedIndex = tilesSelector.SelectedIndex;
            }
        }

        private void PopulatePalettes(string tileset, string selectedItem = null) {
            string palleteNamePrefix = tileset + TilesetAsset.NameSeparator;
            tilesetPaletteSelector.Items.Clear();
            tilesetPaletteSelector.Items.AddRange(
                project.GetAssetsInCategory(AssetCategory.TilesetPalette)
                .Where(a => a.Name.StartsWith(palleteNamePrefix))
                .Select(a => a.Name.Substring(palleteNamePrefix.Length)).ToArray());
            if (selectedItem != null && selectedItem.StartsWith(palleteNamePrefix)) {
                tilesetPaletteSelector.SelectedItem = selectedItem.Substring(palleteNamePrefix.Length);
            }
        }

        private void applyButton_Click(object sender, EventArgs e) {
            Save();
        }

        private void okButton_Click(object sender, EventArgs e) {
            Save();
            Close();
        }

        private void Save() {
            LoadedLevel level = levelEditor.level;
            bool reloadTileset = false;

            if (!level.Level.tilesetTilemapName.Equals(tilesSelector.SelectedItem)) {
                level.Level.tilesetTilemapName = (string)tilesSelector.SelectedItem;
                level.LoadTilesetSuggestions(project);
                reloadTileset = true;
            }

            if (tilesetPaletteSelector.SelectedIndex > -1) {
                string newPaletteName = level.Level.tilesetTilemapName + TilesetAsset.NameSeparator + (string)tilesetPaletteSelector.SelectedItem;
                reloadTileset |= !level.Level.paletteName.Equals(newPaletteName);
                level.Level.paletteName = newPaletteName;
            }

            if (paletteAnimationSelector.SelectedIndex > -1) {
                level.Level.paletteAnimationPtr = ((LevelSettingsPresets.Preset<int>)paletteAnimationSelector.SelectedItem).value;
            }

            if (graphicsAuto.Checked) {
                reloadTileset |= !level.Level.tilesetGraphicsName.Equals(level.Level.tilesetTilemapName);
                level.Level.tilesetGraphicsName = level.Level.tilesetTilemapName;
            } else {
                reloadTileset |= !level.Level.tilesetGraphicsName.Equals(graphicsSelector.SelectedItem);
                level.Level.tilesetGraphicsName = (string)graphicsSelector.SelectedItem;
            }

            if (collisionAuto.Checked) {
                reloadTileset |= !level.Level.tilesetCollisionName.Equals(level.Level.tilesetTilemapName);
                level.Level.tilesetCollisionName = level.Level.tilesetTilemapName;
            } else {
                reloadTileset |= !level.Level.tilesetCollisionName.Equals(collisionSelector.SelectedItem);
                level.Level.tilesetCollisionName = (string)collisionSelector.SelectedItem;
            }

            if (priorityAuto.Checked) {
                reloadTileset |= level.Level.priorityTileCount != level.TilesetSuggestions.PriorityTileCount;
                level.Level.priorityTileCount = level.TilesetSuggestions.PriorityTileCount;
            } else {
                reloadTileset |= level.Level.priorityTileCount != (ushort)prioritySelector.Value;
                level.Level.priorityTileCount = (ushort)prioritySelector.Value;
            }

            if (visibleEndAuto.Checked) {
                reloadTileset |= level.Level.visibleTilesEnd != DefaultVisibleTilesEnd;
                level.Level.visibleTilesEnd = DefaultVisibleTilesEnd;
            } else {
                reloadTileset |= level.Level.visibleTilesEnd != (ushort)visibleEndSelector.Value;
                level.Level.visibleTilesEnd = (ushort)visibleEndSelector.Value;
            }

            if (reloadTileset) {
                level.LoadTiles(project);
            }

            bool reloadSprites = false;
            if (spritePaletteAuto.Checked) {
                reloadSprites |= level.Level.spritePaletteName != PaletteAsset.SpritePaletteName;
                level.Level.spritePaletteName = PaletteAsset.SpritePaletteName;
            } else {
                reloadSprites |= !level.Level.spritePaletteName.Equals(spritePaletteSelector.SelectedItem);
                level.Level.spritePaletteName = (string)spritePaletteSelector.SelectedItem;
            }
            if (reloadSprites) {
                level.LoadSprites(project);
            }

            if (musicSelector.SelectedIndex > -1) {
                level.Level.music = ((LevelSettingsPresets.Preset<ushort>)musicSelector.SelectedItem).value;
            }

            if (soundsSelector.SelectedIndex > -1) {
                level.Level.sounds = ((LevelSettingsPresets.Preset<ushort>)soundsSelector.SelectedItem).value;
            }

            levelEditor.undoManager.ForceDirty();
            levelEditor.Repaint();
        }
    }
}
