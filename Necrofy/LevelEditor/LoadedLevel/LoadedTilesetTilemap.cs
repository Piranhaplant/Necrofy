using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Necrofy
{
    class LoadedTilesetTilemap : LoadedTilemap
    {
        public new TileBlock[] tiles;

        public LoadedTilesetTilemap(Project project, string tilesetTilemapName) : base(project, tilesetTilemapName) {
            tiles = new TileBlock[0x100];
            for (int i = 0; i < tiles.Length; i++) {
                tiles[i] = new TileBlock(this, i);
            }
        }

        public class TileBlock
        {
            private readonly LoadedTilemap tilemap;
            private readonly int i;

            public Tile this[int x, int y] {
                get {
                    return tilemap.tiles[x + y * 8 + i * 8 * 8];
                }
            }

            public TileBlock(LoadedTilemap tilemap, int i) {
                this.tilemap = tilemap;
                this.i = i;
            }
        }
    }
}
