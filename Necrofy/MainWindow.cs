﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Newtonsoft.Json;
using WeifenLuo.WinFormsUI.Docking;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;

namespace Necrofy
{
    partial class MainWindow : Form
    {
        public static readonly string DefaultStatusText = Application.ProductName + " " + Application.ProductVersion;
        private static readonly float[] zoomLevels = new float[] { 0.25f, 0.33f, 0.5f, 0.75f, 1.0f, 2.0f, 4.0f, 8.0f, 16.0f };

        private StartupWindow startupWindow;
        private PreferencesDialog preferencesDialog;
        private RunSettingsDialog runSettingsDialog;
        private AssetExtractorDialog assetExtractorDialog;

        private Project project;

        private Dictionary<string, RunSettings> savedRunSettings;
        private string currentRunSettings;

        public ObjectBrowserForm ObjectBrowser { get; private set; }
        public ProjectBrowser ProjectBrowser { get; private set; }
        public PropertyBrowser PropertyBrowser { get; private set; }
        public BuildResultsWindow BuildResultsWindow { get; private set; }

        private readonly List<ToolStripMenuItem> projectMenuItems;

        private readonly HashSet<EditorWindow> openEditors = new HashSet<EditorWindow>();
        private readonly HashSet<EditorWindow> dirtyEditors = new HashSet<EditorWindow>();
        public EditorWindow activeEditor { get; private set; } = null;

        private bool building = false;
        
        public MainWindow() {
            InitializeComponent();

#if !DEBUG
            AppDomain.CurrentDomain.UnhandledException += (sender, args) => {
                MessageBox.Show("Necrofy has encountered an error. Please provide the following information when reporting this problem:" + Environment.NewLine + args.ExceptionObject);
            };
#endif

            if (Properties.Settings.Default.UpgradeRequired) {
                Properties.Settings.Default.Upgrade();
                Properties.Settings.Default.UpgradeRequired = false;
                Properties.Settings.Default.Save();
            }

            zoomLevelLabel.AutoSize = false;
            zoomLevelLabel.Text = "100 %";
            widthLabel.AutoSize = false;

            ObjectBrowser = new ObjectBrowserForm();
            ProjectBrowser = new ProjectBrowser(this);
            PropertyBrowser = new PropertyBrowser();
            PropertyBrowser.PropertyChanged += PropertyBrowser_PropertyChanged;
            BuildResultsWindow = new BuildResultsWindow();

            string dockLayout = Properties.Settings.Default.DockLayout;
            if (!string.IsNullOrEmpty(dockLayout)) {
                Dictionary<string, DockContent> allPanels = new DockContent[] { ObjectBrowser, ProjectBrowser, PropertyBrowser, BuildResultsWindow }.ToDictionary(o => o.GetType().FullName);
                using (MemoryStream s = new MemoryStream(Encoding.UTF8.GetBytes(dockLayout))) {
                    dockPanel.LoadFromXml(s, persistString => {
                        if (allPanels.TryGetValue(persistString, out DockContent panel)) {
                            return panel;
                        }
                        return null;
                    });
                }
            } else {
                UseDefaultDockLayout();
            }
            menuStrip1.SendToBack(); // Deserializing the dock panel layout sometimes messes this order up

            recentProjects.RemoveFromEnd = Path.DirectorySeparatorChar + Project.defaultProjectFilename;
            string recentProjectsString = Properties.Settings.Default.RecentProjects;
            if (recentProjectsString != "") {
                recentProjects.Files = recentProjectsString.Split(Path.PathSeparator);
            }

            LoadRunSettings();
            viewAnimate.Checked = Properties.Settings.Default.AnimateLevelEditor;
            viewRespawnAreas.Checked = Properties.Settings.Default.ShowRespawnAreas;
            viewAxes.Checked = Properties.Settings.Default.ShowAxes;
            viewTileBorders.Checked = Properties.Settings.Default.ShowTileBorders;
            viewSpriteGrid.Checked = Properties.Settings.Default.ShowSpriteGrid;
            viewTilemapGrid.Checked = Properties.Settings.Default.ShowTilemapGrid;

            projectMenuItems = new List<ToolStripMenuItem>() { projectBuildProject, projectRunProject, projectSettings, projectExtractAssets };
            toolStripGrouper.HideAllSetItems();
            UpdateStatusText();
            UpdateInfo1();
            UpdateInfo2();

            if (Environment.GetCommandLineArgs().Length == 2) {
                OpenProject(Environment.GetCommandLineArgs()[1]);
            } else if (Properties.Settings.Default.ShowStartupWindow) {
                startupWindow = new StartupWindow(DefaultStatusText);
                startupWindow.Show(dockPanel, DockState.Document);
            }
        }

