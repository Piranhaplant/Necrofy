using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Necrofy
{
    class PositionLevelMonster : LevelMonster
    {
        public ushort x { get; set; }
        public ushort y { get; set; }

        public PositionLevelMonster() { }

        public PositionLevelMonster(int type, NStream s) : base(type) {
            this.x = s.ReadInt16();
            this.y = s.ReadInt16();
        }
    }
}
