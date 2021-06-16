namespace Necrofy
{
    partial class NewProjectDialog
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
            this.lblBaseROM = new System.Windows.Forms.Label();
            this.lblLocation = new System.Windows.Forms.Label();
            this.btnBaseROM = new System.Windows.Forms.Button();
            this.btnLocation = new System.Windows.Forms.Button();
            this.txtBaseROM = new System.Windows.Forms.TextBox();
            this.txtLocation = new System.Windows.Forms.TextBox();
            this.btnCreate = new System.Windows.Forms.Button();
            this.ofdBaseROM = new System.Windows.Forms.OpenFileDialog();
            this.btnCancel = new System.Windows.Forms.Button();
            this.sfdLocation = new System.Windows.Forms.SaveFileDialog();
            this.SuspendLayout();
            // 
            // lblBaseROM
            // 
            this.lblBaseROM.AutoSize = true;
            this.lblBaseROM.Location = new System.Drawing.Point(12, 17);
            this.lblBaseROM.Name = "lblBaseROM";
            this.lblBaseROM.Size = new System.Drawing.Size(62, 13);
            this.lblBaseROM.TabIndex = 0;
            this.lblBaseROM.Text = "Base ROM:";
            // 
            // lblLocation
            // 
            this.lblLocation.AutoSize = true;
            this.lblLocation.Location = new System.Drawing.Point(12, 46);
            this.lblLocation.Name = "lblLocation";
            this.lblLocation.Size = new System.Drawing.Size(103, 13);
            this.lblLocation.TabIndex = 1;
            this.lblLocation.Text = "Project Save Folder:";
            // 
            // btnBaseROM
            // 
            this.btnBaseROM.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBaseROM.Location = new System.Drawing.Point(434, 12);
            this.btnBaseROM.Name = "btnBaseROM";
            this.btnBaseROM.Size = new System.Drawing.Size(92, 23);
            this.btnBaseROM.TabIndex = 2;
            this.btnBaseROM.Text = "Browse...";
            this.btnBaseROM.UseVisualStyleBackColor = true;
            this.btnBaseROM.Click += new System.EventHandler(this.btnBaseROM_Click);
            // 
            // btnLocation
            // 
            this.btnLocation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLocation.Location = new System.Drawing.Point(434, 41);
            this.btnLocation.Name = "btnLocation";
            this.btnLocation.Size = new System.Drawing.Size(92, 23);
            this.btnLocation.TabIndex = 3;
            this.btnLocation.Text = "Browse...";
            this.btnLocation.UseVisualStyleBackColor = true;
            this.btnLocation.Click += new System.EventHandler(this.btnLocation_Click);
            // 
            // txtBaseROM
            // 
            this.txtBaseROM.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtBaseROM.Location = new System.Drawing.Point(121, 14);
            this.txtBaseROM.Name = "txtBaseROM";
            this.txtBaseROM.Size = new System.Drawing.Size(307, 20);
            this.txtBaseROM.TabIndex = 4;
            // 
            // txtLocation
            // 
            this.txtLocation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLocation.Location = new System.Drawing.Point(121, 43);
            this.txtLocation.Name = "txtLocation";
            this.txtLocation.Size = new System.Drawing.Size(307, 20);
            this.txtLocation.TabIndex = 5;
            // 
            // btnCreate
            // 
            this.btnCreate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCreate.Location = new System.Drawing.Point(336, 86);
            this.btnCreate.Name = "btnCreate";
            this.btnCreate.Size = new System.Drawing.Size(92, 23);
            this.btnCreate.TabIndex = 6;
            this.btnCreate.Text = "Create";
            this.btnCreate.UseVisualStyleBackColor = true;
            this.btnCreate.Click += new System.EventHandler(this.btnCreate_Click);
            // 
            // ofdBaseROM
            // 
            this.ofdBaseROM.Filter = "SNES ROMs (*.sfc;*.smc)|*.sfc;*.smc|All Files (*.*)|*.*";
            this.ofdBaseROM.Title = "Base ROM";
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(434, 86);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(92, 23);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // sfdLocation
            // 
            this.sfdLocation.AddExtension = false;
            this.sfdLocation.Filter = "Necrofy Project Folder|";
            this.sfdLocation.Title = "Location";
            // 
            // NewProjectDialog
            // 
            this.AcceptButton = this.btnCreate;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(538, 120);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnCreate);
            this.Controls.Add(this.txtLocation);
            this.Controls.Add(this.txtBaseROM);
            this.Controls.Add(this.btnLocation);
            this.Controls.Add(this.btnBaseROM);
            this.Controls.Add(this.lblLocation);
            this.Controls.Add(this.lblBaseROM);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "NewProjectDialog";
            this.Text = "New Project";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblBaseROM;
        private System.Windows.Forms.Label lblLocation;
        private System.Windows.Forms.Button btnBaseROM;
        private System.Windows.Forms.Button btnLocation;
        private System.Windows.Forms.TextBox txtBaseROM;
        private System.Windows.Forms.TextBox txtLocation;
        private System.Windows.Forms.Button btnCreate;
        private System.Windows.Forms.OpenFileDialog ofdBaseROM;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.SaveFileDialog sfdLocation;
    }
}