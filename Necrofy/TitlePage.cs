using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Necrofy
{
    /// <summary>
    /// One of the two screens that are shown before a level is played
    /// </summary>
    class TitlePage
    {
        public List<Word> words { get; set; }

        public TitlePage() { }

        public TitlePage(NStream s) {
            words = new List<Word>();
            while (true) {
                Word w = new Word(s);
                words.Add(w);
                if (w.last) break;
            }
        }

        /// <summary>
        /// One string of characters displayed on a TitlePage
        /// </summary>
        public class Word
        {
            public byte x { get; set; }
            public byte y { get; set; }
            public byte palette { get; set; }
            public List<byte> chars { get; set; }
            [JsonIgnore]
            public bool last { get; private set; }

            public Word() { }

            public Word(NStream s) {
                chars = new List<byte>();
                x = (byte)s.ReadByte();
                y = (byte)s.ReadByte();
                palette = (byte)s.ReadByte();
                s.ReadByte(); // Skip page number
                while (true) {
                    byte num = (byte)s.ReadByte();
                    if (num == 0 || num == 0xff) {
                        last = num == 0;
                        break;
                    }
                    chars.Add(num);
                }
            }
        }
    }
}
