using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Necrofy
{
    public class ProjectSettings
    {
        // The current project version
        private const int curVersion = 1;
        /// <summary>The version of the project</summary>
        public int Version { get; set; }
        /// <summary>State of the folders in the project browser</summary>
        public List<FolderState> FolderStates { get; set; }
        /// <summary>List of asset files that are currently opened for editing</summary>
        public List<string> OpenFiles { get; set; }

        public ProjectSettings() {
            Version = curVersion;
        }

        public class FolderState
        {
            public string Name { get; set; }
            public bool Expanded { get; set; }
            public List<FolderState> Children { get; set; }
        }
    }
}
