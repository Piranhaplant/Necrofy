using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Necrofy
{
    public class BuildResults
    {
        private List<Entry> entries = new List<Entry>();
        public readonly Symbols symbols = new Symbols();

        public IReadOnlyList<Entry> Entries => entries;
        public bool Success => !entries.Any(e => e.level == Entry.Level.ERROR);
        public bool HasWarnOrAbove => entries.Any(e => e.level == Entry.Level.WARNING || e.level == Entry.Level.ERROR);

        public void AddEntry(Entry entry) {
            entries.Add(entry);
        }

        public class Entry
        {
            public readonly Level level;
            public readonly string file;
            public readonly string description;
            public readonly string stackTrace;

            public Entry(Level level, string file, string description, string stackTrace = "") {
                this.level = level;
                this.file = file;
                this.description = description;
                this.stackTrace = stackTrace;
            }

            public enum Level
            {
                ERROR,
                WARNING,
                INFO,
            }
        }
    }
}
