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
            byte[,] result = new byte[8, 8];
            int line = 0;
            int bit = 0;
            for (int iy = index; iy < index + 0x20; iy += 2) {
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
            BitmapData data = bitmap.LockBits(new Rectangle(0, 0, 8, 8), ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);
            for (int i = 0; i < 0x20; i++) {
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
    }
}