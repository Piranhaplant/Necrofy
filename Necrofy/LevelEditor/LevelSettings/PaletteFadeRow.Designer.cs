namespace Necrofy
{
    partial class PaletteFadeRow
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
            this.tilesetPaletteLbl = new System.Windows.Forms.Label();
            this.tilesetPaletteSelector = new AssetComboBox();
            this.titleLabel = new System.Windows.Forms.Label();
            this.spritePaletteLbl = new System.Windows.Forms.Label();
            this.spritePaletteSelector = new AssetComboBox();
            this.SuspendLayout();
            // 
            // tilesetPaletteLbl
            // 
            this.tilesetPaletteLbl.AutoSize = true;
            this.tilesetPaletteLbl.Location = new System.Drawing.Point(91, 2);
            this.tilesetPaletteLbl.Name = "tilesetPaletteLbl";
            this.tilesetPaletteLbl.Size = new System.Drawing.Size(77, 13);
            this.tilesetPaletteLbl.TabIndex = 1;
            this.tilesetPaletteLbl.Text = "Tileset Palette:";
            // 
            // tilesetPaletteSelector
            // 
            this.tilesetPaletteSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.tilesetPaletteSelector.FormattingEnabled = true;
            this.tilesetPaletteSelector.Location = new System.Drawing.Point(168, -1);
            this.tilesetPaletteSelector.Name = "tilesetPaletteSelector";
            this.tilesetPaletteSelector.Size = new System.Drawing.Size(85, 21);
            this.tilesetPaletteSelector.TabIndex = 2;
            // 
            // titleLabel
            // 
            this.titleLabel.AutoSize = true;
            this.titleLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.titleLabel.Location = new System.Drawing.Point(-1, 2);
            this.titleLabel.Name = "titleLabel";
            this.titleLabel.Size = new System.Drawing.Size(79, 13);
            this.titleLabel.TabIndex = 4;
            this.titleLabel.Text = "Palette Fade";
            // 
            // spritePaletteLbl
            // 
            this.spritePaletteLbl.AutoSize = true;
            this.spritePaletteLbl.Location = new System.Drawing.Point(259, 2);
            this.spritePaletteLbl.Name = "spritePaletteLbl";
            this.spritePaletteLbl.Size = new System.Drawing.Size(73, 13);
            this.spritePaletteLbl.TabIndex = 5;
            this.spritePaletteLbl.Text = "Sprite Palette:";
            // 
            // spritePaletteSelector
            // 
            this.spritePaletteSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.spritePaletteSelector.FormattingEnabled = true;
            this.spritePaletteSelector.Location = new System.Drawing.Point(336, -1);
            this.spritePaletteSelector.Name = "spritePaletteSelector";
            this.spritePaletteSelector.Size = new System.Drawing.Size(84, 21);
            this.spritePaletteSelector.TabIndex = 6;
            // 
            // PaletteFadeRow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.spritePaletteSelector);
            this.Controls.Add(this.spritePaletteLbl);
            this.Controls.Add(this.titleLabel);
            this.Controls.Add(this.tilesetPaletteSelector);
            this.Controls.Add(this.tilesetPaletteLbl);
            this.Name = "PaletteFadeRow";
            this.Controls.SetChildIndex(this.tilesetPaletteLbl, 0);
            this.Controls.SetChildIndex(this.tilesetPaletteSelector, 0);
            this.Controls.SetChildIndex(this.titleLabel, 0);
            this.Controls.SetChildIndex(this.spritePaletteLbl, 0);
            this.Controls.SetChildIndex(this.spritePaletteSelector, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label tilesetPaletteLbl;
        private AssetComboBox tilesetPaletteSelector;
        private System.Windows.Forms.Label titleLabel;
        private System.Windows.Forms.Label spritePaletteLbl;
        private AssetComboBox spritePaletteSelector;
    }
}
