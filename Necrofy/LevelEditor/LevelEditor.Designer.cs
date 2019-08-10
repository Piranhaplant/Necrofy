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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LevelEditor));
            this.canvas = new Necrofy.Canvas();
            this.hscroll = new System.Windows.Forms.HScrollBar();
            this.vscroll = new System.Windows.Forms.VScrollBar();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.paintbrushButton = new System.Windows.Forms.ToolStripButton();
            this.spritesButton = new Necrofy.CheckableToolStripSplitButton();
            this.spritesItems = new Necrofy.SeparateCheckToolStripMenuItem();
            this.spritesVictims = new Necrofy.SeparateCheckToolStripMenuItem();
            this.spritesOneShotMonsters = new Necrofy.SeparateCheckToolStripMenuItem();
            this.spritesMonsters = new Necrofy.SeparateCheckToolStripMenuItem();
            this.spritesBossMonsters = new Necrofy.SeparateCheckToolStripMenuItem();
            this.spritesSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.spritesAll = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.toolsMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsPaintbrush = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsSprites = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
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
            this.canvas.MouseDown += new System.Windows.Forms.MouseEventHandler(this.canvas_MouseDown);
            this.canvas.MouseMove += new System.Windows.Forms.MouseEventHandler(this.canvas_MouseMove);
            this.canvas.MouseUp += new System.Windows.Forms.MouseEventHandler(this.canvas_MouseUp);
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
            this.paintbrushButton,
            this.spritesButton});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(300, 25);
            this.toolStrip1.TabIndex = 3;
            this.toolStrip1.Text = "toolStrip1";
            this.toolStrip1.Visible = false;
            // 
            // paintbrushButton
            // 
            this.paintbrushButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.paintbrushButton.Image = ((System.Drawing.Image)(resources.GetObject("paintbrushButton.Image")));
            this.paintbrushButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.paintbrushButton.Name = "paintbrushButton";
            this.paintbrushButton.Size = new System.Drawing.Size(23, 22);
            this.paintbrushButton.Text = "Paintbrush";
            this.paintbrushButton.Click += new System.EventHandler(this.paintbrush_Click);
            // 
            // spritesButton
            // 
            this.spritesButton.Checked = false;
            this.spritesButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.spritesButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.spritesItems,
            this.spritesVictims,
            this.spritesOneShotMonsters,
            this.spritesMonsters,
            this.spritesBossMonsters,
            this.spritesSeparator,
            this.spritesAll});
            this.spritesButton.Image = ((System.Drawing.Image)(resources.GetObject("spritesButton.Image")));
            this.spritesButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.spritesButton.Name = "spritesButton";
            this.spritesButton.Size = new System.Drawing.Size(32, 22);
            this.spritesButton.Text = "Sprites";
            this.spritesButton.ButtonClick += new System.EventHandler(this.sprites_Click);
            // 
            // spritesItems
            // 
            this.spritesItems.Checked = true;
            this.spritesItems.CheckState = System.Windows.Forms.CheckState.Checked;
            this.spritesItems.Image = global::Necrofy.Properties.Resources.item;
            this.spritesItems.Name = "spritesItems";
            this.spritesItems.Size = new System.Drawing.Size(176, 22);
            this.spritesItems.Text = "Items";
            this.spritesItems.CheckedChanged += new System.EventHandler(this.spritesItems_CheckedChanged);
            // 
            // spritesVictims
            // 
            this.spritesVictims.Checked = true;
            this.spritesVictims.CheckState = System.Windows.Forms.CheckState.Checked;
            this.spritesVictims.Image = global::Necrofy.Properties.Resources.victim;
            this.spritesVictims.Name = "spritesVictims";
            this.spritesVictims.Size = new System.Drawing.Size(176, 22);
            this.spritesVictims.Text = "Victims";
            this.spritesVictims.CheckedChanged += new System.EventHandler(this.spritesVictims_CheckedChanged);
            // 
            // spritesOneShotMonsters
            // 
            this.spritesOneShotMonsters.Checked = true;
            this.spritesOneShotMonsters.CheckState = System.Windows.Forms.CheckState.Checked;
            this.spritesOneShotMonsters.Image = global::Necrofy.Properties.Resources.one_shot_monster;
            this.spritesOneShotMonsters.Name = "spritesOneShotMonsters";
            this.spritesOneShotMonsters.Size = new System.Drawing.Size(176, 22);
            this.spritesOneShotMonsters.Text = "One-shot Monsters";
            this.spritesOneShotMonsters.CheckedChanged += new System.EventHandler(this.spritesOneShotMonsters_CheckedChanged);
            // 
            // spritesMonsters
            // 
            this.spritesMonsters.Checked = true;
            this.spritesMonsters.CheckState = System.Windows.Forms.CheckState.Checked;
            this.spritesMonsters.Image = global::Necrofy.Properties.Resources.monster;
            this.spritesMonsters.Name = "spritesMonsters";
            this.spritesMonsters.Size = new System.Drawing.Size(176, 22);
            this.spritesMonsters.Text = "Monsters";
            this.spritesMonsters.CheckedChanged += new System.EventHandler(this.spritesMonsters_CheckedChanged);
            // 
            // spritesBossMonsters
            // 
            this.spritesBossMonsters.Checked = true;
            this.spritesBossMonsters.CheckState = System.Windows.Forms.CheckState.Checked;
            this.spritesBossMonsters.Image = global::Necrofy.Properties.Resources.boss_monster;
            this.spritesBossMonsters.Name = "spritesBossMonsters";
            this.spritesBossMonsters.Size = new System.Drawing.Size(176, 22);
            this.spritesBossMonsters.Text = "Boss Monsters";
            this.spritesBossMonsters.CheckedChanged += new System.EventHandler(this.spritesBossMonsters_CheckedChanged);
            // 
            // spritesSeparator
            // 
            this.spritesSeparator.Name = "spritesSeparator";
            this.spritesSeparator.Size = new System.Drawing.Size(173, 6);
            // 
            // spritesAll
            // 
            this.spritesAll.Name = "spritesAll";
            this.spritesAll.Size = new System.Drawing.Size(176, 22);
            this.spritesAll.Text = "All";
            this.spritesAll.Click += new System.EventHandler(this.spritesAll_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolsMenu});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(300, 24);
            this.menuStrip1.TabIndex = 4;
            this.menuStrip1.Text = "menuStrip1";
            this.menuStrip1.Visible = false;
            // 
            // toolsMenu
            // 
            this.toolsMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolsPaintbrush,
            this.toolsSprites});
            this.toolsMenu.Name = "toolsMenu";
            this.toolsMenu.Size = new System.Drawing.Size(46, 20);
            this.toolsMenu.Text = "Tools";
            // 
            // toolsPaintbrush
            // 
            this.toolsPaintbrush.Name = "toolsPaintbrush";
            this.toolsPaintbrush.Size = new System.Drawing.Size(131, 22);
            this.toolsPaintbrush.Text = "Paintbrush";
            this.toolsPaintbrush.Click += new System.EventHandler(this.paintbrush_Click);
            // 
            // toolsSprites
            // 
            this.toolsSprites.Name = "toolsSprites";
            this.toolsSprites.Size = new System.Drawing.Size(131, 22);
            this.toolsSprites.Text = "Sprites";
            this.toolsSprites.Click += new System.EventHandler(this.sprites_Click);
            // 
            // LevelEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(300, 300);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.vscroll);
            this.Controls.Add(this.hscroll);
            this.Controls.Add(this.canvas);
            this.EditorMenuStrip = this.menuStrip1;
            this.EditorToolStrip = this.toolStrip1;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "LevelEditor";
            this.Text = "LevelEditor";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Canvas canvas;
        private System.Windows.Forms.HScrollBar hscroll;
        private System.Windows.Forms.VScrollBar vscroll;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem toolsMenu;
        private CheckableToolStripSplitButton spritesButton;
        private System.Windows.Forms.ToolStripSeparator spritesSeparator;
        private System.Windows.Forms.ToolStripMenuItem spritesAll;
        private SeparateCheckToolStripMenuItem spritesItems;
        private SeparateCheckToolStripMenuItem spritesVictims;
        private SeparateCheckToolStripMenuItem spritesOneShotMonsters;
        private SeparateCheckToolStripMenuItem spritesMonsters;
        private SeparateCheckToolStripMenuItem spritesBossMonsters;
        private System.Windows.Forms.ToolStripMenuItem toolsSprites;
        private System.Windows.Forms.ToolStripButton paintbrushButton;
        private System.Windows.Forms.ToolStripMenuItem toolsPaintbrush;
    }
}