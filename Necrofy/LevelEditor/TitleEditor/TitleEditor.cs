using System;
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

        public TitleEditor(LevelEditor levelEditor, Project project) {
            InitializeComponent();

            this.levelEditor = levelEditor;
            Level level = levelEditor.level.Level;
            Title = "[Title] " + level.displayName;
            displayName.Text = level.displayName;

            characters = new LoadedLevelTitleCharacters(project);
            pageEditor1.LoadPage(this, level.title1, characters);
            pageEditor2.LoadPage(this, level.title2, characters);

            levelTitleContents = new LevelTitleObjectBrowserContents(characters);
            BrowserContents = levelTitleContents;

            palette.Items.AddRange(new object[] { 0, 2, 4, 6 });
            int defaultPalette = level.title1.words.Union(level.title2.words).Select(w => w.palette).FirstOrDefault();
            palette.SelectedItem = defaultPalette;
            applyToAll.Checked = level.title1.words.Union(level.title2.words).All(w => w.palette == defaultPalette);

            pageEditor1.GotFocus += PageEditor1_GotFocus;
            pageEditor2.GotFocus += PageEditor2_GotFocus;
        }

        private void PageEditor1_GotFocus(object sender, EventArgs e) {
            pageEditor2.SelectNone();
        }

        private void PageEditor2_GotFocus(object sender, EventArgs e) {
            pageEditor1.SelectNone();
        }

        public override int? LevelNumber => levelEditor.level.levelAsset.LevelNumber;

        private void TitleEditor_FormClosed(object sender, FormClosedEventArgs e) {
            characters.Dispose();
        }

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
        }

        public void Repaint() {
            pageEditor1.Invalidate();
            pageEditor2.Invalidate();
        }

        private void palette_SelectedIndexChanged(object sender, EventArgs e) {
            if (palette.SelectedIndex > -1) {
                levelTitleContents.SetPalette((int)palette.SelectedItem);
            }
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
