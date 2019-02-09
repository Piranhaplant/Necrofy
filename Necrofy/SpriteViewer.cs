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
        private Stream romStream;
        private PaletteAsset paletteAsset;
        private GraphicsAsset graphicsAsset;
        private Color[] colors;

        public SpriteViewer() {
            InitializeComponent();
        }

        public void Show(string romPath, Project project) {
            romStream = new FileStream(romPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            paletteAsset = PaletteAsset.FromProject(project, "Sprites");
            graphicsAsset = GraphicsAsset.FromProject(project, GraphicsAsset.SpritesName);
            colors = SNESGraphics.SNESToRGB(paletteAsset.data, true);

            Show();
        }

        private void button1_Click(object sender, EventArgs e) {
            Bitmap image = new Bitmap(512, 512);

            romStream.Seek((long)numericUpDown1.Value, SeekOrigin.Begin);
            int tileCount = romStream.ReadByte();
            if (tileCount != 0xff && tileCount > 0) {
                DrawTile(image, tileCount);
            }

            canvas1.BackgroundImage = image;
            numericUpDown1.Value = romStream.Position;
        }

        private void DrawTile(Bitmap image, int tileCount) {
            int xOffset = (short)romStream.ReadInt16();
            int yOffset = (short)romStream.ReadInt16();
            int properties = romStream.ReadInt16();
            int tileNum = romStream.ReadInt16();

            int palette = (properties >> 9) & 15;
            bool xFlip = (properties & 0x4000) > 0;
            bool yFlip = (properties & 0x8000) > 0;
            
            if (tileCount > 1) {
                DrawTile(image, tileCount - 1);
            }

            SNESGraphics.DrawTile(image, xOffset + 256 + (xFlip ? 8 : 0), yOffset + 256 + (yFlip ? 8 : 0), graphicsAsset.data, tileNum * 0x80 + 0x00, colors, palette * 0x10, xFlip, yFlip);
            SNESGraphics.DrawTile(image, xOffset + 256 + (xFlip ? 0 : 8), yOffset + 256 + (yFlip ? 8 : 0), graphicsAsset.data, tileNum * 0x80 + 0x20, colors, palette * 0x10, xFlip, yFlip);
            SNESGraphics.DrawTile(image, xOffset + 256 + (xFlip ? 8 : 0), yOffset + 256 + (yFlip ? 0 : 8), graphicsAsset.data, tileNum * 0x80 + 0x40, colors, palette * 0x10, xFlip, yFlip);
            SNESGraphics.DrawTile(image, xOffset + 256 + (xFlip ? 0 : 8), yOffset + 256 + (yFlip ? 0 : 8), graphicsAsset.data, tileNum * 0x80 + 0x60, colors, palette * 0x10, xFlip, yFlip);
        }
    }
}
