using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Necrofy
{
    static class Extensions
    {
        public static int GetWidth<T>(this T[,] array) {
            return array.GetLength(0);
        }

        public static int GetHeight<T>(this T[,] array) {
            return array.GetLength(1);
        }
    }
}
