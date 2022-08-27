using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace Necrofy
{
    class SNESGraphics
    {
        public const int ScreenWidth = 256;
        public const int ScreenHeight = 224;

        public static readonly Brush TransparencyGridBrush1 = new SolidBrush(Color.FromArgb(0x99, 0x99, 0x99));
        public static readonly Brush TransparencyGridBrush2 = new SolidBrush(Color.FromArgb(0x70, 0x70, 0x70));
        
        public static ushort RGBToSNES(Color color) {
            return (ushort)((RGBComponentToSNES(color.B) << 10) | (RGBComponentToSNES(color.G) << 5) | RGBComponentToSNES(color.R));
        }

        public static int RGBComponentToSNES(byte component) {
            return component >> 3;
        }

        public static Color SNESToRGB(byte low, byte high) {
            return SNESToRGB((ushort)(low | (high << 8)));
        }

        public static Color SNESToRGB(ushort v) {
            return Color.FromArgb(SNESComponentToRGB(v & 0x1F), SNESComponentToRGB((v >> 5) & 0x1F), SNESComponentToRGB((v >> 10) & 0x1F));
        }

        public static int SNESComponentToRGB(int component) {
            return (component << 3) | (component >> 2);
        }

        public static Color[] SNESToRGB(byte[] data, bool transparent = false) {
            Color[] colors = new Color[data.Length / 2];
            for (int i = 0; i < colors.Length; i++) {
                colors[i] = SNESToRGB(data[i * 2], data[i * 2 + 1]);
                if (transparent && i % 0x10 == 0) {
                    colors[i] = Color.FromArgb(0, colors[i]);
                }
            }
            return colors;
        }

        public static byte[,] PlanarToLinear(byte[] bytes, int index) {
            return PlanarToLinear(bytes, index, 0x20);
        }

        public static byte[,] PlanarToLinear2BPP(byte[] bytes, int index) {
            return PlanarToLinear(bytes, index, 0x10);
        }

        private static byte[,] PlanarToLinear(byte[] bytes, int index, int tileSize) {
            byte[,] result = new byte[8, 8];
            int line = 0;
            int bit = 0;
            for (int iy = index; iy < index + tileSize; iy += 2) {
                for (int ix = 0; ix < 8; ix++) {
                    if ((bytes[iy] & (1 << ix)) != 0) {
                        result[7 - ix, line] |= (byte)(1 << bit);
                    }
                    if ((bytes[iy + 1] & (1 << ix)) != 0) {
                        result[7 - ix, line] |= (byte)(1 << (bit + 1));
                    }
                }
                line += 1;
                if (line == 8) {
                    line = 0;
                    bit += 2;
                }
            }
            return result;
        }
        public static void BitmapToPlanar(Bitmap bitmap, byte[] bytes, int index) {
            BitmapToPlanar(bitmap, bytes, index, 0x20);
        }

        public static void BitmapToPlanar2BPP(Bitmap bitmap, byte[] bytes, int index) {
            BitmapToPlanar(bitmap, bytes, index, 0x10);
        }

        public static void BitmapToPlanar(Bitmap bitmap, byte[] bytes, int index, int tileSize) {
            BitmapData data = bitmap.LockBits(new Rectangle(0, 0, 8, 8), ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);
            for (int i = 0; i < tileSize; i++) {
                bytes[index + i] = 0;
                for (int b = 0; b < 8; b++) {
                    int x = 7 - b;
                    int y = (i / 2) % 8;
                    byte byteValue = Marshal.ReadByte(data.Scan0, y * data.Stride + x);
                    int bit = (i % 2) + (i / 16) * 2;
                    bytes[index + i] |= (byte)(((byteValue >> bit) & 1) << b);
                }
            }
            bitmap.UnlockBits(data);
        }

        public static void DrawTile(Bitmap bmp, int x, int y, byte[,] linearGraphics, Color[] palette, int palIndex, bool xFlip, bool yFlip) {
            int xStep = 1;
            int yStep = 1;
            if (xFlip) {
                x += 7;
                xStep = -1;
            }
            int xOrig = x;
            if (yFlip) {
                y += 7;
                yStep = -1;
            }
            for (int iy = 0; iy <= 7; iy++) {
                for (int ix = 0; ix <= 7; ix++) {
                    Color c = palette[palIndex + linearGraphics[ix, iy]];
                    if (c.A > 0) {
                        bmp.SetPixel(x, y, c);
                    }
                    x += xStep;
                }
                y += yStep;
                x = xOrig;
            }
        }
        
        public static void DrawTile(BitmapData bmp, int x, int y, LoadedTilemap.Tile tile, LoadedGraphics.LinearGraphics linearGraphics) {
            int palIndex = 0x10 * tile.palette;
            int xStep = 1;
            int yStep = 1;
            if (tile.xFlip) {
                x += 7;
                xStep = -1;
            }
            int xOrig = x;
            if (tile.yFlip) {
                y += 7;
                yStep = -1;
            }
            for (int iy = 0; iy <= 7; iy++) {
                for (int ix = 0; ix <= 7; ix++) {
                    Marshal.WriteByte(bmp.Scan0, y * bmp.Stride + x, (byte)(palIndex + linearGraphics[tile.tileNum][ix, iy]));
                    x += xStep;
                }
                y += yStep;
                x = xOrig;
            }
        }

        public static void FillPalette(Bitmap bmp, Color[] colors) {
            ColorPalette pal = bmp.Palette;
            for (int i = 0; i < colors.Length; i++) {
                pal.Entries[i] = colors[i];
            }
            bmp.Palette = pal;
        }

        public static void MakePltTransparent(Bitmap bmp) {
            ColorPalette pal = bmp.Palette;
            for (int i = 0; i < pal.Entries.Length; i += 16) {
                pal.Entries[i] = Color.Transparent;
            }
            bmp.Palette = pal;
        }

        public static void DrawWithPlt(Graphics g, int x, int y, Bitmap bmp, Color[] plt, int colorIdx, int colorCount) {
            DrawWithPlt(g, x, y, bmp, plt, colorIdx, colorCount, false, false);
        }

        public static void DrawWithPlt(Graphics g, int x, int y, Bitmap bmp, Color[] plt, int colorIdx, int colorCount, bool flipX, bool flipY) {
            ColorPalette pal = bmp.Palette;
            for (int i = 0; i < colorCount; i++) {
                pal.Entries[i] = plt[(i + colorIdx) % plt.Length];
            }
            bmp.Palette = pal;
            g.DrawImage(bmp, x + (flipX ? bmp.Width : 0), y + (flipY ? bmp.Height : 0), bmp.Width * (flipX ? -1 : 1), bmp.Height * (flipY ? -1 : 1));
        }

        public static void DrawTransparencyGrid(Graphics g, RectangleF r, float zoom) {
            g.FillRectangle(TransparencyGridBrush1, r);
            float squareSize = 8 / zoom;
            int topSquare = (int)Math.Floor(r.Top / squareSize);
            int leftSquare = (int)Math.Floor(r.Left / squareSize);
            int bottomSquare = (int)Math.Floor(r.Bottom / squareSize);
            int rightSquare = (int)Math.Floor(r.Right / squareSize);
            for (int y = topSquare; y <= bottomSquare; y++) {
                for (int x = leftSquare + ((leftSquare + y) % 2); x <= rightSquare; x += 2) {
                    g.FillRectangle(TransparencyGridBrush2, new RectangleF(x * squareSize, y * squareSize, squareSize, squareSize));
                }
            }
        }

        public static Bitmap[] RenderAllTiles(LoadedGraphics graphics) {
            Bitmap[]  tiles = new Bitmap[graphics.linearGraphics.Length];
            for (int i = 0; i < tiles.Length; i++) {
                Bitmap tile = new Bitmap(8, 8, PixelFormat.Format8bppIndexed);
                BitmapData data = tile.LockBits(new Rectangle(Point.Empty, tile.Size), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);
                DrawTile(data, 0, 0, new LoadedTilemap.Tile(i, 0, false, false), graphics.linearGraphics);
                tile.UnlockBits(data);
                tiles[i] = tile;
            }
            return tiles;
        }
    }
}