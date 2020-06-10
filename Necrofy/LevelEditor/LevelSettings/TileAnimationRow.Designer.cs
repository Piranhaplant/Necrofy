namespace Necrofy
{
    partial class TileAnimationRow
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
            this.presetSelector = new System.Windows.Forms.ComboBox();
            this.titleLabel = new System.Windows.Forms.Label();
            this.presetLbl = new System.Windows.Forms.Label();
            this.customizeButton = new System.Windows.Forms.Button();
            this.customizeLbl = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // presetSelector
            // 
            this.presetSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.presetSelector.FormattingEnabled = true;
            this.presetSelector.Location = new System.Drawing.Point(131, -1);
            this.presetSelector.Name = "presetSelector";
            this.presetSelector.Size = new System.Drawing.Size(122, 21);
            this.presetSelector.TabIndex = 4;
            // 
            // titleLabel
            // 
            this.titleLabel.AutoSize = true;
            this.titleLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.titleLabel.Location = new System.Drawing.Point(-1, 2);
            this.titleLabel.Name = "titleLabel";
            this.titleLabel.Size = new System.Drawing.Size(87, 13);
            this.titleLabel.TabIndex = 5;
            this.titleLabel.Text = "Tile Animation";
            // 
            // presetLbl
            // 
            this.presetLbl.AutoSize = true;
            this.presetLbl.Location = new System.Drawing.Point(91, 2);
            this.presetLbl.Name = "presetLbl";
            this.presetLbl.Size = new System.Drawing.Size(40, 13);
            this.presetLbl.TabIndex = 6;
            this.presetLbl.Text = "Preset:";
            // 
            // customizeButton
            // 
            this.customizeButton.Location = new System.Drawing.Point(335, -2);
            this.customizeButton.Name = "customizeButton";
            this.customizeButton.Size = new System.Drawing.Size(86, 23);
            this.customizeButton.TabIndex = 7;
            this.customizeButton.Text = "Edit...";
            this.customizeButton.UseVisualStyleBackColor = true;
            this.customizeButton.Click += new System.EventHandler(this.customizeButton_Click);
            // 
            // customizeLbl
            // 
            this.customizeLbl.AutoSize = true;
            this.customizeLbl.Location = new System.Drawing.Point(259, 2);
            this.customizeLbl.Name = "customizeLbl";
            this.customizeLbl.Size = new System.Drawing.Size(58, 13);
            this.customizeLbl.TabIndex = 8;
            this.customizeLbl.Text = "Customize:";
            // 
            // TileAnimationRow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.customizeLbl);
            this.Controls.Add(this.customizeButton);
            this.Controls.Add(this.presetLbl);
            this.Controls.Add(this.titleLabel);
            this.Controls.Add(this.presetSelector);
            this.Name = "TileAnimationRow";
            this.Controls.SetChildIndex(this.presetSelector, 0);
            this.Controls.SetChildIndex(this.titleLabel, 0);
            this.Controls.SetChildIndex(this.presetLbl, 0);
            this.Controls.SetChildIndex(this.customizeButton, 0);
            this.Controls.SetChildIndex(this.customizeLbl, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox presetSelector;
        private System.Windows.Forms.Label titleLabel;
        private System.Windows.Forms.Label presetLbl;
        private System.Windows.Forms.Button customizeButton;
        private System.Windows.Forms.Label customizeLbl;
    }
}
