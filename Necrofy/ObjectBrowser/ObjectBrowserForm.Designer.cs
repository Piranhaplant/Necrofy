namespace Necrofy
{
    partial class ObjectBrowserForm
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
            this.objectBrowser = new Necrofy.ObjectBrowserControl();
            this.SuspendLayout();
            // 
            // objectBrowser
            // 
            this.objectBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.objectBrowser.Location = new System.Drawing.Point(0, 0);
            this.objectBrowser.Name = "objectBrowser";
            this.objectBrowser.Size = new System.Drawing.Size(203, 476);
            this.objectBrowser.TabIndex = 0;
            // 
            // ObjectBrowserForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(203, 476);
            this.Controls.Add(this.objectBrowser);
            this.DockAreas = ((WeifenLuo.WinFormsUI.Docking.DockAreas)(((((WeifenLuo.WinFormsUI.Docking.DockAreas.Float | WeifenLuo.WinFormsUI.Docking.DockAreas.DockLeft) 
            | WeifenLuo.WinFormsUI.Docking.DockAreas.DockRight) 
            | WeifenLuo.WinFormsUI.Docking.DockAreas.DockTop) 
            | WeifenLuo.WinFormsUI.Docking.DockAreas.DockBottom)));
            this.HideOnClose = true;
            this.Name = "ObjectBrowserForm";
            this.Text = "Objects";
            this.ResumeLayout(false);

        }

        #endregion

        private ObjectBrowserControl objectBrowser;
    }
}