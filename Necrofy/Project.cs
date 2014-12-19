using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Necrofy
{
    /// <summary>Manages aspects of a ROM hack project including creation, modification, and compilation.</summary>
    class Project
    {
        /// <summary>The directory that is the root of the project</summary>
        public string Path;

        /// <summary>Creates a new project from the given base ROM.</summary>
        /// <param name="baseROM">The path to a ROM that the files in the project will be extracted from.</param>
        /// <param name="projectPath">The path to a directory in which all of thr project files will be placed.</param>
        /// <param name="extractLevels">Sets whether the levels will be extracted from the base ROM.</param>
        public Project(string baseROM, string projectPath, bool extractLevels) {

        }

        /// <summary>Loads an existing project from the given directory.</summary>
        /// <param name="projectPath">The directory from which to load the project files.</param>
        public Project(string projectPath) {

        }

    }
}
