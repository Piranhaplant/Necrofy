using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using System.Diagnostics;

namespace Necrofy
{
    partial class LevelEditor : DockContent
    {
        private LoadedLevel level;

        public LevelEditor() {
            InitializeComponent();
        }

        public void SetLevel(LoadedLevel level) {
            this.level = level;
            this.Invalidate();
        }

        private void LevelEditor_Paint(object sender, PaintEventArgs e) {
            if (level == null)
                return;
            for (int y = 0; y < level.Level.height; y++) {
                for (int x = 0; x < level.Level.width; x++) {
                    e.Graphics.DrawImage(level.tiles[level.Level.background[x, y]], x * 64, y * 64);
                }
            }

            foreach (OneTimeMonster m in level.Level.oneTimeMonsters) {
                level.spriteGraphics.sprites[m.type].Render(e.Graphics, m.x, m.y);
            }
            foreach (Monster m in level.Level.monsters) {
                level.spriteGraphics.sprites[m.type].Render(e.Graphics, m.x, m.y);
            }

            //for (int y = 0; y < level.Level.height; y++) {
            //    for (int x = 0; x < level.Level.width; x++) {
            //        e.Graphics.DrawImage(level.priorityTiles[level.Level.background[x, y]], x * 64, y * 64);
            //    }
            //}
        }
    }
}
