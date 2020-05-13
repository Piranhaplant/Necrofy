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
            this.manualButton = new System.Windows.Forms.Button();
            this.manualLbl = new System.Windows.Forms.Label();
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
            // manualButton
            // 
            this.manualButton.Location = new System.Drawing.Point(335, -2);
            this.manualButton.Name = "manualButton";
            this.manualButton.Size = new System.Drawing.Size(86, 23);
            this.manualButton.TabIndex = 7;
            this.manualButton.Text = "Edit...";
            this.manualButton.UseVisualStyleBackColor = true;
            this.manualButton.Click += new System.EventHandler(this.manualButton_Click);
            // 
            // manualLbl
            // 
            this.manualLbl.AutoSize = true;
            this.manualLbl.Location = new System.Drawing.Point(259, 2);
            this.manualLbl.Name = "manualLbl";
            this.manualLbl.Size = new System.Drawing.Size(45, 13);
            this.manualLbl.TabIndex = 8;
            this.manualLbl.Text = "Manual:";
            // 
            // TileAnimationRow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.manualLbl);
            this.Controls.Add(this.manualButton);
            this.Controls.Add(this.presetLbl);
            this.Controls.Add(this.titleLabel);
            this.Controls.Add(this.presetSelector);
            this.Name = "TileAnimationRow";
            this.Controls.SetChildIndex(this.presetSelector, 0);
            this.Controls.SetChildIndex(this.titleLabel, 0);
            this.Controls.SetChildIndex(this.presetLbl, 0);
            this.Controls.SetChildIndex(this.manualButton, 0);
            this.Controls.SetChildIndex(this.manualLbl, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox presetSelector;
        private System.Windows.Forms.Label titleLabel;
        private System.Windows.Forms.Label presetLbl;
        private System.Windows.Forms.Button manualButton;
        private System.Windows.Forms.Label manualLbl;
    }
}
