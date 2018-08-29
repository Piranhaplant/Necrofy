namespace Necrofy
{
    partial class MainWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            this.dockPanel = new WeifenLuo.WinFormsUI.Docking.DockPanel();
            this.vS2012LightTheme1 = new WeifenLuo.WinFormsUI.Docking.VS2012LightTheme();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.createProjectButton = new System.Windows.Forms.ToolStripButton();
            this.openProjectButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.buildProjectButton = new System.Windows.Forms.ToolStripButton();
            this.runProjectButton = new System.Windows.Forms.ToolStripButton();
            this.runFromLevelButton = new System.Windows.Forms.ToolStripButton();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.fileNewProject = new System.Windows.Forms.ToolStripMenuItem();
            this.fileOpenProject = new System.Windows.Forms.ToolStripMenuItem();
            this.fileSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.fileSave = new System.Windows.Forms.ToolStripMenuItem();
            this.fileSaveAll = new System.Windows.Forms.ToolStripMenuItem();
            this.fileSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.fileExit = new System.Windows.Forms.ToolStripMenuItem();
            this.buildMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.buildBuildProject = new System.Windows.Forms.ToolStripMenuItem();
            this.buildRunProject = new System.Windows.Forms.ToolStripMenuItem();
            this.buildRunFromLevel = new System.Windows.Forms.ToolStripMenuItem();
            this.buildRunSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.openProjectDialog = new System.Windows.Forms.OpenFileDialog();
            this.fileSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.recentProjects = new Necrofy.RecentFilesMenu();
            this.toolStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dockPanel
            // 
            this.dockPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dockPanel.DockBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(239)))), ((int)(((byte)(242)))));
            this.dockPanel.DockBottomPortion = 200D;
            this.dockPanel.DockLeftPortion = 200D;
            this.dockPanel.DockRightPortion = 200D;
            this.dockPanel.DockTopPortion = 200D;
            this.dockPanel.Location = new System.Drawing.Point(0, 49);
            this.dockPanel.Name = "dockPanel";
            this.dockPanel.Padding = new System.Windows.Forms.Padding(6);
            this.dockPanel.ShowAutoHideContentOnHover = false;
            this.dockPanel.Size = new System.Drawing.Size(1061, 601);
            this.dockPanel.TabIndex = 2;
            this.dockPanel.Theme = this.vS2012LightTheme1;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.createProjectButton,
            this.openProjectButton,
            this.toolStripSeparator1,
            this.buildProjectButton,
            this.runProjectButton,
            this.runFromLevelButton});
            this.toolStrip1.Location = new System.Drawing.Point(0, 24);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1061, 25);
            this.toolStrip1.TabIndex = 4;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // createProjectButton
            // 
            this.createProjectButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.createProjectButton.Image = global::Necrofy.Properties.Resources.document__pencil;
            this.createProjectButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.createProjectButton.Name = "createProjectButton";
            this.createProjectButton.Size = new System.Drawing.Size(23, 22);
            this.createProjectButton.Text = "Create project";
            this.createProjectButton.Click += new System.EventHandler(this.createProject);
            // 
            // openProjectButton
            // 
            this.openProjectButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.openProjectButton.Image = global::Necrofy.Properties.Resources.folder_horizontal_open;
            this.openProjectButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.openProjectButton.Name = "openProjectButton";
            this.openProjectButton.Size = new System.Drawing.Size(23, 22);
            this.openProjectButton.Text = "Open Project";
            this.openProjectButton.Click += new System.EventHandler(this.openProject);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // buildProjectButton
            // 
            this.buildProjectButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buildProjectButton.Image = global::Necrofy.Properties.Resources.compile;
            this.buildProjectButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buildProjectButton.Name = "buildProjectButton";
            this.buildProjectButton.Size = new System.Drawing.Size(23, 22);
            this.buildProjectButton.Text = "Build project";
            this.buildProjectButton.Click += new System.EventHandler(this.buildProject);
            // 
            // runProjectButton
            // 
            this.runProjectButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.runProjectButton.Image = global::Necrofy.Properties.Resources.control;
            this.runProjectButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.runProjectButton.Name = "runProjectButton";
            this.runProjectButton.Size = new System.Drawing.Size(23, 22);
            this.runProjectButton.Text = "Run Project";
            // 
            // runFromLevelButton
            // 
            this.runFromLevelButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.runFromLevelButton.Image = global::Necrofy.Properties.Resources.control_cursor;
            this.runFromLevelButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.runFromLevelButton.Name = "runFromLevelButton";
            this.runFromLevelButton.Size = new System.Drawing.Size(23, 22);
            this.runFromLevelButton.Text = "Run From Level";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileMenu,
            this.buildMenu});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1061, 24);
            this.menuStrip1.TabIndex = 6;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileMenu
            // 
            this.fileMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileNewProject,
            this.fileOpenProject,
            this.fileSeparator1,
            this.fileSave,
            this.fileSaveAll,
            this.fileSeparator2,
            this.recentProjects,
            this.fileSeparator3,
            this.fileExit});
            this.fileMenu.Name = "fileMenu";
            this.fileMenu.Size = new System.Drawing.Size(37, 20);
            this.fileMenu.Text = "File";
            // 
            // fileNewProject
            // 
            this.fileNewProject.Image = global::Necrofy.Properties.Resources.document__pencil;
            this.fileNewProject.Name = "fileNewProject";
            this.fileNewProject.Size = new System.Drawing.Size(155, 22);
            this.fileNewProject.Text = "New Project";
            this.fileNewProject.Click += new System.EventHandler(this.createProject);
            // 
            // fileOpenProject
            // 
            this.fileOpenProject.Image = global::Necrofy.Properties.Resources.folder_horizontal_open;
            this.fileOpenProject.Name = "fileOpenProject";
            this.fileOpenProject.Size = new System.Drawing.Size(155, 22);
            this.fileOpenProject.Text = "Open Project";
            this.fileOpenProject.Click += new System.EventHandler(this.openProject);
            // 
            // fileSeparator1
            // 
            this.fileSeparator1.Name = "fileSeparator1";
            this.fileSeparator1.Size = new System.Drawing.Size(152, 6);
            // 
            // fileSave
            // 
            this.fileSave.Enabled = false;
            this.fileSave.Image = global::Necrofy.Properties.Resources.disk;
            this.fileSave.Name = "fileSave";
            this.fileSave.Size = new System.Drawing.Size(155, 22);
            this.fileSave.Text = "Save";
            // 
            // fileSaveAll
            // 
            this.fileSaveAll.Enabled = false;
            this.fileSaveAll.Image = global::Necrofy.Properties.Resources.disks;
            this.fileSaveAll.Name = "fileSaveAll";
            this.fileSaveAll.Size = new System.Drawing.Size(155, 22);
            this.fileSaveAll.Text = "Save All";
            // 
            // fileSeparator2
            // 
            this.fileSeparator2.Name = "fileSeparator2";
            this.fileSeparator2.Size = new System.Drawing.Size(152, 6);
            // 
            // fileExit
            // 
            this.fileExit.Name = "fileExit";
            this.fileExit.Size = new System.Drawing.Size(155, 22);
            this.fileExit.Text = "Exit";
            // 
            // buildMenu
            // 
            this.buildMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.buildBuildProject,
            this.buildRunProject,
            this.buildRunFromLevel,
            this.buildRunSettings});
            this.buildMenu.Name = "buildMenu";
            this.buildMenu.Size = new System.Drawing.Size(46, 20);
            this.buildMenu.Text = "Build";
            // 
            // buildBuildProject
            // 
            this.buildBuildProject.Image = global::Necrofy.Properties.Resources.compile;
            this.buildBuildProject.Name = "buildBuildProject";
            this.buildBuildProject.Size = new System.Drawing.Size(156, 22);
            this.buildBuildProject.Text = "Build Project";
            this.buildBuildProject.Click += new System.EventHandler(this.buildProject);
            // 
            // buildRunProject
            // 
            this.buildRunProject.Image = global::Necrofy.Properties.Resources.control;
            this.buildRunProject.Name = "buildRunProject";
            this.buildRunProject.Size = new System.Drawing.Size(156, 22);
            this.buildRunProject.Text = "Run Project";
            // 
            // buildRunFromLevel
            // 
            this.buildRunFromLevel.Image = global::Necrofy.Properties.Resources.control_cursor;
            this.buildRunFromLevel.Name = "buildRunFromLevel";
            this.buildRunFromLevel.Size = new System.Drawing.Size(156, 22);
            this.buildRunFromLevel.Text = "Run From Level";
            // 
            // buildRunSettings
            // 
            this.buildRunSettings.Name = "buildRunSettings";
            this.buildRunSettings.Size = new System.Drawing.Size(156, 22);
            this.buildRunSettings.Text = "Run Settings...";
            // 
            // openProjectDialog
            // 
            this.openProjectDialog.Filter = "Necrofy project files (*.nfyp)|*.nfyp|All Files (*.*)|*.*";
            this.openProjectDialog.Title = "Open Project";
            // 
            // fileSeparator3
            // 
            this.fileSeparator3.Name = "fileSeparator3";
            this.fileSeparator3.Size = new System.Drawing.Size(152, 6);
            // 
            // recentProjects
            // 
            this.recentProjects.Files = ((System.Collections.Generic.IEnumerable<string>)(resources.GetObject("recentProjects.Files")));
            this.recentProjects.MaxItems = 11;
            this.recentProjects.MaxLength = 60;
            this.recentProjects.Name = "recentProjects";
            this.recentProjects.Separator = this.fileSeparator3;
            this.recentProjects.Size = new System.Drawing.Size(155, 22);
            this.recentProjects.Text = "Recent Projects";
            this.recentProjects.FileClicked += new Necrofy.RecentFilesMenu.FileClickedDelegate(this.recentProjects_FileClicked);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1061, 650);
            this.Controls.Add(this.dockPanel);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.menuStrip1);
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainWindow";
            this.Text = "Necrofy";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private WeifenLuo.WinFormsUI.Docking.DockPanel dockPanel;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton createProjectButton;
        private System.Windows.Forms.ToolStripButton buildProjectButton;
        private WeifenLuo.WinFormsUI.Docking.VS2012LightTheme vS2012LightTheme1;
        private System.Windows.Forms.ToolStripButton openProjectButton;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileMenu;
        private System.Windows.Forms.ToolStripMenuItem fileNewProject;
        private System.Windows.Forms.ToolStripMenuItem fileOpenProject;
        private System.Windows.Forms.ToolStripSeparator fileSeparator1;
        private System.Windows.Forms.ToolStripMenuItem fileSave;
        private System.Windows.Forms.ToolStripMenuItem fileSaveAll;
        private System.Windows.Forms.ToolStripSeparator fileSeparator2;
        private System.Windows.Forms.ToolStripMenuItem fileExit;
        private System.Windows.Forms.ToolStripMenuItem buildMenu;
        private System.Windows.Forms.ToolStripMenuItem buildBuildProject;
        private System.Windows.Forms.ToolStripMenuItem buildRunProject;
        private System.Windows.Forms.ToolStripMenuItem buildRunFromLevel;
        private System.Windows.Forms.ToolStripMenuItem buildRunSettings;
        private System.Windows.Forms.OpenFileDialog openProjectDialog;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton runProjectButton;
        private System.Windows.Forms.ToolStripButton runFromLevelButton;
        private System.Windows.Forms.ToolStripSeparator fileSeparator3;
        private RecentFilesMenu recentProjects;

    }
}

