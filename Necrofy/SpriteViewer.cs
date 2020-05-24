using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Necrofy
{
    partial class SpriteViewer : Form
    {
        private LoadedPalette loadedPalette;
        private LoadedGraphics loadedGraphics;
        private SpritesAsset spritesAsset;

        private LoadedSprite currentSprite = null;

        public SpriteViewer() {
            InitializeComponent();
        }

        public void Show(Project project) {
            loadedPalette = new LoadedPalette(project, "Sprites");
            loadedGraphics = new LoadedGraphics(project, GraphicsAsset.SpritesName);
            spritesAsset = SpritesAsset.FromProject(project);

            numericUpDown1.Maximum = spritesAsset.sprites.Length - 1;
            Show();
        }
        
        private void numericUpDown1_ValueChanged(object sender, EventArgs e) {
            currentSprite = new LoadedSprite(spritesAsset.sprites[(int)numericUpDown1.Value], loadedGraphics.linearGraphics, loadedPalette.colors);
            canvas1.Invalidate();
        }

        private void canvas1_Paint(object sender, PaintEventArgs e) {
            if (currentSprite != null) {
                e.Graphics.TranslateTransform(canvas1.Width / 2, canvas1.Height / 2);
                e.Graphics.DrawImage(currentSprite.image, 0, 0);
                e.Graphics.FillRectangle(Brushes.Red, currentSprite.anchorX, currentSprite.anchorY, 1, 1);
            }
        }

        private void canvas1_SizeChanged(object sender, EventArgs e) {
            canvas1.Invalidate();
        }

        public class LoadedSprite
        {
            public readonly Bitmap image;
            public readonly int anchorX;
            public readonly int anchorY;

            public LoadedSprite(Sprite sprite, LoadedGraphics.LinearGraphics graphics, Color[] colors) {
                int minX = 0, maxX = 0, minY = 0, maxY = 0;
                foreach (Sprite.Tile t in sprite.tiles) {
                    minX = Math.Min(minX, t.xOffset);
                    maxX = Math.Max(maxX, t.xOffset + 16);
                    minY = Math.Min(minY, t.yOffset);
                    maxY = Math.Max(maxY, t.yOffset + 16);
                }

                anchorX = -minX;
                anchorY = -minY;
                if (sprite.tiles.Length > 0) {
                    image = new Bitmap(maxX - minX, maxY - minY);
                } else {
                    image = new Bitmap(1, 1);
                }

                foreach (Sprite.Tile t in sprite.tiles) {
                    SNESGraphics.DrawTile(image, anchorX + t.xOffset + (t.xFlip ? 8 : 0), anchorY + t.yOffset + (t.yFlip ? 8 : 0), graphics[t.tileNum * 4 + 0], colors, t.palette * 0x10, t.xFlip, t.yFlip);
                    SNESGraphics.DrawTile(image, anchorX + t.xOffset + (t.xFlip ? 0 : 8), anchorY + t.yOffset + (t.yFlip ? 8 : 0), graphics[t.tileNum * 4 + 1], colors, t.palette * 0x10, t.xFlip, t.yFlip);
                    SNESGraphics.DrawTile(image, anchorX + t.xOffset + (t.xFlip ? 8 : 0), anchorY + t.yOffset + (t.yFlip ? 0 : 8), graphics[t.tileNum * 4 + 2], colors, t.palette * 0x10, t.xFlip, t.yFlip);
                    SNESGraphics.DrawTile(image, anchorX + t.xOffset + (t.xFlip ? 0 : 8), anchorY + t.yOffset + (t.yFlip ? 0 : 8), graphics[t.tileNum * 4 + 3], colors, t.palette * 0x10, t.xFlip, t.yFlip);
                }
            }
        }

        private void getData_Click(object sender, EventArgs e) {
            Clipboard.SetText(JsonConvert.SerializeObject(spritesAsset.sprites[(int)numericUpDown1.Value]));
        }
    }
}
