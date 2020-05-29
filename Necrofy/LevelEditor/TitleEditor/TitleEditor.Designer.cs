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
            this.screenBounds = new System.Windows.Forms.Panel();
            this.pageEditor2 = new Necrofy.PageEditor();
            this.pageEditor1 = new Necrofy.PageEditor();
            this.displayName = new System.Windows.Forms.TextBox();
            this.displayNameLbl = new System.Windows.Forms.Label();
            this.paletteLbl = new System.Windows.Forms.Label();
            this.palette = new Necrofy.PaletteComboBox();
            this.applyToAll = new System.Windows.Forms.CheckBox();
            this.mainPanel = new System.Windows.Forms.Panel();
            this.screenBounds.SuspendLayout();
            this.mainPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // screenBounds
            // 
            this.screenBounds.Controls.Add(this.pageEditor2);
            this.screenBounds.Controls.Add(this.pageEditor1);
            this.screenBounds.Location = new System.Drawing.Point(0, 0);
            this.screenBounds.Name = "screenBounds";
            this.screenBounds.Size = new System.Drawing.Size(518, 224);
            this.screenBounds.TabIndex = 0;
            // 
            // pageEditor2
            // 
            this.pageEditor2.BackColor = System.Drawing.Color.Black;
            this.pageEditor2.Location = new System.Drawing.Point(262, 0);
            this.pageEditor2.Name = "pageEditor2";
            this.pageEditor2.Size = new System.Drawing.Size(256, 224);
            this.pageEditor2.TabIndex = 1;
            // 
            // pageEditor1
            // 
            this.pageEditor1.BackColor = System.Drawing.Color.Black;
            this.pageEditor1.Location = new System.Drawing.Point(0, 0);
            this.pageEditor1.Name = "pageEditor1";
            this.pageEditor1.Size = new System.Drawing.Size(256, 224);
            this.pageEditor1.TabIndex = 0;
            // 
            // displayName
            // 
            this.displayName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.displayName.Location = new System.Drawing.Point(93, 18);
            this.displayName.Name = "displayName";
            this.displayName.Size = new System.Drawing.Size(404, 20);
            this.displayName.TabIndex = 1;
            this.displayName.TextChanged += new System.EventHandler(this.displayName_TextChanged);
            // 
            // displayNameLbl
            // 
            this.displayNameLbl.AutoSize = true;
            this.displayNameLbl.Location = new System.Drawing.Point(12, 21);
            this.displayNameLbl.Name = "displayNameLbl";
            this.displayNameLbl.Size = new System.Drawing.Size(75, 13);
            this.displayNameLbl.TabIndex = 2;
            this.displayNameLbl.Text = "Display Name:";
            // 
            // paletteLbl
            // 
            this.paletteLbl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.paletteLbl.AutoSize = true;
            this.paletteLbl.Location = new System.Drawing.Point(503, 21);
            this.paletteLbl.Name = "paletteLbl";
            this.paletteLbl.Size = new System.Drawing.Size(43, 13);
            this.paletteLbl.TabIndex = 3;
            this.paletteLbl.Text = "Palette:";
            // 
            // palette
            // 
            this.palette.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.palette.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.palette.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.palette.FormattingEnabled = true;
            this.palette.ItemHeight = 48;
            this.palette.Location = new System.Drawing.Point(552, 0);
            this.palette.Name = "palette";
            this.palette.SelectedPalette = ((byte)(255));
            this.palette.Size = new System.Drawing.Size(71, 54);
            this.palette.TabIndex = 4;
            this.palette.SelectedIndexChanged += new System.EventHandler(this.palette_SelectedIndexChanged);
            // 
            // applyToAll
            // 
            this.applyToAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.applyToAll.AutoSize = true;
            this.applyToAll.Checked = true;
            this.applyToAll.CheckState = System.Windows.Forms.CheckState.Checked;
            this.applyToAll.Location = new System.Drawing.Point(629, 20);
            this.applyToAll.Name = "applyToAll";
            this.applyToAll.Size = new System.Drawing.Size(77, 17);
            this.applyToAll.TabIndex = 5;
            this.applyToAll.Text = "Apply to all";
            this.applyToAll.UseVisualStyleBackColor = true;
            this.applyToAll.CheckedChanged += new System.EventHandler(this.applyToAll_CheckedChanged);
            // 
            // mainPanel
            // 
            this.mainPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mainPanel.AutoScroll = true;
            this.mainPanel.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.mainPanel.Controls.Add(this.screenBounds);
            this.mainPanel.Location = new System.Drawing.Point(0, 54);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.Size = new System.Drawing.Size(718, 370);
            this.mainPanel.TabIndex = 0;
            this.mainPanel.SizeChanged += new System.EventHandler(this.mainPanel_SizeChanged);
            // 
            // TitleEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(718, 424);
            this.Controls.Add(this.mainPanel);
            this.Controls.Add(this.applyToAll);
            this.Controls.Add(this.palette);
            this.Controls.Add(this.paletteLbl);
            this.Controls.Add(this.displayNameLbl);
            this.Controls.Add(this.displayName);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "TitleEditor";
            this.Text = "TitleEditor";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.TitleEditor_FormClosed);
            this.screenBounds.ResumeLayout(false);
            this.mainPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel screenBounds;
        private PageEditor pageEditor2;
        private PageEditor pageEditor1;
        private System.Windows.Forms.TextBox displayName;
        private System.Windows.Forms.Label displayNameLbl;
        private System.Windows.Forms.Label paletteLbl;
        private PaletteComboBox palette;
        private System.Windows.Forms.CheckBox applyToAll;
        private System.Windows.Forms.Panel mainPanel;
    }
}