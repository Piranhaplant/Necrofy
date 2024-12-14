namespace Necrofy
{
    partial class SpriteTilePicker
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
            this.vScrollBar = new System.Windows.Forms.VScrollBar();
            this.hScrollBar = new System.Windows.Forms.HScrollBar();
            this.canvas = new Necrofy.Canvas();
            this.palettePanel = new Necrofy.NoAutoScalePanel();
            this.palette7Button = new System.Windows.Forms.RadioButton();
            this.palette6Button = new System.Windows.Forms.RadioButton();
            this.palette5Button = new System.Windows.Forms.RadioButton();
            this.palette4Button = new System.Windows.Forms.RadioButton();
            this.palette3Button = new System.Windows.Forms.RadioButton();
            this.palette2Button = new System.Windows.Forms.RadioButton();
            this.palette1Button = new System.Windows.Forms.RadioButton();
            this.palette0Button = new System.Windows.Forms.RadioButton();
            this.paletteLabel = new System.Windows.Forms.Label();
            this.palettePanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // vScrollBar
            // 
            this.vScrollBar.Dock = System.Windows.Forms.DockStyle.Right;
            this.vScrollBar.LargeChange = 100;
            this.vScrollBar.Location = new System.Drawing.Point(256, 29);
            this.vScrollBar.Name = "vScrollBar";
            this.vScrollBar.Size = new System.Drawing.Size(17, 442);
            this.vScrollBar.SmallChange = 32;
            this.vScrollBar.TabIndex = 3;
            // 
            // hScrollBar
            // 
            this.hScrollBar.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.hScrollBar.Location = new System.Drawing.Point(0, 471);
            this.hScrollBar.Name = "hScrollBar";
            this.hScrollBar.Size = new System.Drawing.Size(273, 17);
            this.hScrollBar.TabIndex = 5;
            this.hScrollBar.Visible = false;
            // 
            // canvas
            // 
            this.canvas.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.canvas.Dock = System.Windows.Forms.DockStyle.Fill;
            this.canvas.IsMouseDown = false;
            this.canvas.Location = new System.Drawing.Point(0, 29);
            this.canvas.Name = "canvas";
            this.canvas.Size = new System.Drawing.Size(256, 442);
            this.canvas.TabIndex = 4;
            this.canvas.MouseDown += new System.Windows.Forms.MouseEventHandler(this.canvas_MouseDown);
            this.canvas.SizeChanged += new System.EventHandler(this.canvas_SizeChanged);
            this.canvas.Paint += new System.Windows.Forms.PaintEventHandler(this.canvas_Paint);
            this.canvas.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.canvas_MouseDoubleClick);
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
            this.palettePanel.TabIndex = 9;
            // 
            // palette7Button
            // 
            this.palette7Button.Appearance = System.Windows.Forms.Appearance.Button;
            this.palette7Button.AutoSize = true;
            this.palette7Button.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
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
            this.palette6Button.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
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
            this.palette5Button.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
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
            this.palette4Button.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
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
            this.palette3Button.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
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
            this.palette2Button.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
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
            this.palette1Button.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
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
            this.palette0Button.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
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
            this.paletteLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.paletteLabel.Location = new System.Drawing.Point(1, 8);
            this.paletteLabel.Name = "paletteLabel";
            this.paletteLabel.Size = new System.Drawing.Size(43, 13);
            this.paletteLabel.TabIndex = 0;
            this.paletteLabel.Text = "Palette:";
            // 
            // SpriteTilePicker
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.canvas);
            this.Controls.Add(this.vScrollBar);
            this.Controls.Add(this.hScrollBar);
            this.Controls.Add(this.palettePanel);
            this.Name = "SpriteTilePicker";
            this.Size = new System.Drawing.Size(273, 488);
            this.palettePanel.ResumeLayout(false);
            this.palettePanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Canvas canvas;
        private System.Windows.Forms.VScrollBar vScrollBar;
        private System.Windows.Forms.HScrollBar hScrollBar;
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