        private void MainWindow_Load(object sender, EventArgs e) {
            // Setting window state to maximized in the constructor won't have any effect
            if (Properties.Settings.Default.WindowSize != Size.Empty) {
                Size = Properties.Settings.Default.WindowSize;
                if (Properties.Settings.Default.Maximized) {
                    WindowState = FormWindowState.Maximized;
                }
            }
        }

        private void MainWindow_FormClosed(object sender, FormClosedEventArgs e) {
            using (MemoryStream s = new MemoryStream()) {
                dockPanel.SaveAsXml(s, Encoding.UTF8);
                string xml = Encoding.UTF8.GetString(s.ToArray());
                Properties.Settings.Default.DockLayout = xml;
            }
            Properties.Settings.Default.AnimateLevelEditor = viewAnimate.Checked;
            Properties.Settings.Default.ShowRespawnAreas = viewRespawnAreas.Checked;
            Properties.Settings.Default.ShowTileBorders = viewTileBorders.Checked;
            Properties.Settings.Default.ShowAxes = viewAxes.Checked;
            Properties.Settings.Default.ShowSpriteGrid = viewSpriteGrid.Checked;
            Properties.Settings.Default.ShowTilemapGrid = viewTilemapGrid.Checked;
            if (WindowState.HasFlag(FormWindowState.Maximized)) {
                Properties.Settings.Default.WindowSize = RestoreBounds.Size;
                Properties.Settings.Default.Maximized = true;
            } else {
                Properties.Settings.Default.WindowSize = Size;
                Properties.Settings.Default.Maximized = false;
            }
            Properties.Settings.Default.Save();

            CloseProject(closeEditors: false);
        }

        private void UseDefaultDockLayout() {
            BuildResultsWindow.Show(dockPanel, DockState.DockBottomAutoHide);
            BuildResultsWindow.Hide();
            ObjectBrowser.Show(dockPanel, DockState.DockLeft);
            ProjectBrowser.Show(dockPanel, DockState.DockRight);
            PropertyBrowser.Show(ProjectBrowser.Pane, DockAlignment.Bottom, 0.2);
            dockPanel.DockBottomPortion = 200;
            dockPanel.DockLeftPortion = 220;
            dockPanel.DockRightPortion = 200;
            dockPanel.DockTopPortion = 200;
        }
        
        private void LoadRunSettings() {
            if (string.IsNullOrEmpty(Properties.Settings.Default.SavedRunSettings)) {
                SetDefaultRunSettings();
            } else {
                try {
                    savedRunSettings = JsonConvert.DeserializeObject<Dictionary<string, RunSettings>>(Properties.Settings.Default.SavedRunSettings);
                    List<string> invalidSettings = new List<string>();
                    foreach (string settingsName in savedRunSettings.Keys) {
                        RunSettings settings = savedRunSettings[settingsName];
                        if (settings.weaponAmounts.Length != RunSettings.WeaponCount || settings.specialAmounts.Length != RunSettings.SpecialCount) {
                            invalidSettings.Add(settingsName);
                        }
                    }
                    foreach (string settingsName in invalidSettings) {
                        savedRunSettings.Remove(settingsName);
                    }
                    if (savedRunSettings.Count == 0) {
                        SetDefaultRunSettings();
                    } else {
                        currentRunSettings = Properties.Settings.Default.CurrentRunSettingsName;
                        if (!savedRunSettings.ContainsKey(currentRunSettings)) {
                            currentRunSettings = savedRunSettings.Keys.First();
                            SaveRunSettings();
                        }
                    }
                } catch {
                    SetDefaultRunSettings();
                }
            }
        }

