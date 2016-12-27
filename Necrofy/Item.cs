using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Necrofy
{
    class Item
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
    }
}
