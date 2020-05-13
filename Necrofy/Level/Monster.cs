﻿using Newtonsoft.Json;
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
        public byte radius { get; set; }
        public byte delay { get; set; }
        public int type { get; set; }

        [JsonConstructor]
        public Monster(ushort x, ushort y, byte radius, byte delay, int type) {
            this.x = x;
            this.y = y;
            this.radius = radius;
            this.delay = delay;
            this.type = type;
        }

        public Monster(NStream s) {
            delay = (byte)s.ReadByte();
            x = s.ReadInt16();
            y = s.ReadInt16();
            radius = (byte)s.ReadByte();
            type = s.ReadPointer();
        }

        public void Build(MovableData data) {
            data.data.Add(delay);
            data.data.AddInt16(x);
            data.data.AddInt16(y);
            data.data.Add(radius);
            data.data.AddPointer(type);
        }
    }
}
