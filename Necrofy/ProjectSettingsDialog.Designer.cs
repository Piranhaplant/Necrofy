namespace Necrofy
{
    partial class ProjectSettingsDialog
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.patchesTab = new System.Windows.Forms.TabPage();
            this.patchesList = new System.Windows.Forms.CheckedListBox();
            this.patchDescriptionText = new System.Windows.Forms.TextBox();
            this.buttonPanel = new System.Windows.Forms.Panel();
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.patchesTab.SuspendLayout();
            this.buttonPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.patchesTab);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(520, 258);
            this.tabControl1.TabIndex = 0;
            // 
            // patchesTab
            // 
            this.patchesTab.Controls.Add(this.patchDescriptionText);
            this.patchesTab.Controls.Add(this.patchesList);
            this.patchesTab.Location = new System.Drawing.Point(4, 22);
            this.patchesTab.Name = "patchesTab";
            this.patchesTab.Padding = new System.Windows.Forms.Padding(3);
            this.patchesTab.Size = new System.Drawing.Size(512, 232);
            this.patchesTab.TabIndex = 0;
            this.patchesTab.Text = "Patches";
            this.patchesTab.UseVisualStyleBackColor = true;
            // 
            // patchesList
            // 
            this.patchesList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.patchesList.FormattingEnabled = true;
            this.patchesList.Location = new System.Drawing.Point(3, 3);
            this.patchesList.Name = "patchesList";
            this.patchesList.Size = new System.Drawing.Size(506, 226);
            this.patchesList.TabIndex = 1;
            this.patchesList.SelectedIndexChanged += new System.EventHandler(this.patchesList_SelectedIndexChanged);
            // 
            // patchDescriptionText
            // 
            this.patchDescriptionText.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.patchDescriptionText.Location = new System.Drawing.Point(3, 136);
            this.patchDescriptionText.Multiline = true;
            this.patchDescriptionText.Name = "patchDescriptionText";
            this.patchDescriptionText.ReadOnly = true;
            this.patchDescriptionText.Size = new System.Drawing.Size(506, 93);
            this.patchDescriptionText.TabIndex = 2;
            // 
            // buttonPanel
            // 
            this.buttonPanel.Controls.Add(this.okButton);
            this.buttonPanel.Controls.Add(this.cancelButton);
            this.buttonPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.buttonPanel.Location = new System.Drawing.Point(0, 258);
            this.buttonPanel.Name = "buttonPanel";
            this.buttonPanel.Size = new System.Drawing.Size(520, 44);
            this.buttonPanel.TabIndex = 1;
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(433, 9);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 0;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(352, 9);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 1;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // ProjectSettingsDialog
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(520, 302);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.buttonPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ProjectSettingsDialog";
            this.Text = "Project Settings";
            this.tabControl1.ResumeLayout(false);
            this.patchesTab.ResumeLayout(false);
            this.patchesTab.PerformLayout();
            this.buttonPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage patchesTab;
        private System.Windows.Forms.TextBox patchDescriptionText;
        private System.Windows.Forms.CheckedListBox patchesList;
        private System.Windows.Forms.Panel buttonPanel;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
    }
}