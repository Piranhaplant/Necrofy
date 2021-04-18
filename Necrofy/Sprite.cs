using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Necrofy
{
    class Sprite
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? pointer = null;
        public List<Tile> tiles;

        public class Tile
        {
            public ushort tileNum;
            public short xOffset;
            public short yOffset;
            public bool xFlip;
            public bool yFlip;
            public int palette;

            public Tile() { }

            public Tile(short xOffset, short yOffset, ushort properties, ushort tileNum) {
                this.xOffset = xOffset;
                this.yOffset = yOffset;
                this.tileNum = tileNum;
                palette = (properties >> 9) & 7;
                xFlip = (properties & 0x4000) > 0;
                yFlip = (properties & 0x8000) > 0;
            }

            public ushort GetProperties() {
                int properties = 0;
                properties |= palette << 9;
                properties |= (xFlip ? 1 : 0) << 14;
                properties |= (yFlip ? 1 : 0) << 15;
                return (ushort)properties;
            }
        }

        public Rectangle GetBounds() {
            int minX = tiles.Min(t => t.xOffset);
            int maxX = tiles.Max(t => t.xOffset + 16);
            int minY = tiles.Min(t => t.yOffset + 1);
            int maxY = tiles.Max(t => t.yOffset + 1 + 16);
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
                if (t.tileNum < graphics.Length / 4) {
                    int palette = overridePalette ?? t.palette;
                    SNESGraphics.DrawTile(image, anchorX + t.xOffset + (t.xFlip ? 8 : 0), anchorY + t.yOffset + 1 + (t.yFlip ? 8 : 0), graphics[t.tileNum * 4 + 0], colors, palette * 0x10, t.xFlip, t.yFlip);
                    SNESGraphics.DrawTile(image, anchorX + t.xOffset + (t.xFlip ? 0 : 8), anchorY + t.yOffset + 1 + (t.yFlip ? 8 : 0), graphics[t.tileNum * 4 + 1], colors, palette * 0x10, t.xFlip, t.yFlip);
                    SNESGraphics.DrawTile(image, anchorX + t.xOffset + (t.xFlip ? 8 : 0), anchorY + t.yOffset + 1 + (t.yFlip ? 0 : 8), graphics[t.tileNum * 4 + 2], colors, palette * 0x10, t.xFlip, t.yFlip);
                    SNESGraphics.DrawTile(image, anchorX + t.xOffset + (t.xFlip ? 0 : 8), anchorY + t.yOffset + 1 + (t.yFlip ? 0 : 8), graphics[t.tileNum * 4 + 3], colors, palette * 0x10, t.xFlip, t.yFlip);
                }
            }

            return image;
        }

        public static void AddFromROM(List<Sprite> sprites, NStream romStream, int address, ROMInfo romInfo) {
            romStream.Seek(address);
            List<Tile> tiles = new List<Tile>();
            while (true) {
                int pointer = (int)romStream.Position;
                if (ReadTiles(tiles, romStream, romInfo, false)) {
                    return;
                }
                if (tiles.Count > 0) {
                    sprites.Add(new Sprite {
                        pointer = pointer,
                        tiles = tiles
                    });
                    tiles = new List<Tile>();
                }
            }
        }

        private static bool ReadTiles(List<Tile> tiles, NStream romStream, ROMInfo romInfo, bool trackFreespace) {
            int tileCount = romStream.ReadByte();
            if (tileCount == 0xff || tileCount < 0) {
                return true;
            }
            if (tileCount == 0) {
                return false;
            }
            bool hasExtraTiles = (tileCount & 0x80) > 0;
            tileCount &= 0x7f;

            if (trackFreespace) {
                romInfo.Freespace.AddSize((int)romStream.Position - 1, tileCount * 8 + (hasExtraTiles ? 8 : 0) + 1);
            }

            for (int i = 0; i < tileCount; i++) {
                short xOffset = (short)romStream.ReadInt16();
                short yOffset = (short)romStream.ReadInt16();
                ushort properties = romStream.ReadInt16();
                ushort tileNum = romStream.ReadInt16();
                if (tileNum >= 0x1000) {
                    return true; // Probably hit invalid data
                }
                tiles.Insert(0, new Tile(xOffset, yOffset, properties, tileNum)); // Insert at front so that the top most tile will be at the end of the array
            }

            if (hasExtraTiles) {
                romStream.GoToPointerPush();
                ReadTiles(tiles, romStream, romInfo, true);
                romStream.PopPosition();
                romStream.Seek(4, SeekOrigin.Current);
            }
            return false;
        }

        public static void WriteToROM(Sprite[] sprites, Stream romStream, Freespace freespace) {
            foreach (Sprite s in sprites) {
                if (s.pointer == null) {
                    continue;
                }
                romStream.Seek((int)s.pointer);

                int origTileCount = romStream.ReadByte();
                if ((origTileCount & 0x80) > 0) {
                    origTileCount = (origTileCount & 0x7f) + 1;
                }

                romStream.Seek(-1, SeekOrigin.Current);
                if (s.tiles.Count > origTileCount) {
                    romStream.WriteByte((byte)((origTileCount - 1) | 0x80));
                } else {
                    romStream.WriteByte((byte)s.tiles.Count);
                }

                for (int tileIndex = 0; tileIndex < s.tiles.Count; tileIndex++) {
                    Tile t = s.tiles[s.tiles.Count - 1 - tileIndex];

                    if (tileIndex == origTileCount - 1 && s.tiles.Count > origTileCount) {
                        int extraTileCount = s.tiles.Count - origTileCount + 1;
                        int extraSpacePointer = freespace.Claim(extraTileCount * 8 + 1);
                        romStream.WritePointer(extraSpacePointer);
                        romStream.Write(new byte[] { 0, 0, 0, 0 }, 0, 4);
                        romStream.Seek(extraSpacePointer);
                        romStream.WriteByte((byte)extraTileCount);
                    }

                    romStream.WriteInt16((ushort)t.xOffset);
                    romStream.WriteInt16((ushort)t.yOffset);
                    romStream.WriteInt16(t.GetProperties());
                    romStream.WriteInt16(t.tileNum);
                }
                // If there's extra space, fill it with 0s
                for (int tileIndex = s.tiles.Count; tileIndex < origTileCount; tileIndex++) {
                    romStream.Write(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 }, 0, 8);
                }
            }
        }
    }
}
