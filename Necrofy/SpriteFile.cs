using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Necrofy
{
    class SpriteFile
    {
        public List<string> graphicsAssets { get; private set; } = new List<string>();
        public List<Sprite> sprites { get; private set; } = new List<Sprite>();

        public SpriteFile() { }
        public SpriteFile(List<Sprite> sprites) {
            this.sprites = sprites;
        }
        public SpriteFile(List<Sprite> sprites, List<string> graphicsAssets) {
            this.sprites = sprites;
            this.graphicsAssets = graphicsAssets;
        }

        public class Converter : JsonConverter
        {
            public override bool CanConvert(Type objectType) {
                return typeof(SpriteFile).IsAssignableFrom(objectType);
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
                SpriteFile target = new SpriteFile();
                // In older versions, the sprites were stored as just an array
                if (reader.TokenType == JsonToken.StartArray) {
                    JArray jArray = JArray.Load(reader);
                    serializer.Populate(jArray.CreateReader(), target.sprites);
                } else {
                    JObject jObject = JObject.Load(reader);
                    serializer.Populate(jObject.CreateReader(), target);
                }
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
