using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Necrofy
{
    public partial class ColorEditor : UserControl
    {
        private static readonly Bitmap ColorArrow = Properties.Resources.colorArrow;
        private static readonly Bitmap Crosshairs = Properties.Resources.crosshairs;

        private Color selectedColor;
        public Color SelectedColor {
            get {
                return selectedColor;
            }
            private set {
                selectedColor = value;
                hsCanvas.Invalidate();
                lCanvas.Invalidate();

                uiUpdate++;
                rTrackBar.Value = SNESGraphics.RGBComponentToSNES(selectedColor.R);
                gTrackBar.Value = SNESGraphics.RGBComponentToSNES(selectedColor.G);
                bTrackBar.Value = SNESGraphics.RGBComponentToSNES(selectedColor.B);
                uiUpdate--;

                ColorChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public event EventHandler ColorChanged;

        private static Color HexToColor(string hex) {
            try {
                return ColorTranslator.FromHtml("#" + hex);
            } catch (Exception) {
                return Color.Black;
            }
        }

        private static string ColorToHex(Color c) {
            return $"{c.R:X2}{c.G:X2}{c.B:X2}";
        }

        private int uiUpdate = 0;

        private Bitmap lumImage;

        private int selectedH;
        private int selectedS;
        private int selectedL;

        public ColorEditor() {
            InitializeComponent();
            SelectRGB(128, 128, 128);
        }

        public void SelectRGB(int r, int g, int b, bool updateHex = true) {
            SelectRGB(Color.FromArgb(r, g, b), updateHex);
        }

        public void SelectRGB(Color c, bool updateHex = true) {
            selectedH = (int)(c.GetHue() / 360 * Win32.HSLMax);
            selectedS = (int)(c.GetSaturation() * Win32.HSLMax);
            selectedL = (int)(c.GetBrightness() * Win32.HSLMax);
            UpdateLum();
            SelectedColor = c;
            if (updateHex) {
                UpdateHex();
            }
        }

        public void SelectHSL(int h, int s, int l) {
            Color c = Win32.HLSToRGB(h, l, s);
            bool updateLum = h != selectedH || s != selectedS;

            selectedH = h;
            selectedS = s;
            selectedL = l;

            if (updateLum) {
                UpdateLum();
            }

            SelectedColor = c;
            UpdateHex();
        }

        private void UpdateLum() {
            if (lumImage != null) {
                lumImage.Dispose();
            }

            lumImage = new Bitmap(lCanvas.Width - ColorArrow.Width - 2, Win32.HSLMax + 1);
            using (Graphics g = Graphics.FromImage(lumImage)) {
                for (int l = 0; l <= Win32.HSLMax; l++) {
                    using (Pen p = new Pen(Win32.HLSToRGB(selectedH, l, selectedS))) {
                        g.DrawLine(p, 0, Win32.HSLMax - l, lumImage.Width, Win32.HSLMax - l);
                    }
                }
            }
        }

        private void UpdateHex() {
            uiUpdate++;
            hexTextBox.Text = ColorToHex(selectedColor);
            uiUpdate--;
        }

        private void rTrackBar_ValueChanged(object sender, EventArgs e) {
            rNumericUpDown.Value = rTrackBar.Value;
        }

        private void gTrackBar_ValueChanged(object sender, EventArgs e) {
            gNumericUpDown.Value = gTrackBar.Value;
        }

        private void bTrackBar_ValueChanged(object sender, EventArgs e) {
            bNumericUpDown.Value = bTrackBar.Value;
        }

        private void numericUpDown_ValueChanged(object sender, EventArgs e) {
            if (uiUpdate == 0) {
                SelectRGB(SNESGraphics.SNESComponentToRGB((int)rNumericUpDown.Value), SNESGraphics.SNESComponentToRGB((int)gNumericUpDown.Value), SNESGraphics.SNESComponentToRGB((int)bNumericUpDown.Value));
            }
        }

        private void hexTextBox_TextChanged(object sender, EventArgs e) {
            if (uiUpdate == 0) {
                SelectRGB(HexToColor(hexTextBox.Text), updateHex: false);
            }
        }

        private void hsCanvas_Paint(object sender, PaintEventArgs e) {
            int x = selectedH;
            int y = Win32.HSLMax - selectedS;
            e.Graphics.DrawImage(Crosshairs, x - Crosshairs.Width / 2, y - Crosshairs.Height / 2, Crosshairs.Width, Crosshairs.Height);
        }

        private void lCanvas_Paint(object sender, PaintEventArgs e) {
            if (lumImage != null) {
                e.Graphics.DrawImage(lumImage, 0, ColorArrow.Height / 2);
                e.Graphics.DrawImage(ColorArrow, lCanvas.Width - ColorArrow.Width, Win32.HSLMax - selectedL, ColorArrow.Width, ColorArrow.Height);
            }
        }

        private void hsCanvas_MouseDown(object sender, MouseEventArgs e) {
            hsCanvas_MouseMove(sender, e);
        }

        private void hsCanvas_MouseMove(object sender, MouseEventArgs e) {
            if (hsCanvas.IsMouseDown) {
                SelectHSL(Math.Max(0, Math.Min(Win32.HSLMax, e.X)), Win32.HSLMax - Math.Max(0, Math.Min(Win32.HSLMax, e.Y)), selectedL);
            }
        }
        
        private void lCanvas_MouseDown(object sender, MouseEventArgs e) {
            lCanvas_MouseMove(sender, e);
        }

        private void lCanvas_MouseMove(object sender, MouseEventArgs e) {
            if (lCanvas.IsMouseDown) {
                SelectHSL(selectedH, selectedS, Math.Max(0, Math.Min(Win32.HSLMax, Win32.HSLMax - (e.Y - ColorArrow.Height / 2))));
            }
        }
    }
}
