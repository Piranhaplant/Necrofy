using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
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
            [JsonConverter(typeof(StringEnumConverter))]
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
            public int width = 16;
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
            public int width = 32;
            public bool transparency;
            public bool largeTiles;
            [JsonConverter(typeof(StringEnumConverter))]
            public Hinting.Type hinting;

            public TilemapOptions() { }

            public TilemapOptions(int width, bool transparency, bool largeTiles, Hinting.Type hinting = Hinting.Type.None) {
                this.width = width;
                this.transparency = transparency;
                this.largeTiles = largeTiles;
                this.hinting = hinting;
            }
        }

        public class OptionJsonConverter : JsonConverter
        {
            private static readonly Dictionary<AssetCategory, Func<Options>> optionsTemplates = new Dictionary<AssetCategory, Func<Options>>() {
                { AssetCategory.Graphics, () => new GraphicsOptions() },
                { AssetCategory.Tilemap, () => new TilemapOptions() },
            };
            private static readonly Dictionary<int, AssetCategory> legacyConversions = new Dictionary<int, AssetCategory>() {
                { 5, AssetCategory.Graphics },
                { 7, AssetCategory.Tilemap },
            };

            public override bool CanConvert(Type objectType) {
                return typeof(Entry).IsAssignableFrom(objectType);
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
                JObject jObject = JObject.Load(reader);
                Entry target = new Entry();
                JToken categoryToken = jObject["category"];
                AssetCategory category;

                if (categoryToken.Type == JTokenType.Integer) {
                    if (legacyConversions.TryGetValue((int)categoryToken, out category)) {
                        target.options = optionsTemplates[category]();
                    } else {
                        throw new Exception("Unknown asset options type");
                    }
                } else {
                    category = (AssetCategory)Enum.Parse(typeof(AssetCategory), (string)categoryToken);
                    if (optionsTemplates.TryGetValue(category, out Func<Options> optionGetter)) {
                        target.options = optionGetter();
                    } else {
                        throw new Exception("Unknown asset options type");
                    }
                }
                serializer.Populate(jObject.CreateReader(), target);
                target.category = category;
                return target;
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

        public class AssetExtractionJsonConverter : JsonConverter
        {
            private static readonly Dictionary<ExtractionPreset.AssetType, Func<Options>> optionsTemplates = new Dictionary<ExtractionPreset.AssetType, Func<Options>>() {
                { ExtractionPreset.AssetType.Graphics, () => new GraphicsOptions() },
                { ExtractionPreset.AssetType.Graphics2BPP, () => new GraphicsOptions() },
                { ExtractionPreset.AssetType.Tilemap, () => new TilemapOptions() },
                { ExtractionPreset.AssetType.TilemapSized, () => new TilemapOptions() },
            };

            public override bool CanConvert(Type objectType) {
                return typeof(ExtractionPreset).IsAssignableFrom(objectType);
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
                JObject jObject = JObject.Load(reader);
                ExtractionPreset.AssetType assetType = (ExtractionPreset.AssetType)Enum.Parse(typeof(ExtractionPreset.AssetType), (string)jObject["Type"]);
                ExtractionPreset target = new ExtractionPreset();
                if (jObject["Options"] != null && optionsTemplates.TryGetValue(assetType, out Func<Options> optionGetter)) {
                    target.Options = optionGetter();
                }
                serializer.Populate(jObject.CreateReader(), target);
                return target;
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
