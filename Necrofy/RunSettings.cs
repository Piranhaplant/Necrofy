using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Necrofy
{
    public class RunSettings
    {
        public const int WeaponCount = 14;
        public const int SpecialCount = 12;

        public ushort victimCount = 10;
        public ushort[] weaponAmounts = new ushort[WeaponCount];
        public ushort[] specialAmounts = new ushort[SpecialCount];
    }
}
