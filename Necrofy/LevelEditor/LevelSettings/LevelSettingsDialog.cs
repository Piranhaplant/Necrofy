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

            presets = EditorAsset<LevelSettingsPresets>.FromProject(project, "LevelSettingsPresets").data;

            tilesSelector.Add(project.GetAssetsInCategory(AssetCategory.Tilemap).Where(a => a.ParsedName.Tileset != null), TilemapAsset.DefaultName);
            tilesSelector.SelectedName = level.Level.tilesetTilemapName;

            PopulatePalettes(tilesetPaletteSelector);
            tilesetPaletteSelector.SelectedName = level.Level.paletteName;

            if (presets.paletteAnimations != null) {
                paletteAnimationSelector.Items.AddRange(presets.paletteAnimations.ToArray());
                paletteAnimationSelector.SelectedItem = presets.paletteAnimations.Where(p => p.value == level.Level.paletteAnimationPtr).FirstOrDefault();
            }

            graphicsSelector.Add(project.GetAssetsInCategory(AssetCategory.Graphics).Where(a => a.ParsedName.Tileset != null), GraphicsAsset.DefaultName);
            graphicsSelector.SelectedName = level.Level.tilesetGraphicsName;
            string defaultGraphicsName = GetDefaultName(level.Level.tilesetTilemapName, GraphicsAsset.DefaultName);
            SetupAutoCheckBox(graphicsAuto, graphicsSelector, level.Level.tilesetGraphicsName == defaultGraphicsName, graphicsSelector.Contains(defaultGraphicsName));

            collisionSelector.Add(project.GetAssetsInCategory(AssetCategory.Collision).Where(a => a.ParsedName.Tileset != null), CollisionAsset.DefaultName);
            collisionSelector.SelectedName = level.Level.tilesetCollisionName;
            string defaultCollisionName = GetDefaultName(level.Level.tilesetTilemapName, CollisionAsset.DefaultName);
            SetupAutoCheckBox(collisionAuto, collisionSelector, level.Level.tilesetCollisionName == defaultCollisionName, collisionSelector.Contains(defaultCollisionName));

            prioritySelector.Value = level.Level.priorityTileCount;
            SetupAutoCheckBox(priorityAuto, prioritySelector, level.Level.priorityTileCount == level.TilesetSuggestions.PriorityTileCount, level.TilesetSuggestions.PriorityTileCount > 0);

            visibleEndSelector.Value = level.Level.visibleTilesEnd;
            SetupAutoCheckBox(visibleEndAuto, visibleEndSelector, level.Level.visibleTilesEnd == DefaultVisibleTilesEnd, true);

            PopulateSpritePalettes(spritePaletteSelector);
            spritePaletteSelector.SelectedName = level.Level.spritePaletteName;

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
                    bonusList.Items.Add(new LevelSettingsPresets.Preset<ushort>($"Custom (0x{bonus:X})", bonus));
                    bonusList.SetItemChecked(bonusList.Items.Count - 1, true);
                }
            }

            if (presets.secretBonuses != null) {
                secretBonusTypeSelector.Items.AddRange(presets.secretBonuses.ToArray());
                secretBonusTypeSelector.SelectedItem = presets.secretBonuses.Where(p => p.value.Contains(level.Level.secretBonusCodePointer)).FirstOrDefault();
            }

            bonusLevelSelector.Value = level.Level.bonusLevelNumber;
            
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
        
        private void SetupAutoCheckBox(CheckBox autoCheckBox, Control manualControl, bool check, bool enabled) {
            autoCheckBox.CheckedChanged += (sender, e) => {
                manualControl.Visible = !autoCheckBox.Checked;
            };
            autoCheckBox.Checked = check && enabled;
            autoCheckBox.Enabled = enabled;
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
        
        private string GetDefaultName(string tilemapName, string defaultName) {
            return new Asset.ParsedName(tilemapName).Folder + Asset.FolderSeparator + defaultName;
        }

        public void PopulatePalettes(AssetComboBox comboBox) {
            comboBox.Clear();
            if (tilesSelector.SelectedItem != null) {
                string folder = tilesSelector.SelectedItem.ParsedName.Folder;
                comboBox.Add(project.GetAssetsInCategory(AssetCategory.Palette).Where(a => a.ParsedName.Folder == folder), PaletteAsset.DefaultName, showTilesetName: false);
            }
        }

        public void PopulateSpritePalettes(AssetComboBox comboBox) {
            comboBox.Add(project.GetAssetsInCategory(AssetCategory.Palette).Where(a => a.ParsedName.Folder == Asset.SpritesFolder), PaletteAsset.DefaultName);
        }
        
        private void addLevelEffect_Click(object sender, EventArgs e) {
            addPaletteFade.Enabled = !levelMonsters.Any(m => m is PaletteFadeLevelMonster);
            addTileAnimation.Enabled = !levelMonsters.Any(m => m is TileAnimLevelMonster);
            addLevelEffectMenu.Show(addLevelEffect, 0, addLevelEffect.Height);
        }

        private void addPaletteFade_Click(object sender, EventArgs e) {
            PaletteFadeLevelMonster paletteFade = new PaletteFadeLevelMonster(tilesetPaletteSelector.GetAnyName(), spritePaletteSelector.GetAnyName());
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

        private void secretBonusTypeSelector_SelectedIndexChanged(object sender, EventArgs e) {
            bonusLevelSelector.Enabled = ((LevelSettingsPresets.Preset<HashSet<ushort>>)secretBonusTypeSelector.SelectedItem).name == "Bonus level";
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

            if (tilesSelector.SelectedIndex > -1 && level.Level.tilesetTilemapName != tilesSelector.SelectedName) {
                level.Level.tilesetTilemapName = tilesSelector.SelectedName;
                level.LoadTilesetSuggestions(project);
                reloadTileset = true;
            }

            if (tilesetPaletteSelector.SelectedIndex > -1 && level.Level.paletteName != tilesetPaletteSelector.SelectedName) {
                reloadTileset = true;
                level.Level.paletteName = tilesetPaletteSelector.SelectedName;
            }

            if (paletteAnimationSelector.SelectedIndex > -1) {
                level.Level.paletteAnimationPtr = ((LevelSettingsPresets.Preset<int>)paletteAnimationSelector.SelectedItem).value;
            }

            if (graphicsAuto.Checked) {
                string graphicsName = GetDefaultName(level.Level.tilesetTilemapName, GraphicsAsset.DefaultName);
                reloadTileset |= level.Level.tilesetGraphicsName != graphicsName;
                level.Level.tilesetGraphicsName = graphicsName;
            } else if (graphicsSelector.SelectedIndex > -1 && level.Level.tilesetGraphicsName != graphicsSelector.SelectedName) {
                level.Level.tilesetGraphicsName = graphicsSelector.SelectedName;
                reloadTileset = true;
            }

            if (collisionAuto.Checked) {
                string collisionName = GetDefaultName(level.Level.tilesetTilemapName, CollisionAsset.DefaultName);
                reloadTileset |= level.Level.tilesetCollisionName != collisionName;
                level.Level.tilesetCollisionName = collisionName;
            } else if (collisionSelector.SelectedIndex > -1 && level.Level.tilesetCollisionName != collisionSelector.SelectedName) {
                level.Level.tilesetCollisionName = collisionSelector.SelectedName;
                reloadTileset = true;
            }

            ushort priorityValue = priorityAuto.Checked ? level.TilesetSuggestions.PriorityTileCount : (ushort)prioritySelector.Value;
            reloadTileset |= level.Level.priorityTileCount != priorityValue;
            level.Level.priorityTileCount = priorityValue;

            ushort visibleEndValue = visibleEndAuto.Checked ? DefaultVisibleTilesEnd : (ushort)visibleEndSelector.Value;
            reloadTileset |= level.Level.visibleTilesEnd != visibleEndValue;
            level.Level.visibleTilesEnd = visibleEndValue;

            if (level.Level.spritePaletteName != spritePaletteSelector.SelectedName) {
                level.Level.spritePaletteName = spritePaletteSelector.SelectedName;
                level.LoadSprites(project);
            }

            if (musicSelector.SelectedIndex > -1) {
                level.Level.music = ((LevelSettingsPresets.Preset<ushort>)musicSelector.SelectedItem).value;
            }

            if (soundsSelector.SelectedIndex > -1) {
                level.Level.sounds = ((LevelSettingsPresets.Preset<ushort>)soundsSelector.SelectedItem).value;
            }

            if (secretBonusTypeSelector.SelectedIndex > -1) {
                level.Level.secretBonusCodePointer = ((LevelSettingsPresets.Preset<HashSet<ushort>>)secretBonusTypeSelector.SelectedItem).value.FirstOrDefault();
            }

            level.Level.bonusLevelNumber = (ushort)bonusLevelSelector.Value;

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
