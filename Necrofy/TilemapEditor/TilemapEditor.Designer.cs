namespace Necrofy
{
    partial class TilemapEditor
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
            this.graphicsSelector = new Necrofy.AssetSelector();
            this.paletteSelector = new Necrofy.AssetSelector();
            this.canvas = new Necrofy.Canvas();
            this.vScroll = new System.Windows.Forms.VScrollBar();
            this.hScroll = new System.Windows.Forms.HScrollBar();
            this.flipPanel = new Necrofy.NoAutoScalePanel();
            this.flipX = new System.Windows.Forms.CheckBox();
            this.flipY = new System.Windows.Forms.CheckBox();
            this.clearFlip = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.flipPanel.SuspendLayout();
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
            this.splitContainer1.Panel1.Controls.Add(this.flipPanel);
            this.splitContainer1.Panel1.Controls.Add(this.graphicsSelector);
            this.splitContainer1.Panel1.Controls.Add(this.paletteSelector);
            this.splitContainer1.Panel1MinSize = 273;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.canvas);
            this.splitContainer1.Panel2.Controls.Add(this.vScroll);
            this.splitContainer1.Panel2.Controls.Add(this.hScroll);
            this.splitContainer1.Size = new System.Drawing.Size(1402, 804);
            this.splitContainer1.SplitterDistance = 273;
            this.splitContainer1.SplitterIncrement = 16;
            this.splitContainer1.TabIndex = 0;
            // 
            // tilePicker
            // 
            this.tilePicker.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tilePicker.Location = new System.Drawing.Point(0, 65);
            this.tilePicker.Name = "tilePicker";
            this.tilePicker.Palette = 0;
            this.tilePicker.SelectedTile = -1;
            this.tilePicker.Size = new System.Drawing.Size(273, 739);
            this.tilePicker.TabIndex = 0;
            // 
            // graphicsSelector
            // 
            this.graphicsSelector.Dock = System.Windows.Forms.DockStyle.Top;
            this.graphicsSelector.Location = new System.Drawing.Point(0, 21);
            this.graphicsSelector.Name = "graphicsSelector";
            this.graphicsSelector.Size = new System.Drawing.Size(273, 21);
            this.graphicsSelector.TabIndex = 2;
            this.graphicsSelector.SelectedItemChanged += new System.EventHandler(this.graphicsSelector_SelectedItemChanged);
            // 
            // paletteSelector
            // 
            this.paletteSelector.Dock = System.Windows.Forms.DockStyle.Top;
            this.paletteSelector.Location = new System.Drawing.Point(0, 0);
            this.paletteSelector.Name = "paletteSelector";
            this.paletteSelector.Size = new System.Drawing.Size(273, 21);
            this.paletteSelector.TabIndex = 1;
            this.paletteSelector.SelectedItemChanged += new System.EventHandler(this.paletteSelector_SelectedItemChanged);
            // 
            // canvas
            // 
            this.canvas.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.canvas.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.canvas.IsMouseDown = false;
            this.canvas.Location = new System.Drawing.Point(0, 0);
            this.canvas.Name = "canvas";
            this.canvas.Size = new System.Drawing.Size(1108, 787);
            this.canvas.TabIndex = 6;
            // 
            // vScroll
            // 
            this.vScroll.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.vScroll.LargeChange = 100;
            this.vScroll.Location = new System.Drawing.Point(1108, 0);
            this.vScroll.Name = "vScroll";
            this.vScroll.Size = new System.Drawing.Size(17, 787);
            this.vScroll.SmallChange = 16;
            this.vScroll.TabIndex = 8;
            // 
            // hScroll
            // 
            this.hScroll.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.hScroll.LargeChange = 100;
            this.hScroll.Location = new System.Drawing.Point(0, 787);
            this.hScroll.Name = "hScroll";
            this.hScroll.Size = new System.Drawing.Size(1108, 17);
            this.hScroll.SmallChange = 16;
            this.hScroll.TabIndex = 7;
            // 
            // flipPanel
            // 
            this.flipPanel.Controls.Add(this.clearFlip);
            this.flipPanel.Controls.Add(this.flipY);
            this.flipPanel.Controls.Add(this.flipX);
            this.flipPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.flipPanel.Location = new System.Drawing.Point(0, 42);
            this.flipPanel.Name = "flipPanel";
            this.flipPanel.Size = new System.Drawing.Size(273, 23);
            this.flipPanel.TabIndex = 3;
            // 
            // flipX
            // 
            this.flipX.AutoSize = true;
            this.flipX.Location = new System.Drawing.Point(3, 3);
            this.flipX.Name = "flipX";
            this.flipX.Size = new System.Drawing.Size(99, 17);
            this.flipX.TabIndex = 4;
            this.flipX.Text = "Flip Horizontally";
            this.flipX.UseVisualStyleBackColor = true;
            this.flipX.CheckedChanged += new System.EventHandler(this.flipX_CheckedChanged);
            // 
            // flipY
            // 
            this.flipY.AutoSize = true;
            this.flipY.Location = new System.Drawing.Point(108, 3);
            this.flipY.Name = "flipY";
            this.flipY.Size = new System.Drawing.Size(87, 17);
            this.flipY.TabIndex = 5;
            this.flipY.Text = "Flip Vertically";
            this.flipY.UseVisualStyleBackColor = true;
            this.flipY.CheckedChanged += new System.EventHandler(this.flipY_CheckedChanged);
            // 
            // clearFlip
            // 
            this.clearFlip.Location = new System.Drawing.Point(199, 0);
            this.clearFlip.Name = "clearFlip";
            this.clearFlip.Size = new System.Drawing.Size(75, 23);
            this.clearFlip.TabIndex = 6;
            this.clearFlip.Text = "Clear";
            this.clearFlip.UseVisualStyleBackColor = true;
            this.clearFlip.Click += new System.EventHandler(this.clearFlip_Click);
            // 
            // TilemapEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1402, 804);
            this.Controls.Add(this.splitContainer1);
            this.Name = "TilemapEditor";
            this.Text = "TilemapEditor";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.flipPanel.ResumeLayout(false);
            this.flipPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private SpriteTilePicker tilePicker;
        private Canvas canvas;
        private System.Windows.Forms.VScrollBar vScroll;
        private System.Windows.Forms.HScrollBar hScroll;
        private AssetSelector graphicsSelector;
        private AssetSelector paletteSelector;
        private NoAutoScalePanel flipPanel;
        private System.Windows.Forms.Button clearFlip;
        private System.Windows.Forms.CheckBox flipY;
        private System.Windows.Forms.CheckBox flipX;
    }
}