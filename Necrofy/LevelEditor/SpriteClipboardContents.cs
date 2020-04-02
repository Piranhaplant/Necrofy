using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Necrofy
{
    class SpriteClipboardContents
    {
        public List<Item> items = new List<Item>();
        public List<Monster> monsters = new List<Monster>();
        public List<OneShotMonster> oneShotMonsters = new List<OneShotMonster>();
        public List<PositionLevelMonster> bossMonsters = new List<PositionLevelMonster>();

        public bool ShouldSerializeitems() {
            return items.Count > 0;
        }

        public bool ShouldSerializemonsters() {
            return monsters.Count > 0;
        }

        public bool ShouldSerializeoneShotMonsters() {
            return oneShotMonsters.Count > 0;
        }

        public bool ShouldSerializebossMonsters() {
            return bossMonsters.Count > 0;
        }
    }
}
