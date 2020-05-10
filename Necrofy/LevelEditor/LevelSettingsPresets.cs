using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Necrofy
{
    class LevelSettingsPresets
    {
        public List<Preset<int>> paletteAnimation;
        public List<Preset<ushort>> music;
        public List<Preset<ushort>> sounds;

        public class Preset<T>
        {
            public string name;
            public T value;

            public override string ToString() {
                return name ?? "";
            }
        }
    }
}
