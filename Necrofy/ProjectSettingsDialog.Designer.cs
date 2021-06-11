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
            this.generalTab = new System.Windows.Forms.TabPage();
            this.endGameLevel = new System.Windows.Forms.NumericUpDown();
            this.endGameLevelLabel = new System.Windows.Forms.Label();
            this.winLevel = new System.Windows.Forms.NumericUpDown();
            this.winLevelLabel = new System.Windows.Forms.Label();
            this.patchesTab = new System.Windows.Forms.TabPage();
            this.patchDescriptionText = new System.Windows.Forms.TextBox();
            this.patchesList = new System.Windows.Forms.CheckedListBox();
            this.buttonPanel = new System.Windows.Forms.Panel();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.generalTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.endGameLevel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.winLevel)).BeginInit();
            this.patchesTab.SuspendLayout();
            this.buttonPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.generalTab);
            this.tabControl1.Controls.Add(this.patchesTab);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(520, 258);
            this.tabControl1.TabIndex = 0;
            // 
            // generalTab
            // 
            this.generalTab.Controls.Add(this.endGameLevel);
            this.generalTab.Controls.Add(this.endGameLevelLabel);
            this.generalTab.Controls.Add(this.winLevel);
            this.generalTab.Controls.Add(this.winLevelLabel);
            this.generalTab.Location = new System.Drawing.Point(4, 22);
            this.generalTab.Name = "generalTab";
            this.generalTab.Padding = new System.Windows.Forms.Padding(3);
            this.generalTab.Size = new System.Drawing.Size(512, 232);
            this.generalTab.TabIndex = 1;
            this.generalTab.Text = "General";
            this.generalTab.UseVisualStyleBackColor = true;
            // 
            // endGameLevel
            // 
            this.endGameLevel.Location = new System.Drawing.Point(165, 43);
            this.endGameLevel.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.endGameLevel.Name = "endGameLevel";
            this.endGameLevel.Size = new System.Drawing.Size(66, 20);
            this.endGameLevel.TabIndex = 3;
            // 
            // endGameLevelLabel
            // 
            this.endGameLevelLabel.AutoSize = true;
            this.endGameLevelLabel.Location = new System.Drawing.Point(19, 45);
            this.endGameLevelLabel.Name = "endGameLevelLabel";
            this.endGameLevelLabel.Size = new System.Drawing.Size(107, 13);
            this.endGameLevelLabel.TabIndex = 2;
            this.endGameLevelLabel.Text = "End game after level:";
            // 
            // winLevel
            // 
            this.winLevel.Location = new System.Drawing.Point(165, 17);
            this.winLevel.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.winLevel.Name = "winLevel";
            this.winLevel.Size = new System.Drawing.Size(66, 20);
            this.winLevel.TabIndex = 1;
            // 
            // winLevelLabel
            // 
            this.winLevelLabel.AutoSize = true;
            this.winLevelLabel.Location = new System.Drawing.Point(19, 19);
            this.winLevelLabel.Name = "winLevelLabel";
            this.winLevelLabel.Size = new System.Drawing.Size(140, 13);
            this.winLevelLabel.TabIndex = 0;
            this.winLevelLabel.Text = "Show win screen after level:";
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
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.Location = new System.Drawing.Point(352, 9);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 1;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
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
            this.generalTab.ResumeLayout(false);
            this.generalTab.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.endGameLevel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.winLevel)).EndInit();
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
        private System.Windows.Forms.TabPage generalTab;
        private System.Windows.Forms.NumericUpDown endGameLevel;
        private System.Windows.Forms.Label endGameLevelLabel;
        private System.Windows.Forms.NumericUpDown winLevel;
        private System.Windows.Forms.Label winLevelLabel;
    }
}