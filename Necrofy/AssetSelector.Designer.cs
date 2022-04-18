namespace Necrofy
{
    partial class AssetSelector
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.comboTree = new ComboTreeBox();
            this.SuspendLayout();
            // 
            // comboTree
            // 
            this.comboTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.comboTree.DroppedDown = false;
            this.comboTree.Location = new System.Drawing.Point(0, 0);
            this.comboTree.Name = "comboTree";
            this.comboTree.PathSeparator = "/";
            this.comboTree.SelectedNode = null;
            this.comboTree.ShowPath = true;
            this.comboTree.Size = new System.Drawing.Size(150, 21);
            this.comboTree.TabIndex = 0;
            this.comboTree.SelectedNodeChanged += new System.EventHandler(this.comboTreeBox1_SelectedNodeChanged);
            // 
            // AssetSelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.comboTree);
            this.Name = "AssetSelector";
            this.Size = new System.Drawing.Size(150, 21);
            this.ResumeLayout(false);

        }

        #endregion

        private ComboTreeBox comboTree;
    }
}
