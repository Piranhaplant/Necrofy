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
            this.components = new System.ComponentModel.Container();
            this.tilesLabel = new System.Windows.Forms.Label();
            this.tilesSelector = new Necrofy.AssetComboBox();
            this.okButton = new System.Windows.Forms.Button();
            this.applyButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.tilesetPaletteSelector = new Necrofy.AssetComboBox();
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
            this.collisionSelector = new Necrofy.AssetComboBox();
            this.priorityLabel = new System.Windows.Forms.Label();
            this.collisionLabel = new System.Windows.Forms.Label();
            this.graphicsSelector = new Necrofy.AssetComboBox();
            this.graphicsLabel = new System.Windows.Forms.Label();
            this.audioGroup = new System.Windows.Forms.GroupBox();
            this.soundsSelector = new System.Windows.Forms.ComboBox();
            this.soundsLabel = new System.Windows.Forms.Label();
            this.musicSelector = new System.Windows.Forms.ComboBox();
            this.musicLabel = new System.Windows.Forms.Label();
            this.spritePaletteLabel = new System.Windows.Forms.Label();
            this.spritesGroup = new System.Windows.Forms.GroupBox();
            this.spritePaletteSelector = new Necrofy.AssetComboBox();
            this.bonusesGroup = new System.Windows.Forms.GroupBox();
            this.bonusLevelSelector = new System.Windows.Forms.NumericUpDown();
            this.secretBonusTypeSelector = new System.Windows.Forms.ComboBox();
            this.bonusLevelLabel = new System.Windows.Forms.Label();
            this.secretBonusTypeLabel = new System.Windows.Forms.Label();
            this.bonusList = new System.Windows.Forms.CheckedListBox();
            this.levelEffectsGroup = new System.Windows.Forms.GroupBox();
            this.levelMonsterList = new Necrofy.LevelMonsterList();
            this.addLevelEffect = new System.Windows.Forms.Button();
            this.addLevelEffectMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addPaletteFade = new System.Windows.Forms.ToolStripMenuItem();
            this.addTileAnimation = new System.Windows.Forms.ToolStripMenuItem();
            this.removeLevelEffect = new System.Windows.Forms.Button();
            this.tilesetGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.visibleEndSelector)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.prioritySelector)).BeginInit();
            this.audioGroup.SuspendLayout();
            this.spritesGroup.SuspendLayout();
            this.bonusesGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bonusLevelSelector)).BeginInit();
            this.levelEffectsGroup.SuspendLayout();
            this.addLevelEffectMenu.SuspendLayout();
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
            this.tilesSelector.SelectedItem = null;
            this.tilesSelector.SelectedName = null;
            this.tilesSelector.Size = new System.Drawing.Size(165, 21);
            this.tilesSelector.TabIndex = 1;
            this.tilesSelector.SelectedIndexChanged += new System.EventHandler(this.tilesSelector_SelectedIndexChanged);
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.Location = new System.Drawing.Point(423, 490);
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
            this.applyButton.Location = new System.Drawing.Point(342, 490);
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
            this.cancelButton.Location = new System.Drawing.Point(261, 490);
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
            this.tilesetPaletteSelector.SelectedItem = null;
            this.tilesetPaletteSelector.SelectedName = null;
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
            this.collisionSelector.SelectedItem = null;
            this.collisionSelector.SelectedName = null;
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
            this.graphicsSelector.SelectedItem = null;
            this.graphicsSelector.SelectedName = null;
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
            this.spritePaletteSelector.Location = new System.Drawing.Point(116, 21);
            this.spritePaletteSelector.Name = "spritePaletteSelector";
            this.spritePaletteSelector.SelectedItem = null;
            this.spritePaletteSelector.SelectedName = null;
            this.spritePaletteSelector.Size = new System.Drawing.Size(165, 21);
            this.spritePaletteSelector.TabIndex = 27;
            // 
            // bonusesGroup
            // 
            this.bonusesGroup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.bonusesGroup.Controls.Add(this.bonusLevelSelector);
            this.bonusesGroup.Controls.Add(this.secretBonusTypeSelector);
            this.bonusesGroup.Controls.Add(this.bonusLevelLabel);
            this.bonusesGroup.Controls.Add(this.secretBonusTypeLabel);
            this.bonusesGroup.Controls.Add(this.bonusList);
            this.bonusesGroup.Location = new System.Drawing.Point(314, 12);
            this.bonusesGroup.Name = "bonusesGroup";
            this.bonusesGroup.Size = new System.Drawing.Size(183, 375);
            this.bonusesGroup.TabIndex = 11;
            this.bonusesGroup.TabStop = false;
            this.bonusesGroup.Text = "Bonuses";
            // 
            // bonusLevelSelector
            // 
            this.bonusLevelSelector.Location = new System.Drawing.Point(81, 344);
            this.bonusLevelSelector.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.bonusLevelSelector.Name = "bonusLevelSelector";
            this.bonusLevelSelector.Size = new System.Drawing.Size(90, 20);
            this.bonusLevelSelector.TabIndex = 23;
            // 
            // secretBonusTypeSelector
            // 
            this.secretBonusTypeSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.secretBonusTypeSelector.FormattingEnabled = true;
            this.secretBonusTypeSelector.Location = new System.Drawing.Point(6, 317);
            this.secretBonusTypeSelector.Name = "secretBonusTypeSelector";
            this.secretBonusTypeSelector.Size = new System.Drawing.Size(165, 21);
            this.secretBonusTypeSelector.TabIndex = 22;
            this.secretBonusTypeSelector.SelectedIndexChanged += new System.EventHandler(this.secretBonusTypeSelector_SelectedIndexChanged);
            // 
            // bonusLevelLabel
            // 
            this.bonusLevelLabel.AutoSize = true;
            this.bonusLevelLabel.Location = new System.Drawing.Point(6, 346);
            this.bonusLevelLabel.Name = "bonusLevelLabel";
            this.bonusLevelLabel.Size = new System.Drawing.Size(69, 13);
            this.bonusLevelLabel.TabIndex = 21;
            this.bonusLevelLabel.Text = "Bonus Level:";
            // 
            // secretBonusTypeLabel
            // 
            this.secretBonusTypeLabel.AutoSize = true;
            this.secretBonusTypeLabel.Location = new System.Drawing.Point(6, 301);
            this.secretBonusTypeLabel.Name = "secretBonusTypeLabel";
            this.secretBonusTypeLabel.Size = new System.Drawing.Size(101, 13);
            this.secretBonusTypeLabel.TabIndex = 20;
            this.secretBonusTypeLabel.Text = "Secret Bonus Type:";
            // 
            // bonusList
            // 
            this.bonusList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.bonusList.CheckOnClick = true;
            this.bonusList.FormattingEnabled = true;
            this.bonusList.Location = new System.Drawing.Point(6, 19);
            this.bonusList.Name = "bonusList";
            this.bonusList.Size = new System.Drawing.Size(171, 274);
            this.bonusList.TabIndex = 12;
            // 
            // levelEffectsGroup
            // 
            this.levelEffectsGroup.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.levelEffectsGroup.Controls.Add(this.levelMonsterList);
            this.levelEffectsGroup.Location = new System.Drawing.Point(12, 393);
            this.levelEffectsGroup.Name = "levelEffectsGroup";
            this.levelEffectsGroup.Size = new System.Drawing.Size(485, 90);
            this.levelEffectsGroup.TabIndex = 12;
            this.levelEffectsGroup.TabStop = false;
            this.levelEffectsGroup.Text = "Level Effects";
            // 
            // levelMonsterList
            // 
            this.levelMonsterList.AutoScroll = true;
            this.levelMonsterList.Location = new System.Drawing.Point(6, 19);
            this.levelMonsterList.Name = "levelMonsterList";
            this.levelMonsterList.Size = new System.Drawing.Size(473, 65);
            this.levelMonsterList.TabIndex = 33;
            this.levelMonsterList.SelectedRowChanged += new System.EventHandler(this.levelMonsterList_SelectedRowChanged);
            // 
            // addLevelEffect
            // 
            this.addLevelEffect.Image = global::Necrofy.Properties.Resources.plus;
            this.addLevelEffect.Location = new System.Drawing.Point(91, 387);
            this.addLevelEffect.Name = "addLevelEffect";
            this.addLevelEffect.Size = new System.Drawing.Size(23, 23);
            this.addLevelEffect.TabIndex = 33;
            this.addLevelEffect.UseVisualStyleBackColor = true;
            this.addLevelEffect.Click += new System.EventHandler(this.addLevelEffect_Click);
            // 
            // addLevelEffectMenu
            // 
            this.addLevelEffectMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addPaletteFade,
            this.addTileAnimation});
            this.addLevelEffectMenu.Name = "addLevelEffectMenu";
            this.addLevelEffectMenu.Size = new System.Drawing.Size(152, 48);
            // 
            // addPaletteFade
            // 
            this.addPaletteFade.Name = "addPaletteFade";
            this.addPaletteFade.Size = new System.Drawing.Size(151, 22);
            this.addPaletteFade.Text = "Palette Fade";
            this.addPaletteFade.Click += new System.EventHandler(this.addPaletteFade_Click);
            // 
            // addTileAnimation
            // 
            this.addTileAnimation.Name = "addTileAnimation";
            this.addTileAnimation.Size = new System.Drawing.Size(151, 22);
            this.addTileAnimation.Text = "Tile Animation";
            this.addTileAnimation.Click += new System.EventHandler(this.addTileAnimation_Click);
            // 
            // removeLevelEffect
            // 
            this.removeLevelEffect.Enabled = false;
            this.removeLevelEffect.Image = global::Necrofy.Properties.Resources.minus;
            this.removeLevelEffect.Location = new System.Drawing.Point(120, 387);
            this.removeLevelEffect.Name = "removeLevelEffect";
            this.removeLevelEffect.Size = new System.Drawing.Size(23, 23);
            this.removeLevelEffect.TabIndex = 34;
            this.removeLevelEffect.UseVisualStyleBackColor = true;
            this.removeLevelEffect.Click += new System.EventHandler(this.removeLevelEffect_Click);
            // 
            // LevelSettingsDialog
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(510, 525);
            this.Controls.Add(this.removeLevelEffect);
            this.Controls.Add(this.addLevelEffect);
            this.Controls.Add(this.levelEffectsGroup);
            this.Controls.Add(this.bonusesGroup);
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
            this.bonusesGroup.ResumeLayout(false);
            this.bonusesGroup.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bonusLevelSelector)).EndInit();
            this.levelEffectsGroup.ResumeLayout(false);
            this.addLevelEffectMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label tilesLabel;
        private AssetComboBox tilesSelector;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button applyButton;
        private System.Windows.Forms.Button cancelButton;
        private AssetComboBox tilesetPaletteSelector;
        private System.Windows.Forms.Label tilesetPaletteLabel;
        private System.Windows.Forms.CheckBox graphicsAuto;
        private System.Windows.Forms.GroupBox tilesetGroup;
        private System.Windows.Forms.Label graphicsLabel;
        private System.Windows.Forms.CheckBox collisionAuto;
        private AssetComboBox collisionSelector;
        private System.Windows.Forms.Label priorityLabel;
        private System.Windows.Forms.Label collisionLabel;
        private AssetComboBox graphicsSelector;
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
        private AssetComboBox spritePaletteSelector;
        private System.Windows.Forms.GroupBox bonusesGroup;
        private System.Windows.Forms.CheckedListBox bonusList;
        private System.Windows.Forms.GroupBox levelEffectsGroup;
        private LevelMonsterList levelMonsterList;
        private System.Windows.Forms.Button addLevelEffect;
        private System.Windows.Forms.ContextMenuStrip addLevelEffectMenu;
        private System.Windows.Forms.ToolStripMenuItem addPaletteFade;
        private System.Windows.Forms.ToolStripMenuItem addTileAnimation;
        private System.Windows.Forms.Button removeLevelEffect;
        private System.Windows.Forms.NumericUpDown bonusLevelSelector;
        private System.Windows.Forms.ComboBox secretBonusTypeSelector;
        private System.Windows.Forms.Label bonusLevelLabel;
        private System.Windows.Forms.Label secretBonusTypeLabel;
    }
}