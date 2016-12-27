using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Necrofy
{
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

        public class Word
        {
            public byte x { get; set; }
            public byte y { get; set; }
            public byte palette { get; set; }
            public List<byte> chars { get; set; }
            public bool last { get; set; }

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
