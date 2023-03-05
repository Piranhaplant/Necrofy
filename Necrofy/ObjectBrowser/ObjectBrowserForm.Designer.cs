namespace Necrofy
{
    partial class ObjectBrowserForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            this.objectBrowser = new Necrofy.ObjectBrowserControl();
            this.contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.grid = new System.Windows.Forms.ToolStripMenuItem();
            this.list = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // objectBrowser
            // 
            this.objectBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.objectBrowser.Location = new System.Drawing.Point(0, 0);
            this.objectBrowser.Name = "objectBrowser";
            this.objectBrowser.Size = new System.Drawing.Size(203, 476);
            this.objectBrowser.TabIndex = 0;
            // 
            // contextMenu
            // 
            this.contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.grid,
            this.list});
            this.contextMenu.Name = "contextMenu";
            this.contextMenu.Size = new System.Drawing.Size(181, 70);
            this.contextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenu_Opening);
            // 
            // grid
            // 
            this.grid.Checked = true;
            this.grid.CheckState = System.Windows.Forms.CheckState.Checked;
            this.grid.Name = "grid";
            this.grid.Size = new System.Drawing.Size(96, 22);
            this.grid.Text = "Grid";
            this.grid.Click += new System.EventHandler(this.grid_Click);
            // 
            // list
            // 
            this.list.Name = "list";
            this.list.Size = new System.Drawing.Size(180, 22);
            this.list.Text = "List";
            this.list.Click += new System.EventHandler(this.list_Click);
            // 
            // ObjectBrowserForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(203, 476);
            this.Controls.Add(this.objectBrowser);
            this.DockAreas = ((WeifenLuo.WinFormsUI.Docking.DockAreas)(((((WeifenLuo.WinFormsUI.Docking.DockAreas.Float | WeifenLuo.WinFormsUI.Docking.DockAreas.DockLeft) 
            | WeifenLuo.WinFormsUI.Docking.DockAreas.DockRight) 
            | WeifenLuo.WinFormsUI.Docking.DockAreas.DockTop) 
            | WeifenLuo.WinFormsUI.Docking.DockAreas.DockBottom)));
            this.HideOnClose = true;
            this.Name = "ObjectBrowserForm";
            this.TabPageContextMenuStrip = this.contextMenu;
            this.Text = "Objects";
            this.contextMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private ObjectBrowserControl objectBrowser;
        private System.Windows.Forms.ContextMenuStrip contextMenu;
        private System.Windows.Forms.ToolStripMenuItem grid;
        private System.Windows.Forms.ToolStripMenuItem list;
    }
}