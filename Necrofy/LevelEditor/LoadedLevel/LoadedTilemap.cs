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
        public Tile[] tiles;
        public readonly string tilemapName;

        public event EventHandler Updated;

        private int updating = 0;

        public LoadedTilemap(Project project, string tilemapName) {
            this.tilemapName = tilemapName;
            asset = TilemapAsset.FromProject(project, tilemapName);
            asset.Updated += Asset_Updated;

            ReadTiles();
        }

        private void Asset_Updated(object sender, EventArgs e) {
            if (updating == 0) {
                ReadTiles();
                Updated?.Invoke(sender, e);
            }
        }

        private void ReadTiles() {
            tiles = new Tile[asset.data.Length / 2];
            for (int i = 0; i < tiles.Length; i++) {
                tiles[i] = new Tile(asset.data[i * 2], asset.data[i * 2 + 1]);
            }
        }

        public void Save(Project project) {
            for (int i = 0; i < tiles.Length; i++) {
                ushort value = tiles[i].ToUshort();
                asset.data[i] = (byte)(value & 0xff);
                asset.data[i + 1] = (byte)((value >> 8) & 0xff);
            }
            updating++;
            asset.Save(project);
            updating--;
        }
        
        public struct Tile
        {
            public readonly int tileNum;
            public readonly int palette;
            public readonly bool xFlip;
            public readonly bool yFlip;

            public Tile(ushort value) : this((byte)(value & 0xff), (byte)((value >> 8) & 0xff)) { }

            public Tile(byte lowByte, byte highByte) {
                tileNum = lowByte + ((highByte & 1) << 8);
                palette = (highByte >> 2) & 7;
                xFlip = ((highByte >> 6) & 1) > 0;
                yFlip = ((highByte >> 7) & 1) > 0;
            }

            public Tile(Tile original, int tileNum) {
                this.tileNum = tileNum;
                palette = original.palette;
                xFlip = original.xFlip;
                yFlip = original.yFlip;
            }

            public Tile(int tileNum, int palette, bool xFlip, bool yFlip) {
                this.tileNum = tileNum;
                this.palette = palette;
                this.xFlip = xFlip;
                this.yFlip = yFlip;
            }

            public ushort ToUshort() {
                return (ushort)(
                    (tileNum & 0x1ff) |
                    ((palette & 7) << 10) |
                    ((xFlip ? 1 : 0) << 14) |
                    ((yFlip ? 1 : 0) << 15)
                );
            }
        }
    }
}
