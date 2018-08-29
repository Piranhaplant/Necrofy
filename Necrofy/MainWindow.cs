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
    public partial class MainWindow : Form
    {
        private const char pathSeparator = ';';
        private Project project;

        public MainWindow()
        {
            InitializeComponent();

            string recentProjectsString = Properties.Settings.Default.RecentProjects;
            if (recentProjectsString != "") {
                recentProjects.Files = recentProjectsString.Split(pathSeparator);
            }
        }

        private void createProject(object sender, EventArgs e) {
            NewProject newProject = new NewProject();
            if (newProject.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                project = new Project(newProject.BaseROM, newProject.ProjectLocation);
                projectReady();
            }
        }

        private void openProject(object sender, EventArgs e) {
            if (openProjectDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                project = new Project(openProjectDialog.FileName);
                projectReady();
            }
        }

        private void recentProjects_FileClicked(string file) {
            project = new Project(file);
            projectReady();
        }

        private void projectReady() {
            recentProjects.Add(project.settingsPath);
            Properties.Settings.Default.RecentProjects = string.Join(pathSeparator.ToString(), recentProjects.Files);
            Properties.Settings.Default.Save();

            ProjectBrowser browser = new ProjectBrowser(dockPanel, project);
            browser.Show(dockPanel, DockState.DockLeft);
        }

        private void buildProject(object sender, EventArgs e) {
            if (project == null)
                return;
            project.Build();
            // TODO tell the user that it finished
        }
    }
}
