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
            this.palettePanel = new NoAutoScalePanel();
            this.palette7Button = new System.Windows.Forms.RadioButton();
            this.palette6Button = new System.Windows.Forms.RadioButton();
            this.palette5Button = new System.Windows.Forms.RadioButton();
            this.palette4Button = new System.Windows.Forms.RadioButton();
            this.palette3Button = new System.Windows.Forms.RadioButton();
            this.palette2Button = new System.Windows.Forms.RadioButton();
            this.palette1Button = new System.Windows.Forms.RadioButton();
            this.palette0Button = new System.Windows.Forms.RadioButton();
            this.paletteLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.palettePanel.SuspendLayout();
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
            this.canvas.Location = new System.Drawing.Point(0, 0);
            this.canvas.Name = "canvas";
            this.canvas.Size = new System.Drawing.Size(1025, 884);
            this.canvas.TabIndex = 3;
            this.canvas.Paint += new System.Windows.Forms.PaintEventHandler(this.canvas_Paint);
            this.canvas.KeyDown += new System.Windows.Forms.KeyEventHandler(this.canvas_KeyDown);
            this.canvas.MouseDown += new System.Windows.Forms.MouseEventHandler(this.canvas_MouseDown);
            this.canvas.MouseMove += new System.Windows.Forms.MouseEventHandler(this.canvas_MouseMove);
            this.canvas.MouseUp += new System.Windows.Forms.MouseEventHandler(this.canvas_MouseUp);
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
            this.splitContainer1.Panel1.Controls.Add(this.palettePanel);
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
            this.tilePicker.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tilePicker.Location = new System.Drawing.Point(0, 29);
            this.tilePicker.Name = "tilePicker";
            this.tilePicker.Palette = 0;
            this.tilePicker.SelectedTile = -1;
            this.tilePicker.Size = new System.Drawing.Size(273, 872);
            this.tilePicker.TabIndex = 7;
            this.tilePicker.SelectedTileChanged += new Necrofy.SpriteTilePicker.SelectedTileChangedDelegate(this.tilePicker_SelectedTileChanged);
            this.tilePicker.TileDoubleClicked += new Necrofy.SpriteTilePicker.TileDoubleClickedDelegate(this.tilePicker_TileDoubleClicked);
            // 
            // palettePanel
            // 
            this.palettePanel.Controls.Add(this.palette7Button);
            this.palettePanel.Controls.Add(this.palette6Button);
            this.palettePanel.Controls.Add(this.palette5Button);
            this.palettePanel.Controls.Add(this.palette4Button);
            this.palettePanel.Controls.Add(this.palette3Button);
            this.palettePanel.Controls.Add(this.palette2Button);
            this.palettePanel.Controls.Add(this.palette1Button);
            this.palettePanel.Controls.Add(this.palette0Button);
            this.palettePanel.Controls.Add(this.paletteLabel);
            this.palettePanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.palettePanel.Location = new System.Drawing.Point(0, 0);
            this.palettePanel.Name = "palettePanel";
            this.palettePanel.Size = new System.Drawing.Size(273, 29);
            this.palettePanel.TabIndex = 8;
            // 
            // palette7Button
            // 
            this.palette7Button.Appearance = System.Windows.Forms.Appearance.Button;
            this.palette7Button.AutoSize = true;
            this.palette7Button.Location = new System.Drawing.Point(248, 3);
            this.palette7Button.Name = "palette7Button";
            this.palette7Button.Size = new System.Drawing.Size(23, 23);
            this.palette7Button.TabIndex = 8;
            this.palette7Button.Text = "7";
            this.palette7Button.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.palette7Button.UseVisualStyleBackColor = true;
            this.palette7Button.CheckedChanged += new System.EventHandler(this.paletteButton_CheckedChanged);
            // 
            // palette6Button
            // 
            this.palette6Button.Appearance = System.Windows.Forms.Appearance.Button;
            this.palette6Button.AutoSize = true;
            this.palette6Button.Location = new System.Drawing.Point(219, 3);
            this.palette6Button.Name = "palette6Button";
            this.palette6Button.Size = new System.Drawing.Size(23, 23);
            this.palette6Button.TabIndex = 7;
            this.palette6Button.Text = "6";
            this.palette6Button.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.palette6Button.UseVisualStyleBackColor = true;
            this.palette6Button.CheckedChanged += new System.EventHandler(this.paletteButton_CheckedChanged);
            // 
            // palette5Button
            // 
            this.palette5Button.Appearance = System.Windows.Forms.Appearance.Button;
            this.palette5Button.AutoSize = true;
            this.palette5Button.Location = new System.Drawing.Point(190, 3);
            this.palette5Button.Name = "palette5Button";
            this.palette5Button.Size = new System.Drawing.Size(23, 23);
            this.palette5Button.TabIndex = 6;
            this.palette5Button.Text = "5";
            this.palette5Button.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.palette5Button.UseVisualStyleBackColor = true;
            this.palette5Button.CheckedChanged += new System.EventHandler(this.paletteButton_CheckedChanged);
            // 
            // palette4Button
            // 
            this.palette4Button.Appearance = System.Windows.Forms.Appearance.Button;
            this.palette4Button.AutoSize = true;
            this.palette4Button.Location = new System.Drawing.Point(161, 3);
            this.palette4Button.Name = "palette4Button";
            this.palette4Button.Size = new System.Drawing.Size(23, 23);
            this.palette4Button.TabIndex = 5;
            this.palette4Button.Text = "4";
            this.palette4Button.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.palette4Button.UseVisualStyleBackColor = true;
            this.palette4Button.CheckedChanged += new System.EventHandler(this.paletteButton_CheckedChanged);
            // 
            // palette3Button
            // 
            this.palette3Button.Appearance = System.Windows.Forms.Appearance.Button;
            this.palette3Button.AutoSize = true;
            this.palette3Button.Location = new System.Drawing.Point(132, 3);
            this.palette3Button.Name = "palette3Button";
            this.palette3Button.Size = new System.Drawing.Size(23, 23);
            this.palette3Button.TabIndex = 4;
            this.palette3Button.Text = "3";
            this.palette3Button.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.palette3Button.UseVisualStyleBackColor = true;
            this.palette3Button.CheckedChanged += new System.EventHandler(this.paletteButton_CheckedChanged);
            // 
            // palette2Button
            // 
            this.palette2Button.Appearance = System.Windows.Forms.Appearance.Button;
            this.palette2Button.AutoSize = true;
            this.palette2Button.Location = new System.Drawing.Point(103, 3);
            this.palette2Button.Name = "palette2Button";
            this.palette2Button.Size = new System.Drawing.Size(23, 23);
            this.palette2Button.TabIndex = 3;
            this.palette2Button.Text = "2";
            this.palette2Button.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.palette2Button.UseVisualStyleBackColor = true;
            this.palette2Button.CheckedChanged += new System.EventHandler(this.paletteButton_CheckedChanged);
            // 
            // palette1Button
            // 
            this.palette1Button.Appearance = System.Windows.Forms.Appearance.Button;
            this.palette1Button.AutoSize = true;
            this.palette1Button.Location = new System.Drawing.Point(74, 3);
            this.palette1Button.Name = "palette1Button";
            this.palette1Button.Size = new System.Drawing.Size(23, 23);
            this.palette1Button.TabIndex = 2;
            this.palette1Button.Text = "1";
            this.palette1Button.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.palette1Button.UseVisualStyleBackColor = true;
            this.palette1Button.CheckedChanged += new System.EventHandler(this.paletteButton_CheckedChanged);
            // 
            // palette0Button
            // 
            this.palette0Button.Appearance = System.Windows.Forms.Appearance.Button;
            this.palette0Button.AutoSize = true;
            this.palette0Button.Checked = true;
            this.palette0Button.Location = new System.Drawing.Point(45, 3);
            this.palette0Button.Name = "palette0Button";
            this.palette0Button.Size = new System.Drawing.Size(23, 23);
            this.palette0Button.TabIndex = 1;
            this.palette0Button.TabStop = true;
            this.palette0Button.Text = "0";
            this.palette0Button.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.palette0Button.UseVisualStyleBackColor = true;
            this.palette0Button.CheckedChanged += new System.EventHandler(this.paletteButton_CheckedChanged);
            // 
            // paletteLabel
            // 
            this.paletteLabel.AutoSize = true;
            this.paletteLabel.Location = new System.Drawing.Point(1, 8);
            this.paletteLabel.Name = "paletteLabel";
            this.paletteLabel.Size = new System.Drawing.Size(43, 13);
            this.paletteLabel.TabIndex = 0;
            this.paletteLabel.Text = "Palette:";
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
            this.palettePanel.ResumeLayout(false);
            this.palettePanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.VScrollBar vscroll;
        private System.Windows.Forms.HScrollBar hscroll;
        private Canvas canvas;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private SpriteTilePicker tilePicker;
        private NoAutoScalePanel palettePanel;
        private System.Windows.Forms.RadioButton palette7Button;
        private System.Windows.Forms.RadioButton palette6Button;
        private System.Windows.Forms.RadioButton palette5Button;
        private System.Windows.Forms.RadioButton palette4Button;
        private System.Windows.Forms.RadioButton palette3Button;
        private System.Windows.Forms.RadioButton palette2Button;
        private System.Windows.Forms.RadioButton palette1Button;
        private System.Windows.Forms.RadioButton palette0Button;
        private System.Windows.Forms.Label paletteLabel;
    }
}