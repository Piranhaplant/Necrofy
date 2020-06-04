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

        public event EventHandler TilesetPalettesChanged;

        public readonly Project project;
        public readonly LevelEditor levelEditor;
        public readonly LevelSettingsPresets presets;

        private readonly List<LevelMonster> levelMonsters = new List<LevelMonster>();

        public LevelSettingsDialog(Project project, LevelEditor levelEditor) {
            InitializeComponent();
            this.project = project;
            this.levelEditor = levelEditor;
            LoadedLevel level = levelEditor.level;

            // TODO Error handling
            presets = EditorAsset<LevelSettingsPresets>.FromProject(project, "LevelSettingsPresets").data;

            tilesSelector.Items.AddRange(project.GetAssetsInCategory(AssetCategory.TilesetTilemap).Select(a => a.Name).ToArray());
            tilesSelector.SelectedItem = level.Level.tilesetTilemapName;

            PopulatePalettes(tilesetPaletteSelector, selectedItem: level.Level.paletteName);

            if (presets.paletteAnimations != null) {
                paletteAnimationSelector.Items.AddRange(presets.paletteAnimations.ToArray());
                paletteAnimationSelector.SelectedItem = presets.paletteAnimations.Where(p => p.value == level.Level.paletteAnimationPtr).FirstOrDefault();
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
            SetupAutoCheckBox(spritePaletteAuto, spritePaletteSelector, level.Level.spritePaletteName == PaletteAsset.SpritesName);

            if (presets.music != null) {
                musicSelector.Items.AddRange(presets.music.ToArray());
                musicSelector.SelectedItem = presets.music.Where(p => p.value == level.Level.music).FirstOrDefault();
            }

            if (presets.sounds != null) {
                soundsSelector.Items.AddRange(presets.sounds.ToArray());
                soundsSelector.SelectedItem = presets.sounds.Where(p => p.value == level.Level.sounds).FirstOrDefault();
            }

            bonusList.Items.AddRange(presets.bonuses.ToArray());
            foreach (ushort bonus in level.Level.bonuses) {
                int index = presets.bonuses.FindIndex(p => p.value == bonus);
                if (index > -1) {
                    bonusList.SetItemChecked(index, true);
                } else {
                    bonusList.Items.Add(new LevelSettingsPresets.Preset<ushort>($"Custom ({bonus})", bonus));
                    bonusList.SetItemChecked(bonusList.Items.Count - 1, true);
                }
            }
            
            foreach (LevelMonster monster in level.Level.levelMonsters) {
                if (monster is PaletteFadeLevelMonster paletteFadeLevelMonster) {
                    PaletteFadeLevelMonster clone = paletteFadeLevelMonster.JsonClone();
                    levelMonsterList.AddRow(new PaletteFadeRow(clone, this));
                    levelMonsters.Add(clone);
                } else if (monster is TileAnimLevelMonster tileAnimLevelMonster) {
                    TileAnimLevelMonster clone = tileAnimLevelMonster.JsonClone();
                    levelMonsterList.AddRow(new TileAnimationRow(clone, this));
                    levelMonsters.Add(clone);
                } else {
                    levelMonsters.Add(monster);
                }
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
                PopulatePalettes(tilesetPaletteSelector);
                tilesetPaletteSelector.SelectedIndex = 0;
                prevTilesSelectorSelectedIndex = tilesSelector.SelectedIndex;
                TilesetPalettesChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public void PopulatePalettes(ComboBox comboBox, string selectedItem = null) {
            string palleteNamePrefix = tilesSelector.SelectedItem + TilesetAsset.NameSeparator;
            comboBox.Items.Clear();
            comboBox.Items.AddRange(
                project.GetAssetsInCategory(AssetCategory.TilesetPalette)
                .Where(a => a.Name.StartsWith(palleteNamePrefix))
                .Select(a => a.Name.Substring(palleteNamePrefix.Length)).ToArray());
            if (selectedItem != null && selectedItem.StartsWith(palleteNamePrefix)) {
                comboBox.SelectedItem = selectedItem.Substring(palleteNamePrefix.Length);
            }
        }

        public string GetFullPaletteName(string tilesetPalette) {
            return (string)tilesSelector.SelectedItem + TilesetAsset.NameSeparator + tilesetPalette;
        }

        private void addLevelEffect_Click(object sender, EventArgs e) {
            addLevelEffectMenu.Show(addLevelEffect, 0, addLevelEffect.Height);
        }

        private void addPaletteFade_Click(object sender, EventArgs e) {
            PaletteFadeLevelMonster paletteFade = new PaletteFadeLevelMonster(GetFullPaletteName((string)tilesetPaletteSelector.Items[0]), PaletteAsset.SpritesName);
            levelMonsters.Add(paletteFade);
            levelMonsterList.AddRow(new PaletteFadeRow(paletteFade, this));
            levelMonsterList.ScrollToBottom();
        }

        private void addTileAnimation_Click(object sender, EventArgs e) {
            TileAnimLevelMonster tileAnim = new TileAnimLevelMonster(new List<TileAnimLevelMonster.Entry>());
            levelMonsters.Add(tileAnim);
            levelMonsterList.AddRow(new TileAnimationRow(tileAnim, this));
            levelMonsterList.ScrollToBottom();
        }

        private void levelMonsterList_SelectedRowChanged(object sender, EventArgs e) {
            removeLevelEffect.Enabled = levelMonsterList.SelectedRow != null;
        }

        private void removeLevelEffect_Click(object sender, EventArgs e) {
            if (levelMonsterList.SelectedRow != null) {
                levelMonsters.Remove(levelMonsterList.SelectedRow.Monster);
                levelMonsterList.RemoveRow(levelMonsterList.SelectedRow);
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
                string newPaletteName = GetFullPaletteName((string)tilesetPaletteSelector.SelectedItem);
                reloadTileset |= !level.Level.paletteName.Equals(newPaletteName);
                level.Level.paletteName = newPaletteName;
            }

            if (paletteAnimationSelector.SelectedIndex > -1) {
                level.Level.paletteAnimationPtr = ((LevelSettingsPresets.Preset<int>)paletteAnimationSelector.SelectedItem).value;
            }

            string graphicsName = graphicsAuto.Checked ? level.Level.tilesetTilemapName : (string)graphicsSelector.SelectedItem;
            reloadTileset |= level.Level.tilesetGraphicsName != graphicsName;
            level.Level.tilesetGraphicsName = graphicsName;

            string collisionName = collisionAuto.Checked ? level.Level.tilesetTilemapName : (string)collisionSelector.SelectedItem;
            reloadTileset |= level.Level.tilesetCollisionName != collisionName;
            level.Level.tilesetCollisionName = collisionName;

            ushort priorityValue = priorityAuto.Checked ? level.TilesetSuggestions.PriorityTileCount : (ushort)prioritySelector.Value;
            reloadTileset |= level.Level.priorityTileCount != priorityValue;
            level.Level.priorityTileCount = priorityValue;

            ushort visibleEndValue = visibleEndAuto.Checked ? DefaultVisibleTilesEnd : (ushort)visibleEndSelector.Value;
            reloadTileset |= level.Level.visibleTilesEnd != visibleEndValue;
            level.Level.visibleTilesEnd = visibleEndValue;

            string spritePaletteName = spritePaletteAuto.Checked ? PaletteAsset.SpritesName : (string)spritePaletteSelector.SelectedItem;
            if (level.Level.spritePaletteName != spritePaletteName) {
                level.Level.spritePaletteName = spritePaletteName;
                level.LoadSprites(project);
            }

            if (musicSelector.SelectedIndex > -1) {
                level.Level.music = ((LevelSettingsPresets.Preset<ushort>)musicSelector.SelectedItem).value;
            }

            if (soundsSelector.SelectedIndex > -1) {
                level.Level.sounds = ((LevelSettingsPresets.Preset<ushort>)soundsSelector.SelectedItem).value;
            }

            level.Level.bonuses.Clear();
            for (int i = 0; i < bonusList.Items.Count; i++) {
                if (bonusList.GetItemChecked(i)) {
                    level.Level.bonuses.Add(((LevelSettingsPresets.Preset<ushort>)bonusList.Items[i]).value);
                }
            }

            if (!TileAnimationsEqual(level.Level.levelMonsters, levelMonsters)) {
                reloadTileset = true;
            }

            level.Level.levelMonsters.Clear();
            level.Level.levelMonsters.AddRange(levelMonsters);

            if (reloadTileset) {
                level.LoadTiles(project);
            }

            levelEditor.undoManager.ForceDirty();
            levelEditor.Repaint();
            levelEditor.BrowserContents.Repaint();
        }

        private static bool TileAnimationsEqual(List<LevelMonster> list1, List<LevelMonster> list2) {
            List<List<TileAnimLevelMonster.Entry>> entries1 = GetEntries(list1);
            List<List<TileAnimLevelMonster.Entry>> entries2 = GetEntries(list2);
            if (entries1.Count == entries2.Count) {
                for (int i = 0; i < entries1.Count; i++) {
                    if (!entries1[i].SequenceEqual(entries2[i])) {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        private static List<List<TileAnimLevelMonster.Entry>> GetEntries(List<LevelMonster> list) {
            return list.Select(m => m as TileAnimLevelMonster).Where(m => m != null).Select(m => m.entries).ToList();
        }
    }
}
