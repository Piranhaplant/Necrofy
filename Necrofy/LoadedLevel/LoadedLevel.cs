using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;

namespace Necrofy
{
    class LoadedLevel
    {
        public readonly LevelAsset levelAsset;
        public readonly LoadedTilesetTilemap tilemap;
        public readonly LoadedTilesetCollision collision;
        public readonly LoadedTilesetGraphics graphics;
        public readonly LoadedTilesetPalette palette;
        public readonly LoadedSpriteGraphics spriteGraphics;
        public readonly TilesetSuggestionsAsset tilesetSuggestionsAsset;

        public Bitmap[] tiles;
        public Bitmap[] priorityTiles;
        public Bitmap[] solidOnlyTiles;

        public LoadedLevel(Project project, int levelNum) {
            levelAsset = LevelAsset.FromProject(project, levelNum);
            tilemap = new LoadedTilesetTilemap(project, Level.tilesetTilemapName);
            collision = new LoadedTilesetCollision(project, Level.tilesetCollisionName);
            graphics = new LoadedTilesetGraphics(project, Level.tilesetGraphicsName);
            palette = new LoadedTilesetPalette(project, Level.paletteName);
            spriteGraphics = new LoadedSpriteGraphics(project, Level.spritePaletteName);
            tilesetSuggestionsAsset = TilesetSuggestionsAsset.FromProject(project, Level.tilesetTilemapName);
            RenderTiles();
        }

        public void RenderTiles() {
            tiles = new Bitmap[tilemap.tiles.Length];
            priorityTiles = new Bitmap[tilemap.tiles.Length];
            solidOnlyTiles = new Bitmap[tilemap.tiles.Length];

            for (int i = 0; i < tiles.Length; i++) {
                BitmapData curTile = CreateTile(tiles, i);
                BitmapData curPriorityTile = CreateTile(priorityTiles, i, transparent: true);
                BitmapData curSolidOnlyTile = CreateTile(solidOnlyTiles, i);

                for (int y = 0; y < 8; y++) {
                    for (int x = 0; x < 8; x++) {
                        int tileNum = tilemap.tiles[i][x, y].tileNum;
                        if (tileNum <= Level.hiddenTilesStart) {
                            SNESGraphics.DrawTile(curTile, x * 8, y * 8, tilemap.tiles[i][x, y], graphics.linearGraphics);
                            if (tileNum < Level.tilePriorityEnd) {
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
        }

        private BitmapData CreateTile(Bitmap[] allTiles, int i, bool transparent = false) {
            Bitmap tile = new Bitmap(64, 64, PixelFormat.Format8bppIndexed);
            allTiles[i] = tile;
            SNESGraphics.FillPalette(tile, palette.colors);
            if (transparent) {
                SNESGraphics.MakePltTransparent(tile);
            }
            return tile.LockBits(new Rectangle(0, 0, tile.Width, tile.Height), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);
        }

        public Level Level => levelAsset.level;
        public TilesetSuggestions TilesetSuggestions => tilesetSuggestionsAsset.data;
    }
}
