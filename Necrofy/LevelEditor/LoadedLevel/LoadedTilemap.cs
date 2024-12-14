using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Necrofy
{
    class LoadedTilemap
    {
        public readonly TilemapAsset asset;
        public readonly string tilemapName;
        public Tile[] tiles;
        public ushort width;
        public ushort height;

        public event EventHandler Updated;
        private int updating = 0;

        public LoadedTilemap(Project project, string tilemapName, TilemapAsset.Type type) {
            this.tilemapName = tilemapName;
            asset = TilemapAsset.FromProject(project, tilemapName, type);
            asset.Updated += Asset_Updated;

            ReadTiles();
        }

        public TilemapAsset.Type TilemapType => asset.TilemapType;
        public bool IsSized => TilemapType == TilemapAsset.Type.Sized;

        private void Asset_Updated(object sender, EventArgs e) {
            if (updating == 0) {
                ReadTiles();
                Updated?.Invoke(sender, e);
            }
        }

        private void ReadTiles() {
            int tilesStart = 0;
            if (IsSized) {
                tilesStart = 4;
                width = (ushort)(asset.data[0] | asset.data[1] << 8);
                height = (ushort)(asset.data[2] | asset.data[3] << 8);
            }

            tiles = new Tile[(asset.data.Length - tilesStart) / 2];
            for (int i = 0; i < tiles.Length; i++) {
                tiles[i] = new Tile(asset.data[i * 2 + tilesStart], asset.data[i * 2 + 1 + tilesStart]);
            }
        }

        public void Save(Project project) {
            int tilesStart = 0;
            if (IsSized) {
                tilesStart = 4;
                asset.data[0] = (byte)(width & 0xff);
                asset.data[1] = (byte)((width >> 8) & 0xff);
                asset.data[2] = (byte)(height & 0xff);
                asset.data[3] = (byte)((height >> 8) & 0xff);
            }

            for (int i = 0; i < tiles.Length; i++) {
                ushort value = tiles[i].ToUshort();
                asset.data[i * 2 + tilesStart] = (byte)(value & 0xff);
                asset.data[i * 2 + 1 + tilesStart] = (byte)((value >> 8) & 0xff);
            }
            updating++;
            asset.Save(project);
            updating--;
        }
        
        public struct Tile
        {
            public static readonly Tile Empty = new Tile(0, 0, false, false, false);

            public readonly int tileNum;
            public readonly int palette;
            public readonly bool priority;
            public readonly bool xFlip;
            public readonly bool yFlip;

            public Tile(ushort value) : this((byte)(value & 0xff), (byte)((value >> 8) & 0xff)) { }

            public Tile(byte lowByte, byte highByte) {
                tileNum = lowByte + ((highByte & 3) << 8);
                palette = (highByte >> 2) & 7;
                priority = ((highByte >> 5) & 1) > 0;
                xFlip = ((highByte >> 6) & 1) > 0;
                yFlip = ((highByte >> 7) & 1) > 0;
            }

            public Tile(Tile original, int tileNum) {
                this.tileNum = tileNum;
                palette = original.palette;
                priority = original.priority;
                xFlip = original.xFlip;
                yFlip = original.yFlip;
            }

            public Tile(int tileNum, int palette, bool priority, bool xFlip, bool yFlip) {
                this.tileNum = tileNum;
                this.palette = palette;
                this.priority = priority;
                this.xFlip = xFlip;
                this.yFlip = yFlip;
            }

            public ushort ToUshort() {
                return (ushort)(
                    (tileNum & 0x3ff) |
                    ((palette & 7) << 10) |
                    ((priority ? 1 : 0) << 13) |
                    ((xFlip ? 1 : 0) << 14) |
                    ((yFlip ? 1 : 0) << 15)
                );
            }

            public override bool Equals(object obj) {
                if (!(obj is Tile)) {
                    return false;
                }

                var tile = (Tile)obj;
                return tileNum == tile.tileNum &&
                       palette == tile.palette &&
                       priority == tile.priority &&
                       xFlip == tile.xFlip &&
                       yFlip == tile.yFlip;
            }

            public override int GetHashCode() {
                var hashCode = -787655182;
                hashCode = hashCode * -1521134295 + tileNum.GetHashCode();
                hashCode = hashCode * -1521134295 + palette.GetHashCode();
                hashCode = hashCode * -1521134295 + priority.GetHashCode();
                hashCode = hashCode * -1521134295 + xFlip.GetHashCode();
                hashCode = hashCode * -1521134295 + yFlip.GetHashCode();
                return hashCode;
            }

            public static Tile? Flip(Tile? t, bool horizontal) {
                if (t == null) {
                    return null;
                } else if (horizontal) {
                    return new Tile(t.Value.tileNum, t.Value.palette, t.Value.priority, !t.Value.xFlip, t.Value.yFlip);
                } else {
                    return new Tile(t.Value.tileNum, t.Value.palette, t.Value.priority, t.Value.xFlip, !t.Value.yFlip);
                }
            }
        }
    }
}
