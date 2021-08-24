namespace Necrofy
{
    partial class ColorEditor
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
            this.rTrackBar = new System.Windows.Forms.TrackBar();
            this.rNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.rLabel = new System.Windows.Forms.Label();
            this.gLabel = new System.Windows.Forms.Label();
            this.gNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.gTrackBar = new System.Windows.Forms.TrackBar();
            this.bLabel = new System.Windows.Forms.Label();
            this.bNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.bTrackBar = new System.Windows.Forms.TrackBar();
            this.hsCanvas = new Necrofy.Canvas();
            this.lCanvas = new Necrofy.Canvas();
            ((System.ComponentModel.ISupportInitialize)(this.rTrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gTrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bTrackBar)).BeginInit();
            this.SuspendLayout();
            // 
            // rTrackBar
            // 
            this.rTrackBar.AutoSize = false;
            this.rTrackBar.Location = new System.Drawing.Point(21, 257);
            this.rTrackBar.Maximum = 31;
            this.rTrackBar.Name = "rTrackBar";
            this.rTrackBar.Size = new System.Drawing.Size(203, 25);
            this.rTrackBar.TabIndex = 0;
            this.rTrackBar.TickStyle = System.Windows.Forms.TickStyle.None;
            this.rTrackBar.ValueChanged += new System.EventHandler(this.rTrackBar_ValueChanged);
            // 
            // rNumericUpDown
            // 
            this.rNumericUpDown.Location = new System.Drawing.Point(230, 257);
            this.rNumericUpDown.Maximum = new decimal(new int[] {
            31,
            0,
            0,
            0});
            this.rNumericUpDown.Name = "rNumericUpDown";
            this.rNumericUpDown.Size = new System.Drawing.Size(42, 20);
            this.rNumericUpDown.TabIndex = 1;
            this.rNumericUpDown.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.rNumericUpDown.ValueChanged += new System.EventHandler(this.numericUpDown_ValueChanged);
            // 
            // rLabel
            // 
            this.rLabel.AutoSize = true;
            this.rLabel.Location = new System.Drawing.Point(0, 259);
            this.rLabel.Name = "rLabel";
            this.rLabel.Size = new System.Drawing.Size(15, 13);
            this.rLabel.TabIndex = 3;
            this.rLabel.Text = "R";
            // 
            // gLabel
            // 
            this.gLabel.AutoSize = true;
            this.gLabel.Location = new System.Drawing.Point(0, 290);
            this.gLabel.Name = "gLabel";
            this.gLabel.Size = new System.Drawing.Size(15, 13);
            this.gLabel.TabIndex = 7;
            this.gLabel.Text = "G";
            // 
            // gNumericUpDown
            // 
            this.gNumericUpDown.Location = new System.Drawing.Point(230, 288);
            this.gNumericUpDown.Maximum = new decimal(new int[] {
            31,
            0,
            0,
            0});
            this.gNumericUpDown.Name = "gNumericUpDown";
            this.gNumericUpDown.Size = new System.Drawing.Size(42, 20);
            this.gNumericUpDown.TabIndex = 6;
            this.gNumericUpDown.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.gNumericUpDown.ValueChanged += new System.EventHandler(this.numericUpDown_ValueChanged);
            // 
            // gTrackBar
            // 
            this.gTrackBar.AutoSize = false;
            this.gTrackBar.Location = new System.Drawing.Point(21, 288);
            this.gTrackBar.Maximum = 31;
            this.gTrackBar.Name = "gTrackBar";
            this.gTrackBar.Size = new System.Drawing.Size(203, 25);
            this.gTrackBar.TabIndex = 5;
            this.gTrackBar.TickStyle = System.Windows.Forms.TickStyle.None;
            this.gTrackBar.ValueChanged += new System.EventHandler(this.gTrackBar_ValueChanged);
            // 
            // bLabel
            // 
            this.bLabel.AutoSize = true;
            this.bLabel.Location = new System.Drawing.Point(0, 321);
            this.bLabel.Name = "bLabel";
            this.bLabel.Size = new System.Drawing.Size(14, 13);
            this.bLabel.TabIndex = 10;
            this.bLabel.Text = "B";
            // 
            // bNumericUpDown
            // 
            this.bNumericUpDown.Location = new System.Drawing.Point(230, 319);
            this.bNumericUpDown.Maximum = new decimal(new int[] {
            31,
            0,
            0,
            0});
            this.bNumericUpDown.Name = "bNumericUpDown";
            this.bNumericUpDown.Size = new System.Drawing.Size(42, 20);
            this.bNumericUpDown.TabIndex = 9;
            this.bNumericUpDown.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.bNumericUpDown.ValueChanged += new System.EventHandler(this.numericUpDown_ValueChanged);
            // 
            // bTrackBar
            // 
            this.bTrackBar.AutoSize = false;
            this.bTrackBar.Location = new System.Drawing.Point(21, 319);
            this.bTrackBar.Maximum = 31;
            this.bTrackBar.Name = "bTrackBar";
            this.bTrackBar.Size = new System.Drawing.Size(203, 25);
            this.bTrackBar.TabIndex = 8;
            this.bTrackBar.TickStyle = System.Windows.Forms.TickStyle.None;
            this.bTrackBar.ValueChanged += new System.EventHandler(this.bTrackBar_ValueChanged);
            // 
            // hsCanvas
            // 
            this.hsCanvas.BackgroundImage = global::Necrofy.Properties.Resources.spectrum1;
            this.hsCanvas.IsMouseDown = false;
            this.hsCanvas.Location = new System.Drawing.Point(3, 10);
            this.hsCanvas.Name = "hsCanvas";
            this.hsCanvas.Size = new System.Drawing.Size(241, 241);
            this.hsCanvas.TabIndex = 2;
            this.hsCanvas.MouseDown += new System.Windows.Forms.MouseEventHandler(this.hsCanvas_MouseDown);
            this.hsCanvas.MouseMove += new System.Windows.Forms.MouseEventHandler(this.hsCanvas_MouseMove);
            this.hsCanvas.Paint += new System.Windows.Forms.PaintEventHandler(this.hsCanvas_Paint);
            // 
            // lCanvas
            // 
            this.lCanvas.IsMouseDown = false;
            this.lCanvas.Location = new System.Drawing.Point(257, 0);
            this.lCanvas.Name = "lCanvas";
            this.lCanvas.Size = new System.Drawing.Size(31, 261);
            this.lCanvas.TabIndex = 4;
            this.lCanvas.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lCanvas_MouseDown);
            this.lCanvas.MouseMove += new System.Windows.Forms.MouseEventHandler(this.lCanvas_MouseMove);
            this.lCanvas.Paint += new System.Windows.Forms.PaintEventHandler(this.lCanvas_Paint);
            // 
            // ColorEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.bLabel);
            this.Controls.Add(this.bNumericUpDown);
            this.Controls.Add(this.bTrackBar);
            this.Controls.Add(this.gLabel);
            this.Controls.Add(this.gNumericUpDown);
            this.Controls.Add(this.gTrackBar);
            this.Controls.Add(this.rLabel);
            this.Controls.Add(this.hsCanvas);
            this.Controls.Add(this.rNumericUpDown);
            this.Controls.Add(this.rTrackBar);
            this.Controls.Add(this.lCanvas);
            this.Name = "ColorEditor";
            this.Size = new System.Drawing.Size(288, 345);
            ((System.ComponentModel.ISupportInitialize)(this.rTrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gTrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bTrackBar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TrackBar rTrackBar;
        private System.Windows.Forms.NumericUpDown rNumericUpDown;
        private Canvas hsCanvas;
        private System.Windows.Forms.Label rLabel;
        private Canvas lCanvas;
        private System.Windows.Forms.Label gLabel;
        private System.Windows.Forms.NumericUpDown gNumericUpDown;
        private System.Windows.Forms.TrackBar gTrackBar;
        private System.Windows.Forms.Label bLabel;
        private System.Windows.Forms.NumericUpDown bNumericUpDown;
        private System.Windows.Forms.TrackBar bTrackBar;
    }
}
