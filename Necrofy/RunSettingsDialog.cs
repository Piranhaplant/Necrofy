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
    public partial class RunSettingsDialog : Form
    {
        private readonly Dictionary<string, RunSettings> savedSettings;
        public string currentSettings;

        public RunSettingsDialog(Dictionary<string, RunSettings> savedSettings, string currentSettings) {
            InitializeComponent();
            
            this.savedSettings = savedSettings;
            this.currentSettings = currentSettings;

            foreach (string presetName in savedSettings.Keys) {
                TreeNode node = presetTree.Nodes.Add(presetName);
                if (presetName == currentSettings) {
                    presetTree.SelectedNode = node;
                }
            }
            ListChanged();
        }

        private void ListChanged() {
            removeButton.Enabled = savedSettings.Count > 1;
        }

        private void RunSettingsDialog_Load(object sender, EventArgs e) {
            propertyGrid.SetSplitPosition(150);
        }

        private void closeButton_Click(object sender, EventArgs e) {
            Close();
        }
        
        private void presetTree_AfterSelect(object sender, TreeViewEventArgs e) {
            currentSettings = e.Node.Text;
            propertyGrid.SelectedObject = new SettingsWrapper(savedSettings[currentSettings]);
        }

        private void presetTree_AfterLabelEdit(object sender, NodeLabelEditEventArgs e) {
            string originalName = e.Node.Text;
            string newName = e.Label;
            if (newName == null || newName == originalName) {
                return;
            }
            if (savedSettings.ContainsKey(newName)) {
                newName = GenerateName(newName);
                e.CancelEdit = true;
                e.Node.Text = newName;
            }
            RunSettings settings = savedSettings[originalName];
            savedSettings.Remove(originalName);
            savedSettings[newName] = settings;
            currentSettings = newName;
        }
        
        private string GenerateName(string name) {
            if (!savedSettings.ContainsKey(name)) {
                return name;
            }
            int i = 2;
            while (savedSettings.ContainsKey(name + i.ToString())) {
                i++;
            }
            return name + i.ToString();
        }

        private void addButton_Click(object sender, EventArgs e) {
            string name = GenerateName("Enter preset name");
            savedSettings[name] = new RunSettings();

            TreeNode node = presetTree.Nodes.Add(name);
            presetTree.SelectedNode = node;
            node.BeginEdit();
            ListChanged();
        }

        private void removeButton_Click(object sender, EventArgs e) {
            if (MessageBox.Show($"Are you sure you want to delete the preset \"{currentSettings}\"?", "Delete preset?", MessageBoxButtons.OKCancel) == DialogResult.OK) {
                savedSettings.Remove(currentSettings);
                presetTree.Nodes.Remove(presetTree.SelectedNode);
                ListChanged();
            }
        }

        private class SettingsWrapper
        {
            // Putting the "\r"s at the beginning forces them to be sorted in the desired order
            private const string GeneralCategory = "\r\rGeneral";
            private const string WeaponsCategory = "\rWeapons";
            private const string SpecialsCategory = "Specials";

            private readonly RunSettings settings;

            public SettingsWrapper(RunSettings settings) {
                this.settings = settings;
            }

            private void Parse(ref ushort dest, string value, ushort min, ushort max) {
                if (ushort.TryParse(value, out ushort parsed)) {
                    if (parsed < min) parsed = min;
                    if (parsed > max) parsed = max;
                    dest = parsed;
                }
            }

            [Category(GeneralCategory)]
            public string Victims {
                get => settings.victimCount.ToString();
                set => Parse(ref settings.victimCount, value, 1, 10);
            }

            [Category(WeaponsCategory)]
            public string Squirtgun {
                get => settings.weaponAmounts[0].ToString();
                set => Parse(ref settings.weaponAmounts[0], value, 0, 999);
            }

            [Category(WeaponsCategory)]
            [DisplayName("Fire extinguisher")]
            public string FireExtinguisher {
                get => settings.weaponAmounts[1].ToString();
                set => Parse(ref settings.weaponAmounts[1], value, 0, 999);
            }

            [Category(WeaponsCategory)]
            [DisplayName("Bubble gun")]
            public string BubbleGun {
                get => settings.weaponAmounts[2].ToString();
                set => Parse(ref settings.weaponAmounts[2], value, 0, 999);
            }

            [Category(WeaponsCategory)]
            [DisplayName("Weed whacker")]
            public string WeedWhacker {
                get => settings.weaponAmounts[3].ToString();
                set => Parse(ref settings.weaponAmounts[3], value, 0, 999);
            }

            [Category(WeaponsCategory)]
            [DisplayName("Ancient Artifact")]
            public string AncientArtifact {
                get => settings.weaponAmounts[4].ToString();
                set => Parse(ref settings.weaponAmounts[4], value, 0, 999);
            }

            [Category(WeaponsCategory)]
            public string Bazooka {
                get => settings.weaponAmounts[5].ToString();
                set => Parse(ref settings.weaponAmounts[5], value, 0, 999);
            }

            [Category(WeaponsCategory)]
            [DisplayName("Pop Cans")]
            public string PopCans {
                get => settings.weaponAmounts[6].ToString();
                set => Parse(ref settings.weaponAmounts[6], value, 0, 999);
            }

            [Category(WeaponsCategory)]
            public string Tomato {
                get => settings.weaponAmounts[7].ToString();
                set => Parse(ref settings.weaponAmounts[7], value, 0, 999);
            }

            [Category(WeaponsCategory)]
            public string Popsicle {
                get => settings.weaponAmounts[8].ToString();
                set => Parse(ref settings.weaponAmounts[8], value, 0, 999);
            }

            [Category(WeaponsCategory)]
            [DisplayName("Banana (unused)")]
            public string Banana {
                get => settings.weaponAmounts[9].ToString();
                set => Parse(ref settings.weaponAmounts[9], value, 0, 999);
            }

            [Category(WeaponsCategory)]
            public string Plate {
                get => settings.weaponAmounts[10].ToString();
                set => Parse(ref settings.weaponAmounts[10], value, 0, 999);
            }

            [Category(WeaponsCategory)]
            public string Silverware {
                get => settings.weaponAmounts[11].ToString();
                set => Parse(ref settings.weaponAmounts[11], value, 0, 999);
            }

            [Category(WeaponsCategory)]
            public string Football {
                get => settings.weaponAmounts[12].ToString();
                set => Parse(ref settings.weaponAmounts[12], value, 0, 999);
            }

            [Category(WeaponsCategory)]
            public string Flamethrower {
                get => settings.weaponAmounts[13].ToString();
                set => Parse(ref settings.weaponAmounts[13], value, 0, 999);
            }

            [Category(SpecialsCategory)]
            [DisplayName("First aid kit")]
            public string FirstAidKit {
                get => settings.specialAmounts[7].ToString();
                set => Parse(ref settings.specialAmounts[7], value, 0, 99);
            }

            [Category(SpecialsCategory)]
            [DisplayName("Pandora's box")]
            public string PandorasBox {
                get => settings.specialAmounts[8].ToString();
                set => Parse(ref settings.specialAmounts[8], value, 0, 99);
            }

            [Category(SpecialsCategory)]
            [DisplayName("Skull key")]
            public string SkullKey {
                get => settings.specialAmounts[9].ToString();
                set => Parse(ref settings.specialAmounts[9], value, 0, 99);
            }

            [Category(SpecialsCategory)]
            public string Clown {
                get => settings.specialAmounts[10].ToString();
                set => Parse(ref settings.specialAmounts[10], value, 0, 99);
            }

            [Category(SpecialsCategory)]
            [DisplayName("Machine (unused)")]
            public string Machine {
                get => settings.specialAmounts[11].ToString();
                set => Parse(ref settings.specialAmounts[11], value, 0, 99);
            }

            [Category(SpecialsCategory)]
            public string Key {
                get => settings.specialAmounts[0].ToString();
                set => Parse(ref settings.specialAmounts[0], value, 0, 99);
            }

            [Category(SpecialsCategory)]
            [DisplayName("Speed shoes")]
            public string SpeedShoes {
                get => settings.specialAmounts[1].ToString();
                set => Parse(ref settings.specialAmounts[1], value, 0, 99);
            }

            [Category(SpecialsCategory)]
            [DisplayName("Monster potion")]
            public string MonsterPotion {
                get => settings.specialAmounts[2].ToString();
                set => Parse(ref settings.specialAmounts[2], value, 0, 99);
            }

            [Category(SpecialsCategory)]
            [DisplayName("Blue potion")]
            public string BluePotion {
                get => settings.specialAmounts[3].ToString();
                set => Parse(ref settings.specialAmounts[3], value, 0, 99);
            }

            [Category(SpecialsCategory)]
            [DisplayName("Mystery potion")]
            public string MysteryPotion {
                get => settings.specialAmounts[4].ToString();
                set => Parse(ref settings.specialAmounts[4], value, 0, 99);
            }

            [Category(SpecialsCategory)]
            [DisplayName("Grey potion (unused)")]
            public string GreyPotion {
                get => settings.specialAmounts[5].ToString();
                set => Parse(ref settings.specialAmounts[5], value, 0, 99);
            }

            [Category(SpecialsCategory)]
            [DisplayName("Yellow potion (unused)")]
            public string YellowPotion {
                get => settings.specialAmounts[6].ToString();
                set => Parse(ref settings.specialAmounts[6], value, 0, 99);
            }
        }
    }
}
