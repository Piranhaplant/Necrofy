using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Necrofy
{
    /// <summary>
    /// A spawn point for respawning monsters
    /// </summary>
    class Monster : LevelObject
    {
        public ushort x { get; set; }
        public ushort y { get; set; }
        public byte radius { get; set; }
        public byte delay { get; set; }
        public int type { get; set; }

        public Monster() { }

        public Monster(NStream s) {
            radius = (byte)s.ReadByte();
            x = s.ReadInt16();
            y = s.ReadInt16();
            delay = (byte)s.ReadByte();
            type = s.ReadPointer();
        }

        public void Build(MovableData data) {
            data.data.Add(radius);
            data.data.AddInt16(x);
            data.data.AddInt16(y);
            data.data.Add(delay);
            data.data.AddPointer(type);
        }
    }
}
