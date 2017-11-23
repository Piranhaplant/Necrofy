using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Necrofy
{
    class LoadedLevel
    {
        public readonly LevelAsset levelAsset;
        public readonly LoadedTileset tileset;

        public LoadedLevel(Project project, int levelNum) {
            levelAsset = LevelAsset.FromProject(project, levelNum);
            tileset = new LoadedTileset(project, Level.tilesetTilemapName, Level.tilesetCollisionName, Level.tilesetGraphicsName, Level.paletteName);
        }

        public Level Level {
            get { return levelAsset.level; }
        }
    }
}
