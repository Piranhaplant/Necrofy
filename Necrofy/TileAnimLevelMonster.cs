using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Necrofy
{
    /// <summary>
    /// Level monster that animates certain tiles in the level background
    /// </summary>
    class TileAnimLevelMonster : LevelMonster
    {
        public const int Type = 0x157cf;

        public List<Entry> entries { get; set; }

        public static void RegisterLoader() {
            LevelMonster.AddLoader(Type,
                (r, s) => {
                    s.GoToPointerPush();
                    LevelMonster m = new TileAnimLevelMonster(s);
                    s.PopPosition();
                    return m;
                },
                () => new TileAnimLevelMonster());
        }

        public TileAnimLevelMonster() { }

        public TileAnimLevelMonster(NStream s) : base(Type) {
            entries = new List<Entry>();
            while (s.PeekInt16() > 0) {
                s.GoToRelativePointerPush();
                entries.Add(new Entry(s));
                s.PopPosition();
            }
        }

        public class Entry
        {
            public List<ushort> tiles { get; set; }
            public bool flag { get; set; }

            public Entry() { }

            public Entry(NStream s) {
                tiles = new List<ushort>();
                while (true) {
                    ushort value = s.ReadInt16();
                    if (value >= 0xfffe) {
                        flag = value == 0xfffe;
                        return;
                    }
                    tiles.Add(value);
                }
            }
        }
    }
}
