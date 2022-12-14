using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Necrofy
{
    abstract class GraphicsTool : MapTool
    {
        protected readonly GraphicsEditor editor;

        public GraphicsTool(GraphicsEditor editor) : base(editor) {
            this.editor = editor;
            AddSubTool(new PasteTool(editor));
        }

        public override bool CanCopy => mapEditor.SelectionExists;
        public override bool CanPaste => true;
        public override bool CanDelete => true;
        public override bool HasSelection => true;

        public override void MouseMove(MapMouseEventArgs e) {
            base.MouseMove(e);
            string info = $"Cursor: ({e.X}, {e.Y})";
            int tile = editor.GetPixelTileNum(e.X, e.Y);
            if (tile >= 0) {
                if (editor.tileSize == 2) {
                    tile /= 4;
                }
                info += $" tile 0x{tile:X}";
            }
            Info1 = info;
        }

        public override void MouseLeave() {
            base.MouseLeave();
            Info1 = "Cursor: N/A";
        }

        protected delegate bool GetPixelDelegate(int x, int y, out byte pixel);
        protected static void ReadPixelData(GraphicsEditor editor, Action<GetPixelDelegate> action) {
            Dictionary<int, BitmapData> loadedTiles = new Dictionary<int, BitmapData>();
            bool GetPixel(int x, int y, out byte pixel) {
                int tileNum = editor.GetPixelTileNum(x, y);
                if (tileNum < 0) {
                    pixel = 0;
                    return false;
                }
                BitmapData tileData;
                if (!loadedTiles.ContainsKey(tileNum)) {
                    Bitmap tile = editor.tiles.GetTemporarily(tileNum);
                    tileData = tile.LockBits(new Rectangle(Point.Empty, tile.Size), ImageLockMode.ReadOnly, tile.PixelFormat);
                    loadedTiles[tileNum] = tileData;
                } else {
                    tileData = loadedTiles[tileNum];
                }
                pixel = Marshal.ReadByte(tileData.Scan0, (y % 8) * tileData.Stride + (x % 8));
                return true;
            }

            action(GetPixel);

            foreach (KeyValuePair<int, BitmapData> pair in loadedTiles) {
                editor.tiles.GetTemporarily(pair.Key).UnlockBits(pair.Value);
            }
        }

        private class PasteTool : MapPasteTool
        {
            private const string PNGClipboardFormat = "PNG";

            protected readonly GraphicsEditor editor;

            private bool transparent;
            private sbyte[,] pasteData;
            private Bitmap pasteImage;

            public PasteTool(GraphicsEditor editor) : base(editor) {
                this.editor = editor;
            }

            protected override int GetSnapAmount() {
                return editor.tileSize * 8;
            }

            public override void Copy() {
                Rectangle bounds = editor.Selection.GetSelectedAreaBounds();

                Bitmap image = new Bitmap(bounds.Width, bounds.Height, PixelFormat.Format32bppArgb);
                sbyte[,] rawData = new sbyte[bounds.Width, bounds.Height];
                BitmapData bitmapData = image.LockBits(new Rectangle(Point.Empty, image.Size), ImageLockMode.WriteOnly, image.PixelFormat);

                ReadPixelData(editor, getPixel => {
                    for (int y = 0; y < image.Height; y++) {
                        for (int x = 0; x < image.Width; x++) {
                            if (editor.Selection.GetPoint(x + bounds.X, y + bounds.Y) && getPixel(x + bounds.X, y + bounds.Y, out byte pixel)) {
                                Marshal.WriteInt32(bitmapData.Scan0, y * bitmapData.Stride + x * 4, editor.Colors[editor.selectedPalette * editor.colorsPerPalette + pixel].ToArgb());
                                rawData[x, y] = (sbyte)pixel;
                            } else {
                                rawData[x, y] = -1;
                            }
                        }
                    }
                });
                image.UnlockBits(bitmapData);

                DataObject clipboardData = new DataObject();
                clipboardData.SetImage(image);
                clipboardData.SetText(JsonConvert.SerializeObject(new ClipboardContents(rawData)));
                using (MemoryStream s = new MemoryStream()) {
                    image.Save(s, ImageFormat.Png);
                    clipboardData.SetData(PNGClipboardFormat, false, s);
                    Clipboard.SetDataObject(clipboardData, true);
                }
                
                image.Dispose();
            }

            protected override Size ReadPaste() {
                transparent = editor.transparency;
                if (Clipboard.ContainsText()) {
                    pasteData = JsonConvert.DeserializeObject<ClipboardContents>(Clipboard.GetText()).graphics;
                    if (pasteData == null) {
                        return Size.Empty;
                    }
                } else if (Clipboard.ContainsData(PNGClipboardFormat)) {
                    using (MemoryStream s = Clipboard.GetData("PNG") as MemoryStream)
                    using (Bitmap image = new Bitmap(s)) {
                        pasteData = ImageToData(image);
                    }
                } else if (Clipboard.ContainsImage()) {
                    pasteData = ImageToData(Clipboard.GetImage());
                } else {
                    return Size.Empty;
                }

                pasteImage = new Bitmap(pasteData.GetWidth(), pasteData.GetHeight(), PixelFormat.Format8bppIndexed);
                BitmapData bitmapData = pasteImage.LockBits(new Rectangle(Point.Empty, pasteImage.Size), ImageLockMode.WriteOnly, pasteImage.PixelFormat);
                for (int y = 0; y < pasteImage.Height; y++) {
                    for (int x = 0; x < pasteImage.Width; x++) {
                        pasteData[x, y] = (sbyte)(pasteData[x, y] % editor.colorsPerPalette);
                        byte value = pasteData[x, y] >= 0 ? (byte)pasteData[x, y] : (byte)255;
                        Marshal.WriteByte(bitmapData.Scan0, y * bitmapData.Stride + x, value);
                    }
                }
                pasteImage.UnlockBits(bitmapData);

                ColorPalette palette = pasteImage.Palette;
                for (int i = 0; i < editor.colorsPerPalette; i++) {
                    palette.Entries[i] = editor.Colors[editor.selectedPalette * editor.colorsPerPalette + i];
                }
                palette.Entries[255] = Color.Transparent;
                pasteImage.Palette = palette;
                return pasteImage.Size;
            }

            private sbyte[,] ImageToData(Image image) {
                Bitmap bitmap = new Bitmap(image);
                sbyte[,] data = new sbyte[bitmap.Width, bitmap.Height];

                Dictionary<Color, sbyte> colorMap = new Dictionary<Color, sbyte>();
                for (int y = 0; y < bitmap.Height; y++) {
                    for (int x = 0; x < bitmap.Width; x++) {
                        Color c = bitmap.GetPixel(x, y);
                        if (c.A > 0) {
                            if (colorMap.TryGetValue(c, out sbyte value)) {
                                data[x, y] = value;
                            } else {
                                data[x, y] = GetClosestPaletteEntry(c);
                                colorMap[c] = data[x, y];
                            }
                        } else {
                            data[x, y] = -1;
                        }
                    }
                }
                bitmap.Dispose();
                return data;
            }

            private sbyte GetClosestPaletteEntry(Color c) {
                sbyte bestPaletteEntry = 0;
                float bestDistance = 100000f;
                for (sbyte i = 0; i < editor.colorsPerPalette; i++) {
                    Color paletteEntry = editor.Colors[editor.selectedPalette * editor.colorsPerPalette + i];
                    if (paletteEntry.A > 0) {
                        float distance = Square(c.R - paletteEntry.R) + Square(c.B - paletteEntry.B) + Square(c.G - paletteEntry.G);
                        if (distance == 0f) {
                            return i;
                        } else if (distance < bestDistance) {
                            bestPaletteEntry = i;
                            bestDistance = distance;
                        }
                    }
                }
                return bestPaletteEntry;
            }

            private static float Square(float f) {
                return f * f;
            }

            protected override bool PointInPaste(int x, int y) {
                return pasteData[x, y] >= 0;
            }

            protected override void DoPasteAction(int pasteX, int pasteY) {
                editor.undoManager.Do(new PasteGraphicsAction(pasteData, pasteX, pasteY, transparent));
            }

            protected override void RenderPaste(Graphics g, int pixelX, int pixelY, GraphicsPath path) {
                g.DrawImage(pasteImage, pixelX, pixelY);
            }

            protected override void ClearPasteData() {
                pasteData = null;
                pasteImage.Dispose();
            }

            public override void Delete() {
                editor.undoManager.Do(new DeleteGraphicsAction());
            }
        }

        private class ClipboardContents
        {
            public readonly sbyte[,] graphics;

            public ClipboardContents(sbyte[,] graphics) {
                this.graphics = graphics;
            }
        }
    }
}
