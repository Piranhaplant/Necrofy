namespace Necrofy
{
    partial class LevelMonsterRow
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
            this.alignment4 = new Necrofy.Canvas();
            this.alignment2 = new Necrofy.Canvas();
            this.alignment3 = new Necrofy.Canvas();
            this.alignment1 = new Necrofy.Canvas();
            this.SuspendLayout();
            // 
            // alignment4
            // 
            this.alignment4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.alignment4.Location = new System.Drawing.Point(336, 0);
            this.alignment4.Name = "alignment4";
            this.alignment4.Size = new System.Drawing.Size(1, 23);
            this.alignment4.TabIndex = 3;
            // 
            // alignment2
            // 
            this.alignment2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.alignment2.Location = new System.Drawing.Point(168, 0);
            this.alignment2.Name = "alignment2";
            this.alignment2.Size = new System.Drawing.Size(1, 23);
            this.alignment2.TabIndex = 2;
            // 
            // alignment3
            // 
            this.alignment3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.alignment3.Location = new System.Drawing.Point(252, 0);
            this.alignment3.Name = "alignment3";
            this.alignment3.Size = new System.Drawing.Size(1, 23);
            this.alignment3.TabIndex = 1;
            // 
            // alignment1
            // 
            this.alignment1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.alignment1.Location = new System.Drawing.Point(84, 0);
            this.alignment1.Name = "alignment1";
            this.alignment1.Size = new System.Drawing.Size(1, 23);
            this.alignment1.TabIndex = 0;
            // 
            // LevelMonsterRow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.alignment4);
            this.Controls.Add(this.alignment2);
            this.Controls.Add(this.alignment3);
            this.Controls.Add(this.alignment1);
            this.Name = "LevelMonsterRow";
            this.Size = new System.Drawing.Size(421, 21);
            this.Load += new System.EventHandler(this.LevelMonsterRow_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private Canvas alignment1;
        private Canvas alignment3;
        private Canvas alignment2;
        private Canvas alignment4;
    }
}
