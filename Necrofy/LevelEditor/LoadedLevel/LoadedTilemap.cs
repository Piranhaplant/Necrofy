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
                tiles[i].WriteBytes(asset.data, i * 2);
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

            public void WriteBytes(byte[] array, int index) {
                array[index] = (byte)(tileNum & 0xff);
                array[index + 1] = (byte)(((tileNum & 0x100) >> 8) | ((palette & 7) << 2) | ((xFlip ? 1 : 0) << 6) | ((yFlip ? 1 : 0) << 7));
            }
        }
    }
}
