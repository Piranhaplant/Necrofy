using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Necrofy
{
    class ExtractionPreset
    {
        public enum AssetType {
            Binary,
            Graphics,
            Graphics2BPP,
            Palette,
            Tilemap,
            TilemapSized,
        }

        public string Description { get; set; }
        public string Filename { get; set; }
        public string Category { get; set; }
        public int Address { get; set; }
        public int Length { get; set; }
        public bool Compressed { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public AssetType Type { get; set; }
        public AssetOptions.Options Options { get; set; }

    }
}