        private void SetDefaultRunSettings() {
            savedRunSettings = new Dictionary<string, RunSettings>();

            RunSettings starting = new RunSettings();
            starting.weaponAmounts[0] = 150;
            starting.specialAmounts[7] = 1;
            savedRunSettings["Starting"] = starting;

            RunSettings max = new RunSettings();
            for (int i = 0; i < max.weaponAmounts.Length; i++) {
                max.weaponAmounts[i] = 999;
            }
            max.weaponAmounts[9] = 0;
            for (int i = 0; i < max.specialAmounts.Length; i++) {
                max.specialAmounts[i] = 99;
            }
            max.specialAmounts[5] = 0;
            max.specialAmounts[6] = 0;
            max.specialAmounts[11] = 0;
            savedRunSettings["Max"] = max;

            currentRunSettings = "Max";
            SaveRunSettings();
        }

        private void SaveRunSettings() {
            Properties.Settings.Default.SavedRunSettings = JsonConvert.SerializeObject(savedRunSettings);
            Properties.Settings.Default.CurrentRunSettingsName = currentRunSettings;
            Properties.Settings.Default.Save();
        }

        public ToolStripSplitButton UndoButton => undoButton;
        public ToolStripSplitButton RedoButton => redoButton;

        public void OpenAsset(Asset.NameInfo assetInfo) {
            EditorWindow existingEditor = openEditors.Where(e => assetInfo.Equals(e.AssetInfo)).FirstOrDefault();
            if (existingEditor != null) {
                existingEditor.Activate();
                return;
            }
            EditorWindow editor = assetInfo.GetEditor(project);
            if (editor != null) {
                //editor.Icon = ProjectBrowser.GetEditorIcon(assetInfo.Category); //TODO
                ShowEditor(editor, assetInfo);
            }
        }

        public void ShowEditor(EditorWindow editor, Asset.NameInfo assetInfo, DockAlignment? alignment = null) {
            openEditors.Add(editor);
            editor.Setup(this, project, assetInfo);
            if (alignment != null && activeEditor != null) {
                editor.Show(activeEditor.Pane, (DockAlignment)alignment, 0.5);
            } else {
                editor.Show(dockPanel, DockState.Document);
            }
            editor.DirtyChanged += Editor_DirtyChanged;
            editor.SelectionChanged += Editor_SelectionChanged;
            editor.TextChanged += Editor_TextChanged;
            editor.StatusChanged += Editor_StatusChanged;
            editor.Info1Changed += Editor_Info1Changed;
            editor.Info2Changed += Editor_Info2Changed;
        }

        public bool EditorVisible(EditorWindow editor) {
            foreach (DockPane pane in dockPanel.DockWindows[DockState.Document].VisibleNestedPanes) {
                if (pane.ActiveContent == editor) {
                    return true;
                }
            }
            return false;
        }

        private void Editor_DirtyChanged(object sender, EventArgs e) {
            EditorWindow editor = sender as EditorWindow;
            if (editor != null) {
                if (editor.Dirty) {
                    dirtyEditors.Add(editor);
                } else {
                    dirtyEditors.Remove(editor);
                }
            }
            if (editor == activeEditor) {
                saveButton.Enabled = editor?.Dirty ?? false;
            }
            saveAllButton.Enabled = dirtyEditors.Count > 0;
        }

        private void Editor_SelectionChanged(object sender, EventArgs e) {
            if (sender == activeEditor) {
                editCopy.Enabled = activeEditor?.CanCopy ?? false;
                editPaste.Enabled = activeEditor?.CanPaste ?? false;
                editDelete.Enabled = activeEditor?.CanDelete ?? false;
                editCut.Enabled = editCopy.Enabled && editDelete.Enabled;

                editSelectAll.Enabled = activeEditor?.HasSelection ?? false;
                editSelectNone.Enabled = activeEditor?.HasSelection ?? false;
                editMoveSelection.Enabled = activeEditor?.CanCopy ?? false;
            }
        }

