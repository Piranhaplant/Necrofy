using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Necrofy
{
    public class Symbols
    {
        private Dictionary<string, string> labels = new Dictionary<string, string>();

        public void AddWLASymbols(string filename) {
            bool inLabelSection = false;
            foreach (string line in File.ReadLines(filename)) {
                if (line.Length == 0) {
                    continue;
                }
                if (line.StartsWith("[")) {
                    if (line == "[labels]") {
                        inLabelSection = true;
                    }
                } else if (inLabelSection) {
                    string[] parts = line.Split(new char[] { ' ' }, 2);
                    if (parts.Length == 2) {
                        string address = parts[0];
                        string label = parts[1];
                        if (!label.StartsWith(":")) { // Ignore special labels
                            labels[address] = Path.ChangeExtension(Path.GetFileName(filename), "asm") + "_" + label;
                        }
                    }
                }
            }
        }

        public void WriteBSNES(string romFilename) {
            List<string> lines = new List<string>();
            lines.Add("#SNES65816");
            lines.Add("[SYMBOL]");
            foreach (KeyValuePair<string, string> entry in labels) {
                lines.Add(entry.Key.ToLowerInvariant() + " " + entry.Value + " ANY 1");
            }
            File.WriteAllText(Path.ChangeExtension(romFilename, "cpu.sym"), String.Join("\n", lines));
        }
    }
}
