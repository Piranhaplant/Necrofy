﻿using System;
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
        /// <summary>The level after which the winner screen will be shown</summary>
        public int WinLevel { get; set; }
        /// <summary>The level after which the game will end and the player will be sent back to the main menu loop</summary>
        public int EndGameLevel { get; set; }
        /// <summary>List of the built-in patches that will be applied on build</summary>
        public List<Patch> EnabledPatches { get; set; } = new List<Patch>();

        private ProjectSettings() { }

        public static ProjectSettings CreateNew() {
            ProjectSettings settings = new ProjectSettings();
            settings.MajorVersion = CurMajorVersion;
            settings.MinorVersion = CurMinorVersion;
            settings.EnabledPatches.Add(new Patch(Project.ROMExpandPatchName));
            settings.EnabledPatches.Add(new Patch(Project.OtherExpandPatchName));
            return settings;
        }

        public class Patch
        {
            public string Name { get; set; }

            public Patch() { }

            public Patch(string name) {
                Name = name;
            }

            public override string ToString() {
                return Name;
            }
        }
    }
}
