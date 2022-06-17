using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Necrofy
{
    class Win32
    {
        [DllImport("shell32.dll")]
        private static extern long FindExecutable(string lpFile, string lpDirectory, [Out] StringBuilder lpResult);

        public static string FindExecutable(string filename) {
            try {
                StringBuilder executablePath = new StringBuilder();
                long errorCode = FindExecutable(filename, string.Empty, executablePath);
                if (errorCode > 32) {
                    return executablePath.ToString();
                } else {
                    return null;
                }
            } catch (Exception ex) {
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.StackTrace);
                return null;
            }
        }

        [DllImport("shlwapi.dll")]
        private static extern int ColorHLSToRGB(int H, int L, int S);

        public const int HSLMax = 240;

        public static Color HLSToRGB(int H, int L, int S) {
            if (S == 0) {
                int v = (int)Math.Round((double)L / HSLMax * 255);
                return Color.FromArgb(v, v, v);
            }
            return ColorTranslator.FromWin32(ColorHLSToRGB(H, L, S));
        }
    }
}
