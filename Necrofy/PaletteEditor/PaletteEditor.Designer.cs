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
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // colorEditor
            // 
            this.colorEditor.Location = new System.Drawing.Point(297, 3);
            this.colorEditor.Name = "colorEditor";
            this.colorEditor.Size = new System.Drawing.Size(288, 378);
            this.colorEditor.TabIndex = 0;
            // 
            // colorSelector
            // 
            this.colorSelector.BackColor = System.Drawing.SystemColors.ControlDark;
            this.colorSelector.Colors = null;
            this.colorSelector.Location = new System.Drawing.Point(3, 3);
            this.colorSelector.Name = "colorSelector";
            this.colorSelector.Size = new System.Drawing.Size(288, 378);
            this.colorSelector.TabIndex = 1;
            this.colorSelector.Enter += new System.EventHandler(this.colorSelector_Enter);
            this.colorSelector.Leave += new System.EventHandler(this.colorSelector_Leave);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoScroll = true;
            this.flowLayoutPanel1.Controls.Add(this.colorSelector);
            this.flowLayoutPanel1.Controls.Add(this.colorEditor);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(1281, 755);
            this.flowLayoutPanel1.TabIndex = 2;
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
            this.ResumeLayout(false);

        }

        #endregion

        private ColorEditor colorEditor;
        private ColorSelector colorSelector;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
    }
}