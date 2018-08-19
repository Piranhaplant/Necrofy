namespace Necrofy
{
    partial class Form1
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
            this.dockPanel = new WeifenLuo.WinFormsUI.Docking.DockPanel();
            this.vS2012LightTheme1 = new WeifenLuo.WinFormsUI.Docking.VS2012LightTheme();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.createProjectButton = new System.Windows.Forms.ToolStripButton();
            this.openProjectButton = new System.Windows.Forms.ToolStripButton();
            this.buildProjectButton = new System.Windows.Forms.ToolStripButton();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dockPanel
            // 
            this.dockPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dockPanel.DockBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(239)))), ((int)(((byte)(242)))));
            this.dockPanel.Location = new System.Drawing.Point(0, 25);
            this.dockPanel.Name = "dockPanel";
            this.dockPanel.Padding = new System.Windows.Forms.Padding(6);
            this.dockPanel.ShowAutoHideContentOnHover = false;
            this.dockPanel.Size = new System.Drawing.Size(554, 303);
            this.dockPanel.TabIndex = 2;
            this.dockPanel.Theme = this.vS2012LightTheme1;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.createProjectButton,
            this.openProjectButton,
            this.buildProjectButton});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(554, 25);
            this.toolStrip1.TabIndex = 4;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // createProjectButton
            // 
            this.createProjectButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.createProjectButton.Image = global::Necrofy.Properties.Resources.document;
            this.createProjectButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.createProjectButton.Name = "createProjectButton";
            this.createProjectButton.Size = new System.Drawing.Size(23, 22);
            this.createProjectButton.Text = "Create project";
            this.createProjectButton.Click += new System.EventHandler(this.createProjectButton_Click);
            // 
            // openProjectButton
            // 
            this.openProjectButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.openProjectButton.Image = global::Necrofy.Properties.Resources.folder_horizontal_open;
            this.openProjectButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.openProjectButton.Name = "openProjectButton";
            this.openProjectButton.Size = new System.Drawing.Size(23, 22);
            this.openProjectButton.Text = "Open Project";
            this.openProjectButton.Click += new System.EventHandler(this.openProjectButton_Click);
            // 
            // buildProjectButton
            // 
            this.buildProjectButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buildProjectButton.Image = global::Necrofy.Properties.Resources.compile;
            this.buildProjectButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buildProjectButton.Name = "buildProjectButton";
            this.buildProjectButton.Size = new System.Drawing.Size(23, 22);
            this.buildProjectButton.Text = "Build project";
            this.buildProjectButton.Click += new System.EventHandler(this.buildProjectButton_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(554, 328);
            this.Controls.Add(this.dockPanel);
            this.Controls.Add(this.toolStrip1);
            this.IsMdiContainer = true;
            this.Name = "Form1";
            this.Text = "Form1";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
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

    }
}

