using Newtonsoft.Json;
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
            AddLoader(Type,
                (r, s, tileset) => {
                    s.GoToPointerPush();
                    LevelMonster m = new TileAnimLevelMonster(s);
                    s.PopPosition();
                    return m;
                },
                () => new TileAnimLevelMonster(new List<Entry>()));
        }

        [JsonConstructor]
        public TileAnimLevelMonster(List<Entry> entries) : base(Type) {
            this.entries = entries;
        }

        public TileAnimLevelMonster(NStream s) : base(Type) {
            entries = new List<Entry>();
            while (s.PeekInt16() > 0) {
                s.GoToRelativePointerPush();
                entries.Add(new Entry(s));
                s.PopPosition();
            }
        }

        public override void Build(MovableData data, ROMInfo rom) {
            data.data.AddPointer(type);
            MovableData animData = new MovableData();
            foreach (Entry entry in entries) {
                MovableData entryData = new MovableData();
                entry.Build(entryData);
                animData.AddPointer(MovableData.PointerSize.TwoBytes, entryData);
            }
            animData.data.AddInt16(0);
            data.AddPointer(MovableData.PointerSize.FourBytes, animData);
        }

        public class Entry
        {
            public List<ushort> tiles { get; set; }
            public bool loop { get; set; }

            public Entry() { }

            public Entry(NStream s) {
                tiles = new List<ushort>();
                while (true) {
                    ushort value = s.ReadInt16();
                    if (value >= 0xfffe) {
                        loop = value == 0xffff;
                        return;
                    }
                    tiles.Add(value);
                }
            }

            public void Build(MovableData data) {
                foreach (ushort tile in tiles) {
                    data.data.AddInt16(tile);
                }
                if (loop) {
                    data.data.AddInt16(0xffff);
                } else {
                    data.data.AddInt16(0xfffe);
                }
            }

            public override bool Equals(object obj) {
                var entry = obj as Entry;
                return entry != null &&
                       tiles.SequenceEqual(entry.tiles) &&
                       loop == entry.loop;
            }

            public override int GetHashCode() {
                var hashCode = 465692521;
                hashCode = hashCode * -1521134295 + EqualityComparer<List<ushort>>.Default.GetHashCode(tiles);
                hashCode = hashCode * -1521134295 + loop.GetHashCode();
                return hashCode;
            }
        }
    }
}
