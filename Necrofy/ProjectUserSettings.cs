using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Necrofy
{
    class ProjectUserSettings
    {
        /// <summary>State of the folders in the project browser</summary>
        public List<FolderState> FolderStates { get; set; }
        /// <summary>List of asset files that are currently opened for editing</summary>
        public List<string> OpenFiles { get; set; }

        public class FolderState
        {
            public string Name { get; set; }
            public bool Expanded { get; set; }
            public List<FolderState> Children { get; set; }
        }
    }
}
