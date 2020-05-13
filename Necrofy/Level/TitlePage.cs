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

        [JsonConstructor]
        public TitlePage(List<Word> words) {
            this.words = words;
        }

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

        public override string ToString() {
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < words.Count; i++) {
                if (i > 0) {
                    result.Append(" ");
                }
                result.Append(words[i].ToString());
            }
            return result.ToString();
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

            [JsonConstructor]
            public Word(byte x, byte y, byte palette) {
                this.x = x;
                this.y = y;
                this.palette = palette;
                chars = new List<byte>();
            }

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

            public override string ToString() {
                StringBuilder result = new StringBuilder(chars.Count);
                bool capitalize = true;
                for (int i = 0; i < chars.Count; ) {
                    int value = 0;
                    int valueLength = 0;
                    for (int j = i; j < chars.Count && j < i + maxByteLength; j++) {
                        value = (value << 8) | chars[j];
                        valueLength++;
                    }
                    bool found = false;
                    while (valueLength > 0 && !found) {
                        foreach (Tuple<string, int> t in characterMap) {
                            if (t.Item2 == value) {
                                string s = Capitalize(t.Item1, capitalize);
                                capitalize = s == " ";
                                result.Append(s);
                                i += valueLength;
                                found = true;
                                break;
                            }
                        }
                        value = value >> 8;
                        valueLength--;
                    }
                    if (!found) {
                        capitalize = false;
                        result.Append("?");
                        i++;
                    }
                }
                return result.ToString();
            }

            private static string Capitalize(string s, bool capitalize) {
                if (capitalize && s.Length > 0) {
                    return s.Substring(0, 1).ToUpperInvariant() + s.Substring(1);
                }
                return s;
            }
        }

        // TODO: "a", "p" should be randomized too
        // TODO: Test that these are all right
        private static readonly TupleList<string, int> characterMap = new TupleList<string, int>() {
            {"nt", 0x21}, {"th", 0x22}, {"te", 0x23}, {"ste", 0x24}, {"it", 0x25}, {"lt", 0x25}, {"et", 0x26},
            {"sti", 0x27}, {"stl", 0x27}, {"str", 0x2771}, {"str", 0x277a}, {"sth", 0x2772}, {"stn", 0x276f},
            {"rti", 0x28}, {"rtl", 0x28}, {"rtr", 0x2871}, {"rtr", 0x287a}, {"rth", 0x2872}, {"rtn", 0x286f},
            {"to", 0x29}, {"nts", 0x2a77}, {"rty", 0x612b}, {"rty", 0x6a2b}, {"ctor", 0x2c7879},
            {"ctoi", 0x2c78}, {"ctol", 0x2c78}, {"t", 0x2d}, {"nta", 0x7b2e}, {"ita", 0x2e}, {"lta", 0x2e},
            {"pts", 0x2f77}, {"0", 0x30}, {"1", 0x31}, {"2", 0x32}, {"3", 0x33}, {"4", 0x34}, {"5", 0x35},
            {"6", 0x36}, {"7", 0x37}, {"8", 0x38}, {"9", 0x39}, {"ro", 0x613a}, {"ro", 0x6a3a}, {"q", 0x3b},
            {"a", 0x41}, {"b", 0x42}, {"c", 0x43}, {"d", 0x44}, {"e", 0x45}, {"f", 0x46}, {"g", 0x47}, {"h", 0x48},
            {"a", 0x49}, {"j", 0x4a}, {"k", 0x4b}, {"l", 0x4c}, {"m", 0x4d}, {"n", 0x4e}, {"o", 0x4f}, {"p", 0x50},
            {"r", 0x51}, {"r", 0x52}, {"s", 0x53}, {"p", 0x54}, {"u", 0x55}, {"ei", 0x56}, {"li", 0x57}, {"x", 0x58},
            {"y", 0x59}, {"z", 0x5a}, {"re", 0x615b}, {"re", 0x6a5b}, {"rs", 0x615c}, {"rs", 0x6a5c},
            {"htm", 0x73745d}, {"itm", 0x745d}, {"ltm", 0x745d}, {"e", 0x62}, {"e", 0x63}, {"i", 0x64}, {"l", 0x64},
            {"o", 0x65}, {"s", 0x66}, {"!", 0x67}, {"fe", 0x6869}, {"fo", 0x687a}, {"r", 0x616b}, {"r", 0x6a6b},
            {"le", 0x6c6d}, {"w", 0x6e6f}, {"i", 0x70}, {"v", 0x75}, {"ve", 0x7576}, {"vel", 0x757664}, {" ", 0x7e},
            {" ", 0x7d}, {" ", 0x20}, {" ", 0x3c}};
        private const int maxByteLength = 3;
        private const int maxStringLength = 4;
    }
}
