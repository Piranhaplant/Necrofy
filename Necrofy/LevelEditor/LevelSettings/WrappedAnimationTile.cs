using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Necrofy
{
    internal class WrappedAnimationTile : ISelectableObject
    {
        public readonly TileAnimLevelMonster.Entry.Frame frame;
        public readonly TileAnimLevelMonster.Entry entry;
        public readonly int x;
        public readonly int y;

        public WrappedAnimationTile(TileAnimLevelMonster.Entry entry, int x, int y) {
            this.entry = entry;
            this.frame = null;
            this.x = x;
            this.y = y;
        }

        public WrappedAnimationTile(TileAnimLevelMonster.Entry.Frame frame, int x, int y) {
            this.entry = null;
            this.frame = frame;
            this.x = x;
            this.y = y;
        }

        public Rectangle Bounds => new Rectangle(x, y, 8, 8);

        public bool Selectable => true;

        public int GetX() {
            return x;
        }

        public int GetY() {
            return y;
        }

        public override bool Equals(object obj) {
            return obj is WrappedAnimationTile tile &&
                   EqualityComparer<TileAnimLevelMonster.Entry.Frame>.Default.Equals(frame, tile.frame) &&
                   EqualityComparer<TileAnimLevelMonster.Entry>.Default.Equals(entry, tile.entry);
        }

        public override int GetHashCode() {
            int hashCode = -1605752535;
            hashCode = hashCode * -1521134295 + EqualityComparer<TileAnimLevelMonster.Entry.Frame>.Default.GetHashCode(frame);
            hashCode = hashCode * -1521134295 + EqualityComparer<TileAnimLevelMonster.Entry>.Default.GetHashCode(entry);
            return hashCode;
        }
    }
}
