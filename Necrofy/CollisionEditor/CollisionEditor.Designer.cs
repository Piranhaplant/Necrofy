namespace Necrofy
{
    partial class CollisionEditor
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tilePicker = new Necrofy.SpriteTilePicker();
            this.paletteSelector = new Necrofy.AssetSelector();
            this.graphicsSelector = new Necrofy.AssetSelector();
            this.tilemapSelector = new Necrofy.AssetSelector();
            this.panel1 = new System.Windows.Forms.Panel();
            this.presetList = new System.Windows.Forms.ListBox();
            this.tilesetPreviewCollision = new Necrofy.Canvas();
            this.checkBoxF = new Necrofy.TwoColorCheckbox();
            this.presetLabel = new System.Windows.Forms.Label();
            this.checkBoxP = new Necrofy.TwoColorCheckbox();
            this.checkBoxO = new System.Windows.Forms.CheckBox();
            this.checkBoxN = new System.Windows.Forms.CheckBox();
            this.checkBoxM = new System.Windows.Forms.CheckBox();
            this.checkBoxL = new System.Windows.Forms.CheckBox();
            this.checkBoxK = new Necrofy.TwoColorCheckbox();
            this.checkBoxJ = new Necrofy.TwoColorCheckbox();
            this.tilesetPreviewLabel = new System.Windows.Forms.Label();
            this.tilesetPreview = new Necrofy.Canvas();
            this.checkBoxI = new Necrofy.TwoColorCheckbox();
            this.checkBoxH = new System.Windows.Forms.CheckBox();
            this.checkBoxG = new System.Windows.Forms.CheckBox();
            this.checkBoxE = new System.Windows.Forms.CheckBox();
            this.checkBoxD = new System.Windows.Forms.CheckBox();
            this.checkBoxC = new System.Windows.Forms.CheckBox();
            this.checkBoxB = new System.Windows.Forms.CheckBox();
            this.checkBoxA = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tilePicker);
            this.splitContainer1.Panel1.Controls.Add(this.paletteSelector);
            this.splitContainer1.Panel1.Controls.Add(this.graphicsSelector);
            this.splitContainer1.Panel1.Controls.Add(this.tilemapSelector);
            this.splitContainer1.Panel1MinSize = 273;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.AutoScroll = true;
            this.splitContainer1.Panel2.Controls.Add(this.panel1);
            this.splitContainer1.Size = new System.Drawing.Size(1616, 839);
            this.splitContainer1.SplitterDistance = 273;
            this.splitContainer1.SplitterIncrement = 16;
            this.splitContainer1.TabIndex = 0;
            // 
            // tilePicker
            // 
            this.tilePicker.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tilePicker.FlipX = false;
            this.tilePicker.FlipY = false;
            this.tilePicker.Location = new System.Drawing.Point(0, 63);
            this.tilePicker.Name = "tilePicker";
            this.tilePicker.Palette = 0;
            this.tilePicker.SelectedTile = -1;
            this.tilePicker.Size = new System.Drawing.Size(273, 776);
            this.tilePicker.TabIndex = 0;
            this.tilePicker.SelectedTileChanged += new Necrofy.SpriteTilePicker.SelectedTileChangedDelegate(this.tilePicker_SelectedTileChanged);
            // 
            // paletteSelector
            // 
            this.paletteSelector.Dock = System.Windows.Forms.DockStyle.Top;
            this.paletteSelector.Location = new System.Drawing.Point(0, 42);
            this.paletteSelector.Name = "paletteSelector";
            this.paletteSelector.Size = new System.Drawing.Size(273, 21);
            this.paletteSelector.TabIndex = 2;
            this.paletteSelector.SelectedItemChanged += new System.EventHandler(this.assetSelector_SelectedItemChanged);
            // 
            // graphicsSelector
            // 
            this.graphicsSelector.Dock = System.Windows.Forms.DockStyle.Top;
            this.graphicsSelector.Location = new System.Drawing.Point(0, 21);
            this.graphicsSelector.Name = "graphicsSelector";
            this.graphicsSelector.Size = new System.Drawing.Size(273, 21);
            this.graphicsSelector.TabIndex = 1;
            this.graphicsSelector.SelectedItemChanged += new System.EventHandler(this.assetSelector_SelectedItemChanged);
            // 
            // tilemapSelector
            // 
            this.tilemapSelector.Dock = System.Windows.Forms.DockStyle.Top;
            this.tilemapSelector.Location = new System.Drawing.Point(0, 0);
            this.tilemapSelector.Name = "tilemapSelector";
            this.tilemapSelector.Size = new System.Drawing.Size(273, 21);
            this.tilemapSelector.TabIndex = 1;
            this.tilemapSelector.SelectedItemChanged += new System.EventHandler(this.assetSelector_SelectedItemChanged);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.presetList);
            this.panel1.Controls.Add(this.tilesetPreviewCollision);
            this.panel1.Controls.Add(this.checkBoxF);
            this.panel1.Controls.Add(this.presetLabel);
            this.panel1.Controls.Add(this.checkBoxP);
            this.panel1.Controls.Add(this.checkBoxO);
            this.panel1.Controls.Add(this.checkBoxN);
            this.panel1.Controls.Add(this.checkBoxM);
            this.panel1.Controls.Add(this.checkBoxL);
            this.panel1.Controls.Add(this.checkBoxK);
            this.panel1.Controls.Add(this.checkBoxJ);
            this.panel1.Controls.Add(this.tilesetPreviewLabel);
            this.panel1.Controls.Add(this.tilesetPreview);
            this.panel1.Controls.Add(this.checkBoxI);
            this.panel1.Controls.Add(this.checkBoxH);
            this.panel1.Controls.Add(this.checkBoxG);
            this.panel1.Controls.Add(this.checkBoxE);
            this.panel1.Controls.Add(this.checkBoxD);
            this.panel1.Controls.Add(this.checkBoxC);
            this.panel1.Controls.Add(this.checkBoxB);
            this.panel1.Controls.Add(this.checkBoxA);
            this.panel1.Location = new System.Drawing.Point(8, 7);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(729, 801);
            this.panel1.TabIndex = 0;
            // 
            // presetList
            // 
            this.presetList.FormattingEnabled = true;
            this.presetList.Location = new System.Drawing.Point(6, 427);
            this.presetList.Name = "presetList";
            this.presetList.Size = new System.Drawing.Size(318, 368);
            this.presetList.TabIndex = 21;
            this.presetList.SelectedIndexChanged += new System.EventHandler(this.presetList_SelectedIndexChanged);
            // 
            // tilesetPreviewCollision
            // 
            this.tilesetPreviewCollision.BackColor = System.Drawing.SystemColors.ControlDark;
            this.tilesetPreviewCollision.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tilesetPreviewCollision.IsMouseDown = false;
            this.tilesetPreviewCollision.Location = new System.Drawing.Point(337, 411);
            this.tilesetPreviewCollision.Name = "tilesetPreviewCollision";
            this.tilesetPreviewCollision.Size = new System.Drawing.Size(384, 384);
            this.tilesetPreviewCollision.TabIndex = 20;
            this.tilesetPreviewCollision.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tilesetPreview_MouseDown);
            this.tilesetPreviewCollision.MouseMove += new System.Windows.Forms.MouseEventHandler(this.tilesetPreview_MouseMove);
            this.tilesetPreviewCollision.Paint += new System.Windows.Forms.PaintEventHandler(this.tilesetPreviewCollision_Paint);
            // 
            // checkBoxF
            // 
            this.checkBoxF.ForeColor2 = System.Drawing.SystemColors.ControlText;
            this.checkBoxF.Location = new System.Drawing.Point(6, 136);
            this.checkBoxF.Name = "checkBoxF";
            this.checkBoxF.Size = new System.Drawing.Size(318, 17);
            this.checkBoxF.TabIndex = 5;
            this.checkBoxF.Text1 = "f. Drop items (if a), ";
            this.checkBoxF.Text2 = "right conveyor belt (if d)";
            this.checkBoxF.UseVisualStyleBackColor = true;
            this.checkBoxF.CheckStateChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // presetLabel
            // 
            this.presetLabel.AutoSize = true;
            this.presetLabel.Location = new System.Drawing.Point(3, 411);
            this.presetLabel.Name = "presetLabel";
            this.presetLabel.Size = new System.Drawing.Size(40, 13);
            this.presetLabel.TabIndex = 18;
            this.presetLabel.Text = "Preset:";
            // 
            // checkBoxP
            // 
            this.checkBoxP.ForeColor2 = System.Drawing.SystemColors.ControlText;
            this.checkBoxP.Location = new System.Drawing.Point(6, 366);
            this.checkBoxP.Name = "checkBoxP";
            this.checkBoxP.Size = new System.Drawing.Size(325, 17);
            this.checkBoxP.TabIndex = 17;
            this.checkBoxP.Text1 = "p. Boss spider webs (if no others set), ";
            this.checkBoxP.Text2 = "skull key door (if a and d)";
            this.checkBoxP.UseVisualStyleBackColor = true;
            this.checkBoxP.CheckStateChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // checkBoxO
            // 
            this.checkBoxO.Location = new System.Drawing.Point(6, 343);
            this.checkBoxO.Name = "checkBoxO";
            this.checkBoxO.Size = new System.Drawing.Size(325, 17);
            this.checkBoxO.TabIndex = 16;
            this.checkBoxO.Text = "o. Weeds (if no others set)";
            this.checkBoxO.UseVisualStyleBackColor = true;
            this.checkBoxO.CheckStateChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // checkBoxN
            // 
            this.checkBoxN.Location = new System.Drawing.Point(6, 320);
            this.checkBoxN.Name = "checkBoxN";
            this.checkBoxN.Size = new System.Drawing.Size(325, 17);
            this.checkBoxN.TabIndex = 15;
            this.checkBoxN.Text = "n. Allow ants to climb over";
            this.checkBoxN.UseVisualStyleBackColor = true;
            this.checkBoxN.CheckStateChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // checkBoxM
            // 
            this.checkBoxM.Location = new System.Drawing.Point(6, 297);
            this.checkBoxM.Name = "checkBoxM";
            this.checkBoxM.Size = new System.Drawing.Size(325, 17);
            this.checkBoxM.TabIndex = 14;
            this.checkBoxM.Text = "m. Allow snakeoids to traverse";
            this.checkBoxM.UseVisualStyleBackColor = true;
            this.checkBoxM.CheckStateChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // checkBoxL
            // 
            this.checkBoxL.Location = new System.Drawing.Point(6, 274);
            this.checkBoxL.Name = "checkBoxL";
            this.checkBoxL.Size = new System.Drawing.Size(325, 17);
            this.checkBoxL.TabIndex = 13;
            this.checkBoxL.Text = "l. Pop up wall (if a)";
            this.checkBoxL.UseVisualStyleBackColor = true;
            this.checkBoxL.CheckStateChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // checkBoxK
            // 
            this.checkBoxK.ForeColor2 = System.Drawing.SystemColors.ControlText;
            this.checkBoxK.Location = new System.Drawing.Point(6, 251);
            this.checkBoxK.Name = "checkBoxK";
            this.checkBoxK.Size = new System.Drawing.Size(325, 17);
            this.checkBoxK.TabIndex = 12;
            this.checkBoxK.Text1 = "k. Damage players (if no others set), ";
            this.checkBoxK.Text2 = "down conveyor belt (if d)";
            this.checkBoxK.UseVisualStyleBackColor = true;
            this.checkBoxK.CheckStateChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // checkBoxJ
            // 
            this.checkBoxJ.ForeColor2 = System.Drawing.SystemColors.ControlText;
            this.checkBoxJ.Location = new System.Drawing.Point(6, 228);
            this.checkBoxJ.Name = "checkBoxJ";
            this.checkBoxJ.Size = new System.Drawing.Size(325, 17);
            this.checkBoxJ.TabIndex = 11;
            this.checkBoxJ.Text1 = "j. Trampoline edge (if a), ";
            this.checkBoxJ.Text2 = "left conveyor belt (if d)";
            this.checkBoxJ.UseVisualStyleBackColor = true;
            this.checkBoxJ.CheckStateChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // tilesetPreviewLabel
            // 
            this.tilesetPreviewLabel.AutoSize = true;
            this.tilesetPreviewLabel.Location = new System.Drawing.Point(331, 3);
            this.tilesetPreviewLabel.Name = "tilesetPreviewLabel";
            this.tilesetPreviewLabel.Size = new System.Drawing.Size(82, 13);
            this.tilesetPreviewLabel.TabIndex = 10;
            this.tilesetPreviewLabel.Text = "Tileset Preview:";
            // 
            // tilesetPreview
            // 
            this.tilesetPreview.BackColor = System.Drawing.SystemColors.ControlDark;
            this.tilesetPreview.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tilesetPreview.IsMouseDown = false;
            this.tilesetPreview.Location = new System.Drawing.Point(337, 21);
            this.tilesetPreview.Name = "tilesetPreview";
            this.tilesetPreview.Size = new System.Drawing.Size(384, 384);
            this.tilesetPreview.TabIndex = 9;
            this.tilesetPreview.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tilesetPreview_MouseDown);
            this.tilesetPreview.MouseMove += new System.Windows.Forms.MouseEventHandler(this.tilesetPreview_MouseMove);
            this.tilesetPreview.Paint += new System.Windows.Forms.PaintEventHandler(this.tilesetPreview_Paint);
            // 
            // checkBoxI
            // 
            this.checkBoxI.ForeColor2 = System.Drawing.SystemColors.ControlText;
            this.checkBoxI.Location = new System.Drawing.Point(6, 205);
            this.checkBoxI.Name = "checkBoxI";
            this.checkBoxI.Size = new System.Drawing.Size(325, 17);
            this.checkBoxI.TabIndex = 8;
            this.checkBoxI.Text1 = "i. Water (if a), ";
            this.checkBoxI.Text2 = "up conveyor belt (if d)";
            this.checkBoxI.UseVisualStyleBackColor = true;
            this.checkBoxI.CheckStateChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // checkBoxH
            // 
            this.checkBoxH.Location = new System.Drawing.Point(6, 182);
            this.checkBoxH.Name = "checkBoxH";
            this.checkBoxH.Size = new System.Drawing.Size(325, 17);
            this.checkBoxH.TabIndex = 7;
            this.checkBoxH.Text = "h. Allow weeds to grow";
            this.checkBoxH.UseVisualStyleBackColor = true;
            this.checkBoxH.CheckStateChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // checkBoxG
            // 
            this.checkBoxG.Location = new System.Drawing.Point(6, 159);
            this.checkBoxG.Name = "checkBoxG";
            this.checkBoxG.Size = new System.Drawing.Size(325, 17);
            this.checkBoxG.TabIndex = 6;
            this.checkBoxG.Text = "g. Destructible (if b)";
            this.checkBoxG.UseVisualStyleBackColor = true;
            this.checkBoxG.CheckStateChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // checkBoxE
            // 
            this.checkBoxE.Location = new System.Drawing.Point(6, 113);
            this.checkBoxE.Name = "checkBoxE";
            this.checkBoxE.Size = new System.Drawing.Size(325, 17);
            this.checkBoxE.TabIndex = 4;
            this.checkBoxE.Text = "e. Regular key door (if a)";
            this.checkBoxE.UseVisualStyleBackColor = true;
            this.checkBoxE.CheckStateChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // checkBoxD
            // 
            this.checkBoxD.Location = new System.Drawing.Point(6, 90);
            this.checkBoxD.Name = "checkBoxD";
            this.checkBoxD.Size = new System.Drawing.Size(325, 17);
            this.checkBoxD.TabIndex = 3;
            this.checkBoxD.Text = "d. Conveyor belt, skull key door, or trampoline center";
            this.checkBoxD.UseVisualStyleBackColor = true;
            this.checkBoxD.CheckStateChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // checkBoxC
            // 
            this.checkBoxC.Location = new System.Drawing.Point(6, 67);
            this.checkBoxC.Name = "checkBoxC";
            this.checkBoxC.Size = new System.Drawing.Size(325, 17);
            this.checkBoxC.TabIndex = 2;
            this.checkBoxC.Text = "c. Solid to weapons";
            this.checkBoxC.UseVisualStyleBackColor = true;
            this.checkBoxC.CheckStateChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // checkBoxB
            // 
            this.checkBoxB.Location = new System.Drawing.Point(6, 44);
            this.checkBoxB.Name = "checkBoxB";
            this.checkBoxB.Size = new System.Drawing.Size(325, 17);
            this.checkBoxB.TabIndex = 1;
            this.checkBoxB.Text = "b. Solid to monsters";
            this.checkBoxB.UseVisualStyleBackColor = true;
            this.checkBoxB.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // checkBoxA
            // 
            this.checkBoxA.BackColor = System.Drawing.SystemColors.Control;
            this.checkBoxA.Location = new System.Drawing.Point(6, 21);
            this.checkBoxA.Name = "checkBoxA";
            this.checkBoxA.Size = new System.Drawing.Size(325, 17);
            this.checkBoxA.TabIndex = 0;
            this.checkBoxA.Text = "a. Soild to players";
            this.checkBoxA.UseVisualStyleBackColor = false;
            this.checkBoxA.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // CollisionEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1616, 839);
            this.Controls.Add(this.splitContainer1);
            this.Name = "CollisionEditor";
            this.Text = "CollisionEditor";
            this.Layout += new System.Windows.Forms.LayoutEventHandler(this.CollisionEditor_Layout);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private SpriteTilePicker tilePicker;
        private AssetSelector paletteSelector;
        private AssetSelector graphicsSelector;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label presetLabel;
        private TwoColorCheckbox checkBoxP;
        private System.Windows.Forms.CheckBox checkBoxO;
        private System.Windows.Forms.CheckBox checkBoxN;
        private System.Windows.Forms.CheckBox checkBoxM;
        private System.Windows.Forms.CheckBox checkBoxL;
        private TwoColorCheckbox checkBoxK;
        private TwoColorCheckbox checkBoxJ;
        private System.Windows.Forms.Label tilesetPreviewLabel;
        private Canvas tilesetPreview;
        private TwoColorCheckbox checkBoxI;
        private System.Windows.Forms.CheckBox checkBoxH;
        private System.Windows.Forms.CheckBox checkBoxG;
        private TwoColorCheckbox checkBoxF;
        private System.Windows.Forms.CheckBox checkBoxE;
        private System.Windows.Forms.CheckBox checkBoxD;
        private System.Windows.Forms.CheckBox checkBoxC;
        private System.Windows.Forms.CheckBox checkBoxB;
        private System.Windows.Forms.CheckBox checkBoxA;
        private AssetSelector tilemapSelector;
        private Canvas tilesetPreviewCollision;
        private System.Windows.Forms.ListBox presetList;
    }
}