        private void Editor_TextChanged(object sender, EventArgs e) {
            if (sender == activeEditor) {
                UpdateWindowText();
            }
        }

        private void Editor_StatusChanged(object sender, EventArgs e) {
            if (sender == activeEditor) {
                UpdateStatusText();
            }
        }

        private void Editor_Info1Changed(object sender, EventArgs e) {
            if (sender == activeEditor) {
                UpdateInfo1();
            }
        }

        private void Editor_Info2Changed(object sender, EventArgs e) {
            if (sender == activeEditor) {
                UpdateInfo2();
            }
        }

        private void dockPanel_ContentRemoved(object sender, DockContentEventArgs e) {
            if (e.Content is EditorWindow editor) {
                openEditors.Remove(editor);
                dirtyEditors.Remove(editor);
                Editor_DirtyChanged(null, e); // Update the "save all" button state
                if (openEditors.Count == 0) {
                    undoButton.Enabled = false;
                    redoButton.Enabled = false;
                }
            }
        }
        
        private void dockPanel_ActiveDocumentChanged(object sender, EventArgs e) {
            EditorWindow editor = dockPanel.ActiveDocument as EditorWindow;

            if (activeEditor != null) {
                toolStripGrouper.HideAllSetItems();
                activeEditor.Hidden();
            }

            activeEditor = editor;
            if (editor != null) {
                toolStripGrouper.ShowItemSet(editor.ToolStripItemSet);
                editor.Displayed();
            }
            UpdateWindowText();
            Editor_DirtyChanged(editor, e);
            Editor_SelectionChanged(editor, e);
            ObjectBrowser.Browser.Contents = activeEditor?.BrowserContents;
            PropertyBrowser.SetObjects(activeEditor?.PropertyBrowserObjects);
            projectRunFromLevel.Enabled = activeEditor?.LevelNumber != null;
            projectRecordDemo.Enabled = activeEditor?.LevelNumber != null;
            fileClose.Enabled = activeEditor != null;
            UpdateStatusText();
            UpdateInfo1();
            UpdateInfo2();
            UpdateZoom();
        }

        private void UpdateWindowText() {
            Text = Application.ProductName;
            if (activeEditor != null && activeEditor.Title.Length > 0) {
                Text += " - " + activeEditor.Title;
            }
        }

        private void UpdateStatusText() {
            statusLabel.Text = activeEditor?.Status ?? DefaultStatusText;
        }

        private void UpdateInfo1() {
            infoLabel1.Text = activeEditor?.Info1 ?? "";
            infoLabel1.Visible = !string.IsNullOrEmpty(activeEditor?.Info1);
        }

        private void UpdateInfo2() {
            infoLabel2.Text = activeEditor?.Info2 ?? "";
            infoLabel2.Visible = !string.IsNullOrEmpty(activeEditor?.Info2);
        }

        private void UpdateZoom() {
            bool enabled = activeEditor?.CanZoom ?? false;
            if (enabled) {
                int zoomIndex = Array.IndexOf(zoomLevels, activeEditor.Zoom);
                viewZoomOut.Enabled = zoomIndex > 0;
                viewZoomIn.Enabled = zoomIndex < zoomLevels.Length - 1;
                zoomLevelLabel.Enabled = true;
                zoomLevelLabel.Text = $"{activeEditor.Zoom:P0}";
            } else {
                viewZoomOut.Enabled = false;
                viewZoomIn.Enabled = false;
                zoomLevelLabel.Enabled = false;
            }
        }

        private void PropertyBrowser_PropertyChanged(object sender, PropertyValueChangedEventArgs e) {
            activeEditor?.PropertyBrowserPropertyChanged(e);
        }

