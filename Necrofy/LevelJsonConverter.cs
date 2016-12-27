using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Necrofy
{
    class LevelJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) {
            return typeof(LevelMonster).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
            JObject jObject = JObject.Load(reader);
            LevelMonster target = LevelMonster.GetBlank((int)jObject["type"]);
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
