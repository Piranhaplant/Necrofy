using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Necrofy
{
    class LoadedTilemap
    {
        public Tile[] tiles;

        public LoadedTilemap(Project project, string tilemapName) {
            byte[] data = TilemapAsset.FromProject(project, tilemapName).data;
            tiles = new Tile[data.Length / 2];
            for (int i = 0; i < tiles.Length; i++) {
                tiles[i] = new Tile(data[i * 2], data[i * 2 + 1]);
            }
        }
        
        public struct Tile
        {
            public int tileNum;
            public int palette;
            public bool xFlip;
            public bool yFlip;

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
        }
    }
}
