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
    partial class LevelEditor : EditorWindow
    {
        private const int LevelPadding = 64;

        private readonly LoadedLevel level;
        private readonly ScrollWrapper scrollWrapper;
        
        public LevelEditor(LoadedLevel level) {
            InitializeComponent();

            this.level = level;
            this.Text = level.levelAsset.GetDisplayText();

            scrollWrapper = new ScrollWrapper(canvas, hscroll, vscroll);
            scrollWrapper.SetClientSize(level.Level.width * 64 + LevelPadding * 2, level.Level.height * 64 + LevelPadding * 2);
            scrollWrapper.Scrolled += new ScrollWrapper.ScrollDelegate(scrollWrapper_Scrolled);

            Repaint();
        }

        protected override void Displayed() {
            mainWindow.ObjectBrowser.Browser.Contents = new TilesetObjectBrowserContents(level);
            mainWindow.ObjectBrowser.Activate();
        }

        void scrollWrapper_Scrolled() {
            Repaint();
        }

        public void Repaint() {
            canvas.Invalidate();
        }

        private void canvas_Paint(object sender, PaintEventArgs e) {
            if (level == null) {
                return;
            }
            e.Graphics.TranslateTransform(scrollWrapper.LeftPosition + LevelPadding, scrollWrapper.TopPosition + LevelPadding);

            for (int y = 0; y < level.Level.height; y++) {
                for (int x = 0; x < level.Level.width; x++) {
                    e.Graphics.DrawImage(level.tiles[level.Level.background[x, y]], x * 64, y * 64);
                }
            }

            foreach (OneTimeMonster m in level.Level.oneTimeMonsters) {
                level.spriteGraphics.Render(m.type, e.Graphics, m.x, m.y);
            }
            foreach (Monster m in level.Level.monsters) {
                level.spriteGraphics.Render(m.type, e.Graphics, m.x, m.y);
            }
            foreach (Item i in level.Level.items) {
                level.spriteGraphics.Render(i.type, e.Graphics, i.x, i.y);
            }

            //for (int y = 0; y < level.Level.height; y++) {
            //    for (int x = 0; x < level.Level.width; x++) {
            //        e.Graphics.DrawImage(level.priorityTiles[level.Level.background[x, y]], x * 64, y * 64);
            //    }
            //}
        }
    }
}
