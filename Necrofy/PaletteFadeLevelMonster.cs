using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Necrofy
{
    class PaletteFadeLevelMonster : LevelMonster
    {
        public const int Type = 0x12b95;

        public string bgPal { get; set; }
        public string spritePal { get; set; }

        public static void RegisterLoader() {
            LevelMonster.AddLoader(Type,
                (r, s) => {
                    s.GoToPointerPush();
                    LevelMonster m = new PaletteFadeLevelMonster(r, s);
                    s.PopPosition();
                    return m;
                },
                () => new PaletteFadeLevelMonster());
        }

        public PaletteFadeLevelMonster() { }

        public PaletteFadeLevelMonster(ROMInfo r, NStream s) : base(Type) {
            bgPal = r.GetPaletteName(s.ReadPointer());
            spritePal = r.GetPaletteName(s.ReadPointer());
        }
    }
}
