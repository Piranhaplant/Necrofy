using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Necrofy
{
    public class SpriteDisplayList
    {
        public List<ImageSpriteDisplay> imageSprites;
        public List<TextSpriteDisplay> textSprites;
    }

    public abstract class SpriteDisplay
    {
        public Key key;
        public Category category;

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
            OneShotMonster,
            Victim,
            Player,
            CreditHead,
            LevelMonster,
        }
    }

    public class ImageSpriteDisplay : SpriteDisplay
    {
        public int spriteIndex;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? overridePalette;
    }

    public class TextSpriteDisplay : SpriteDisplay
    {
        public string text;
    }
}
