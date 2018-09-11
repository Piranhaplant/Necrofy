namespace Necrofy
{
    partial class LevelEditor
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
            this.canvas = new Necrofy.Canvas();
            this.hscroll = new System.Windows.Forms.HScrollBar();
            this.vscroll = new System.Windows.Forms.VScrollBar();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.itemsButton = new System.Windows.Forms.ToolStripButton();
            this.victimsButton = new System.Windows.Forms.ToolStripButton();
            this.oneShotMonstersButton = new System.Windows.Forms.ToolStripButton();
            this.monstersButton = new System.Windows.Forms.ToolStripButton();
            this.bossMonstersButton = new System.Windows.Forms.ToolStripButton();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // canvas
            // 
            this.canvas.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.canvas.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.canvas.Location = new System.Drawing.Point(0, 0);
            this.canvas.Name = "canvas";
            this.canvas.Size = new System.Drawing.Size(283, 283);
            this.canvas.TabIndex = 0;
            this.canvas.Paint += new System.Windows.Forms.PaintEventHandler(this.canvas_Paint);
            // 
            // hscroll
            // 
            this.hscroll.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.hscroll.LargeChange = 100;
            this.hscroll.Location = new System.Drawing.Point(0, 283);
            this.hscroll.Name = "hscroll";
            this.hscroll.Size = new System.Drawing.Size(283, 17);
            this.hscroll.SmallChange = 16;
            this.hscroll.TabIndex = 1;
            // 
            // vscroll
            // 
            this.vscroll.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.vscroll.LargeChange = 100;
            this.vscroll.Location = new System.Drawing.Point(283, 0);
            this.vscroll.Name = "vscroll";
            this.vscroll.Size = new System.Drawing.Size(17, 283);
            this.vscroll.SmallChange = 16;
            this.vscroll.TabIndex = 2;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.itemsButton,
            this.victimsButton,
            this.oneShotMonstersButton,
            this.monstersButton,
            this.bossMonstersButton});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(300, 25);
            this.toolStrip1.TabIndex = 3;
            this.toolStrip1.Text = "toolStrip1";
            this.toolStrip1.Visible = false;
            // 
            // itemsButton
            // 
            this.itemsButton.CheckOnClick = true;
            this.itemsButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.itemsButton.Image = global::Necrofy.Properties.Resources.item;
            this.itemsButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.itemsButton.Name = "itemsButton";
            this.itemsButton.Size = new System.Drawing.Size(23, 22);
            this.itemsButton.Text = "Items";
            // 
            // victimsButton
            // 
            this.victimsButton.CheckOnClick = true;
            this.victimsButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.victimsButton.Image = global::Necrofy.Properties.Resources.victim;
            this.victimsButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.victimsButton.Name = "victimsButton";
            this.victimsButton.Size = new System.Drawing.Size(23, 22);
            this.victimsButton.Text = "Victims";
            // 
            // oneShotMonstersButton
            // 
            this.oneShotMonstersButton.CheckOnClick = true;
            this.oneShotMonstersButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.oneShotMonstersButton.Image = global::Necrofy.Properties.Resources.one_shot_monster;
            this.oneShotMonstersButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.oneShotMonstersButton.Name = "oneShotMonstersButton";
            this.oneShotMonstersButton.Size = new System.Drawing.Size(23, 22);
            this.oneShotMonstersButton.Text = "One-shot Monsters";
            // 
            // monstersButton
            // 
            this.monstersButton.CheckOnClick = true;
            this.monstersButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.monstersButton.Image = global::Necrofy.Properties.Resources.monster;
            this.monstersButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.monstersButton.Name = "monstersButton";
            this.monstersButton.Size = new System.Drawing.Size(23, 22);
            this.monstersButton.Text = "Monsters";
            // 
            // bossMonstersButton
            // 
            this.bossMonstersButton.CheckOnClick = true;
            this.bossMonstersButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bossMonstersButton.Image = global::Necrofy.Properties.Resources.boss_monster;
            this.bossMonstersButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bossMonstersButton.Name = "bossMonstersButton";
            this.bossMonstersButton.Size = new System.Drawing.Size(23, 22);
            this.bossMonstersButton.Text = "Boss Monsters";
            // 
            // LevelEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(300, 300);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.vscroll);
            this.Controls.Add(this.hscroll);
            this.Controls.Add(this.canvas);
            this.EditorToolStrip = this.toolStrip1;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "LevelEditor";
            this.Text = "LevelEditor";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Canvas canvas;
        private System.Windows.Forms.HScrollBar hscroll;
        private System.Windows.Forms.VScrollBar vscroll;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton itemsButton;
        private System.Windows.Forms.ToolStripButton victimsButton;
        private System.Windows.Forms.ToolStripButton oneShotMonstersButton;
        private System.Windows.Forms.ToolStripButton monstersButton;
        private System.Windows.Forms.ToolStripButton bossMonstersButton;

    }
}