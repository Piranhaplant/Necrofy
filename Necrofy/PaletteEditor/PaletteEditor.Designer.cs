namespace Necrofy
{
    partial class PaletteEditor
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
            this.colorEditor = new Necrofy.ColorEditor();
            this.colorSelector = new Necrofy.ColorSelector();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.lblScratchPad = new System.Windows.Forms.Label();
            this.scratchPad = new Necrofy.ColorSelector();
            this.flowLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // colorEditor
            // 
            this.colorEditor.Location = new System.Drawing.Point(344, 3);
            this.colorEditor.Name = "colorEditor";
            this.colorEditor.Size = new System.Drawing.Size(288, 378);
            this.colorEditor.TabIndex = 0;
            this.colorEditor.ColorChanged += new System.EventHandler(this.ColorEditor_ColorChanged);
            // 
            // colorSelector
            // 
            this.colorSelector.BackColor = System.Drawing.SystemColors.ControlDark;
            this.colorSelector.Colors = null;
            this.colorSelector.Location = new System.Drawing.Point(3, 3);
            this.colorSelector.MultiSelect = true;
            this.colorSelector.Name = "colorSelector";
            this.colorSelector.Size = new System.Drawing.Size(329, 175);
            this.colorSelector.TabIndex = 1;
            this.colorSelector.SelectionChanged += new System.EventHandler(this.ColorSelector_SelectionChanged);
            this.colorSelector.Enter += new System.EventHandler(this.colorSelector_Enter);
            this.colorSelector.Leave += new System.EventHandler(this.colorSelector_Leave);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoScroll = true;
            this.flowLayoutPanel1.Controls.Add(this.flowLayoutPanel2);
            this.flowLayoutPanel1.Controls.Add(this.colorEditor);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(1281, 755);
            this.flowLayoutPanel1.TabIndex = 2;
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.AutoSize = true;
            this.flowLayoutPanel2.Controls.Add(this.colorSelector);
            this.flowLayoutPanel2.Controls.Add(this.lblScratchPad);
            this.flowLayoutPanel2.Controls.Add(this.scratchPad);
            this.flowLayoutPanel2.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel2.Location = new System.Drawing.Point(3, 3);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(335, 375);
            this.flowLayoutPanel2.TabIndex = 2;
            // 
            // lblScratchPad
            // 
            this.lblScratchPad.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblScratchPad.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblScratchPad.Location = new System.Drawing.Point(3, 181);
            this.lblScratchPad.Name = "lblScratchPad";
            this.lblScratchPad.Size = new System.Drawing.Size(329, 13);
            this.lblScratchPad.TabIndex = 2;
            this.lblScratchPad.Text = "▼ Scratch Pad";
            this.lblScratchPad.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblScratchPad.Click += new System.EventHandler(this.lblScratchPad_Click);
            // 
            // scratchPad
            // 
            this.scratchPad.BackColor = System.Drawing.SystemColors.ControlDark;
            this.scratchPad.Colors = null;
            this.scratchPad.Location = new System.Drawing.Point(3, 197);
            this.scratchPad.MultiSelect = true;
            this.scratchPad.Name = "scratchPad";
            this.scratchPad.Size = new System.Drawing.Size(329, 175);
            this.scratchPad.TabIndex = 3;
            this.scratchPad.Visible = false;
            this.scratchPad.SelectionChanged += new System.EventHandler(this.ColorSelector_SelectionChanged);
            this.scratchPad.Enter += new System.EventHandler(this.colorSelector_Enter);
            this.scratchPad.Leave += new System.EventHandler(this.colorSelector_Leave);
            // 
            // PaletteEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1281, 755);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Name = "PaletteEditor";
            this.Text = "PaletteEditor";
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.flowLayoutPanel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private ColorEditor colorEditor;
        private ColorSelector colorSelector;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.Label lblScratchPad;
        private ColorSelector scratchPad;
    }
}