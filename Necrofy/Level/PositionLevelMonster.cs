using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Necrofy
{
    /// <summary>
    /// A level monster that starts at a specific position in the level
    /// </summary>
    class PositionLevelMonster : LevelMonster
    {
        public ushort x { get; set; }
        public ushort y { get; set; }

        [JsonConstructor]
        public PositionLevelMonster(ushort x, ushort y, int type) : base(type) {
            this.x = x;
            this.y = y;
        }

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
