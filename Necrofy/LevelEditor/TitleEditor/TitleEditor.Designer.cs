namespace Necrofy
{
    partial class TitleEditor
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.pageEditor1 = new Necrofy.PageEditor();
            this.pageEditor2 = new Necrofy.PageEditor();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.panel1.Controls.Add(this.pageEditor2);
            this.panel1.Controls.Add(this.pageEditor1);
            this.panel1.Location = new System.Drawing.Point(100, 100);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(518, 224);
            this.panel1.TabIndex = 0;
            // 
            // pageEditor1
            // 
            this.pageEditor1.BackColor = System.Drawing.Color.Black;
            this.pageEditor1.Location = new System.Drawing.Point(0, 0);
            this.pageEditor1.Name = "pageEditor1";
            this.pageEditor1.Size = new System.Drawing.Size(256, 224);
            this.pageEditor1.TabIndex = 0;
            // 
            // pageEditor2
            // 
            this.pageEditor2.BackColor = System.Drawing.Color.Black;
            this.pageEditor2.Location = new System.Drawing.Point(262, 0);
            this.pageEditor2.Name = "pageEditor2";
            this.pageEditor2.Size = new System.Drawing.Size(256, 224);
            this.pageEditor2.TabIndex = 1;
            // 
            // TitleEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.ClientSize = new System.Drawing.Size(718, 424);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "TitleEditor";
            this.Text = "TitleEditor";
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private PageEditor pageEditor2;
        private PageEditor pageEditor1;
    }
}