using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Necrofy
{
    class LevelSettingsPresets
    {
        public List<Preset<int>> paletteAnimations;
        public List<Preset<ushort>> music;
        public List<Preset<ushort>> sounds;
        public List<Preset<ushort>> bonuses;
        public List<Preset<ushort>> secretBonuses;
        public List<Preset<List<TileAnimLevelMonster.Entry>>> tileAnimations;

        public class Preset<T>
        {
            public string name;
            public T value;

            public Preset() { }

            public Preset(string name, T value) {
                this.name = name;
                this.value = value;
            }

            public override string ToString() {
                return name ?? "";
            }
        }
    }
}
