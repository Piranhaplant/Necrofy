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
    public partial class SpriteViewer : Form
    {
        private PaletteAsset paletteAsset;
        private GraphicsAsset graphicsAsset;
        private SpritesAsset spritesAsset;
        private Color[] colors;

        public SpriteViewer() {
            InitializeComponent();
        }

        public void Show(Project project) {
            paletteAsset = PaletteAsset.FromProject(project, "Sprites");
            graphicsAsset = GraphicsAsset.FromProject(project, GraphicsAsset.SpritesName);
            spritesAsset = SpritesAsset.FromProject(project);
            colors = SNESGraphics.SNESToRGB(paletteAsset.data, true);

            numericUpDown1.Maximum = spritesAsset.sprites.Length - 1;
            Show();
        }
        
        private void numericUpDown1_ValueChanged(object sender, EventArgs e) {
            LoadedSprite sprite = new LoadedSprite(spritesAsset.sprites[(int)numericUpDown1.Value], graphicsAsset.data, colors);
            canvas1.BackgroundImage = sprite.image;
        }

        public class LoadedSprite
        {
            public readonly Bitmap image;
            private readonly int anchorX;
            private readonly int anchorY;

            public LoadedSprite(Sprite sprite, byte[] graphics, Color[] colors) {
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
                    SNESGraphics.DrawTile(image, anchorX + t.xOffset + (t.xFlip ? 8 : 0), anchorY + t.yOffset + (t.yFlip ? 8 : 0), graphics, t.tileNum * 0x80 + 0x00, colors, t.palette * 0x10, t.xFlip, t.yFlip);
                    SNESGraphics.DrawTile(image, anchorX + t.xOffset + (t.xFlip ? 0 : 8), anchorY + t.yOffset + (t.yFlip ? 8 : 0), graphics, t.tileNum * 0x80 + 0x20, colors, t.palette * 0x10, t.xFlip, t.yFlip);
                    SNESGraphics.DrawTile(image, anchorX + t.xOffset + (t.xFlip ? 8 : 0), anchorY + t.yOffset + (t.yFlip ? 0 : 8), graphics, t.tileNum * 0x80 + 0x40, colors, t.palette * 0x10, t.xFlip, t.yFlip);
                    SNESGraphics.DrawTile(image, anchorX + t.xOffset + (t.xFlip ? 0 : 8), anchorY + t.yOffset + (t.yFlip ? 0 : 8), graphics, t.tileNum * 0x80 + 0x60, colors, t.palette * 0x10, t.xFlip, t.yFlip);
                }
            }
        }
    }
}
