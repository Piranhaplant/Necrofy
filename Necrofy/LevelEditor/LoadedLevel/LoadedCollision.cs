using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Necrofy
{
    class LoadedCollision
    {
        public readonly ushort[] tiles;

        public LoadedCollision(Project project, string collisionName) {
            CollisionAsset collisionAsset = CollisionAsset.FromProject(project, collisionName);

            tiles = new ushort[collisionAsset.data.Length / 2];
            for (int i = 0; i < tiles.Length; i++) {
                tiles[i] = (ushort)(collisionAsset.data[i * 2] + (collisionAsset.data[i * 2 + 1] << 8));
            }
        }
    }
}
