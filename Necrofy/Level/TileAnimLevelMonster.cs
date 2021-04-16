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
                    if (s.PeekPointer() >= 0) {
                        s.GoToPointerPush();
                        LevelMonster m = new TileAnimLevelMonster(s);
                        s.PopPosition();
                        return m;
                    } else {
                        return new TileAnimLevelMonster(new List<Entry>());
                    }
                },
                () => new TileAnimLevelMonster(new List<Entry>()));
        }

        [JsonConstructor]
        public TileAnimLevelMonster(List<Entry> entries) : base(Type) {
            this.entries = entries;
        }

        public TileAnimLevelMonster(NStream s) : base(Type) {
            entries = new List<Entry>();
            while (s.PeekInt16() >= 0x8000) {
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
            public ushort initialTile { get; set; }
            public List<Frame> frames { get; set; }
            public bool loop { get; set; }

            public Entry() { }

            public Entry(NStream s) {
                initialTile = s.ReadInt16();
                frames = new List<Frame>();
                while (true) {
                    ushort delay = s.ReadInt16();
                    if (delay >= 0xfffe) {
                        loop = delay == 0xffff;
                        return;
                    }
                    ushort tile = s.ReadInt16();
                    if (tile > 0x200) {
                        // Probably invalid data, just ignore
                        frames.Clear();
                        return;
                    }
                    frames.Add(new Frame(delay, tile));
                }
            }

            public void Build(MovableData data) {
                data.data.AddInt16(initialTile);
                foreach (Frame frame in frames) {
                    data.data.AddInt16(frame.delay);
                    data.data.AddInt16(frame.tile);
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
                       initialTile == entry.initialTile &&
                       frames.SequenceEqual(entry.frames) &&
                       loop == entry.loop;
            }

            public override int GetHashCode() {
                var hashCode = 601590297;
                hashCode = hashCode * -1521134295 + initialTile.GetHashCode();
                foreach (Frame frame in frames) {
                    hashCode = hashCode * -1521134295 + frame.GetHashCode();
                }
                hashCode = hashCode * -1521134295 + loop.GetHashCode();
                return hashCode;
            }
            
            public class Frame
            {
                public ushort delay { get; set; }
                public ushort tile { get; set; }

                public Frame(ushort delay, ushort tile) {
                    this.delay = delay;
                    this.tile = tile;
                }

                public override bool Equals(object obj) {
                    var frame = obj as Frame;
                    return frame != null &&
                           delay == frame.delay &&
                           tile == frame.tile;
                }

                public override int GetHashCode() {
                    var hashCode = 1256124513;
                    hashCode = hashCode * -1521134295 + delay.GetHashCode();
                    hashCode = hashCode * -1521134295 + tile.GetHashCode();
                    return hashCode;
                }
            }
        }
    }
}
