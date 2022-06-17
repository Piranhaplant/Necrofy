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
        private readonly LevelAsset levelAsset;
        public LoadedTileset tileset;
        public LoadedLevelSprites spriteGraphics;

        public event EventHandler SpritesChanged;
        public event EventHandler TilesChanged;

        public Level Level { get; private set; }
        public int LevelNumber => levelAsset.LevelNumber;
        public TilesetSuggestions TilesetSuggestions { get; private set; }

        public LoadedLevel(Project project, int levelNum) {
            levelAsset = LevelAsset.FromProject(project, levelNum);
            Level = levelAsset.level.JsonClone(new LevelJsonConverter());
            LoadSprites(project);
            LoadTilesetSuggestions(project);
            LoadTiles(project);
        }

        public void Save(Project project) {
            levelAsset.level = Level.JsonClone(new LevelJsonConverter());
            levelAsset.Save(project);
        }
        
        public void Dispose() {
            DisposeTiles();
            DisposeSprites();
        }

        private void DisposeTiles() {
            tileset?.Dispose();
        }

        private void DisposeSprites() {
            if (spriteGraphics != null) {
                spriteGraphics.Updated -= SpriteGraphics_Updated;
                spriteGraphics.Dispose();
            }
        }

        public void LoadSprites(Project project) {
            DisposeSprites();
            spriteGraphics = new LoadedLevelSprites(project, Level.spritePaletteName);
            spriteGraphics.Updated += SpriteGraphics_Updated;
            SpritesChanged?.Invoke(this, EventArgs.Empty);
        }

        private void SpriteGraphics_Updated(object sender, EventArgs e) {
            SpritesChanged?.Invoke(sender, e);
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
            bool animationRunning = tileset?.tileAnimator?.Running ?? false;
            DisposeTiles();

            TileAnimLevelMonster tileAnimLevelMonster = null;
            foreach (LevelMonster levelMonster in Level.levelMonsters) {
                if (levelMonster is TileAnimLevelMonster t) {
                    tileAnimLevelMonster = t;
                }
            }

            tileset = new LoadedTileset(project, Level.paletteName, Level.tilesetGraphicsName, Level.tilesetTilemapName, Level.tilesetCollisionName, Level.visibleTilesEnd, Level.priorityTileCount, tileAnimLevelMonster);
            tileset.TilesChanged += Tileset_TilesChanged;

            if (animationRunning) {
                tileset.tileAnimator.Run();
            }
            TilesChanged?.Invoke(this, EventArgs.Empty);
        }

        private void Tileset_TilesChanged(object sender, EventArgs e) {
            TilesChanged?.Invoke(this, EventArgs.Empty);
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
