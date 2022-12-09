namespace Necrofy
{
    partial class SpriteEditor
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
            this.vscroll = new System.Windows.Forms.VScrollBar();
            this.hscroll = new System.Windows.Forms.HScrollBar();
            this.canvas = new Necrofy.Canvas();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tilePicker = new Necrofy.SpriteTilePicker();
            this.graphicsPanel = new System.Windows.Forms.Panel();
            this.showAllGraphics = new System.Windows.Forms.CheckBox();
            this.paletteSelector = new Necrofy.AssetSelector();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.graphicsPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // vscroll
            // 
            this.vscroll.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.vscroll.LargeChange = 100;
            this.vscroll.Location = new System.Drawing.Point(1025, 0);
            this.vscroll.Name = "vscroll";
            this.vscroll.Size = new System.Drawing.Size(17, 884);
            this.vscroll.SmallChange = 16;
            this.vscroll.TabIndex = 5;
            // 
            // hscroll
            // 
            this.hscroll.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.hscroll.LargeChange = 100;
            this.hscroll.Location = new System.Drawing.Point(0, 884);
            this.hscroll.Name = "hscroll";
            this.hscroll.Size = new System.Drawing.Size(1025, 17);
            this.hscroll.SmallChange = 16;
            this.hscroll.TabIndex = 4;
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
            this.canvas.Size = new System.Drawing.Size(1025, 884);
            this.canvas.TabIndex = 3;
            this.canvas.MouseDown += new System.Windows.Forms.MouseEventHandler(this.canvas_MouseDown);
            this.canvas.MouseMove += new System.Windows.Forms.MouseEventHandler(this.canvas_MouseMove);
            this.canvas.MouseUp += new System.Windows.Forms.MouseEventHandler(this.canvas_MouseUp);
            this.canvas.Paint += new System.Windows.Forms.PaintEventHandler(this.canvas_Paint);
            this.canvas.KeyDown += new System.Windows.Forms.KeyEventHandler(this.canvas_KeyDown);
            this.canvas.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.canvas_PreviewKeyDown);
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
            this.splitContainer1.Panel1.Controls.Add(this.graphicsPanel);
            this.splitContainer1.Panel1.Controls.Add(this.paletteSelector);
            this.splitContainer1.Panel1MinSize = 273;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.canvas);
            this.splitContainer1.Panel2.Controls.Add(this.vscroll);
            this.splitContainer1.Panel2.Controls.Add(this.hscroll);
            this.splitContainer1.Size = new System.Drawing.Size(1319, 901);
            this.splitContainer1.SplitterDistance = 273;
            this.splitContainer1.SplitterIncrement = 16;
            this.splitContainer1.TabIndex = 6;
            // 
            // tilePicker
            // 
            this.tilePicker.ColorsPerPalette = 16;
            this.tilePicker.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tilePicker.FlipX = false;
            this.tilePicker.FlipY = false;
            this.tilePicker.Location = new System.Drawing.Point(0, 53);
            this.tilePicker.Name = "tilePicker";
            this.tilePicker.Palette = 0;
            this.tilePicker.SelectedTile = -1;
            this.tilePicker.Size = new System.Drawing.Size(273, 848);
            this.tilePicker.TabIndex = 7;
            this.tilePicker.SelectedTileChanged += new Necrofy.SpriteTilePicker.SelectedTileChangedDelegate(this.tilePicker_SelectedTileChanged);
            this.tilePicker.TileDoubleClicked += new Necrofy.SpriteTilePicker.TileDoubleClickedDelegate(this.tilePicker_TileDoubleClicked);
            this.tilePicker.PaletteChanged += new Necrofy.SpriteTilePicker.PaletteChangedDelegate(this.tilePicker_PaletteChanged);
            // 
            // graphicsPanel
            // 
            this.graphicsPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.graphicsPanel.Controls.Add(this.showAllGraphics);
            this.graphicsPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.graphicsPanel.Location = new System.Drawing.Point(0, 21);
            this.graphicsPanel.Name = "graphicsPanel";
            this.graphicsPanel.Size = new System.Drawing.Size(273, 32);
            this.graphicsPanel.TabIndex = 0;
            // 
            // showAllGraphics
            // 
            this.showAllGraphics.AutoSize = true;
            this.showAllGraphics.Location = new System.Drawing.Point(5, 7);
            this.showAllGraphics.Name = "showAllGraphics";
            this.showAllGraphics.Size = new System.Drawing.Size(112, 17);
            this.showAllGraphics.TabIndex = 0;
            this.showAllGraphics.Text = "Show All Graphics";
            this.showAllGraphics.UseVisualStyleBackColor = true;
            this.showAllGraphics.CheckedChanged += new System.EventHandler(this.showAllGraphics_CheckedChanged);
            // 
            // paletteSelector
            // 
            this.paletteSelector.Dock = System.Windows.Forms.DockStyle.Top;
            this.paletteSelector.Location = new System.Drawing.Point(0, 0);
            this.paletteSelector.Name = "paletteSelector";
            this.paletteSelector.Size = new System.Drawing.Size(273, 21);
            this.paletteSelector.TabIndex = 7;
            this.paletteSelector.SelectedItemChanged += new System.EventHandler(this.paletteSelector_SelectedItemChanged);
            // 
            // SpriteEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1319, 901);
            this.Controls.Add(this.splitContainer1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "SpriteEditor";
            this.Text = "SpriteEditor";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.graphicsPanel.ResumeLayout(false);
            this.graphicsPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.VScrollBar vscroll;
        private System.Windows.Forms.HScrollBar hscroll;
        private Canvas canvas;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private SpriteTilePicker tilePicker;
        private System.Windows.Forms.Panel graphicsPanel;
        private System.Windows.Forms.CheckBox showAllGraphics;
        private AssetSelector paletteSelector;
    }
}