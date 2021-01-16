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
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
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
            this.tilePicker.Location = new System.Drawing.Point(0, 0);
            this.tilePicker.Name = "tilePicker";
            this.tilePicker.Palette = 0;
            this.tilePicker.SelectedTile = -1;
            this.tilePicker.Size = new System.Drawing.Size(273, 901);
            this.tilePicker.TabIndex = 7;
            this.tilePicker.SelectedTileChanged += new Necrofy.SpriteTilePicker.SelectedTileChangedDelegate(this.tilePicker_SelectedTileChanged);
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
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.VScrollBar vscroll;
        private System.Windows.Forms.HScrollBar hscroll;
        private Canvas canvas;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private SpriteTilePicker tilePicker;
    }
}