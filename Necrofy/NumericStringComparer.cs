using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace Necrofy
{
    public class NumericStringComparer : IComparer<string>
    {
        private static readonly char[] digits = new char[] {'0', '1', '2', '3', '4', '5', '6', '7', '8', '9'};

        public static readonly NumericStringComparer instance = new NumericStringComparer();

        public int Compare(string x, string y) {
            int curPos = 0;
            while (curPos < x.Length && curPos < y.Length) {
                int xNextPos = GetNextPos(x, curPos);
                int yNextPos = GetNextPos(y, curPos);
                string xChunk = x.Substring(curPos, xNextPos - curPos);
                string yChunk = y.Substring(curPos, yNextPos - curPos);
                int chunkCompare;
                if (digits.Contains(x[curPos]) && digits.Contains(y[curPos])) {
                    chunkCompare = BigInteger.Parse(xChunk).CompareTo(BigInteger.Parse(yChunk));
                    // Determine order in cases like "1" vs "01"
                    if (chunkCompare == 0) {
                        chunkCompare = string.Compare(xChunk, yChunk, true);
                    }
                } else {
                    chunkCompare = string.Compare(xChunk, yChunk, true);
                }
                if (chunkCompare != 0) {
                    return chunkCompare;
                }
                // xNextPos == yNextPos since the chunks were equal
                curPos = xNextPos;
            }
            // Strings are equal up to the length of the shorter string, so they can be compared by length only
            return x.Length.CompareTo(y.Length);
        }

        private static int GetNextPos(string s, int curPos) {
            if (digits.Contains(s[curPos])) {
                do {
                    curPos += 1;
                } while (curPos < s.Length && digits.Contains(s[curPos]));
            } else {
                curPos = s.IndexOfAny(digits, curPos);
                if (curPos < 0) {
                    curPos = s.Length;
                }
            }
            return curPos;
        }
    }
}
