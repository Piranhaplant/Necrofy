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

        /// <summary>Builds the TitlePage for insterting into a ROM</summary>
        /// <param name="data">The data to build into</param>
        /// <param name="page">The number of the page. 0 is the first page, 1 is the second page</param>
        public MovableData Build(int page) {
            MovableData data = new MovableData();
            // Page number used internally is backwards
            byte actualPage = (byte)(page == 0 ? 1 : 0);
            for (int i = 0; i < words.Count; i++) {
                words[i].Build(data, actualPage, i == words.Count - 1);
            }
            return data;
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

            public void Build(MovableData data, byte page, bool last) {
                data.data.Add(x);
                data.data.Add(y);
                data.data.Add(palette);
                data.data.Add(page);
                foreach (byte c in chars) {
                    data.data.Add(c);
                }
                data.data.Add((byte)(last ? 0 : 0xff));
            }
        }
    }
}
