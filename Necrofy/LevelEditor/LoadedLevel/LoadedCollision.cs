using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Necrofy
{
    class LoadedCollision
    {
        public readonly CollisionAsset asset;
        public readonly string collisionName;
        public ushort[] tiles;

        public event EventHandler Updated;
        private int updating = 0;

        public LoadedCollision(Project project, string collisionName) {
            this.collisionName = collisionName;
            asset = CollisionAsset.FromProject(project, collisionName);
            asset.Updated += Asset_Updated;

            ReadCollision();
        }

        private void Asset_Updated(object sender, EventArgs e) {
            if (updating == 0) {
                ReadCollision();
                Updated?.Invoke(sender, e);
            }
        }

        private void ReadCollision() {
            tiles = new ushort[asset.data.Length / 2];
            for (int i = 0; i < tiles.Length; i++) {
                tiles[i] = (ushort)(asset.data[i * 2] + (asset.data[i * 2 + 1] << 8));
            }
        }

        public void Save(Project project) {
            for (int i = 0; i < tiles.Length; i++) {
                asset.data[i * 2] = (byte)(tiles[i] & 0xff);
                asset.data[i * 2 + 1] = (byte)((tiles[i] >> 8) & 0xff);
            }
            updating++;
            asset.Save(project);
            updating--;
        }
    }
}
