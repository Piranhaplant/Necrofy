using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Necrofy
{
    /// <summary>
    /// A level monster that starts at a specific position in the level
    /// </summary>
    class PositionLevelMonster : LevelMonster, LevelObject
    {
        public ushort x { get; set; }
        public ushort y { get; set; }

        public PositionLevelMonster() { }

        public PositionLevelMonster(int type, NStream s) : base(type) {
            x = s.ReadInt16();
            y = s.ReadInt16();
        }

        public override void Build(MovableData data, ROMInfo rom) {
            data.data.AddPointer(type);
            data.data.AddInt16(x);
            data.data.AddInt16(y);
        }
    }
}
