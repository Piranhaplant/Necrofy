namespace Necrofy
{
    partial class LevelSettingsDialog
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
            this.tilesLabel = new System.Windows.Forms.Label();
            this.tilesSelector = new System.Windows.Forms.ComboBox();
            this.okButton = new System.Windows.Forms.Button();
            this.applyButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.tilesetPaletteSelector = new System.Windows.Forms.ComboBox();
            this.tilesetPaletteLabel = new System.Windows.Forms.Label();
            this.graphicsAuto = new System.Windows.Forms.CheckBox();
            this.tilesetGroup = new System.Windows.Forms.GroupBox();
            this.visibleEndSelector = new System.Windows.Forms.NumericUpDown();
            this.prioritySelector = new System.Windows.Forms.NumericUpDown();
            this.visibleEndAuto = new System.Windows.Forms.CheckBox();
            this.priorityAuto = new System.Windows.Forms.CheckBox();
            this.paletteAnimationSelector = new System.Windows.Forms.ComboBox();
            this.paletteAnimationLabel = new System.Windows.Forms.Label();
            this.visibleEndLabel = new System.Windows.Forms.Label();
            this.collisionAuto = new System.Windows.Forms.CheckBox();
            this.collisionSelector = new System.Windows.Forms.ComboBox();
            this.priorityLabel = new System.Windows.Forms.Label();
            this.collisionLabel = new System.Windows.Forms.Label();
            this.graphicsSelector = new System.Windows.Forms.ComboBox();
            this.graphicsLabel = new System.Windows.Forms.Label();
            this.audioGroup = new System.Windows.Forms.GroupBox();
            this.soundsSelector = new System.Windows.Forms.ComboBox();
            this.soundsLabel = new System.Windows.Forms.Label();
            this.musicSelector = new System.Windows.Forms.ComboBox();
            this.musicLabel = new System.Windows.Forms.Label();
            this.spritePaletteLabel = new System.Windows.Forms.Label();
            this.spritesGroup = new System.Windows.Forms.GroupBox();
            this.spritePaletteSelector = new System.Windows.Forms.ComboBox();
            this.spritePaletteAuto = new System.Windows.Forms.CheckBox();
            this.tilesetGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.visibleEndSelector)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.prioritySelector)).BeginInit();
            this.audioGroup.SuspendLayout();
            this.spritesGroup.SuspendLayout();
            this.SuspendLayout();
            // 
            // tilesLabel
            // 
            this.tilesLabel.AutoSize = true;
            this.tilesLabel.Location = new System.Drawing.Point(16, 24);
            this.tilesLabel.Name = "tilesLabel";
            this.tilesLabel.Size = new System.Drawing.Size(32, 13);
            this.tilesLabel.TabIndex = 0;
            this.tilesLabel.Text = "Tiles:";
            // 
            // tilesSelector
            // 
            this.tilesSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.tilesSelector.FormattingEnabled = true;
            this.tilesSelector.Location = new System.Drawing.Point(116, 21);
            this.tilesSelector.Name = "tilesSelector";
            this.tilesSelector.Size = new System.Drawing.Size(165, 21);
            this.tilesSelector.TabIndex = 1;
            this.tilesSelector.SelectedIndexChanged += new System.EventHandler(this.tilesSelector_SelectedIndexChanged);
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.Location = new System.Drawing.Point(581, 462);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 2;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // applyButton
            // 
            this.applyButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.applyButton.Location = new System.Drawing.Point(500, 462);
            this.applyButton.Name = "applyButton";
            this.applyButton.Size = new System.Drawing.Size(75, 23);
            this.applyButton.TabIndex = 3;
            this.applyButton.Text = "Apply";
            this.applyButton.UseVisualStyleBackColor = true;
            this.applyButton.Click += new System.EventHandler(this.applyButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(419, 462);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 4;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // tilesetPaletteSelector
            // 
            this.tilesetPaletteSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.tilesetPaletteSelector.FormattingEnabled = true;
            this.tilesetPaletteSelector.Location = new System.Drawing.Point(116, 48);
            this.tilesetPaletteSelector.Name = "tilesetPaletteSelector";
            this.tilesetPaletteSelector.Size = new System.Drawing.Size(165, 21);
            this.tilesetPaletteSelector.TabIndex = 6;
            // 
            // tilesetPaletteLabel
            // 
            this.tilesetPaletteLabel.AutoSize = true;
            this.tilesetPaletteLabel.Location = new System.Drawing.Point(16, 51);
            this.tilesetPaletteLabel.Name = "tilesetPaletteLabel";
            this.tilesetPaletteLabel.Size = new System.Drawing.Size(43, 13);
            this.tilesetPaletteLabel.TabIndex = 5;
            this.tilesetPaletteLabel.Text = "Palette:";
            // 
            // graphicsAuto
            // 
            this.graphicsAuto.AutoSize = true;
            this.graphicsAuto.Location = new System.Drawing.Point(116, 104);
            this.graphicsAuto.Name = "graphicsAuto";
            this.graphicsAuto.Size = new System.Drawing.Size(48, 17);
            this.graphicsAuto.TabIndex = 7;
            this.graphicsAuto.Text = "Auto";
            this.graphicsAuto.UseVisualStyleBackColor = true;
            // 
            // tilesetGroup
            // 
            this.tilesetGroup.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tilesetGroup.Controls.Add(this.visibleEndSelector);
            this.tilesetGroup.Controls.Add(this.prioritySelector);
            this.tilesetGroup.Controls.Add(this.visibleEndAuto);
            this.tilesetGroup.Controls.Add(this.priorityAuto);
            this.tilesetGroup.Controls.Add(this.paletteAnimationSelector);
            this.tilesetGroup.Controls.Add(this.paletteAnimationLabel);
            this.tilesetGroup.Controls.Add(this.visibleEndLabel);
            this.tilesetGroup.Controls.Add(this.collisionAuto);
            this.tilesetGroup.Controls.Add(this.collisionSelector);
            this.tilesetGroup.Controls.Add(this.priorityLabel);
            this.tilesetGroup.Controls.Add(this.collisionLabel);
            this.tilesetGroup.Controls.Add(this.graphicsSelector);
            this.tilesetGroup.Controls.Add(this.graphicsAuto);
            this.tilesetGroup.Controls.Add(this.tilesSelector);
            this.tilesetGroup.Controls.Add(this.tilesLabel);
            this.tilesetGroup.Controls.Add(this.graphicsLabel);
            this.tilesetGroup.Controls.Add(this.tilesetPaletteSelector);
            this.tilesetGroup.Controls.Add(this.tilesetPaletteLabel);
            this.tilesetGroup.Location = new System.Drawing.Point(12, 12);
            this.tilesetGroup.Name = "tilesetGroup";
            this.tilesetGroup.Size = new System.Drawing.Size(296, 220);
            this.tilesetGroup.TabIndex = 8;
            this.tilesetGroup.TabStop = false;
            this.tilesetGroup.Text = "Tileset";
            // 
            // visibleEndSelector
            // 
            this.visibleEndSelector.Hexadecimal = true;
            this.visibleEndSelector.Location = new System.Drawing.Point(170, 184);
            this.visibleEndSelector.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.visibleEndSelector.Name = "visibleEndSelector";
            this.visibleEndSelector.Size = new System.Drawing.Size(111, 20);
            this.visibleEndSelector.TabIndex = 25;
            // 
            // prioritySelector
            // 
            this.prioritySelector.Hexadecimal = true;
            this.prioritySelector.Location = new System.Drawing.Point(170, 157);
            this.prioritySelector.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.prioritySelector.Name = "prioritySelector";
            this.prioritySelector.Size = new System.Drawing.Size(111, 20);
            this.prioritySelector.TabIndex = 16;
            // 
            // visibleEndAuto
            // 
            this.visibleEndAuto.AutoSize = true;
            this.visibleEndAuto.Location = new System.Drawing.Point(116, 185);
            this.visibleEndAuto.Name = "visibleEndAuto";
            this.visibleEndAuto.Size = new System.Drawing.Size(48, 17);
            this.visibleEndAuto.TabIndex = 24;
            this.visibleEndAuto.Text = "Auto";
            this.visibleEndAuto.UseVisualStyleBackColor = true;
            // 
            // priorityAuto
            // 
            this.priorityAuto.AutoSize = true;
            this.priorityAuto.Location = new System.Drawing.Point(116, 158);
            this.priorityAuto.Name = "priorityAuto";
            this.priorityAuto.Size = new System.Drawing.Size(48, 17);
            this.priorityAuto.TabIndex = 15;
            this.priorityAuto.Text = "Auto";
            this.priorityAuto.UseVisualStyleBackColor = true;
            // 
            // paletteAnimationSelector
            // 
            this.paletteAnimationSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.paletteAnimationSelector.FormattingEnabled = true;
            this.paletteAnimationSelector.Location = new System.Drawing.Point(116, 75);
            this.paletteAnimationSelector.Name = "paletteAnimationSelector";
            this.paletteAnimationSelector.Size = new System.Drawing.Size(165, 21);
            this.paletteAnimationSelector.TabIndex = 18;
            // 
            // paletteAnimationLabel
            // 
            this.paletteAnimationLabel.AutoSize = true;
            this.paletteAnimationLabel.Location = new System.Drawing.Point(16, 78);
            this.paletteAnimationLabel.Name = "paletteAnimationLabel";
            this.paletteAnimationLabel.Size = new System.Drawing.Size(92, 13);
            this.paletteAnimationLabel.TabIndex = 17;
            this.paletteAnimationLabel.Text = "Palette Animation:";
            // 
            // visibleEndLabel
            // 
            this.visibleEndLabel.AutoSize = true;
            this.visibleEndLabel.Location = new System.Drawing.Point(16, 186);
            this.visibleEndLabel.Name = "visibleEndLabel";
            this.visibleEndLabel.Size = new System.Drawing.Size(87, 13);
            this.visibleEndLabel.TabIndex = 23;
            this.visibleEndLabel.Text = "Visible Tiles End:";
            // 
            // collisionAuto
            // 
            this.collisionAuto.AutoSize = true;
            this.collisionAuto.Location = new System.Drawing.Point(116, 131);
            this.collisionAuto.Name = "collisionAuto";
            this.collisionAuto.Size = new System.Drawing.Size(48, 17);
            this.collisionAuto.TabIndex = 13;
            this.collisionAuto.Text = "Auto";
            this.collisionAuto.UseVisualStyleBackColor = true;
            // 
            // collisionSelector
            // 
            this.collisionSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.collisionSelector.FormattingEnabled = true;
            this.collisionSelector.Location = new System.Drawing.Point(170, 129);
            this.collisionSelector.Name = "collisionSelector";
            this.collisionSelector.Size = new System.Drawing.Size(111, 21);
            this.collisionSelector.TabIndex = 12;
            // 
            // priorityLabel
            // 
            this.priorityLabel.AutoSize = true;
            this.priorityLabel.Location = new System.Drawing.Point(16, 159);
            this.priorityLabel.Name = "priorityLabel";
            this.priorityLabel.Size = new System.Drawing.Size(92, 13);
            this.priorityLabel.TabIndex = 11;
            this.priorityLabel.Text = "Priority Tile Count:";
            // 
            // collisionLabel
            // 
            this.collisionLabel.AutoSize = true;
            this.collisionLabel.Location = new System.Drawing.Point(16, 132);
            this.collisionLabel.Name = "collisionLabel";
            this.collisionLabel.Size = new System.Drawing.Size(48, 13);
            this.collisionLabel.TabIndex = 10;
            this.collisionLabel.Text = "Collision:";
            // 
            // graphicsSelector
            // 
            this.graphicsSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.graphicsSelector.FormattingEnabled = true;
            this.graphicsSelector.Location = new System.Drawing.Point(170, 102);
            this.graphicsSelector.Name = "graphicsSelector";
            this.graphicsSelector.Size = new System.Drawing.Size(111, 21);
            this.graphicsSelector.TabIndex = 9;
            // 
            // graphicsLabel
            // 
            this.graphicsLabel.AutoSize = true;
            this.graphicsLabel.Location = new System.Drawing.Point(16, 105);
            this.graphicsLabel.Name = "graphicsLabel";
            this.graphicsLabel.Size = new System.Drawing.Size(52, 13);
            this.graphicsLabel.TabIndex = 8;
            this.graphicsLabel.Text = "Graphics:";
            // 
            // audioGroup
            // 
            this.audioGroup.Controls.Add(this.soundsSelector);
            this.audioGroup.Controls.Add(this.soundsLabel);
            this.audioGroup.Controls.Add(this.musicSelector);
            this.audioGroup.Controls.Add(this.musicLabel);
            this.audioGroup.Location = new System.Drawing.Point(12, 302);
            this.audioGroup.Name = "audioGroup";
            this.audioGroup.Size = new System.Drawing.Size(296, 85);
            this.audioGroup.TabIndex = 9;
            this.audioGroup.TabStop = false;
            this.audioGroup.Text = "Audio";
            // 
            // soundsSelector
            // 
            this.soundsSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.soundsSelector.FormattingEnabled = true;
            this.soundsSelector.Location = new System.Drawing.Point(116, 48);
            this.soundsSelector.Name = "soundsSelector";
            this.soundsSelector.Size = new System.Drawing.Size(165, 21);
            this.soundsSelector.TabIndex = 22;
            // 
            // soundsLabel
            // 
            this.soundsLabel.AutoSize = true;
            this.soundsLabel.Location = new System.Drawing.Point(16, 51);
            this.soundsLabel.Name = "soundsLabel";
            this.soundsLabel.Size = new System.Drawing.Size(73, 13);
            this.soundsLabel.TabIndex = 21;
            this.soundsLabel.Text = "Extra Sounds:";
            // 
            // musicSelector
            // 
            this.musicSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.musicSelector.FormattingEnabled = true;
            this.musicSelector.Location = new System.Drawing.Point(116, 21);
            this.musicSelector.Name = "musicSelector";
            this.musicSelector.Size = new System.Drawing.Size(165, 21);
            this.musicSelector.TabIndex = 20;
            // 
            // musicLabel
            // 
            this.musicLabel.AutoSize = true;
            this.musicLabel.Location = new System.Drawing.Point(16, 24);
            this.musicLabel.Name = "musicLabel";
            this.musicLabel.Size = new System.Drawing.Size(38, 13);
            this.musicLabel.TabIndex = 19;
            this.musicLabel.Text = "Music:";
            // 
            // spritePaletteLabel
            // 
            this.spritePaletteLabel.AutoSize = true;
            this.spritePaletteLabel.Location = new System.Drawing.Point(16, 24);
            this.spritePaletteLabel.Name = "spritePaletteLabel";
            this.spritePaletteLabel.Size = new System.Drawing.Size(43, 13);
            this.spritePaletteLabel.TabIndex = 10;
            this.spritePaletteLabel.Text = "Palette:";
            // 
            // spritesGroup
            // 
            this.spritesGroup.Controls.Add(this.spritePaletteSelector);
            this.spritesGroup.Controls.Add(this.spritePaletteAuto);
            this.spritesGroup.Controls.Add(this.spritePaletteLabel);
            this.spritesGroup.Location = new System.Drawing.Point(12, 238);
            this.spritesGroup.Name = "spritesGroup";
            this.spritesGroup.Size = new System.Drawing.Size(296, 58);
            this.spritesGroup.TabIndex = 10;
            this.spritesGroup.TabStop = false;
            this.spritesGroup.Text = "Sprites";
            // 
            // spritePaletteSelector
            // 
            this.spritePaletteSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.spritePaletteSelector.FormattingEnabled = true;
            this.spritePaletteSelector.Location = new System.Drawing.Point(170, 21);
            this.spritePaletteSelector.Name = "spritePaletteSelector";
            this.spritePaletteSelector.Size = new System.Drawing.Size(111, 21);
            this.spritePaletteSelector.TabIndex = 27;
            // 
            // spritePaletteAuto
            // 
            this.spritePaletteAuto.AutoSize = true;
            this.spritePaletteAuto.Location = new System.Drawing.Point(116, 23);
            this.spritePaletteAuto.Name = "spritePaletteAuto";
            this.spritePaletteAuto.Size = new System.Drawing.Size(48, 17);
            this.spritePaletteAuto.TabIndex = 26;
            this.spritePaletteAuto.Text = "Auto";
            this.spritePaletteAuto.UseVisualStyleBackColor = true;
            // 
            // LevelSettingsDialog
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(668, 497);
            this.Controls.Add(this.spritesGroup);
            this.Controls.Add(this.audioGroup);
            this.Controls.Add(this.tilesetGroup);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.applyButton);
            this.Controls.Add(this.okButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LevelSettingsDialog";
            this.Text = "Level Settings";
            this.tilesetGroup.ResumeLayout(false);
            this.tilesetGroup.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.visibleEndSelector)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.prioritySelector)).EndInit();
            this.audioGroup.ResumeLayout(false);
            this.audioGroup.PerformLayout();
            this.spritesGroup.ResumeLayout(false);
            this.spritesGroup.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label tilesLabel;
        private System.Windows.Forms.ComboBox tilesSelector;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button applyButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.ComboBox tilesetPaletteSelector;
        private System.Windows.Forms.Label tilesetPaletteLabel;
        private System.Windows.Forms.CheckBox graphicsAuto;
        private System.Windows.Forms.GroupBox tilesetGroup;
        private System.Windows.Forms.Label graphicsLabel;
        private System.Windows.Forms.CheckBox collisionAuto;
        private System.Windows.Forms.ComboBox collisionSelector;
        private System.Windows.Forms.Label priorityLabel;
        private System.Windows.Forms.Label collisionLabel;
        private System.Windows.Forms.ComboBox graphicsSelector;
        private System.Windows.Forms.NumericUpDown prioritySelector;
        private System.Windows.Forms.CheckBox priorityAuto;
        private System.Windows.Forms.NumericUpDown visibleEndSelector;
        private System.Windows.Forms.CheckBox visibleEndAuto;
        private System.Windows.Forms.ComboBox paletteAnimationSelector;
        private System.Windows.Forms.Label paletteAnimationLabel;
        private System.Windows.Forms.Label visibleEndLabel;
        private System.Windows.Forms.GroupBox audioGroup;
        private System.Windows.Forms.ComboBox soundsSelector;
        private System.Windows.Forms.Label soundsLabel;
        private System.Windows.Forms.ComboBox musicSelector;
        private System.Windows.Forms.Label musicLabel;
        private System.Windows.Forms.Label spritePaletteLabel;
        private System.Windows.Forms.GroupBox spritesGroup;
        private System.Windows.Forms.ComboBox spritePaletteSelector;
        private System.Windows.Forms.CheckBox spritePaletteAuto;
    }
}