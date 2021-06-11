namespace Necrofy
{
    partial class PreferencesDialog
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
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.emulatorGroup = new System.Windows.Forms.GroupBox();
            this.emulatorBrowse = new System.Windows.Forms.Button();
            this.emulatorText = new System.Windows.Forms.TextBox();
            this.systemDefaultEmulator = new System.Windows.Forms.CheckBox();
            this.openEmulator = new System.Windows.Forms.OpenFileDialog();
            this.emulatorGroup.SuspendLayout();
            this.SuspendLayout();
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.Location = new System.Drawing.Point(229, 96);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 3;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(310, 96);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 2;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // emulatorGroup
            // 
            this.emulatorGroup.Controls.Add(this.emulatorBrowse);
            this.emulatorGroup.Controls.Add(this.emulatorText);
            this.emulatorGroup.Controls.Add(this.systemDefaultEmulator);
            this.emulatorGroup.Location = new System.Drawing.Point(12, 12);
            this.emulatorGroup.Name = "emulatorGroup";
            this.emulatorGroup.Size = new System.Drawing.Size(373, 74);
            this.emulatorGroup.TabIndex = 4;
            this.emulatorGroup.TabStop = false;
            this.emulatorGroup.Text = "Emulator";
            // 
            // emulatorBrowse
            // 
            this.emulatorBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.emulatorBrowse.Location = new System.Drawing.Point(292, 40);
            this.emulatorBrowse.Name = "emulatorBrowse";
            this.emulatorBrowse.Size = new System.Drawing.Size(75, 23);
            this.emulatorBrowse.TabIndex = 7;
            this.emulatorBrowse.Text = "Browse...";
            this.emulatorBrowse.UseVisualStyleBackColor = true;
            this.emulatorBrowse.Click += new System.EventHandler(this.emulatorBrowse_Click);
            // 
            // emulatorText
            // 
            this.emulatorText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.emulatorText.Location = new System.Drawing.Point(6, 42);
            this.emulatorText.Name = "emulatorText";
            this.emulatorText.Size = new System.Drawing.Size(280, 20);
            this.emulatorText.TabIndex = 6;
            // 
            // systemDefaultEmulator
            // 
            this.systemDefaultEmulator.AutoSize = true;
            this.systemDefaultEmulator.Location = new System.Drawing.Point(6, 19);
            this.systemDefaultEmulator.Name = "systemDefaultEmulator";
            this.systemDefaultEmulator.Size = new System.Drawing.Size(115, 17);
            this.systemDefaultEmulator.TabIndex = 5;
            this.systemDefaultEmulator.Text = "Use system default";
            this.systemDefaultEmulator.UseVisualStyleBackColor = true;
            this.systemDefaultEmulator.CheckedChanged += new System.EventHandler(this.systemDefaultEmulator_CheckedChanged);
            // 
            // openEmulator
            // 
            this.openEmulator.Filter = "Executable Files (*.exe)|*.exe|All Files (*.*)|*.*";
            // 
            // PreferencesDialog
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(397, 131);
            this.Controls.Add(this.emulatorGroup);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.cancelButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "PreferencesDialog";
            this.ShowIcon = false;
            this.Text = "Necrofy Preferences";
            this.emulatorGroup.ResumeLayout(false);
            this.emulatorGroup.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.GroupBox emulatorGroup;
        private System.Windows.Forms.Button emulatorBrowse;
        private System.Windows.Forms.TextBox emulatorText;
        private System.Windows.Forms.CheckBox systemDefaultEmulator;
        private System.Windows.Forms.OpenFileDialog openEmulator;
    }
}