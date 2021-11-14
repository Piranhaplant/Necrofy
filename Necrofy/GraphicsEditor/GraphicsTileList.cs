using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Necrofy
{
    class GraphicsTileList
    {
        private readonly List<Bitmap> tiles = new List<Bitmap>();
        private readonly Dictionary<Bitmap, int> useCount = new Dictionary<Bitmap, int>();
        
        public int Count => tiles.Count;

        public void Dispose() {
            foreach (Bitmap tile in useCount.Keys) {
                tile.Dispose();
            }
            tiles.Clear();
            useCount.Clear();
        }

        public void Add(Bitmap tile) {
            tiles.Add(tile);
            useCount[tile] = 1;
        }

        public Bitmap Get(int i) {
            Bitmap tile = tiles[i];
            useCount[tile]++;
            return tile;
        }

        public Bitmap GetTemporarily(int i) {
            return tiles[i];
        }

        public Bitmap Clone(int i) {
            Bitmap original = tiles[i];
            Bitmap clone = original.Clone(new Rectangle(Point.Empty, original.Size), original.PixelFormat);
            useCount[clone] = 1;
            return clone;
        }

        public void Set(int i, Bitmap tile) {
            Bitmap old = tiles[i];
            if (old == tile) {
                return;
            }
            DecrementUses(old);

            tiles[i] = tile;
            useCount[tile]++;
        }

        public void Use(Bitmap tile) {
            useCount[tile]++;
        }

        public void DoneUsing(Bitmap tile) {
            DecrementUses(tile);
        }

        private void DecrementUses(Bitmap tile) {
            int uses = useCount[tile];
            if (uses == 1) {
                useCount.Remove(tile);
                tile.Dispose();
            } else {
                useCount[tile] = uses - 1;
            }
        }
    }
}
