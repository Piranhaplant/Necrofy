using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Necrofy
{
    /// <summary>
    /// An item that can be picked up by the player
    /// </summary>
    class Item : LevelObject
    {
        public ushort x { get; set; }
        public ushort y { get; set; }
        public byte type { get; set; }

        public Item() { }

        public Item(NStream s) {
            x = s.ReadInt16();
            y = s.ReadInt16();
            type = (byte)s.ReadByte();
        }

        public void Build(MovableData data) {
            data.data.AddInt16(x);
            data.data.AddInt16(y);
            data.data.Add(type);
        }
    }
}
