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
    public partial class Form1 : Form
    {
        private Project project;

        public Form1()
        {
            InitializeComponent();
            DockForm f2 = new DockForm();
            f2.Show(docker, DockState.DockLeft);
            DockForm f3 = new DockForm();
            f3.Show(docker, DockState.DockRight);
        }

        private void createProjectButton_Click(object sender, EventArgs e) {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;
            string projectPath = Path.Combine(Path.GetDirectoryName(ofd.FileName), "zamn_project");
            if (Directory.Exists(projectPath)) {
                Directory.Delete(projectPath, true);
            }
            Directory.CreateDirectory(projectPath);
            project = new Project(ofd.FileName, projectPath);
        }

        private void openProjectButton_Click(object sender, EventArgs e) {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Necrofy project files (*.nfyp)|*.nfyp|All Files (*.*)|*.*";
            if (ofd.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;
            project = new Project(ofd.FileName);
        }

        private void buildProjectButton_Click(object sender, EventArgs e) {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;
            project.Build(ofd.FileName, ofd.FileName + ".new.sfc");
        }

        int levelnum = 1;

        private void openLevelButton_Click(object sender, EventArgs e) {
            LoadedLevel loadedLevel = new LoadedLevel(project, levelnum++);

            LevelEditor levelEditor = new LevelEditor(loadedLevel);
            levelEditor.Show(docker, DockState.Document);
        }
    }
}
