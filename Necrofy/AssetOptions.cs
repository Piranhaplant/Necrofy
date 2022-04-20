using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Necrofy
{
    public class AssetOptions
    {
        private static readonly Dictionary<AssetCategory, Func<Options>> optionsTemplates = new Dictionary<AssetCategory, Func<Options>>() {
            { AssetCategory.Graphics, () => new GraphicsOptions() },
            { AssetCategory.Tilemap, () => new TilemapOptions() },
        };

        public List<Entry> entries = new List<Entry>();

        private Entry GetEntry(AssetCategory category, string name) {
            foreach (Entry entry in entries) {
                if (entry.category == category && entry.name == name) {
                    return entry;
                }
            }
            return null;
        }

        public Options GetOptions(AssetCategory category, string name) {
            return GetEntry(category, name)?.options;
        }

        public void SetOptions(AssetCategory category, string name, Options options) {
            if (options == null) {
                return;
            }
            Entry entry = GetEntry(category, name);
            if (entry == null) {
                entries.Add(new Entry(category, name, options));
            } else {
                entry.options = options;
            }
        }

        public void Merge(AssetOptions others) {
            foreach (Entry e in others.entries) {
                if (GetEntry(e.category, e.name) == null) {
                    entries.Add(e);
                }
            }
        }

        public class Entry
        {
            public AssetCategory category;
            public string name;
            public Options options;

            public Entry() { }

            public Entry(AssetCategory category, string name, Options options) {
                this.category = category;
                this.name = name;
                this.options = options;
            }
        }

        public class Options { }

        public class GraphicsOptions : Options
        {
            public int width;
            public bool transparency;
            public bool largeTiles;

            public GraphicsOptions() { }

            public GraphicsOptions(int width, bool transparency, bool largeTiles) {
                this.width = width;
                this.transparency = transparency;
                this.largeTiles = largeTiles;
            }
        }

        public class TilemapOptions : Options
        {
            public int width;
            public bool transparency;
            public Hinting.Type hinting;

            public TilemapOptions() { }

            public TilemapOptions(int width, bool transparency, Hinting.Type hinting = Hinting.Type.None) {
                this.width = width;
                this.transparency = transparency;
                this.hinting = hinting;
            }
        }

        public class OptionJsonConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType) {
                return typeof(Entry).IsAssignableFrom(objectType);
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
                JObject jObject = JObject.Load(reader);
                if (optionsTemplates.TryGetValue((AssetCategory)(int)jObject["category"], out Func<Options> optionGetter)) {
                    Entry target = new Entry();
                    target.options = optionGetter();
                    serializer.Populate(jObject.CreateReader(), target);
                    return target;
                }
                return null;
            }

            public override bool CanWrite {
                get {
                    return false;
                }
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
                throw new NotImplementedException();
            }
        }
    }
}
