using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace Necrofy
{
    class Sprite
    {
        public Tile[] tiles;

        public class Tile
        {
            public ushort tileNum;
            public short xOffset;
            public short yOffset;
            public bool xFlip;
            public bool yFlip;
            public int palette;
        }

        public Rectangle GetBounds() {
            int minX = 0, maxX = 0, minY = 0, maxY = 0;
            foreach (Tile t in tiles) {
                minX = Math.Min(minX, t.xOffset);
                maxX = Math.Max(maxX, t.xOffset + 16);
                minY = Math.Min(minY, t.yOffset + 1);
                maxY = Math.Max(maxY, t.yOffset + 1 + 16);
            }
            return new Rectangle(minX, minY, maxX - minX, maxY - minY);
        }

        public Bitmap Render(LoadedGraphics.LinearGraphics graphics, Color[] colors, int? overridePalette, out int anchorX, out int anchorY) {
            Rectangle bounds = GetBounds();

            Bitmap image;
            if (bounds.Width > 0) {
                image = new Bitmap(bounds.Width, bounds.Height);
            } else {
                image = new Bitmap(1, 1);
            }

            anchorX = -bounds.X;
            anchorY = -bounds.Y;

            foreach (Tile t in tiles) {
                int palette = overridePalette ?? t.palette;
                SNESGraphics.DrawTile(image, anchorX + t.xOffset + (t.xFlip ? 8 : 0), anchorY + t.yOffset + 1 + (t.yFlip ? 8 : 0), graphics[t.tileNum * 4 + 0], colors, palette * 0x10, t.xFlip, t.yFlip);
                SNESGraphics.DrawTile(image, anchorX + t.xOffset + (t.xFlip ? 0 : 8), anchorY + t.yOffset + 1 + (t.yFlip ? 8 : 0), graphics[t.tileNum * 4 + 1], colors, palette * 0x10, t.xFlip, t.yFlip);
                SNESGraphics.DrawTile(image, anchorX + t.xOffset + (t.xFlip ? 8 : 0), anchorY + t.yOffset + 1 + (t.yFlip ? 0 : 8), graphics[t.tileNum * 4 + 2], colors, palette * 0x10, t.xFlip, t.yFlip);
                SNESGraphics.DrawTile(image, anchorX + t.xOffset + (t.xFlip ? 0 : 8), anchorY + t.yOffset + 1 + (t.yFlip ? 0 : 8), graphics[t.tileNum * 4 + 3], colors, palette * 0x10, t.xFlip, t.yFlip);
            }

            return image;
        }

        // TODO: Read sprites with extra tiles
        public static void AddFromROM(List<Sprite> sprites, Stream romStream, int address) {
            romStream.Seek(address);
            while (true) {
                int tileCount = romStream.ReadByte();
                if (tileCount == 0xff || tileCount < 0) {
                    return;
                }
                if (tileCount == 0) {
                    continue;
                }
                Sprite s = new Sprite {
                    tiles = new Tile[tileCount]
                };
                // Loop in reverse order so that the top most tile will be at the end of the array
                for (int i = tileCount - 1; i >= 0; i--) {
                    short xOffset = (short)romStream.ReadInt16();
                    short yOffset = (short)romStream.ReadInt16();
                    int properties = romStream.ReadInt16();
                    ushort tileNum = romStream.ReadInt16();
                    
                    int palette = (properties >> 9) & 7;
                    bool xFlip = (properties & 0x4000) > 0;
                    bool yFlip = (properties & 0x8000) > 0;
                    s.tiles[i] = new Tile {
                        tileNum = tileNum,
                        xOffset = xOffset,
                        yOffset = yOffset,
                        xFlip = xFlip,
                        yFlip = yFlip,
                        palette = palette
                    };
                }
                sprites.Add(s);
            }
        }

        public static int WriteToROM(Sprite[] sprites, int index, Stream romStream, int address, Freespace freespace) {
            romStream.Seek(address);
            int i;
            for (i = index; i < sprites.Length; i++) {
                Sprite s = sprites[i];

                int origTileCount = 0;
                while (origTileCount == 0) {
                    origTileCount = romStream.ReadByte();
                }
                romStream.Seek(-1, SeekOrigin.Current);
                if (origTileCount == 0xff || origTileCount < 0) {
                    break;
                }

                if (s.tiles.Length > origTileCount) {
                    romStream.WriteByte((byte)((origTileCount - 1) | 0x80));
                } else {
                    romStream.WriteByte((byte)s.tiles.Length);
                }

                long nextSpriteLocation = -1;
                for (int tileIndex = 0; tileIndex < s.tiles.Length; tileIndex++) {
                    Tile t = s.tiles[s.tiles.Length - 1 - tileIndex];

                    if (tileIndex == origTileCount - 1 && s.tiles.Length > origTileCount) {
                        int extraTileCount = s.tiles.Length - origTileCount + 1;
                        int extraSpacePointer = freespace.Claim(extraTileCount * 8 + 1);
                        romStream.WritePointer(extraSpacePointer);
                        romStream.Write(new byte[] { 0, 0, 0, 0 }, 0, 4);
                        nextSpriteLocation = romStream.Position;
                        romStream.Seek(extraSpacePointer);
                        romStream.WriteByte((byte)extraTileCount);
                    }

                    int properties = 0;
                    properties |= t.palette << 9;
                    properties |= (t.xFlip ? 1 : 0) << 14;
                    properties |= (t.yFlip ? 1 : 0) << 15;

                    romStream.WriteInt16((ushort)t.xOffset);
                    romStream.WriteInt16((ushort)t.yOffset);
                    romStream.WriteInt16((ushort)properties);
                    romStream.WriteInt16(t.tileNum);
                }
                // If there's extra space, fill it with 0s
                for (int tileIndex = s.tiles.Length; tileIndex < origTileCount; tileIndex++) {
                    romStream.Write(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 }, 0, 8);
                }
                if (nextSpriteLocation >= 0) {
                    romStream.Seek(nextSpriteLocation);
                }
                SpriteEditor a;
            }
            return i;
        }
    }
}
