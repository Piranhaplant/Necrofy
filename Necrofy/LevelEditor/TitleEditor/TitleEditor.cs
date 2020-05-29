﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Necrofy
{
    partial class TitleEditor : EditorWindow
    {
        private readonly LevelEditor levelEditor;
        private readonly LoadedLevelTitleCharacters characters;

        private LevelTitleObjectBrowserContents levelTitleContents;
        public UndoManager<TitleEditor> undoManager;

        private int updatingData = 0;

        public string DisplayName {
            get {
                return displayName.Text;
            }
            set {
                updatingData++;
                displayName.Text = value;
                updatingData--;
            }
        }

        private PageEditor activeEditor;

        public TitleEditor(LevelEditor levelEditor, Project project) {
            InitializeComponent();
            activeEditor = pageEditor1;

            this.levelEditor = levelEditor;
            Level level = levelEditor.level.Level;
            Title = "[Title] " + level.displayName;

            updatingData++;
            displayName.Text = level.displayName;
            updatingData--;

            characters = new LoadedLevelTitleCharacters(project);
            pageEditor1.LoadPage(this, level.title1, characters);
            pageEditor2.LoadPage(this, level.title2, characters);

            levelTitleContents = new LevelTitleObjectBrowserContents(characters);
            BrowserContents = levelTitleContents;

            palette.AddPalette(characters, 0);
            palette.AddPalette(characters, 2);
            palette.AddPalette(characters, 4);
            palette.AddPalette(characters, 6);
            byte defaultPalette = level.title1.words.Union(level.title2.words).Select(w => w.palette).FirstOrDefault();
            SetPalette(defaultPalette);
            applyToAll.Checked = level.title1.words.Union(level.title2.words).All(w => w.palette == defaultPalette);

            pageEditor1.GotFocus += PageEditor1_GotFocus;
            pageEditor2.GotFocus += PageEditor2_GotFocus;
            pageEditor1.SelectedWordsChanged += SelectedWordsChanged;
            pageEditor2.SelectedWordsChanged += SelectedWordsChanged;
        }

        private void TitleEditor_FormClosed(object sender, FormClosedEventArgs e) {
            palette.Items.Clear(); // Fixes a bug where the items try to be painted after disposing
            characters.Dispose();
        }

        private void PageEditor1_GotFocus(object sender, EventArgs e) {
            activeEditor = pageEditor1;
            pageEditor2.SelectNone();
            SelectedWordsChanged(activeEditor, e);
        }

        private void PageEditor2_GotFocus(object sender, EventArgs e) {
            activeEditor = pageEditor2;
            pageEditor1.SelectNone();
            SelectedWordsChanged(activeEditor, e);
        }

        private void SelectedWordsChanged(object sender, EventArgs e) {
            if (sender == activeEditor) {
                UpdateSelectedPalette();
                PropertyBrowserObjects = activeEditor.SelectedWords.ToArray();
            }
        }

        public void Repaint() {
            pageEditor1.Invalidate();
            pageEditor2.Invalidate();
        }

        public void RefreshPropertyBrowser() {
            mainWindow.PropertyBrowser.RefreshProperties();
        }

        public void UpdateSelectedPalette() {
            if (activeEditor.SelectedWords.Count > 0) {
                byte firstPalette = activeEditor.SelectedWords.First().Palette;
                if (activeEditor.SelectedWords.All(w => w.Palette == firstPalette)) {
                    SetPalette(firstPalette);
                } else {
                    SetPalette(255);
                }
                palette.Enabled = true;
            } else {
                palette.Enabled = applyToAll.Checked;
            }
        }

        private void SetPalette(byte value) {
            updatingData++;
            palette.SelectedPalette = value;
            updatingData--;
        }

        public override bool CanCopy => false; // TODO
        public override bool CanPaste => true;
        public override bool CanDelete => false; // TODO
        public override bool HasSelection => true;

        public override void Copy() {
            // TODO
        }

        public override void Paste() {
            // TODO
        }

        public override void Delete() {
            // TODO
        }

        public override void SelectAll() {
            activeEditor.SelectAll();
        }

        public override void SelectNone() {
            activeEditor.SelectNone();
        }

        public override int? LevelNumber => levelEditor.level.levelAsset.LevelNumber;

        protected override UndoManager Setup() {
            undoManager = new UndoManager<TitleEditor>(mainWindow.UndoButton, mainWindow.RedoButton, this);
            return undoManager;
        }

        protected override void DoSave(Project project) {
            Level level = levelEditor.level.Level;
            level.displayName = displayName.Text;
            level.title1 = pageEditor1.page;
            level.title2 = pageEditor2.page;
            levelEditor.undoManager.ForceDirty();
            // TODO: propogate new display name to UI
        }

        public override void PropertyBrowserPropertyChanged(PropertyValueChangedEventArgs e) {
            string value = e.ChangedItem.Value as string;
            foreach (WrappedTitleWord w in activeEditor.SelectedWords) {
                w.ClearBrowsableProperties();
            }
            if (value != null) {
                value = value.Trim();
                switch (e.ChangedItem.Label) {
                    case WrappedTitleWord.XProperty:
                        ParsePositionProperty(value, o => o.X,
                            dx => new MoveWordAction(activeEditor.SelectedWords, dx, 0),
                            x => new MoveWordAction(activeEditor.SelectedWords, x, null));
                        break;
                    case WrappedTitleWord.YProperty:
                        ParsePositionProperty(value, o => o.Y,
                            dy => new MoveWordAction(activeEditor.SelectedWords, 0, dy),
                            y => new MoveWordAction(activeEditor.SelectedWords, null, y));
                        break;
                    case WrappedTitleWord.PaletteProperty:
                        if (byte.TryParse(value, out byte palette)) {
                            undoManager.Do(new ChangeWordPaletteAction(activeEditor.SelectedWords, palette));
                        }
                        break;
                }
            }
        }

        private void ParsePositionProperty(string value, Func<WrappedTitleWord, byte> getter, Func<int, MoveWordAction> deltaMoveGetter, Func<byte, MoveWordAction> absoluteMoveGetter) {
            if (int.TryParse(value, out int intValue)) {
                if (value.StartsWith("+") || value.StartsWith("-")) {
                    int min = activeEditor.SelectedWords.Min(getter);
                    int max = activeEditor.SelectedWords.Max(getter);
                    undoManager.Do(deltaMoveGetter(Math.Max(-min, Math.Min(byte.MaxValue - max, intValue))));
                } else if (intValue >= byte.MinValue && intValue <= byte.MaxValue) {
                    undoManager.Do(absoluteMoveGetter((byte)intValue));
                }
            }
        }

        private string prevDisplayName = "";
        private void displayName_TextChanged(object sender, EventArgs e) {
            if (updatingData == 0) {
                undoManager.Do(new ChangeDisplayNameAction(prevDisplayName, displayName.Text));
            }
            prevDisplayName = displayName.Text;
        }

        private void palette_SelectedIndexChanged(object sender, EventArgs e) {
            if (palette.SelectedIndex > -1) {
                levelTitleContents.SetPalette(palette.SelectedPalette);
                if (updatingData == 0) {
                    if (applyToAll.Checked) {
                        undoManager.Do(new ChangeWordPaletteAction(pageEditor1.AllWords.Union(pageEditor2.AllWords), palette.SelectedPalette));
                    } else {
                        undoManager.Do(new ChangeWordPaletteAction(activeEditor.SelectedWords, palette.SelectedPalette));
                    }
                }
            }
        }

        private void applyToAll_CheckedChanged(object sender, EventArgs e) {
            UpdateSelectedPalette();
        }

        private void mainPanel_SizeChanged(object sender, EventArgs e) {
            int x = screenBounds.Location.X;
            int y = screenBounds.Location.Y;
            if (screenBounds.Width < mainPanel.Width) {
                x = (mainPanel.Width - screenBounds.Width) / 2;
            }
            if (screenBounds.Height < mainPanel.Height) {
                y = (mainPanel.Height - screenBounds.Height) / 2;
            }
            screenBounds.Location = new Point(x, y);
        }
    }
}