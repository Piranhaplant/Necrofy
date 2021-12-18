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
        public GraphicsBrushTool(GraphicsEditor editor) : base(editor) {
            AddSubTool(new BrushTool(editor));
        }

        private class BrushTool : MapBrushTool
        {
            private readonly GraphicsEditor editor;

            public BrushTool(GraphicsEditor editor) : base(editor) {
                this.editor = editor;
                Status = "Click to paint. Hold Ctrl to select a color.";
            }

            protected override void DrawLine(int x1, int y1, int x2, int y2) {
                editor.undoManager.Do(new PaintGraphicsAction(x1, y1, x2, y2, (byte)(editor.SelectedColor % 16)));
            }

            protected override void SelectTile(int x, int y) {
                int tileNum = editor.GetPixelTileNum(x, y);
                if (tileNum >= 0) {
                    Bitmap bitmap = editor.tiles.GetTemporarily(tileNum);
                    BitmapData data = bitmap.LockBits(new Rectangle(x % 8, y % 8, 1, 1), ImageLockMode.ReadOnly, bitmap.PixelFormat);
                    byte color = Marshal.ReadByte(data.Scan0);
                    bitmap.UnlockBits(data);
                    editor.SetSelectedColor(color);
                }
            }

            public override void MouseUp(MapMouseEventArgs e) {
                editor.undoManager.ForceNoMerge();
            }
        }
    }
}
