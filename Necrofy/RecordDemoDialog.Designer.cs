namespace Necrofy
{
    partial class RecordDemoDialog
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
            this.lblLevel = new System.Windows.Forms.Label();
            this.lblCharacter = new System.Windows.Forms.Label();
            this.characterBox = new System.Windows.Forms.ComboBox();
            this.recordingPanel = new System.Windows.Forms.Panel();
            this.cancelRecord = new System.Windows.Forms.Button();
            this.recordingLabel = new System.Windows.Forms.Label();
            this.savePanel = new System.Windows.Forms.Panel();
            this.slot3 = new System.Windows.Forms.RadioButton();
            this.slot2 = new System.Windows.Forms.RadioButton();
            this.slot1 = new System.Windows.Forms.RadioButton();
            this.slot0 = new System.Windows.Forms.RadioButton();
            this.levelBox = new System.Windows.Forms.Label();
            this.saveButton = new System.Windows.Forms.Button();
            this.cancelSave = new System.Windows.Forms.Button();
            this.slotLabel = new System.Windows.Forms.Label();
            this.findSRAMPanel = new System.Windows.Forms.Panel();
            this.browseButton = new System.Windows.Forms.Button();
            this.saveDataNotFound = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.endedEarlyPanel = new System.Windows.Forms.Panel();
            this.okButton = new System.Windows.Forms.Button();
            this.endedEarlyLabel = new System.Windows.Forms.Label();
            this.openSRAM = new System.Windows.Forms.OpenFileDialog();
            this.demoLengthLabel = new System.Windows.Forms.Label();
            this.demoLengthBox = new System.Windows.Forms.Label();
            this.recordingPanel.SuspendLayout();
            this.savePanel.SuspendLayout();
            this.findSRAMPanel.SuspendLayout();
            this.endedEarlyPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblLevel
            // 
            this.lblLevel.AutoSize = true;
            this.lblLevel.Location = new System.Drawing.Point(12, 9);
            this.lblLevel.Name = "lblLevel";
            this.lblLevel.Size = new System.Drawing.Size(36, 13);
            this.lblLevel.TabIndex = 0;
            this.lblLevel.Text = "Level:";
            // 
            // lblCharacter
            // 
            this.lblCharacter.AutoSize = true;
            this.lblCharacter.Location = new System.Drawing.Point(12, 35);
            this.lblCharacter.Name = "lblCharacter";
            this.lblCharacter.Size = new System.Drawing.Size(56, 13);
            this.lblCharacter.TabIndex = 1;
            this.lblCharacter.Text = "Character:";
            // 
            // characterBox
            // 
            this.characterBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.characterBox.FormattingEnabled = true;
            this.characterBox.Items.AddRange(new object[] {
            "Zeke",
            "Julie"});
            this.characterBox.Location = new System.Drawing.Point(103, 32);
            this.characterBox.Name = "characterBox";
            this.characterBox.Size = new System.Drawing.Size(122, 21);
            this.characterBox.TabIndex = 3;
            // 
            // recordingPanel
            // 
            this.recordingPanel.Controls.Add(this.cancelRecord);
            this.recordingPanel.Controls.Add(this.recordingLabel);
            this.recordingPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.recordingPanel.Location = new System.Drawing.Point(0, 0);
            this.recordingPanel.Name = "recordingPanel";
            this.recordingPanel.Size = new System.Drawing.Size(237, 89);
            this.recordingPanel.TabIndex = 7;
            // 
            // cancelRecord
            // 
            this.cancelRecord.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelRecord.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelRecord.Location = new System.Drawing.Point(150, 54);
            this.cancelRecord.Name = "cancelRecord";
            this.cancelRecord.Size = new System.Drawing.Size(75, 23);
            this.cancelRecord.TabIndex = 6;
            this.cancelRecord.Text = "Cancel";
            this.cancelRecord.UseVisualStyleBackColor = true;
            this.cancelRecord.Click += new System.EventHandler(this.CloseButton_click);
            // 
            // recordingLabel
            // 
            this.recordingLabel.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.recordingLabel.AutoSize = true;
            this.recordingLabel.Location = new System.Drawing.Point(86, 22);
            this.recordingLabel.Name = "recordingLabel";
            this.recordingLabel.Size = new System.Drawing.Size(65, 13);
            this.recordingLabel.TabIndex = 0;
            this.recordingLabel.Text = "Recording...";
            // 
            // savePanel
            // 
            this.savePanel.Controls.Add(this.demoLengthLabel);
            this.savePanel.Controls.Add(this.slot3);
            this.savePanel.Controls.Add(this.slot2);
            this.savePanel.Controls.Add(this.slot1);
            this.savePanel.Controls.Add(this.slot0);
            this.savePanel.Controls.Add(this.demoLengthBox);
            this.savePanel.Controls.Add(this.levelBox);
            this.savePanel.Controls.Add(this.saveButton);
            this.savePanel.Controls.Add(this.characterBox);
            this.savePanel.Controls.Add(this.lblCharacter);
            this.savePanel.Controls.Add(this.cancelSave);
            this.savePanel.Controls.Add(this.lblLevel);
            this.savePanel.Controls.Add(this.slotLabel);
            this.savePanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.savePanel.Location = new System.Drawing.Point(0, 325);
            this.savePanel.Name = "savePanel";
            this.savePanel.Size = new System.Drawing.Size(237, 200);
            this.savePanel.TabIndex = 8;
            // 
            // slot3
            // 
            this.slot3.Appearance = System.Windows.Forms.Appearance.Button;
            this.slot3.Location = new System.Drawing.Point(163, 91);
            this.slot3.Name = "slot3";
            this.slot3.Size = new System.Drawing.Size(36, 29);
            this.slot3.TabIndex = 8;
            this.slot3.Tag = "";
            this.slot3.Text = "3";
            this.slot3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.slot3.UseVisualStyleBackColor = true;
            this.slot3.CheckedChanged += new System.EventHandler(this.slot_CheckedChanged);
            // 
            // slot2
            // 
            this.slot2.Appearance = System.Windows.Forms.Appearance.Button;
            this.slot2.Location = new System.Drawing.Point(121, 91);
            this.slot2.Name = "slot2";
            this.slot2.Size = new System.Drawing.Size(36, 29);
            this.slot2.TabIndex = 8;
            this.slot2.Tag = "";
            this.slot2.Text = "2";
            this.slot2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.slot2.UseVisualStyleBackColor = true;
            this.slot2.CheckedChanged += new System.EventHandler(this.slot_CheckedChanged);
            // 
            // slot1
            // 
            this.slot1.Appearance = System.Windows.Forms.Appearance.Button;
            this.slot1.Location = new System.Drawing.Point(79, 91);
            this.slot1.Name = "slot1";
            this.slot1.Size = new System.Drawing.Size(36, 29);
            this.slot1.TabIndex = 8;
            this.slot1.Tag = "";
            this.slot1.Text = "1";
            this.slot1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.slot1.UseVisualStyleBackColor = true;
            this.slot1.CheckedChanged += new System.EventHandler(this.slot_CheckedChanged);
            // 
            // slot0
            // 
            this.slot0.Appearance = System.Windows.Forms.Appearance.Button;
            this.slot0.Location = new System.Drawing.Point(37, 91);
            this.slot0.Name = "slot0";
            this.slot0.Size = new System.Drawing.Size(36, 29);
            this.slot0.TabIndex = 8;
            this.slot0.Tag = "";
            this.slot0.Text = "0";
            this.slot0.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.slot0.UseVisualStyleBackColor = true;
            this.slot0.CheckedChanged += new System.EventHandler(this.slot_CheckedChanged);
            // 
            // levelBox
            // 
            this.levelBox.AutoSize = true;
            this.levelBox.Location = new System.Drawing.Point(100, 9);
            this.levelBox.Name = "levelBox";
            this.levelBox.Size = new System.Drawing.Size(13, 13);
            this.levelBox.TabIndex = 6;
            this.levelBox.Text = "0";
            // 
            // saveButton
            // 
            this.saveButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.saveButton.Enabled = false;
            this.saveButton.Location = new System.Drawing.Point(150, 165);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(75, 23);
            this.saveButton.TabIndex = 6;
            this.saveButton.Text = "Save";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // cancelSave
            // 
            this.cancelSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelSave.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelSave.Location = new System.Drawing.Point(69, 165);
            this.cancelSave.Name = "cancelSave";
            this.cancelSave.Size = new System.Drawing.Size(75, 23);
            this.cancelSave.TabIndex = 7;
            this.cancelSave.Text = "Cancel";
            this.cancelSave.UseVisualStyleBackColor = true;
            this.cancelSave.Click += new System.EventHandler(this.CloseButton_click);
            // 
            // slotLabel
            // 
            this.slotLabel.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.slotLabel.Location = new System.Drawing.Point(12, 124);
            this.slotLabel.Name = "slotLabel";
            this.slotLabel.Size = new System.Drawing.Size(213, 35);
            this.slotLabel.TabIndex = 1;
            this.slotLabel.Text = "Select slot.";
            this.slotLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // findSRAMPanel
            // 
            this.findSRAMPanel.Controls.Add(this.browseButton);
            this.findSRAMPanel.Controls.Add(this.saveDataNotFound);
            this.findSRAMPanel.Controls.Add(this.button1);
            this.findSRAMPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.findSRAMPanel.Location = new System.Drawing.Point(0, 89);
            this.findSRAMPanel.Name = "findSRAMPanel";
            this.findSRAMPanel.Size = new System.Drawing.Size(237, 149);
            this.findSRAMPanel.TabIndex = 9;
            // 
            // browseButton
            // 
            this.browseButton.Location = new System.Drawing.Point(69, 84);
            this.browseButton.Name = "browseButton";
            this.browseButton.Size = new System.Drawing.Size(98, 23);
            this.browseButton.TabIndex = 9;
            this.browseButton.Text = "Browse...";
            this.browseButton.UseVisualStyleBackColor = true;
            this.browseButton.Click += new System.EventHandler(this.browseButton_Click);
            // 
            // saveDataNotFound
            // 
            this.saveDataNotFound.Location = new System.Drawing.Point(12, 8);
            this.saveDataNotFound.Name = "saveDataNotFound";
            this.saveDataNotFound.Size = new System.Drawing.Size(213, 73);
            this.saveDataNotFound.TabIndex = 8;
            this.saveDataNotFound.Text = "Emulator data could not be found automatically. Click the browse button and selec" +
    "t the save game file for \"recordDemo\". For Snes9x, this will be recordDemo.srm i" +
    "n the Saves folder. ";
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button1.Location = new System.Drawing.Point(150, 114);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 7;
            this.button1.Text = "Cancel";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.CloseButton_click);
            // 
            // endedEarlyPanel
            // 
            this.endedEarlyPanel.Controls.Add(this.okButton);
            this.endedEarlyPanel.Controls.Add(this.endedEarlyLabel);
            this.endedEarlyPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.endedEarlyPanel.Location = new System.Drawing.Point(0, 238);
            this.endedEarlyPanel.Name = "endedEarlyPanel";
            this.endedEarlyPanel.Size = new System.Drawing.Size(237, 87);
            this.endedEarlyPanel.TabIndex = 10;
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.okButton.Location = new System.Drawing.Point(150, 52);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 10;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.CloseButton_click);
            // 
            // endedEarlyLabel
            // 
            this.endedEarlyLabel.Location = new System.Drawing.Point(12, 10);
            this.endedEarlyLabel.Name = "endedEarlyLabel";
            this.endedEarlyLabel.Size = new System.Drawing.Size(213, 35);
            this.endedEarlyLabel.TabIndex = 9;
            this.endedEarlyLabel.Text = "Emulator was closed before recording finished, so recording cannot be saved.";
            // 
            // openSRAM
            // 
            this.openSRAM.Filter = "SRAM Files (*.srm)|*.srm|All Files (*.*)|*.*";
            // 
            // demoLengthLabel
            // 
            this.demoLengthLabel.AutoSize = true;
            this.demoLengthLabel.Location = new System.Drawing.Point(12, 62);
            this.demoLengthLabel.Name = "demoLengthLabel";
            this.demoLengthLabel.Size = new System.Drawing.Size(74, 13);
            this.demoLengthLabel.TabIndex = 9;
            this.demoLengthLabel.Text = "Demo Length:";
            // 
            // demoLengthBox
            // 
            this.demoLengthBox.AutoSize = true;
            this.demoLengthBox.Location = new System.Drawing.Point(100, 62);
            this.demoLengthBox.Name = "demoLengthBox";
            this.demoLengthBox.Size = new System.Drawing.Size(13, 13);
            this.demoLengthBox.TabIndex = 6;
            this.demoLengthBox.Text = "0";
            // 
            // RecordDemoDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(237, 525);
            this.Controls.Add(this.recordingPanel);
            this.Controls.Add(this.findSRAMPanel);
            this.Controls.Add(this.endedEarlyPanel);
            this.Controls.Add(this.savePanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RecordDemoDialog";
            this.Text = "Record Demo";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.RecordDemoDialog_FormClosing);
            this.recordingPanel.ResumeLayout(false);
            this.recordingPanel.PerformLayout();
            this.savePanel.ResumeLayout(false);
            this.savePanel.PerformLayout();
            this.findSRAMPanel.ResumeLayout(false);
            this.endedEarlyPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblLevel;
        private System.Windows.Forms.Label lblCharacter;
        private System.Windows.Forms.ComboBox characterBox;
        private System.Windows.Forms.Panel recordingPanel;
        private System.Windows.Forms.Label recordingLabel;
        private System.Windows.Forms.Panel savePanel;
        private System.Windows.Forms.Label slotLabel;
        private System.Windows.Forms.Button cancelRecord;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.Button cancelSave;
        private System.Windows.Forms.Label levelBox;
        private System.Windows.Forms.RadioButton slot3;
        private System.Windows.Forms.RadioButton slot2;
        private System.Windows.Forms.RadioButton slot1;
        private System.Windows.Forms.RadioButton slot0;
        private System.Windows.Forms.Panel findSRAMPanel;
        private System.Windows.Forms.Button browseButton;
        private System.Windows.Forms.Label saveDataNotFound;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Panel endedEarlyPanel;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Label endedEarlyLabel;
        private System.Windows.Forms.OpenFileDialog openSRAM;
        private System.Windows.Forms.Label demoLengthLabel;
        private System.Windows.Forms.Label demoLengthBox;
    }
}