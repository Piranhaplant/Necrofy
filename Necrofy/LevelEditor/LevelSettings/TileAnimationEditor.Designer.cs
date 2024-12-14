namespace Necrofy
{
    partial class TileAnimationEditor
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            this.sidePanel = new System.Windows.Forms.Panel();
            this.tilePicker = new Necrofy.SpriteTilePicker();
            this.previewCanvas = new Necrofy.Canvas();
            this.previewControlsPanel = new System.Windows.Forms.Panel();
            this.closeButton = new System.Windows.Forms.Button();
            this.stopButton = new System.Windows.Forms.Button();
            this.advanceFrameButton = new System.Windows.Forms.Button();
            this.playPauseButton = new System.Windows.Forms.Button();
            this.restartButton = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.mainPanel = new System.Windows.Forms.Panel();
            this.mainCanvas = new Necrofy.Canvas();
            this.vScroll = new System.Windows.Forms.VScrollBar();
            this.hScroll = new System.Windows.Forms.HScrollBar();
            this.timeBar = new Necrofy.Canvas();
            this.sidePanel.SuspendLayout();
            this.previewControlsPanel.SuspendLayout();
            this.mainPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // sidePanel
            // 
            this.sidePanel.Controls.Add(this.tilePicker);
            this.sidePanel.Controls.Add(this.previewCanvas);
            this.sidePanel.Controls.Add(this.previewControlsPanel);
            this.sidePanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.sidePanel.Location = new System.Drawing.Point(0, 0);
            this.sidePanel.Name = "sidePanel";
            this.sidePanel.Size = new System.Drawing.Size(273, 503);
            this.sidePanel.TabIndex = 0;
            // 
            // tilePicker
            // 
            this.tilePicker.ColorsPerPalette = 16;
            this.tilePicker.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tilePicker.FlipX = false;
            this.tilePicker.FlipY = false;
            this.tilePicker.Location = new System.Drawing.Point(0, 0);
            this.tilePicker.Name = "tilePicker";
            this.tilePicker.Palette = 0;
            this.tilePicker.PalettePerTile = null;
            this.tilePicker.SelectedTile = -1;
            this.tilePicker.Size = new System.Drawing.Size(273, 201);
            this.tilePicker.TabIndex = 1;
            this.tilePicker.SelectedTileChanged += new Necrofy.SpriteTilePicker.SelectedTileChangedDelegate(this.tilePicker_SelectedTileChanged);
            // 
            // previewCanvas
            // 
            this.previewCanvas.BackColor = System.Drawing.SystemColors.ControlDark;
            this.previewCanvas.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.previewCanvas.IsMouseDown = false;
            this.previewCanvas.Location = new System.Drawing.Point(0, 201);
            this.previewCanvas.Name = "previewCanvas";
            this.previewCanvas.Size = new System.Drawing.Size(273, 273);
            this.previewCanvas.TabIndex = 1;
            this.previewCanvas.MouseDown += new System.Windows.Forms.MouseEventHandler(this.previewCanvas_MouseDown);
            this.previewCanvas.MouseMove += new System.Windows.Forms.MouseEventHandler(this.previewCanvas_MouseMove);
            this.previewCanvas.Paint += new System.Windows.Forms.PaintEventHandler(this.previewCanvas_Paint);
            // 
            // previewControlsPanel
            // 
            this.previewControlsPanel.Controls.Add(this.closeButton);
            this.previewControlsPanel.Controls.Add(this.stopButton);
            this.previewControlsPanel.Controls.Add(this.advanceFrameButton);
            this.previewControlsPanel.Controls.Add(this.playPauseButton);
            this.previewControlsPanel.Controls.Add(this.restartButton);
            this.previewControlsPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.previewControlsPanel.Location = new System.Drawing.Point(0, 474);
            this.previewControlsPanel.Name = "previewControlsPanel";
            this.previewControlsPanel.Size = new System.Drawing.Size(273, 29);
            this.previewControlsPanel.TabIndex = 0;
            // 
            // closeButton
            // 
            this.closeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.closeButton.Location = new System.Drawing.Point(161, 3);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(109, 23);
            this.closeButton.TabIndex = 2;
            this.closeButton.Text = "Return to settings";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            // 
            // stopButton
            // 
            this.stopButton.Image = global::Necrofy.Properties.Resources.control_stop_square;
            this.stopButton.Location = new System.Drawing.Point(90, 3);
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(23, 23);
            this.stopButton.TabIndex = 5;
            this.toolTip1.SetToolTip(this.stopButton, "Stop");
            this.stopButton.UseVisualStyleBackColor = true;
            this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
            // 
            // advanceFrameButton
            // 
            this.advanceFrameButton.Image = global::Necrofy.Properties.Resources.control_stop;
            this.advanceFrameButton.Location = new System.Drawing.Point(61, 3);
            this.advanceFrameButton.Name = "advanceFrameButton";
            this.advanceFrameButton.Size = new System.Drawing.Size(23, 23);
            this.advanceFrameButton.TabIndex = 4;
            this.toolTip1.SetToolTip(this.advanceFrameButton, "Advance frame");
            this.advanceFrameButton.UseVisualStyleBackColor = true;
            this.advanceFrameButton.Click += new System.EventHandler(this.advanceFrameButton_Click);
            // 
            // playPauseButton
            // 
            this.playPauseButton.Image = global::Necrofy.Properties.Resources.control;
            this.playPauseButton.Location = new System.Drawing.Point(32, 3);
            this.playPauseButton.Name = "playPauseButton";
            this.playPauseButton.Size = new System.Drawing.Size(23, 23);
            this.playPauseButton.TabIndex = 3;
            this.toolTip1.SetToolTip(this.playPauseButton, "Play");
            this.playPauseButton.UseVisualStyleBackColor = true;
            this.playPauseButton.Click += new System.EventHandler(this.playPauseButton_Click);
            // 
            // restartButton
            // 
            this.restartButton.Image = global::Necrofy.Properties.Resources.control_skip_180;
            this.restartButton.Location = new System.Drawing.Point(3, 3);
            this.restartButton.Name = "restartButton";
            this.restartButton.Size = new System.Drawing.Size(23, 23);
            this.restartButton.TabIndex = 2;
            this.toolTip1.SetToolTip(this.restartButton, "Restart");
            this.restartButton.UseVisualStyleBackColor = true;
            this.restartButton.Click += new System.EventHandler(this.restartButton_Click);
            // 
            // mainPanel
            // 
            this.mainPanel.Controls.Add(this.mainCanvas);
            this.mainPanel.Controls.Add(this.vScroll);
            this.mainPanel.Controls.Add(this.hScroll);
            this.mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainPanel.Location = new System.Drawing.Point(273, 27);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.Size = new System.Drawing.Size(928, 476);
            this.mainPanel.TabIndex = 3;
            // 
            // mainCanvas
            // 
            this.mainCanvas.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mainCanvas.BackColor = System.Drawing.SystemColors.ControlDark;
            this.mainCanvas.IsMouseDown = false;
            this.mainCanvas.Location = new System.Drawing.Point(0, 0);
            this.mainCanvas.Name = "mainCanvas";
            this.mainCanvas.Size = new System.Drawing.Size(911, 459);
            this.mainCanvas.TabIndex = 6;
            this.mainCanvas.MouseDown += new System.Windows.Forms.MouseEventHandler(this.mainCanvas_MouseDown);
            this.mainCanvas.MouseMove += new System.Windows.Forms.MouseEventHandler(this.mainCanvas_MouseMove);
            this.mainCanvas.MouseUp += new System.Windows.Forms.MouseEventHandler(this.mainCanvas_MouseUp);
            this.mainCanvas.Paint += new System.Windows.Forms.PaintEventHandler(this.mainCanvas_Paint);
            this.mainCanvas.KeyDown += new System.Windows.Forms.KeyEventHandler(this.mainCanvas_KeyDown);
            this.mainCanvas.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.mainCanvas_PreviewKeyDown);
            // 
            // vScroll
            // 
            this.vScroll.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.vScroll.Location = new System.Drawing.Point(911, 0);
            this.vScroll.Name = "vScroll";
            this.vScroll.Size = new System.Drawing.Size(17, 459);
            this.vScroll.TabIndex = 5;
            // 
            // hScroll
            // 
            this.hScroll.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.hScroll.Location = new System.Drawing.Point(0, 459);
            this.hScroll.Name = "hScroll";
            this.hScroll.Size = new System.Drawing.Size(911, 17);
            this.hScroll.TabIndex = 4;
            // 
            // timeBar
            // 
            this.timeBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.timeBar.IsMouseDown = false;
            this.timeBar.Location = new System.Drawing.Point(273, 0);
            this.timeBar.Name = "timeBar";
            this.timeBar.Size = new System.Drawing.Size(928, 27);
            this.timeBar.TabIndex = 2;
            // 
            // TileAnimationEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.mainPanel);
            this.Controls.Add(this.timeBar);
            this.Controls.Add(this.sidePanel);
            this.Name = "TileAnimationEditor";
            this.Size = new System.Drawing.Size(1201, 503);
            this.sidePanel.ResumeLayout(false);
            this.previewControlsPanel.ResumeLayout(false);
            this.mainPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel sidePanel;
        private Canvas previewCanvas;
        private System.Windows.Forms.Panel previewControlsPanel;
        private System.Windows.Forms.Button restartButton;
        private System.Windows.Forms.Button stopButton;
        private System.Windows.Forms.Button advanceFrameButton;
        private System.Windows.Forms.Button playPauseButton;
        private System.Windows.Forms.ToolTip toolTip1;
        private SpriteTilePicker tilePicker;
        private Canvas timeBar;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.Panel mainPanel;
        private System.Windows.Forms.HScrollBar hScroll;
        private Canvas mainCanvas;
        private System.Windows.Forms.VScrollBar vScroll;
    }
}
