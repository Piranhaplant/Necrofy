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
        private Color selectedColor;
        public Color SelectedColor {
            get {
                return selectedColor;
            }
            private set {
                selectedColor = value;
                hsCanvas.Invalidate();
                lCanvas.Invalidate();

                dataUpdate++;
                rTrackBar.Value = selectedColor.R / 8;
                gTrackBar.Value = selectedColor.G / 8;
                bTrackBar.Value = selectedColor.B / 8;
                dataUpdate--;

                SelectedColorChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public event EventHandler SelectedColorChanged;

        [DllImport("shlwapi.dll")]
        public static extern int ColorHLSToRGB(int H, int L, int S);
        private static Color HLSToRGB(int H, int L, int S) {
            if (S == 0) {
                int v = (int)Math.Round(L / 240.0 * 255);
                return Color.FromArgb(v, v, v);
            }
            return ColorTranslator.FromWin32(ColorHLSToRGB(H, L, S));
        }

        private bool mouseDown = false;
        private int dataUpdate = 0;

        private Bitmap lumImage;

        private int selectedH;
        private int selectedS;
        private int selectedL;

        public ColorEditor() {
            InitializeComponent();
            SelectRGB(128, 128, 128);
        }

        public void SelectRGB(int r, int g, int b) {
            Color c = Color.FromArgb(r, g, b);
            selectedH = (int)(c.GetHue() / 360 * 240);
            selectedS = (int)(c.GetSaturation() * 240);
            selectedL = (int)(c.GetBrightness() * 240);
            UpdateLum();
            SelectedColor = c;
        }

        public void SelectHSL(int h, int s, int l) {
            Color c = HLSToRGB(h, l, s);
            bool updateLum = h != selectedH || s != selectedS;

            selectedH = h;
            selectedS = s;
            selectedL = l;

            if (updateLum) {
                UpdateLum();
            }
            
            SelectedColor = c;
        }

        private void UpdateLum() {
            if (lumImage != null) {
                lumImage.Dispose();
            }

            lumImage = new Bitmap(lCanvas.Width - 12, 241);
            using (Graphics g = Graphics.FromImage(lumImage)) {
                for (int l = 0; l <= 240; l++) {
                    using (Pen p = new Pen(HLSToRGB(selectedH, l, selectedS))) {
                        g.DrawLine(p, 0, 240 - l, lumImage.Width, 240 - l);
                    }
                }
            }
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
            if (dataUpdate == 0) {
                SelectRGB((int)rNumericUpDown.Value * 8, (int)gNumericUpDown.Value * 8, (int)bNumericUpDown.Value * 8);
            }
        }

        private void hsCanvas_Paint(object sender, PaintEventArgs e) {
            int x = selectedH;
            int y = 240 - selectedS;
            Image image = Properties.Resources.crosshairs;
            e.Graphics.DrawImage(image, x - image.Width / 2, y - image.Height / 2, image.Width, image.Height);
        }

        private void lCanvas_Paint(object sender, PaintEventArgs e) {
            if (lumImage != null) {
                e.Graphics.DrawImage(lumImage, 0, 10);
                e.Graphics.DrawImage(Properties.Resources.colorArrow, lCanvas.Width - 10, 240 - selectedL, 10, 20);
            }
        }

        private void hsCanvas_MouseDown(object sender, MouseEventArgs e) {
            mouseDown = true;
            hsCanvas_MouseMove(sender, e);
        }

        private void hsCanvas_MouseMove(object sender, MouseEventArgs e) {
            if (mouseDown) {
                SelectHSL(Math.Max(0, Math.Min(240, e.X)), 240 - Math.Max(0, Math.Min(240, e.Y)), selectedL);
            }
        }

        private void hsCanvas_MouseUp(object sender, MouseEventArgs e) {
            mouseDown = false;
        }

        private void lCanvas_MouseDown(object sender, MouseEventArgs e) {
            mouseDown = true;
            lCanvas_MouseMove(sender, e);
        }

        private void lCanvas_MouseMove(object sender, MouseEventArgs e) {
            if (mouseDown) {
                SelectHSL(selectedH, selectedS, Math.Max(0, Math.Min(240, 240 - (e.Y - 10))));
            }
        }

        private void lCanvas_MouseUp(object sender, MouseEventArgs e) {
            mouseDown = false;
        }
    }
}
