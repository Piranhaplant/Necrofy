using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;

namespace Necrofy
{
    class LoadedLevel : IDisposable
    {
        public readonly LevelAsset levelAsset;
        public LoadedGraphics graphics;
        public LoadedTilesetTilemap tilemap;
        public LoadedLevelSprites spriteGraphics;
        public LoadedCollision collision;

        public Bitmap[] tiles;
        public Bitmap[] priorityTiles;
        public Bitmap[] solidOnlyTiles;

        public TileAnimator tileAnimator = new TileAnimator();

        public event EventHandler SpritesChanged;
        public event EventHandler TilesChanged;

        public Level Level => levelAsset.level;
        public TilesetSuggestions TilesetSuggestions { get; private set; }

        public LoadedLevel(Project project, int levelNum) {
            levelAsset = LevelAsset.FromProject(project, levelNum);
            LoadSprites(project);
            LoadTilesetSuggestions(project);
            LoadTiles(project);
        }
        
        public void Dispose() {
            DisposeTiles();
            DisposeSprites();
        }

        private void DisposeTiles() {
            tileAnimator.Pause();
            tileAnimator.Animated -= TileAnimator_Animated;
            if (tiles != null) {
                foreach (Bitmap b in tiles.Union(priorityTiles).Union(solidOnlyTiles)) {
                    b.Dispose();
                }
            }
        }

        private void DisposeSprites() {
            if (spriteGraphics != null) {
                spriteGraphics.Dispose();
            }
        }

        public void LoadSprites(Project project) {
            DisposeSprites();
            spriteGraphics = new LoadedLevelSprites(project, Level.spritePaletteName);
            SpritesChanged?.Invoke(this, EventArgs.Empty);
        }

        public void LoadTilesetSuggestions(Project project) {
            try {
                TilesetSuggestions = TilesetSuggestionsAsset.FromProject(project, Level.tilesetTilemapName).data;
            } catch (Exception e) {
                Console.WriteLine(e);
                TilesetSuggestions = new TilesetSuggestions();
            }
        }

        public void LoadTiles(Project project) {
            bool animationRunning = tileAnimator.Running;
            DisposeTiles();

            tilemap = new LoadedTilesetTilemap(project, Level.tilesetTilemapName);
            collision = new LoadedCollision(project, Level.tilesetCollisionName);
            graphics = new LoadedGraphics(project, Level.tilesetGraphicsName);
            LoadedPalette palette = new LoadedPalette(project, Level.paletteName);

            tiles = new Bitmap[tilemap.tiles.Length];
            priorityTiles = new Bitmap[tilemap.tiles.Length];
            solidOnlyTiles = new Bitmap[tilemap.tiles.Length];

            tileAnimator = new TileAnimator();
            foreach (LevelMonster levelMonster in Level.levelMonsters) {
                if (levelMonster is TileAnimLevelMonster tileAnimLevelMonster) {
                    tileAnimator = new TileAnimator(this, tileAnimLevelMonster);
                }
            }
            tileAnimator.Animated += TileAnimator_Animated;
            if (animationRunning) {
                tileAnimator.Run();
            }

            for (int i = 0; i < tiles.Length; i++) {
                BitmapData curTile = CreateTile(tiles, i, palette);
                BitmapData curPriorityTile = CreateTile(priorityTiles, i, palette, transparent: true);
                BitmapData curSolidOnlyTile = CreateTile(solidOnlyTiles, i, palette);

                for (int y = 0; y < 8; y++) {
                    for (int x = 0; x < 8; x++) {
                        int tileNum = tilemap.tiles[i][x, y].tileNum;
                        if (tileNum <= Level.visibleTilesEnd) {
                            tileAnimator.ProcessTile(i, x, y, tileNum);
                            SNESGraphics.DrawTile(curTile, x * 8, y * 8, tilemap.tiles[i][x, y], graphics.linearGraphics);
                            if (tileNum < Level.priorityTileCount) {
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

        private void TileAnimator_Animated(object sender, EventArgs e) {
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

        public IEnumerable<WrappedLevelObject> GetAllObjects(bool items = true, bool victims = true, bool oneShotMonsters = true, bool monsters = true, bool bossMonsters = true, bool players = true) {
            if (monsters) {
                foreach (Monster m in Level.monsters) {
                    yield return new WrappedMonster(m, spriteGraphics);
                }
            }
            foreach (OneShotMonster m in Level.oneShotMonsters) {
                if (victims && m.victimNumber > 0 || oneShotMonsters && m.victimNumber == 0) {
                    yield return new WrappedOneShotMonster(m, spriteGraphics);
                }
            }
            if (bossMonsters) {
                foreach (LevelMonster m in Level.levelMonsters) {
                    if (m is PositionLevelMonster positionLevelMonster) {
                        yield return new WrappedPositionLevelMonster(positionLevelMonster, spriteGraphics);
                    }
                }
            }
            if (players) {
                yield return new WrappedPlayer1StartPosition(spriteGraphics, Level);
                yield return new WrappedPlayer2StartPosition(spriteGraphics, Level);
            }
            if (items) {
                foreach (Item i in Level.items) {
                    yield return new WrappedItem(i, spriteGraphics);
                }
            }
        }
    }
}
