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
            this.components = new System.ComponentModel.Container();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tilePicker = new Necrofy.SpriteTilePicker();
            this.optionsPanel = new Necrofy.NoAutoScalePanel();
            this.priorityLabel = new System.Windows.Forms.Label();
            this.priority = new System.Windows.Forms.CheckBox();
            this.lockPriority = new System.Windows.Forms.CheckBox();
            this.lockFlip = new System.Windows.Forms.CheckBox();
            this.lockPalette = new System.Windows.Forms.CheckBox();
            this.lockTileNum = new System.Windows.Forms.CheckBox();
            this.lockLabel = new System.Windows.Forms.Label();
            this.flipLabel = new System.Windows.Forms.Label();
            this.flipX = new System.Windows.Forms.CheckBox();
            this.flipY = new System.Windows.Forms.CheckBox();
            this.paletteSelector = new Necrofy.AssetSelector();
            this.graphicsSelector = new Necrofy.AssetSelector();
            this.canvas = new Necrofy.Canvas();
            this.vScroll = new System.Windows.Forms.VScrollBar();
            this.hScroll = new System.Windows.Forms.HScrollBar();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.optionsPanel.SuspendLayout();
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
            this.splitContainer1.Panel1.Controls.Add(this.optionsPanel);
            this.splitContainer1.Panel1.Controls.Add(this.paletteSelector);
            this.splitContainer1.Panel1.Controls.Add(this.graphicsSelector);
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
            this.tilePicker.ColorsPerPalette = 16;
            this.tilePicker.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tilePicker.FlipX = false;
            this.tilePicker.FlipY = false;
            this.tilePicker.Location = new System.Drawing.Point(0, 70);
            this.tilePicker.Name = "tilePicker";
            this.tilePicker.Palette = 0;
            this.tilePicker.SelectedTile = -1;
            this.tilePicker.Size = new System.Drawing.Size(273, 734);
            this.tilePicker.TabIndex = 0;
            this.tilePicker.SelectedTileChanged += new Necrofy.SpriteTilePicker.SelectedTileChangedDelegate(this.tilePicker_SelectedTileChanged);
            this.tilePicker.PaletteChanged += new Necrofy.SpriteTilePicker.PaletteChangedDelegate(this.tilePicker_PaletteChanged);
            // 
            // optionsPanel
            // 
            this.optionsPanel.Controls.Add(this.priorityLabel);
            this.optionsPanel.Controls.Add(this.priority);
            this.optionsPanel.Controls.Add(this.lockPriority);
            this.optionsPanel.Controls.Add(this.lockFlip);
            this.optionsPanel.Controls.Add(this.lockPalette);
            this.optionsPanel.Controls.Add(this.lockTileNum);
            this.optionsPanel.Controls.Add(this.lockLabel);
            this.optionsPanel.Controls.Add(this.flipLabel);
            this.optionsPanel.Controls.Add(this.flipX);
            this.optionsPanel.Controls.Add(this.flipY);
            this.optionsPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.optionsPanel.Location = new System.Drawing.Point(0, 42);
            this.optionsPanel.Name = "optionsPanel";
            this.optionsPanel.Size = new System.Drawing.Size(273, 28);
            this.optionsPanel.TabIndex = 3;
            // 
            // priorityLabel
            // 
            this.priorityLabel.AutoSize = true;
            this.priorityLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.priorityLabel.Location = new System.Drawing.Point(82, 8);
            this.priorityLabel.Name = "priorityLabel";
            this.priorityLabel.Size = new System.Drawing.Size(28, 13);
            this.priorityLabel.TabIndex = 1;
            this.priorityLabel.Text = "Prio:";
            // 
            // priority
            // 
            this.priority.Appearance = System.Windows.Forms.Appearance.Button;
            this.priority.AutoSize = true;
            this.priority.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.priority.Image = global::Necrofy.Properties.Resources.fill_090;
            this.priority.Location = new System.Drawing.Point(112, 3);
            this.priority.Name = "priority";
            this.priority.Size = new System.Drawing.Size(22, 22);
            this.priority.TabIndex = 6;
            this.toolTip1.SetToolTip(this.priority, "Priority (p)");
            this.priority.UseVisualStyleBackColor = true;
            this.priority.CheckedChanged += new System.EventHandler(this.priority_CheckedChanged);
            // 
            // lockPriority
            // 
            this.lockPriority.Appearance = System.Windows.Forms.Appearance.Button;
            this.lockPriority.AutoSize = true;
            this.lockPriority.Checked = true;
            this.lockPriority.CheckState = System.Windows.Forms.CheckState.Checked;
            this.lockPriority.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.lockPriority.Image = global::Necrofy.Properties.Resources.fill_090;
            this.lockPriority.Location = new System.Drawing.Point(245, 3);
            this.lockPriority.Name = "lockPriority";
            this.lockPriority.Size = new System.Drawing.Size(22, 22);
            this.lockPriority.TabIndex = 10;
            this.toolTip1.SetToolTip(this.lockPriority, "Lock Priority (r)");
            this.lockPriority.UseVisualStyleBackColor = true;
            this.lockPriority.CheckedChanged += new System.EventHandler(this.lock_CheckedChanged);
            // 
            // lockFlip
            // 
            this.lockFlip.Appearance = System.Windows.Forms.Appearance.Button;
            this.lockFlip.AutoSize = true;
            this.lockFlip.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.lockFlip.Image = global::Necrofy.Properties.Resources.layer_flip;
            this.lockFlip.Location = new System.Drawing.Point(222, 3);
            this.lockFlip.Name = "lockFlip";
            this.lockFlip.Size = new System.Drawing.Size(22, 22);
            this.lockFlip.TabIndex = 9;
            this.toolTip1.SetToolTip(this.lockFlip, "Lock Flip (e)");
            this.lockFlip.UseVisualStyleBackColor = true;
            this.lockFlip.CheckedChanged += new System.EventHandler(this.lock_CheckedChanged);
            // 
            // lockPalette
            // 
            this.lockPalette.Appearance = System.Windows.Forms.Appearance.Button;
            this.lockPalette.AutoSize = true;
            this.lockPalette.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.lockPalette.Image = global::Necrofy.Properties.Resources.color;
            this.lockPalette.Location = new System.Drawing.Point(199, 3);
            this.lockPalette.Name = "lockPalette";
            this.lockPalette.Size = new System.Drawing.Size(22, 22);
            this.lockPalette.TabIndex = 8;
            this.toolTip1.SetToolTip(this.lockPalette, "Lock Palette (w)");
            this.lockPalette.UseVisualStyleBackColor = true;
            this.lockPalette.CheckedChanged += new System.EventHandler(this.lock_CheckedChanged);
            // 
            // lockTileNum
            // 
            this.lockTileNum.Appearance = System.Windows.Forms.Appearance.Button;
            this.lockTileNum.AutoSize = true;
            this.lockTileNum.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.lockTileNum.Image = global::Necrofy.Properties.Resources.image;
            this.lockTileNum.Location = new System.Drawing.Point(176, 3);
            this.lockTileNum.Name = "lockTileNum";
            this.lockTileNum.Size = new System.Drawing.Size(22, 22);
            this.lockTileNum.TabIndex = 7;
            this.toolTip1.SetToolTip(this.lockTileNum, "Lock Tile Number (q)");
            this.lockTileNum.UseVisualStyleBackColor = true;
            this.lockTileNum.CheckedChanged += new System.EventHandler(this.lock_CheckedChanged);
            // 
            // lockLabel
            // 
            this.lockLabel.AutoSize = true;
            this.lockLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.lockLabel.Location = new System.Drawing.Point(140, 8);
            this.lockLabel.Name = "lockLabel";
            this.lockLabel.Size = new System.Drawing.Size(34, 13);
            this.lockLabel.TabIndex = 7;
            this.lockLabel.Text = "Lock:";
            // 
            // flipLabel
            // 
            this.flipLabel.AutoSize = true;
            this.flipLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.flipLabel.Location = new System.Drawing.Point(3, 8);
            this.flipLabel.Name = "flipLabel";
            this.flipLabel.Size = new System.Drawing.Size(26, 13);
            this.flipLabel.TabIndex = 6;
            this.flipLabel.Text = "Flip:";
            // 
            // flipX
            // 
            this.flipX.Appearance = System.Windows.Forms.Appearance.Button;
            this.flipX.AutoSize = true;
            this.flipX.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.flipX.Image = global::Necrofy.Properties.Resources.layer_flip;
            this.flipX.Location = new System.Drawing.Point(31, 3);
            this.flipX.Name = "flipX";
            this.flipX.Size = new System.Drawing.Size(22, 22);
            this.flipX.TabIndex = 4;
            this.toolTip1.SetToolTip(this.flipX, "Flip Horizontally (x)");
            this.flipX.UseVisualStyleBackColor = true;
            this.flipX.CheckedChanged += new System.EventHandler(this.flipX_CheckedChanged);
            // 
            // flipY
            // 
            this.flipY.Appearance = System.Windows.Forms.Appearance.Button;
            this.flipY.AutoSize = true;
            this.flipY.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.flipY.Image = global::Necrofy.Properties.Resources.layer_flip_vertical;
            this.flipY.Location = new System.Drawing.Point(54, 3);
            this.flipY.Name = "flipY";
            this.flipY.Size = new System.Drawing.Size(22, 22);
            this.flipY.TabIndex = 5;
            this.toolTip1.SetToolTip(this.flipY, "Flip Vertically (y)");
            this.flipY.UseVisualStyleBackColor = true;
            this.flipY.CheckedChanged += new System.EventHandler(this.flipY_CheckedChanged);
            // 
            // paletteSelector
            // 
            this.paletteSelector.Dock = System.Windows.Forms.DockStyle.Top;
            this.paletteSelector.Location = new System.Drawing.Point(0, 21);
            this.paletteSelector.Name = "paletteSelector";
            this.paletteSelector.Size = new System.Drawing.Size(273, 21);
            this.paletteSelector.TabIndex = 1;
            this.paletteSelector.SelectedItemChanged += new System.EventHandler(this.paletteSelector_SelectedItemChanged);
            // 
            // graphicsSelector
            // 
            this.graphicsSelector.Dock = System.Windows.Forms.DockStyle.Top;
            this.graphicsSelector.Location = new System.Drawing.Point(0, 0);
            this.graphicsSelector.Name = "graphicsSelector";
            this.graphicsSelector.Size = new System.Drawing.Size(273, 21);
            this.graphicsSelector.TabIndex = 2;
            this.graphicsSelector.SelectedItemChanged += new System.EventHandler(this.graphicsSelector_SelectedItemChanged);
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
            this.canvas.KeyDown += new System.Windows.Forms.KeyEventHandler(this.canvas_KeyDown);
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
            this.optionsPanel.ResumeLayout(false);
            this.optionsPanel.PerformLayout();
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
        private NoAutoScalePanel optionsPanel;
        private System.Windows.Forms.CheckBox flipY;
        private System.Windows.Forms.CheckBox flipX;
        private System.Windows.Forms.CheckBox lockFlip;
        private System.Windows.Forms.CheckBox lockPalette;
        private System.Windows.Forms.CheckBox lockTileNum;
        private System.Windows.Forms.Label lockLabel;
        private System.Windows.Forms.Label flipLabel;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.CheckBox lockPriority;
        private System.Windows.Forms.Label priorityLabel;
        private System.Windows.Forms.CheckBox priority;
    }
}