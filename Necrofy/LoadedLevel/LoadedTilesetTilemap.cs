using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Necrofy
{
    class LoadedTilesetTilemap : LoadedTilemap
    {
        public new TileBlock[] tiles;

        public LoadedTilesetTilemap(Project project, string tilesetTilemapName)
            : base(TilesetTilemapAsset.FromProject(project, tilesetTilemapName).data) {
            tiles = new TileBlock[0x100];
            for (int i = 0; i < tiles.Length; i++) {
                tiles[i] = new TileBlock(base.tiles, i);
            }
        }

        public class TileBlock
        {
            private readonly Tile[] tiles;
            private readonly int i;

            public Tile this[int x, int y] {
                get {
                    return tiles[x + y * 8 + i * 8 * 8];
                }
            }

            public TileBlock(Tile[] tiles, int i) {
                this.tiles = tiles;
                this.i = i;
            }
        }
    }
}
