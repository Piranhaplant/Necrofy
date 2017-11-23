using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Necrofy
{
    class LoadedTileset
    {
        public readonly TilesetTilemapAsset tilemapAsset;
        public readonly TilesetCollisionAsset collisionAsset;
        public readonly TilesetGraphicsAsset graphicsAsset;
        public readonly TilesetPaletteAsset paletteAsset;

        public LoadedTileset(Project project, string tilesetTilemapName, string tilesetCollisionName, string tilesetGraphicsName, string paletteName) {
            tilemapAsset = TilesetTilemapAsset.FromProject(project, tilesetTilemapName);
            collisionAsset = TilesetCollisionAsset.FromProject(project, tilesetCollisionName);
            graphicsAsset = TilesetGraphicsAsset.FromProject(project, tilesetGraphicsName);
            paletteAsset = TilesetPaletteAsset.FromProject(project, paletteName);
        }
    }
}
