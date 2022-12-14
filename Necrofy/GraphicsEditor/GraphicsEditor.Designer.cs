namespace Necrofy
{
    partial class GraphicsEditor
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
            this.hScroll = new System.Windows.Forms.HScrollBar();
            this.vScroll = new System.Windows.Forms.VScrollBar();
            this.panel1 = new System.Windows.Forms.Panel();
            this.colorSelector = new Necrofy.ColorSelector();
            this.paletteSelector = new Necrofy.AssetSelector();
            this.canvas = new Necrofy.Canvas();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // hScroll
            // 
            this.hScroll.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.hScroll.LargeChange = 16;
            this.hScroll.Location = new System.Drawing.Point(256, 433);
            this.hScroll.Name = "hScroll";
            this.hScroll.Size = new System.Drawing.Size(527, 17);
            this.hScroll.SmallChange = 16;
            this.hScroll.TabIndex = 3;
            // 
            // vScroll
            // 
            this.vScroll.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.vScroll.LargeChange = 16;
            this.vScroll.Location = new System.Drawing.Point(783, 0);
            this.vScroll.Name = "vScroll";
            this.vScroll.Size = new System.Drawing.Size(17, 433);
            this.vScroll.SmallChange = 16;
            this.vScroll.TabIndex = 2;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.colorSelector);
            this.panel1.Controls.Add(this.paletteSelector);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(256, 450);
            this.panel1.TabIndex = 1;
            // 
            // colorSelector
            // 
            this.colorSelector.Colors = null;
            this.colorSelector.Dock = System.Windows.Forms.DockStyle.Fill;
            this.colorSelector.Location = new System.Drawing.Point(0, 21);
            this.colorSelector.MultiSelect = false;
            this.colorSelector.Name = "colorSelector";
            this.colorSelector.SelectionEnd = new System.Drawing.Point(-1, -1);
            this.colorSelector.SelectionStart = new System.Drawing.Point(-1, -1);
            this.colorSelector.Size = new System.Drawing.Size(256, 429);
            this.colorSelector.SquaresPerRow = 16;
            this.colorSelector.TabIndex = 3;
            this.colorSelector.SelectionChanged += new System.EventHandler(this.colorSelector_SelectionChanged);
            this.colorSelector.Enter += colorSelector_Enter;
            // 
            // paletteSelector
            // 
            this.paletteSelector.Dock = System.Windows.Forms.DockStyle.Top;
            this.paletteSelector.Location = new System.Drawing.Point(0, 0);
            this.paletteSelector.Name = "paletteSelector";
            this.paletteSelector.Size = new System.Drawing.Size(256, 21);
            this.paletteSelector.TabIndex = 4;
            this.paletteSelector.SelectedItemChanged += PaletteSelector_SelectedItemChanged;
            // 
            // canvas
            // 
            this.canvas.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.canvas.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.canvas.IsMouseDown = false;
            this.canvas.Location = new System.Drawing.Point(256, 0);
            this.canvas.Name = "canvas";
            this.canvas.Size = new System.Drawing.Size(527, 433);
            this.canvas.TabIndex = 0;
            this.canvas.KeyDown += new System.Windows.Forms.KeyEventHandler(this.canvas_KeyDown);
            // 
            // GraphicsEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.hScroll);
            this.Controls.Add(this.vScroll);
            this.Controls.Add(this.canvas);
            this.Controls.Add(this.panel1);
            this.Name = "GraphicsEditor";
            this.Text = "GraphicsEditor";
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Canvas canvas;
        private System.Windows.Forms.Panel panel1;
        private ColorSelector colorSelector;
        private System.Windows.Forms.VScrollBar vScroll;
        private System.Windows.Forms.HScrollBar hScroll;
        private AssetSelector paletteSelector;
    }
}