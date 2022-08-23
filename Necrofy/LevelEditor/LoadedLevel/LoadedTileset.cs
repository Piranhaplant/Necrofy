using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Necrofy
{
    class LoadedTileset
    {
        private readonly int visibleTilesEnd;
        private readonly int priorityTileCount;
        private readonly TileAnimLevelMonster tileAnimLevelMonster;

        public LoadedPalette palette;
        public LoadedGraphics graphics;
        public LoadedTilesetTilemap tilemap;
        public LoadedCollision collision;

        public TileAnimator tileAnimator = new TileAnimator();

        public Bitmap[] tiles;
        public Bitmap[] priorityTiles;
        public Bitmap[] solidOnlyTiles;

        public event EventHandler TilesChanged;

        public LoadedTileset(Project project, string paletteName, string graphicsName, string tilemapName, string collisionName, int visibleTilesEnd, int priorityTileCount, TileAnimLevelMonster tileAnimLevelMonster) {
            this.visibleTilesEnd = visibleTilesEnd;
            this.priorityTileCount = priorityTileCount;
            this.tileAnimLevelMonster = tileAnimLevelMonster;

            tilemap = new LoadedTilesetTilemap(project, tilemapName);
            collision = new LoadedCollision(project, collisionName);
            graphics = new LoadedGraphics(project, graphicsName, GraphicsAsset.Type.Normal);
            palette = new LoadedPalette(project, paletteName);

            tilemap.Updated += Asset_Updated;
            collision.Updated += Asset_Updated;
            graphics.Updated += Asset_Updated;
            palette.Updated += Asset_Updated;

            Load();
        }

        private void Asset_Updated(object sender, EventArgs e) {
            Load();
        }

        public void Dispose() {
            tileAnimator.Pause();
            tileAnimator.Animated -= TileAnimator_Animated;
            if (tiles != null) {
                foreach (Bitmap b in tiles.Union(priorityTiles).Union(solidOnlyTiles)) {
                    b.Dispose();
                }
            }
        }

        private void Load() {
            Dispose();

            tiles = new Bitmap[tilemap.tiles.Length];
            priorityTiles = new Bitmap[tilemap.tiles.Length];
            solidOnlyTiles = new Bitmap[tilemap.tiles.Length];

            if (tileAnimLevelMonster == null) {
                tileAnimator = new TileAnimator();
            } else {
                tileAnimator = new TileAnimator(this, tileAnimLevelMonster);
            }

            tileAnimator.Animated += TileAnimator_Animated;

            for (int i = 0; i < tiles.Length; i++) {
                BitmapData curTile = CreateTile(tiles, i, palette);
                BitmapData curPriorityTile = CreateTile(priorityTiles, i, palette, transparent: true);
                BitmapData curSolidOnlyTile = CreateTile(solidOnlyTiles, i, palette);

                for (int y = 0; y < 8; y++) {
                    for (int x = 0; x < 8; x++) {
                        int tileNum = tilemap.tiles[i][x, y].tileNum;
                        if (tileNum <= visibleTilesEnd) {
                            tileAnimator.ProcessTile(i, x, y, tileNum);
                            SNESGraphics.DrawTile(curTile, x * 8, y * 8, tilemap.tiles[i][x, y], graphics.linearGraphics);
                            if (tileNum < priorityTileCount) {
                                SNESGraphics.DrawTile(curPriorityTile, x * 8, y * 8, tilemap.tiles[i][x, y], graphics.linearGraphics);
                            }
                            if ((collision.tiles[tileNum] & 1) > 0 && (collision.tiles[tileNum] & 0x100) == 0) {
                                SNESGraphics.DrawTile(curSolidOnlyTile, x * 8, y * 8, tilemap.tiles[i][x, y], graphics.linearGraphics);
                            }
                        }
                    }
                }

                tiles[i].UnlockBits(curTile);
                priorityTiles[i].UnlockBits(curPriorityTile);
                solidOnlyTiles[i].UnlockBits(curSolidOnlyTile);
            }

            TilesChanged?.Invoke(this, EventArgs.Empty);
        }

        private BitmapData CreateTile(Bitmap[] allTiles, int i, LoadedPalette palette, bool transparent = false) {
            Bitmap tile = new Bitmap(64, 64, PixelFormat.Format8bppIndexed);
            allTiles[i] = tile;
            SNESGraphics.FillPalette(tile, palette.colors);
            if (transparent) {
                SNESGraphics.MakePltTransparent(tile);
            }
            return tile.LockBits(new Rectangle(0, 0, tile.Width, tile.Height), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);
        }

        private void TileAnimator_Animated(object sender, EventArgs e) {
            TilesChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
