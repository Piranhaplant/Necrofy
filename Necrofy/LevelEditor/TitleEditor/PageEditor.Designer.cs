﻿namespace Necrofy
{
    partial class PageEditor
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
            this.SuspendLayout();
            // 
            // PageEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.DoubleBuffered = true;
            this.Name = "PageEditor";
            this.Size = new System.Drawing.Size(256, 224);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.PageEditor_Paint);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PageEditor_MouseDown);
            this.MouseLeave += new System.EventHandler(this.PageEditor_MouseLeave);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.PageEditor_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.PageEditor_MouseUp);
            this.ResumeLayout(false);

        }

        #endregion
    }
}