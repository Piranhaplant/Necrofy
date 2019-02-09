using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Necrofy
{
    public class ProjectSettings
    {
        // The current version of the project
        private const int curVersion = 1;
        /// <summary>The version of the project</summary>
        public int Version { get; set; }

        public ProjectSettings() {
            Version = curVersion;
        }
    }
}
