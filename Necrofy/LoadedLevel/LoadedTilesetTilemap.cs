using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Necrofy
{
    class LoadedTilesetTilemap
    {
        public readonly TilesetTilemapAsset tilemapAsset;
        public Tile[][,] tiles;

        public LoadedTilesetTilemap(Project project, string tilesetTilemapName) {
            tilemapAsset = TilesetTilemapAsset.FromProject(project, tilesetTilemapName);
            
            tiles = new Tile[0x100][,];
            int i = 0;
            for (int t = 0; t < tiles.Length; t++) {
                tiles[t] = new Tile[8, 8];
                for (int y = 0; y < 8; y++) {
                    for (int x = 0; x < 8; x++) {
                        tiles[t][x, y] = new Tile(tilemapAsset.data[i], tilemapAsset.data[i + 1]);
                        i += 2;
                    }
                }
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
        }
    }
}
