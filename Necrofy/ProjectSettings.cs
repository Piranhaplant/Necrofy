using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Necrofy
{
    public class ProjectSettings
    {
        public const int CurMajorVersion = 1;
        public const int CurMinorVersion = 0;
        /// <summary>The major version of the project. Projects with a newer major version cannot be opened in old versions of the program</summary>
        public int MajorVersion { get; set; }
        /// <summary>The minor version of the project. Different versions here are still compatible with older versions of the program</summary>
        public int MinorVersion { get; set; }
        /// <summary>State of the folders in the project browser</summary>
        public List<FolderState> FolderStates { get; set; }
        /// <summary>List of asset files that are currently opened for editing</summary>
        public List<string> OpenFiles { get; set; }

        public ProjectSettings() {
            MajorVersion = CurMajorVersion;
            MinorVersion = CurMinorVersion;
        }

        public class FolderState
        {
            public string Name { get; set; }
            public bool Expanded { get; set; }
            public List<FolderState> Children { get; set; }
        }
    }
}