        private bool CloseProject(bool closeEditors, bool promptForSave = true) {
            if (project != null) {
                try {
                    ProjectBrowser.SaveFolderStates();
                    project.userSettings.OpenFiles = openEditors.Select(e => e.AssetInfo?.GetFilename("")?.Replace(Path.DirectorySeparatorChar, '/')).Where(e => e != null).ToList();
                    project.WriteSettings();
                } catch (Exception) { }

                if (closeEditors) {
                    foreach (EditorWindow editor in new List<EditorWindow>(openEditors)) {
                        if (promptForSave) {
                            editor.Close();
                        } else {
                            editor.CloseWithoutSaving();
                        }
                        if (editor.Visible) {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private void CreateProject(object sender, EventArgs e) {
            bool cancel = false;
            foreach (EditorWindow editor in openEditors) {
                editor.PromptForSave(ref cancel);
            }
            if (cancel) {
                return;
            }

            NewProjectDialog newProjectDialog = new NewProjectDialog();
            if (newProjectDialog.ShowDialog() == DialogResult.OK) {
                if (CloseProject(closeEditors: true, promptForSave: false)) {
                    return;
                }
#if !DEBUG
                try {
#endif
                project = new Project(newProjectDialog.BaseROM, newProjectDialog.ProjectLocation, this);
#if !DEBUG
                } catch (Exception ex) {
                    Console.WriteLine(ex.StackTrace);
                    MessageBox.Show($"Error creating project: {Environment.NewLine}{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
#endif
                ProjectReady();
            }
        }

        private void OpenProject(object sender, EventArgs e) {
            if (openProjectDialog.ShowDialog() == DialogResult.OK) {
                OpenProject(openProjectDialog.FileName);
            }
        }

        private void recentProjects_FileClicked(string file) {
            OpenProject(file);
        }

        private void OpenProject(string settingsFilename) {
            if (CloseProject(closeEditors: true)) {
                return;
            }
#if !DEBUG
            try {
#endif
            project = new Project(settingsFilename, this);
#if !DEBUG
            } catch (Exception ex) {
                MessageBox.Show($"Error opening project: {Environment.NewLine}{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
#endif
            ProjectReady();
        }

        private void ProjectReady() {
            if (startupWindow != null) {
                startupWindow.Close();
                startupWindow = null;
            }

            recentProjects.Add(project.SettingsPath);
            Properties.Settings.Default.RecentProjects = string.Join(Path.PathSeparator.ToString(), recentProjects.Files);
            Properties.Settings.Default.Save();

            ProjectBrowser.OpenProject(project);
            project.Assets.RenameCompleted += RenameCompleted;

            if (project.userSettings.OpenFiles != null) {
                foreach (string filename in project.userSettings.OpenFiles) {
                    Asset.NameInfo info = Asset.GetInfo(project, filename.Replace('/', Path.DirectorySeparatorChar));
                    if (info != null) {
                        try {
                            OpenAsset(info);
                        } catch (Exception e) {
                            Console.WriteLine(e.Message);
                            Console.WriteLine(e.StackTrace);
                            openEditors.RemoveWhere(ed => ed.AssetInfo == info);
                        }
                    }
                }
            }

            foreach (ToolStripMenuItem item in projectMenuItems) {
                item.Enabled = true;
            }
        }

        private void RenameCompleted(object sender, RenameCompletedEventArgs e) {
            if (e.Results.referenceUpdateErrors.Count > 0) {
                MessageBox.Show($"Error updating the following assets: {Environment.NewLine}{string.Join(Environment.NewLine, e.Results.referenceUpdateErrors)}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            foreach (EditorWindow editor in openEditors) {
                editor.RenameAssetReferences(e.Results);
            }
        }

        private void Save(object sender, EventArgs e) {
            activeEditor?.Save();
        }

        private void SaveAll(object sender, EventArgs e) {
            // Calling save on the editors will modify this set, so operate on a copy
            foreach (EditorWindow editor in new HashSet<EditorWindow>(dirtyEditors)) {
                editor.Save();
            }
        }

        private void CloseFile(object sender, EventArgs e) {
            activeEditor?.Close();
        }

        private void Exit(object sender, EventArgs e) {
            Close();
        }

        private void Undo(object sender, EventArgs e) {
            activeEditor?.Undo();
        }

        private void Redo(object sender, EventArgs e) {
            activeEditor?.Redo();
        }

        private void Cut(object sender, EventArgs e) {
            activeEditor?.Copy();
            activeEditor?.Delete();
        }

        private void Copy(object sender, EventArgs e) {
            activeEditor?.Copy();
        }

        private void Paste(object sender, EventArgs e) {
            activeEditor?.Paste();
        }
        
        private void Delete(object sender, EventArgs e) {
            if (dockPanel.ActiveContent == activeEditor) {
                activeEditor?.Delete();
            }
        }

        private void SelectAll(object sender, EventArgs e) {
            activeEditor?.SelectAll();
        }

        private void SelectNone(object sender, EventArgs e) {
            activeEditor?.SelectNone();
        }

        private void preferencesToolStripMenuItem_Click(object sender, EventArgs e) {
            if (preferencesDialog != null) {
                preferencesDialog.Activate();
            } else {
                preferencesDialog = new PreferencesDialog();
                preferencesDialog.FormClosed += (sender2, e2) => {
                    preferencesDialog = null;
                };
                preferencesDialog.Show();
            }
        }

        private void BuildProject(object sender, EventArgs e) {
            Task t = DoBuild(() => project?.Build()); // Don't await since this should run in the background
        }

        private void RunProject(object sender, EventArgs e) {
            Task t = DoBuild(() => project?.Run()); // Don't await since this should run in the background
        }

        private void RunFromLevel(object sender, EventArgs e) {
            if (activeEditor?.LevelNumber != null) {
                Task t = DoBuild(() => project?.RunFromLevel((int)activeEditor?.LevelNumber, savedRunSettings[currentRunSettings])); // Don't await since this should run in the background
            }
        }

        private async Task DoBuild(Func<BuildResults> buildMethod) {
            if (building) {
                return;
            }
            SaveAll(this, EventArgs.Empty);
            building = true;

            buildStatusLabel.Text = "Building...";
            buildStatusLabel.Visible = true;

            CancellationTokenSource buildAnimationCancel = new CancellationTokenSource();
            ShowBuildAnimation(buildAnimationCancel.Token);

            BuildResults results = await Task.Run(delegate {
                try {
                    return buildMethod();
                } catch (Exception ex) {
                    results = new BuildResults();
                    results.AddEntry(new BuildResults.Entry(BuildResults.Entry.Level.ERROR, "", "Could not run ROM. You may need to set an emulator from the Preferences on the Edit menu.", ex.Message));
                    return results;
                }
            });

            buildAnimationCancel.Cancel();
            ShowBuildResults(results);

            if (results.Success) {
                buildStatusLabel.Text = "Build succeeded";
                buildStatusLabel.Image = Properties.Resources.tick_button;
            } else {
                buildStatusLabel.Text = "Build failed";
                buildStatusLabel.Image = Properties.Resources.cross_circle;
            }
            building = false;
        }

        private static readonly Image[] buildAnimationFrames = { Properties.Resources.arrow_circle, Properties.Resources.arrow_circle_2,
            Properties.Resources.arrow_circle_3, Properties.Resources.arrow_circle_4 };

        private async void ShowBuildAnimation(CancellationToken cancelToken) {
            int frame = 0;
            while (!cancelToken.IsCancellationRequested) {
                buildStatusLabel.Image = buildAnimationFrames[frame];
                frame = (frame + 1) % buildAnimationFrames.Length;
                await Task.Delay(500);
            }
        }

        private void ShowBuildResults(BuildResults results) {
            if (results == null) {
                return;
            }
            if (results.HasWarnOrAbove) {
                ShowDockContent(BuildResultsWindow);
                BuildResultsWindow.UnAutoHide();
            }
            BuildResultsWindow.ShowResults(results);
        }

        private void RunFromLevelSettings(object sender, EventArgs e) {
            if (runSettingsDialog != null) {
                runSettingsDialog.Activate();
            } else {
                runSettingsDialog = new RunSettingsDialog(savedRunSettings, currentRunSettings);
                runSettingsDialog.FormClosed += (sender2, e2) => {
                    currentRunSettings = runSettingsDialog.currentSettings;
                    SaveRunSettings();
                    runSettingsDialog = null;
                };
                runSettingsDialog.Show();
            }
        }

        private async void RecordDemo(object sender, EventArgs e) {
            if (activeEditor?.LevelNumber != null) {
                BuildAndRunResults results = null;
                await DoBuild(delegate {
                    results = project?.RecordDemo((int)activeEditor?.LevelNumber);
                    return results.BuildResults;
                });
                if (results != null && results.EmulatorProcess != null) {
                    new RecordDemoDialog(project, results.EmulatorProcess, (int)activeEditor?.LevelNumber).ShowDialog();
                }
            }
        }

        private void projectSettings_Click(object sender, EventArgs e) {
            if (project == null) {
                return;
            }
            if (new ProjectSettingsDialog(project.settings).ShowDialog() == DialogResult.OK) {
                project.WriteSettings();
            }
        }

        private void projectExtractAssets_Click(object sender, EventArgs e) {
            if (assetExtractorDialog != null) {
                assetExtractorDialog.Activate();
            } else {
                assetExtractorDialog = new AssetExtractorDialog(project);
                assetExtractorDialog.FormClosed += (sender2, e2) => {
                    assetExtractorDialog = null;
                };
                assetExtractorDialog.Show();
            }
        }

        private void ZoomOut(object sender, EventArgs e) {
            doZoom(-1);
        }

        private void ZoomIn(object sender, EventArgs e) {
            doZoom(1);
        }

        private void doZoom(int direction) {
            if (activeEditor != null) {
                int zoomIndex = Array.IndexOf(zoomLevels, activeEditor.Zoom) + direction;
                if (zoomIndex >= 0 && zoomIndex < zoomLevels.Length) {
                    activeEditor.Zoom = zoomLevels[zoomIndex];
                    UpdateZoom();
                }
            }
        }

        private void windowProject_Click(object sender, EventArgs e) {
            ShowDockContent(ProjectBrowser);
        }

        private void windowObjects_Click(object sender, EventArgs e) {
            ShowDockContent(ObjectBrowser);
        }

        private void windowProperties_Click(object sender, EventArgs e) {
            ShowDockContent(PropertyBrowser);
        }

        private void ShowDockContent(DockContent dockContent) {
            if (dockContent.DockState == DockState.Hidden) {
                dockContent.Show();
            }
            dockContent.Activate();
        }

        private void windowRestore_Click(object sender, EventArgs e) {
            if (MessageBox.Show("Are you sure you want to revert to the default window layout?", "Restore default layout", MessageBoxButtons.YesNo) == DialogResult.Yes) {
                UseDefaultDockLayout();
            }
        }

        private void helpWiki_Click(object sender, EventArgs e) {
            Process.Start("https://wiki.zamnhacking.net/");
        }

        private void helpAbout_Click(object sender, EventArgs e) {
            new AboutDialog().ShowDialog();
        }

        private void toolStripGrouper_ItemClick(object sender, ToolStripGrouper.ItemEventArgs e) {
            activeEditor?.ToolStripItemClicked(e.Type);
        }

        private void toolStripGrouper_ItemCheckedChanged(object sender, ToolStripGrouper.ItemEventArgs e) {
            foreach (EditorWindow editor in openEditors) {
                editor.ToolStripItemCheckedChanged(e.Type);
            }
        }

        public ToolStripGrouper.ItemProxy GetToolStripItem(ToolStripGrouper.ItemType type) {
            return toolStripGrouper.GetItem(type);
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e) {
            if (dockPanel.ActiveContent is EditorWindow || e.Modifiers == Keys.Control) {
                if (e.KeyCode == Keys.Subtract || e.KeyCode == Keys.OemMinus) {
                    ZoomOut(sender, e);
                    e.Handled = true;
                } else if (e.KeyCode == Keys.Add || e.KeyCode == Keys.Oemplus) {
                    ZoomIn(sender, e);
                    e.Handled = true;
                }
            }
        }
    }
}
