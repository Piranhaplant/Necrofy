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

        public IEnumerable<Entry> Entries => entries;
        public bool Success => !entries.Any(e => e.level == Entry.Level.ERROR);

        public void AddEntry(Entry entry) {
            entries.Add(entry);
        }

        public class Entry
        {
            public readonly Level level;
            public readonly string file;
            public readonly string description;

            public Entry(Level level, string file, string description) {
                this.level = level;
                this.file = file;
                this.description = description;
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
