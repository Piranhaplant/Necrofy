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

        public static Color[] SNESToRGB(byte[] data) {
            Color[] colors = new Color[data.Length / 2];
            for (int i = 0; i < colors.Length; i++) {
                colors[i] = SNESToRGB(data[i * 2], data[i * 2 + 1]);
            }
            return colors;
        }

        public static byte[,] PlanarToLinear(byte[] bytes, int index) {
            byte[,] result = new byte[8, 8];
            int line = 0;
            int bit = 0;
            for (int l = index; l <= index + 0x1f; l += 2) {
                for (int m = 0; m <= 7; m++) {
                    if ((bytes[l] & (1 << m)) != 0) {
                        result[line, 7 - m] = (byte)(result[line, 7 - m] | (1 << bit));
                    }
                    if ((bytes[l + 1] & (1 << m)) != 0) {
                        result[line, 7 - m] = (byte)(result[line, 7 - m] | (1 << bit + 1));
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

        public static void DrawTile(Bitmap bmp, int x, int y, byte[] gfx, int gfxindex, Color[] palette, int palIndex, bool xFlip, bool yFlip) {
            byte[,] tile = PlanarToLinear(gfx, gfxindex);
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
            for (int l = 0; l <= 7; l++) {
                for (int m = 0; m <= 7; m++) {
                    if (palette[palIndex + tile[l, m]].A > 0) {
                        bmp.SetPixel(x, y, palette[palIndex + tile[l, m]]);
                    }
                    x += xStep;
                }
                y += yStep;
                x = xOrig;
            }
        }

        public static void DrawTile(Bitmap bmp, int x, int y, Stream s, Color[] palette, int palIndex, bool xFlip, bool yFlip) {
            byte[] gfx = new byte[32];
            s.Read(gfx, 0, 32);
            DrawTile(bmp, x, y, gfx, 0, palette, palIndex, xFlip, yFlip);
        }

        public static void DrawTile(BitmapData bmp, int x, int y, LoadedTilesetTilemap.Tile tile, byte[][,] tiles) {
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
            for (int l = 0; l <= 7; l++) {
                for (int m = 0; m <= 7; m++) {
                    Marshal.WriteByte(bmp.Scan0, y * bmp.Stride + x, (byte)(palIndex + tiles[tile.tileNum][l, m]));
                    x += xStep;
                }
                y += yStep;
                x = xOrig;
            }
        }

        public static void FillPalette(Bitmap bmp, Color[] colors) {
            ColorPalette pal = bmp.Palette;
            for (int l = 0; l <= colors.Length - 1; l++) {
                pal.Entries[l] = colors[l];
            }
            bmp.Palette = pal;
        }

        public static void MakePltTransparent(Bitmap bmp) {
            ColorPalette pal = bmp.Palette;
            for (int l = 0; l <= pal.Entries.Length - 1; l += 16) {
                pal.Entries[l] = Color.Transparent;
            }
            bmp.Palette = pal;
        }

        public static void DrawWithPlt(Graphics g, int x, int y, Bitmap bmp, Color[] plt, int colorIdx, int colorCount) {
            ColorPalette pal = bmp.Palette;
            for (int l = 0; l <= colorCount - 1; l++) {
                pal.Entries[l] = plt[(l + colorIdx) % plt.Length];
            }
            bmp.Palette = pal;
            g.DrawImage(bmp, x, y);
        }
    }
}