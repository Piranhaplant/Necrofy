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
    class TitlePage {
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
        public class Word {
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
                BytesToString(chars, 0, chars.Count, true, out string s);
                return s;
            }
        }

        public static bool BytesToString(List<byte> bytes, int start, int end, bool capitalize, out string result) {
            StringBuilder builder = new StringBuilder();
            bool success = true;

            bool capitalizeNextChar = capitalize;
            for (int i = start; i < end;) {
                int value = 0;
                int valueLength = 0;
                for (int j = i; j < end && j < i + MaxByteLength; j++) {
                    value = (value << 8) | bytes[j];
                    valueLength++;
                }
                bool found = false;
                while (valueLength > 0 && !found) {
                    if (BytesToStringMap.TryGetValue(value, out string s)) {
                        s = Capitalize(s, capitalizeNextChar);
                        capitalizeNextChar = capitalize && s == " ";
                        builder.Append(s);
                        i += valueLength;
                        found = true;
                        break;
                    }
                    value = value >> 8;
                    valueLength--;
                }
                if (!found) {
                    success = false;
                    i++;
                }
            }
            result = builder.ToString();
            return success;
        }

        private static string Capitalize(string s, bool capitalize) {
            if (capitalize && s.Length > 0) {
                return s.Substring(0, 1).ToUpperInvariant() + s.Substring(1);
            }
            return s;
        }

        private static readonly Random random = new Random();

        public static List<byte> StringToBytes(string s) {
            List<byte> result = new List<byte>();
            for (int i = 0; i < s.Length;) {
                bool found = false;
                for (int subLen = Math.Min(s.Length - i, MaxStringLength); subLen > 0; subLen--) {
                    if (StringToBytesMap.TryGetValue(s.Substring(i, subLen), out List<int> allBytes)) {
                        int bytes = allBytes[random.Next(0, allBytes.Count)];
                        List<byte> newBytes = new List<byte>();
                        while (bytes > 0) {
                            newBytes.Add((byte)(bytes & 0xff));
                            bytes >>= 8;
                        }
                        result.AddRange(Enumerable.Reverse(newBytes));
                        found = true;
                        i += subLen;
                        break;
                    }
                }
                if (!found) {
                    i++;
                }
            }
            return result;
        }

        public static readonly Dictionary<int, string> BytesToStringMap;
        public static readonly Dictionary<string, List<int>> StringToBytesMap;

        public static readonly HashSet<char> ValidChars;
        public static readonly int MaxByteLength;
        public static readonly int MaxStringLength;
        public static readonly byte SpaceCharValue;

        static TitlePage() {
            TupleList<int, string> pairs = new TupleList<int, string>() {
                {0x21, "nt"}, {0x22, "th"}, {0x23, "te"}, {0x24, "ste"}, {0x25, "it"}, {0x25, "lt"}, {0x26, "et"},
                {0x27, "sti"}, {0x27, "stl"}, {0x2771, "str"}, {0x277a, "str"}, {0x2772, "sth"}, {0x276f, "stn"},
                {0x28, "rti"}, {0x28, "rtl"}, {0x2871, "rtr"}, {0x287a, "rtr"}, {0x2872, "rth"}, {0x286f, "rtn"},
                {0x29, "to"}, {0x2a77, "nts"}, {0x612b, "rty"}, {0x6a2b, "rty"}, {0x2c7879, "ctor"},
                {0x2c78, "ctoi"}, {0x2c78, "ctol"}, {0x2d, "t"}, {0x7b2e, "nta"}, {0x2e, "ita"}, {0x2e, "lta"},
                {0x2f77, "pts"}, {0x30, "0"}, {0x31, "1"}, {0x32, "2"}, {0x33, "3"}, {0x34, "4"}, {0x35, "5"},
                {0x36, "6"}, {0x37, "7"}, {0x38, "8"}, {0x39, "9"}, {0x613a, "ro"}, {0x6a3a, "ro"}, {0x3b, "q"},
                {0x41, "a"}, {0x42, "b"}, {0x43, "c"}, {0x44, "d"}, {0x45, "e"}, {0x46, "f"}, {0x47, "g"}, {0x48, "h"},
                {0x49, "a"}, {0x4a, "j"}, {0x4b, "k"}, {0x4c, "l"}, {0x4d, "m"}, {0x4e, "n"}, {0x4f, "o"}, {0x50, "p"},
                {0x51, "r"}, {0x52, "r"}, {0x53, "s"}, {0x54, "p"}, {0x55, "u"}, {0x56, "ei"}, {0x57, "li"}, {0x58, "x"},
                {0x59, "y"}, {0x5a, "z"}, {0x615b, "re"}, {0x6a5b, "re"}, {0x615c, "rs"}, {0x6a5c, "rs"},
                {0x73745d, "htm"}, {0x745d, "itm"}, {0x745d, "ltm"}, {0x62, "e"}, {0x63, "e"}, {0x64, "i"}, {0x64, "l"},
                {0x65, "o"}, {0x66, "s"}, {0x67, "!"}, {0x6869, "fe"}, {0x687a, "fo"}, {0x616b, "r"}, {0x6a6b, "r"},
                {0x6c6d, "le"}, {0x6e6f, "w"}, {0x70, "i"}, {0x75, "v"}, {0x7576, "ve"}, {0x757664, "vel"}, {0x7d, " "},
                {0x7c, " "}, {0x20, " "}, {0x3c, " "}};

            BytesToStringMap = new Dictionary<int, string>();
            foreach (Tuple<int, string> t in pairs) {
                if (!BytesToStringMap.ContainsKey(t.Item1)) {
                    BytesToStringMap[t.Item1] = t.Item2;
                }
            }

            StringToBytesMap = new Dictionary<string, List<int>>();
            foreach (Tuple<int, string> t in pairs) {
                if (!StringToBytesMap.ContainsKey(t.Item2)) {
                    StringToBytesMap[t.Item2] = new List<int>();
                }
                List<int> list = StringToBytesMap[t.Item2];
                if (list.Count == 0 || t.Item2 != " ") {
                    list.Add(t.Item1);
                }
            }

            ValidChars = new HashSet<char>(pairs.Select(p => p.Item2).SelectMany(s => s.ToCharArray()));
            MaxByteLength = pairs.Select(p => (p.Item1.ToString("X").Length + 1) / 2).Max();
            MaxStringLength = pairs.Select(p => p.Item2.Length).Max();
            SpaceCharValue = (byte)StringToBytesMap[" "][0];
        }
    }
}
