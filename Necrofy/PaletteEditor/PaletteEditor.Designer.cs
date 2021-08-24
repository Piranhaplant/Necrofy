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
            this.SuspendLayout();
            // 
            // colorEditor
            // 
            this.colorEditor.Location = new System.Drawing.Point(306, 12);
            this.colorEditor.Name = "colorEditor";
            this.colorEditor.Size = new System.Drawing.Size(288, 345);
            this.colorEditor.TabIndex = 0;
            // 
            // colorSelector
            // 
            this.colorSelector.BackColor = System.Drawing.SystemColors.ControlDark;
            this.colorSelector.Colors = null;
            this.colorSelector.Location = new System.Drawing.Point(12, 12);
            this.colorSelector.Name = "colorSelector";
            this.colorSelector.Size = new System.Drawing.Size(288, 345);
            this.colorSelector.TabIndex = 1;
            // 
            // PaletteEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.colorSelector);
            this.Controls.Add(this.colorEditor);
            this.Name = "PaletteEditor";
            this.Text = "PaletteEditor";
            this.ResumeLayout(false);

        }

        #endregion

        private ColorEditor colorEditor;
        private ColorSelector colorSelector;
    }
}