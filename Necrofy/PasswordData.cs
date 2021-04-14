using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Necrofy
{
    class PasswordData
    {
        public string level0Password = "";
        public string[,] normalPasswords = { };

        public PasswordData() { }

        public PasswordData(NStream s) {
            // TODO: loading from necrofy generated ROM
            byte[] innerCharsPerLevel = s.ReadBytes(26);
            byte[] outerCharsPerVictimCount = s.ReadBytes(20);
            string chars = GameToASCII(Encoding.ASCII.GetString(s.ReadBytes(21)));
            byte[] outerCharsOffsetPerLevel = s.ReadBytes(26);

            normalPasswords = new string[outerCharsPerVictimCount.Length / 2, innerCharsPerLevel.Length / 2];
            for (int level = 0; level < normalPasswords.GetLength(1); level++) {
                for (int victims = 0; victims < normalPasswords.GetLength(0); victims++) {
                    int char1 = outerCharsPerVictimCount[victims * 2] + outerCharsOffsetPerLevel[level * 2];
                    int char4 = outerCharsPerVictimCount[victims * 2 + 1] + outerCharsOffsetPerLevel[level * 2 + 1];
                    byte char2 = innerCharsPerLevel[level * 2 + 1];
                    byte char3 = innerCharsPerLevel[level * 2];
                    normalPasswords[victims, level] = "" + chars[char1 & 0xFF] + chars[char2] + chars[char3] + chars[char4 & 0xFF];
                }
            }

            s.PushPosition();
            s.Seek(ROMPointers.Level0Password1);
            level0Password += GameToASCII(Encoding.ASCII.GetString(s.ReadBytes(2)));
            s.Seek(ROMPointers.Level0Password2);
            level0Password += GameToASCII(Encoding.ASCII.GetString(s.ReadBytes(2)));
            s.PopPosition();
        }

        public void WriteToROM(NStream s, ROMInfo romInfo) {
            int pointer = romInfo.Freespace.Claim(normalPasswords.Cast<string>().Sum(str => str.Length));
            s.Seek(pointer);

            for (int level = 0; level < normalPasswords.GetLength(1); level++) {
                for (int victims = 0; victims < normalPasswords.GetLength(0); victims++) {
                    s.Write(Encoding.ASCII.GetBytes(ASCIIToGame(normalPasswords[victims, level])));
                }
            }

            s.Seek(ROMPointers.Level0Password1);
            s.Write(Encoding.ASCII.GetBytes(ASCIIToGame(level0Password.Substring(0, 2))));
            s.Seek(ROMPointers.Level0Password2);
            s.Write(Encoding.ASCII.GetBytes(ASCIIToGame(level0Password.Substring(2, 2))));

            romInfo.exportedDefines["PASSWORD_DATA"] = pointer.ToString();
            romInfo.exportedDefines["PASSWORD_MAX_LEVEL"] = (normalPasswords.GetLength(1) * 4 + 5).ToString();
        }

        private string GameToASCII(string password) {
            return password.Replace("?", "!").Replace("/", " ");
        }

        private string ASCIIToGame(string password) {
            return password.Replace("!", "?").Replace(" ", "/");
        }
    }
}
