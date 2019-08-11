using System;
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

namespace Necrofy
{
    partial class MainWindow : Form
    {
        private const char pathSeparator = ';';
        private Project project;

        public ObjectBrowserForm ObjectBrowser { get; private set; }
        public ProjectBrowser ProjectBrowser { get; private set; }

        private readonly List<ToolStripMenuItem> projectMenuItems;

        private List<EditorWindow> openEditors = new List<EditorWindow>();
        private EditorWindow activeEditor = null;
        private List<ToolStripItem> editorToolStripItems = new List<ToolStripItem>();
        private List<ToolStripItem> editorMenuStripItems = new List<ToolStripItem>();

        public MainWindow()
        {
            InitializeComponent();

            ObjectBrowser = new ObjectBrowserForm();
            ObjectBrowser.Show(dockPanel, DockState.DockLeft);

            ProjectBrowser = new ProjectBrowser(this);
            ProjectBrowser.Show(dockPanel, DockState.DockLeft);

            string recentProjectsString = Properties.Settings.Default.RecentProjects;
            if (recentProjectsString != "") {
                recentProjects.Files = recentProjectsString.Split(pathSeparator);
            }

            ToolBarMenuLinker.Link(saveButton, fileSave);
            ToolBarMenuLinker.Link(saveAllButton, fileSaveAll);
            ToolBarMenuLinker.Link(cutButton, editCut);
            ToolBarMenuLinker.Link(copyButton, editCopy);
            ToolBarMenuLinker.Link(pasteButton, editPaste);
            ToolBarMenuLinker.Link(undoButton, editUndo);
            ToolBarMenuLinker.Link(redoButton, editRedo);
            ToolBarMenuLinker.Link(buildProjectButton, buildBuildProject);
            ToolBarMenuLinker.Link(runProjectButton, buildRunProject);
            ToolBarMenuLinker.Link(runFromLevelButton, buildRunFromLevel);
            projectMenuItems = new List<ToolStripMenuItem>() { buildBuildProject, buildRunProject };
        }

        public ToolStripSplitButton UndoButton => undoButton;
        public ToolStripSplitButton RedoButton => redoButton;

        public void ShowEditor(EditorWindow editor) {
            openEditors.Add(editor);
            editor.Setup(this);
            editor.Show(dockPanel, DockState.Document);
        }

        private void dockPanel_ContentRemoved(object sender, DockContentEventArgs e) {
            if (e.Content is EditorWindow editor) {
                openEditors.Remove(editor);
                if (openEditors.Count == 0) {
                    ProjectBrowser.Activate();
                }
            }
        }

        private void dockPanel_ActiveDocumentChanged(object sender, EventArgs e) {
            EditorWindow editor = dockPanel.ActiveContent as EditorWindow;

            if (activeEditor != null) {
                if (activeEditor.EditorMenuStrip != null) {
                    foreach (ToolStripItem item in editorMenuStripItems) {
                        // Item is automatically removed from the existing tools strip when it is added to a different one
                        activeEditor.EditorMenuStrip.Items.Add(item);
                    }
                }
                if (activeEditor.EditorToolStrip != null) {
                    foreach (ToolStripItem item in editorToolStripItems) {
                        activeEditor.EditorToolStrip.Items.Add(item);
                    }
                }
            }
            editorMenuStripItems.Clear();
            editorToolStripItems.Clear();

            if (editor != null) {
                if (editor.EditorMenuStrip != null) {
                    while (editor.EditorMenuStrip.Items.Count > 0) {
                        ToolStripItem item = editor.EditorMenuStrip.Items[0];
                        menuStrip1.Items.Add(item);
                        editorMenuStripItems.Add(item);
                    }
                }
                if (editor.EditorToolStrip != null) {
                    while (editor.EditorToolStrip.Items.Count > 0) {
                        ToolStripItem item = editor.EditorToolStrip.Items[0];
                        toolStrip1.Items.Add(item);
                        editorToolStripItems.Add(item);
                    }
                }

                editor.Displayed();
            }
            activeEditor = editor;
            endToolStripSeparator.Visible = editorToolStripItems.Count > 0;
        }

        private void CreateProject(object sender, EventArgs e) {
            NewProjectDialog newProjectDialog = new NewProjectDialog();
            if (newProjectDialog.ShowDialog() == DialogResult.OK) {
                // TODO: close already open project if there is one
                project = new Project(newProjectDialog.BaseROM, newProjectDialog.ProjectLocation);
                ProjectReady();
            }
        }

        private void OpenProject(object sender, EventArgs e) {
            if (openProjectDialog.ShowDialog() == DialogResult.OK) {
                // TODO: close already open project if there is one
                project = new Project(openProjectDialog.FileName);
                ProjectReady();
            }
        }

        private void recentProjects_FileClicked(string file) {
            project = new Project(file);
            ProjectReady();
        }

        private void ProjectReady() {
            recentProjects.Add(project.SettingsPath);
            Properties.Settings.Default.RecentProjects = string.Join(pathSeparator.ToString(), recentProjects.Files);
            Properties.Settings.Default.Save();

            ProjectBrowser.OpenProject(project);
            ProjectBrowser.Activate();

            foreach (ToolStripMenuItem item in projectMenuItems) {
                item.Enabled = true;
            }
        }

        private void BuildProject(object sender, EventArgs e) {
            if (project == null)
                return;
            project.Build();
            // TODO tell the user that it finished
        }

        private void buildRunSettings_Click(object sender, EventArgs e) {
            if (project == null)
                return;
            new SpriteViewer().Show(project);
        }

        private void undo_Click(object sender, EventArgs e) {
            activeEditor?.Undo();
        }

        private void redo_Click(object sender, EventArgs e) {
            activeEditor?.Redo();
        }
    }
}
