using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Necrofy
{
    /// <summary>
    /// An item that can be picked up by the player
    /// </summary>
    class Item
    {
        public ushort x { get; set; }
        public ushort y { get; set; }
        public byte type { get; set; }
        
        [JsonConstructor]
        public Item(ushort x, ushort y, byte type) {
            this.x = x;
            this.y = y;
            this.type = type;
        }

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
