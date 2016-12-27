using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Necrofy
{
    class OneTimeMonster
    {
        public ushort x { get; set; }
        public ushort y { get; set; }
        public ushort extra { get; set; }
        public ushort victimNumber { get; set; }
        public int type { get; set; }

        public OneTimeMonster() { }

        public OneTimeMonster(NStream s) {
            x = s.ReadInt16();
            y = s.ReadInt16();
            extra = s.ReadInt16();
            victimNumber = s.ReadInt16();
            if (victimNumber == 0x10)
                victimNumber = 10;
            type = s.ReadPointer();
        }
    }
}
