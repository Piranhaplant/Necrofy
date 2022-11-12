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
            this.graphicsListPanel = new System.Windows.Forms.Panel();
            this.graphicsList = new System.Windows.Forms.ListBox();
            this.graphicsSelector = new Necrofy.AssetSelector();
            this.addGraphicsButton = new System.Windows.Forms.Button();
            this.graphicsHeaderPanel = new System.Windows.Forms.Panel();
            this.graphicsExpandLabel = new System.Windows.Forms.Label();
            this.graphicsLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.graphicsListPanel.SuspendLayout();
            this.graphicsHeaderPanel.SuspendLayout();
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
            this.splitContainer1.Panel1.Controls.Add(this.graphicsListPanel);
            this.splitContainer1.Panel1.Controls.Add(this.graphicsHeaderPanel);
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
            this.tilePicker.Location = new System.Drawing.Point(0, 122);
            this.tilePicker.Name = "tilePicker";
            this.tilePicker.Palette = 0;
            this.tilePicker.SelectedTile = -1;
            this.tilePicker.Size = new System.Drawing.Size(273, 779);
            this.tilePicker.TabIndex = 7;
            this.tilePicker.SelectedTileChanged += new Necrofy.SpriteTilePicker.SelectedTileChangedDelegate(this.tilePicker_SelectedTileChanged);
            this.tilePicker.TileDoubleClicked += new Necrofy.SpriteTilePicker.TileDoubleClickedDelegate(this.tilePicker_TileDoubleClicked);
            this.tilePicker.PaletteChanged += new Necrofy.SpriteTilePicker.PaletteChangedDelegate(this.tilePicker_PaletteChanged);
            // 
            // graphicsListPanel
            // 
            this.graphicsListPanel.Controls.Add(this.graphicsList);
            this.graphicsListPanel.Controls.Add(this.graphicsSelector);
            this.graphicsListPanel.Controls.Add(this.addGraphicsButton);
            this.graphicsListPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.graphicsListPanel.Location = new System.Drawing.Point(0, 32);
            this.graphicsListPanel.Name = "graphicsListPanel";
            this.graphicsListPanel.Size = new System.Drawing.Size(273, 90);
            this.graphicsListPanel.TabIndex = 6;
            this.graphicsListPanel.Visible = false;
            // 
            // graphicsList
            // 
            this.graphicsList.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.graphicsList.FormattingEnabled = true;
            this.graphicsList.Location = new System.Drawing.Point(0, 21);
            this.graphicsList.Name = "graphicsList";
            this.graphicsList.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.graphicsList.Size = new System.Drawing.Size(273, 69);
            this.graphicsList.TabIndex = 2;
            // 
            // graphicsSelector
            // 
            this.graphicsSelector.Location = new System.Drawing.Point(0, 0);
            this.graphicsSelector.Name = "graphicsSelector";
            this.graphicsSelector.Size = new System.Drawing.Size(252, 21);
            this.graphicsSelector.TabIndex = 0;
            // 
            // addGraphicsButton
            // 
            this.addGraphicsButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.addGraphicsButton.Image = global::Necrofy.Properties.Resources.plus;
            this.addGraphicsButton.Location = new System.Drawing.Point(251, -1);
            this.addGraphicsButton.Name = "addGraphicsButton";
            this.addGraphicsButton.Size = new System.Drawing.Size(23, 23);
            this.addGraphicsButton.TabIndex = 1;
            this.addGraphicsButton.UseVisualStyleBackColor = true;
            this.addGraphicsButton.Click += new System.EventHandler(this.addGraphicsButton_Click);
            // 
            // graphicsHeaderPanel
            // 
            this.graphicsHeaderPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.graphicsHeaderPanel.Controls.Add(this.graphicsExpandLabel);
            this.graphicsHeaderPanel.Controls.Add(this.graphicsLabel);
            this.graphicsHeaderPanel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.graphicsHeaderPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.graphicsHeaderPanel.Location = new System.Drawing.Point(0, 0);
            this.graphicsHeaderPanel.Name = "graphicsHeaderPanel";
            this.graphicsHeaderPanel.Size = new System.Drawing.Size(273, 32);
            this.graphicsHeaderPanel.TabIndex = 0;
            this.graphicsHeaderPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.graphicsHeaderPanel_MouseDown);
            // 
            // graphicsExpandLabel
            // 
            this.graphicsExpandLabel.AutoSize = true;
            this.graphicsExpandLabel.Location = new System.Drawing.Point(247, 9);
            this.graphicsExpandLabel.Name = "graphicsExpandLabel";
            this.graphicsExpandLabel.Size = new System.Drawing.Size(16, 13);
            this.graphicsExpandLabel.TabIndex = 1;
            this.graphicsExpandLabel.Text = "▼";
            this.graphicsExpandLabel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.graphicsHeaderPanel_MouseDown);
            // 
            // graphicsLabel
            // 
            this.graphicsLabel.AutoSize = true;
            this.graphicsLabel.Location = new System.Drawing.Point(12, 9);
            this.graphicsLabel.Name = "graphicsLabel";
            this.graphicsLabel.Size = new System.Drawing.Size(49, 13);
            this.graphicsLabel.TabIndex = 0;
            this.graphicsLabel.Text = "Graphics";
            this.graphicsLabel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.graphicsHeaderPanel_MouseDown);
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
            this.graphicsListPanel.ResumeLayout(false);
            this.graphicsHeaderPanel.ResumeLayout(false);
            this.graphicsHeaderPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.VScrollBar vscroll;
        private System.Windows.Forms.HScrollBar hscroll;
        private Canvas canvas;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private SpriteTilePicker tilePicker;
        private System.Windows.Forms.Panel graphicsListPanel;
        private System.Windows.Forms.Button addGraphicsButton;
        private AssetSelector graphicsSelector;
        private System.Windows.Forms.Panel graphicsHeaderPanel;
        private System.Windows.Forms.Label graphicsLabel;
        private System.Windows.Forms.ListBox graphicsList;
        private System.Windows.Forms.Label graphicsExpandLabel;
    }
}