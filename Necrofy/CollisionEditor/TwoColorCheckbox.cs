using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Necrofy
{
    class TwoColorCheckbox : CheckBox
    {
        private string text1;
        public string Text1 {
            get {
                return text1;
            }
            set {
                if (value != text1) {
                    text1 = value;
                    Invalidate();
                }
            }
        }

        private string text2;
        public string Text2 {
            get {
                return text2;
            }
            set {
                if (value != text2) {
                    text2 = value;
                    Invalidate();
                }
            }
        }

        private Color foreColor2 = SystemColors.ControlText;
        public Color ForeColor2 {
            get {
                return foreColor2;
            }
            set {
                if (value != foreColor2) {
                    foreColor2 = value;
                    Invalidate();
                }
            }
        }
        
        protected override void OnPaint(PaintEventArgs pevent) {
            base.OnPaint(pevent);
            int x = Height;

            DrawText(pevent.Graphics, ref x, text1, ForeColor);
            DrawText(pevent.Graphics, ref x, text2, foreColor2);
        }

        private void DrawText(Graphics g, ref int x, string s, Color color) {
            Rectangle rect = new Rectangle(x, 0, Width - x, Height);
            TextRenderer.DrawText(g, s, Font, rect, color, TextFormatFlags.VerticalCenter);
            x += TextRenderer.MeasureText(g, s, Font, rect.Size, TextFormatFlags.NoPadding).Width;
        }
    }
}
