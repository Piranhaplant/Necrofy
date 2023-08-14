namespace Necrofy
{
    partial class ProjectBrowser
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
            this.tree = new System.Windows.Forms.TreeView();
            this.treeImages = new System.Windows.Forms.ImageList(this.components);
            this.contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.contextOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.contextRename = new System.Windows.Forms.ToolStripMenuItem();
            this.contextSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.contextCut = new System.Windows.Forms.ToolStripMenuItem();
            this.contextCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.contextPaste = new System.Windows.Forms.ToolStripMenuItem();
            this.contextDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // tree
            // 
            this.tree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tree.ImageIndex = 0;
            this.tree.ImageList = this.treeImages;
            this.tree.Location = new System.Drawing.Point(0, 0);
            this.tree.Name = "tree";
            this.tree.SelectedImageIndex = 0;
            this.tree.Size = new System.Drawing.Size(284, 262);
            this.tree.TabIndex = 0;
            this.tree.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.tree_AfterLabelEdit);
            this.tree.BeforeCollapse += new System.Windows.Forms.TreeViewCancelEventHandler(this.tree_BeforeCollapse);
            this.tree.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.tree_BeforeExpand);
            this.tree.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.tree_NodeMouseClick);
            this.tree.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.tree_NodeMouseDoubleClick);
            // 
            // treeImages
            // 
            this.treeImages.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.treeImages.ImageSize = new System.Drawing.Size(16, 16);
            this.treeImages.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // contextMenu
            // 
            this.contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.contextOpen,
            this.contextRename,
            this.contextSeparator1,
            this.contextCut,
            this.contextCopy,
            this.contextPaste,
            this.contextDelete});
            this.contextMenu.Name = "treeMenu";
            this.contextMenu.Size = new System.Drawing.Size(118, 142);
            // 
            // contextOpen
            // 
            this.contextOpen.Image = global::Necrofy.Properties.Resources.folder_horizontal_open;
            this.contextOpen.Name = "contextOpen";
            this.contextOpen.Size = new System.Drawing.Size(117, 22);
            this.contextOpen.Text = "Open";
            this.contextOpen.Click += new System.EventHandler(this.contextOpen_Click);
            // 
            // contextRename
            // 
            this.contextRename.Image = global::Necrofy.Properties.Resources.blue_document_rename;
            this.contextRename.Name = "contextRename";
            this.contextRename.Size = new System.Drawing.Size(117, 22);
            this.contextRename.Text = "Rename";
            this.contextRename.Click += new System.EventHandler(this.contextRename_Click);
            // 
            // contextSeparator1
            // 
            this.contextSeparator1.Name = "contextSeparator1";
            this.contextSeparator1.Size = new System.Drawing.Size(114, 6);
            // 
            // contextCut
            // 
            this.contextCut.Image = global::Necrofy.Properties.Resources.scissors;
            this.contextCut.Name = "contextCut";
            this.contextCut.Size = new System.Drawing.Size(117, 22);
            this.contextCut.Text = "Cut";
            this.contextCut.Click += new System.EventHandler(this.contextCut_Click);
            // 
            // contextCopy
            // 
            this.contextCopy.Image = global::Necrofy.Properties.Resources.document_copy;
            this.contextCopy.Name = "contextCopy";
            this.contextCopy.Size = new System.Drawing.Size(117, 22);
            this.contextCopy.Text = "Copy";
            this.contextCopy.Click += new System.EventHandler(this.contextCopy_Click);
            // 
            // contextPaste
            // 
            this.contextPaste.Image = global::Necrofy.Properties.Resources.clipboard_paste;
            this.contextPaste.Name = "contextPaste";
            this.contextPaste.Size = new System.Drawing.Size(117, 22);
            this.contextPaste.Text = "Paste";
            this.contextPaste.Click += new System.EventHandler(this.contextPaste_Click);
            // 
            // contextDelete
            // 
            this.contextDelete.Image = global::Necrofy.Properties.Resources.cross_script;
            this.contextDelete.Name = "contextDelete";
            this.contextDelete.Size = new System.Drawing.Size(117, 22);
            this.contextDelete.Text = "Delete";
            this.contextDelete.Click += new System.EventHandler(this.contextDelete_Click);
            // 
            // ProjectBrowser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.tree);
            this.DockAreas = ((WeifenLuo.WinFormsUI.Docking.DockAreas)(((((WeifenLuo.WinFormsUI.Docking.DockAreas.Float | WeifenLuo.WinFormsUI.Docking.DockAreas.DockLeft) 
            | WeifenLuo.WinFormsUI.Docking.DockAreas.DockRight) 
            | WeifenLuo.WinFormsUI.Docking.DockAreas.DockTop) 
            | WeifenLuo.WinFormsUI.Docking.DockAreas.DockBottom)));
            this.HideOnClose = true;
            this.Name = "ProjectBrowser";
            this.Text = "Project";
            this.contextMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView tree;
        private System.Windows.Forms.ImageList treeImages;
        private System.Windows.Forms.ContextMenuStrip contextMenu;
        private System.Windows.Forms.ToolStripMenuItem contextOpen;
        private System.Windows.Forms.ToolStripMenuItem contextRename;
        private System.Windows.Forms.ToolStripSeparator contextSeparator1;
        private System.Windows.Forms.ToolStripMenuItem contextCut;
        private System.Windows.Forms.ToolStripMenuItem contextCopy;
        private System.Windows.Forms.ToolStripMenuItem contextPaste;
        private System.Windows.Forms.ToolStripMenuItem contextDelete;
    }
}