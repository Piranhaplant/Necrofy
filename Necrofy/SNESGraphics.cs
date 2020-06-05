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

        public static byte RGBToSNESLo(Color color) {
            return (byte)((color.B / 8 * 0x400 + color.G / 8 * 0x20 + color.R / 8) % 0x100);
        }

        public static byte RGBToSNESHi(Color color) {
            return (byte)((color.B / 8 * 0x400 + color.G / 8 * 32 + color.R / 8) / 0x100);
        }

        public static Color SNESToRGB(byte low, byte high) {
            int v = low + 0x100 * high;
            return Color.FromArgb((v % 0x20) * 8, ((v / 0x20) % 0x20) * 8, ((v / 0x400) % 0x20) * 8);
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
            byte[,] result = new byte[8, 8];
            int line = 0;
            int bit = 0;
            for (int iy = index; iy <= index + 0x1f; iy += 2) {
                for (int ix = 0; ix <= 7; ix++) {
                    if ((bytes[iy] & (1 << ix)) != 0) {
                        result[7 - ix, line] = (byte)(result[7 - ix, line] | (1 << bit));
                    }
                    if ((bytes[iy + 1] & (1 << ix)) != 0) {
                        result[7 - ix, line] = (byte)(result[7 - ix, line] | (1 << bit + 1));
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
            ColorPalette pal = bmp.Palette;
            for (int i = 0; i < colorCount; i++) {
                pal.Entries[i] = plt[(i + colorIdx) % plt.Length];
            }
            bmp.Palette = pal;
            g.DrawImage(bmp, x, y);
        }
    }
}