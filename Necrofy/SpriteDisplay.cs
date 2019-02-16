using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Necrofy
{
    class SpriteDisplay
    {
        public int spriteIndex;
        public Key key;
        public Category category;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? overridePalette;

        public SpriteDisplay() { }

        public SpriteDisplay(int spriteIndex, Key.Type keyType, int keyValue, Category category, int? overridePalette = null) {
            this.spriteIndex = spriteIndex;
            this.key = new Key(keyType, keyValue);
            this.category = category;
            this.overridePalette = overridePalette;
        }

        public class Key
        {
            public Type type;
            public int value;

            public Key() { }

            public Key(Type type, int value) {
                this.type = type;
                this.value = value;
            }

            public override bool Equals(object obj) {
                var key = obj as Key;
                return key != null &&
                       type == key.type &&
                       value == key.value;
            }

            public override int GetHashCode() {
                var hashCode = 1148455455;
                hashCode = hashCode * -1521134295 + type.GetHashCode();
                hashCode = hashCode * -1521134295 + value.GetHashCode();
                return hashCode;
            }

            public enum Type
            {
                Pointer,
                Item,
                Player,
                CreditHead,
            }
        }
        
        public enum Category
        {
            Item,
            Monster,
            OneTimeMonster,
            Victim,
            Player,
            CreditHead,
        }
    }
}
