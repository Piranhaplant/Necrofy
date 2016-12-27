using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Newtonsoft.Json;

namespace Necrofy
{
    /// <summary>Manages aspects of a ROM hack project including creation, modification, and compilation.</summary>
    class Project
    {
        /// <summary>Creates a new project from the given base ROM.</summary>
        /// <param name="baseROM">The path to a ROM that the files in the project will be extracted from.</param>
        /// <param name="projectPath">The path to a directory in which all of the project files will be placed.</param>
        /// <param name="extractLevels">Sets whether the levels will be extracted from the base ROM.</param>
        public Project(string baseROM, string projectPath, bool extractLevels) {
            NStream s = new NStream(new FileStream(baseROM, FileMode.Open, FileAccess.Read, FileShare.Read));
            ROMInfo info = new ROMInfo(this, s);
            // Extract all files from the ROM needed for a project
            if (extractLevels) {
                Directory.CreateDirectory(Path.Combine(projectPath, "Levels"));
                for (int i = 0; i < info.Levels.Count; i++) {
                    Level l = info.Levels[i];
                    File.WriteAllText(Path.Combine(projectPath, "Levels", i.ToString() + ".json"), JsonConvert.SerializeObject(l));
                }
            }
        }

        /// <summary>Loads an existing project from the given directory.</summary>
        /// <param name="projectPath">The directory from which to load the project files.</param>
        public Project(string projectPath) {

        }
    }
}
