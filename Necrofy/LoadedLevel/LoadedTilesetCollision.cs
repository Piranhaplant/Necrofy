using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Necrofy
{
    class LoadedTilesetCollision
    {
        public readonly ushort[] tiles;

        public LoadedTilesetCollision(Project project, string tilesetCollisionName) {
            TilesetCollisionAsset collisionAsset = TilesetCollisionAsset.FromProject(project, tilesetCollisionName);

            tiles = new ushort[collisionAsset.data.Length / 2];
            for (int i = 0; i < tiles.Length; i++) {
                tiles[i] = (ushort)(collisionAsset.data[i * 2] + (collisionAsset.data[i * 2 + 1] << 8));
            }
        }
    }
}
