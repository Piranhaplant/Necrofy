using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Necrofy
{
    /// <summary>
    /// A monster that will not respawn once it is destroyed, or a victim
    /// </summary>
    class OneTimeMonster
    {
        public ushort x { get; set; }
        public ushort y { get; set; }
        /// <summary>Special bit of extra data available to the monster. Only used by the NPCs in the credits level.</summary>
        public ushort extra { get; set; }
        /// <summary>
        /// The number (1-10) of the victim, which specifies the order in which the victims appear when the player has less than 10 victims remaining.
        /// Victim 1 is always present, but victim 10 only appears if they player has all 10.
        /// For monsters this value is 0.
        /// </summary>
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
