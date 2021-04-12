using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Necrofy
{
    /// <summary>
    /// A spawn point for respawning monsters
    /// </summary>
    class Monster
    {
        public ushort x { get; set; }
        public ushort y { get; set; }
        public byte areaSize { get; set; }

        private byte _delay;
        public byte delay {
            get {
                return _delay;
            }
            set {
                _delay = Math.Max((byte)1, value);
            }
        }

        public int type { get; set; }

        [JsonConstructor]
        public Monster(ushort x, ushort y, byte areaSize, byte delay, int type) {
            this.x = x;
            this.y = y;
            this.areaSize = areaSize;
            this.delay = delay;
            this.type = type;
        }

        public Monster(NStream s) {
            delay = (byte)s.ReadByte();
            x = s.ReadInt16();
            y = s.ReadInt16();
            areaSize = (byte)s.ReadByte();
            type = s.ReadPointer();
        }

        public void Build(MovableData data) {
            data.data.Add(delay);
            data.data.AddInt16(x);
            data.data.AddInt16(y);
            data.data.Add(areaSize);
            data.data.AddPointer(type);
        }
    }
}
