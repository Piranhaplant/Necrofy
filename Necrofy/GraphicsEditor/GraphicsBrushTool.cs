using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Necrofy
{
    class GraphicsBrushTool : GraphicsTool
    {
        private bool mouseDown = false;
        private bool mouseDownFirstPoint = false;
        private bool selecting = false;
        private Point prevLocation = Point.Empty;

        public GraphicsBrushTool(GraphicsEditor editor) : base(editor) {
            Status = "Click to paint. Hold Ctrl to select a color.";
        }

        public override void MouseDown(MouseEventArgs e) {
            mouseDown = true;
            mouseDownFirstPoint = true;
            prevLocation = e.Location;
            selecting = Control.ModifierKeys == Keys.Control;
            MouseMove(e);
        }

        public override void MouseMove(MouseEventArgs e) {
            if (mouseDown && (e.Location != prevLocation || mouseDownFirstPoint)) {
                mouseDownFirstPoint = false;
                if (selecting) {
                    int tileNum = editor.GetPixelTileNum(e.X, e.Y);
                    Bitmap bitmap = editor.tiles.GetTemporarily(tileNum);
                    BitmapData data = bitmap.LockBits(new Rectangle(e.X % 8, e.Y % 8, 1, 1), ImageLockMode.ReadOnly, bitmap.PixelFormat);
                    byte color = Marshal.ReadByte(data.Scan0);
                    bitmap.UnlockBits(data);
                    editor.SetSelectedColor(color);
                } else {
                    editor.undoManager.Do(new PaintGraphicsAction(prevLocation.X, prevLocation.Y, e.Location.X, e.Location.Y, (byte)(editor.SelectedColor % 16)));
                    prevLocation = e.Location;
                }
            }
        }

        public override void MouseUp(MouseEventArgs e) {
            mouseDown = false;
            editor.undoManager.ForceNoMerge();
        }

        public override void DoneBeingUsed() {
            mouseDown = false;
        }
    }
}
