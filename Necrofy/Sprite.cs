using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Necrofy
{
    public class Sprite
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

        public static void AddFromROM(List<Sprite> sprites, Stream romStream, int address) {
            romStream.Seek(address, SeekOrigin.Begin);
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

        public static int WriteToROM(Sprite[] sprites, int index, Stream romStream, int address) {
            romStream.Seek(address, SeekOrigin.Begin);
            int i;
            for (i = index; i < sprites.Length; i++) {
                Sprite s = sprites[i];

                int tileCount = 0;
                while (tileCount == 0) {
                    tileCount = romStream.ReadByte();
                }
                romStream.Seek(-1, SeekOrigin.Current);
                if (tileCount == 0xff || tileCount < 0) {
                    break;
                }
                romStream.WriteByte((byte)s.tiles.Length);
                for (int tileIndex = s.tiles.Length - 1; tileIndex >= 0; tileIndex--) {
                    Tile t = s.tiles[tileIndex];

                    int properties = 0;
                    properties |= t.palette << 9;
                    properties |= (t.xFlip ? 1 : 0) << 14;
                    properties |= (t.yFlip ? 1 : 0) << 15;

                    romStream.WriteInt16((ushort)t.xOffset);
                    romStream.WriteInt16((ushort)t.yOffset);
                    romStream.WriteInt16((ushort)properties);
                    romStream.WriteInt16(t.tileNum);
                }
            }
            return i;
        }
    }
}
