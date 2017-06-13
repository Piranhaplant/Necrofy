using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Necrofy
{
    abstract class TilesetAsset : Asset
    {
        private const string Folder = "Tilesets";

        protected string tilesetName;

        /// <summary>Checks that the given filename is a valid name for a TilesetAsset</summary>
        /// <param name="path">The path to check (relative to the project)</param>
        /// <param name="desiredFilename">The ending filename that is used for this kind of TilesetAsset</param>
        /// <param name="tilesetName">The name of the tileset extracted from the filename</param>
        /// <returns>Whether the path is valid</returns>
        protected static bool CheckPath(string path, string desiredFilename, out string tilesetName) {
            string[] parts = path.Split(Path.DirectorySeparatorChar);
            if (parts.Length == 3 && parts[0] == Folder && parts[2] == desiredFilename) {
                tilesetName = parts[1];
                return true;
            }
            tilesetName = null;
            return false;
        }

        /// <summary>Checks that the given filename is a valid name for a TilesetAsset</summary>
        /// <param name="path">The path to check (relative to the project)</param>
        /// <param name="extension">The end of the filename that is used for this kind of TilesetAsset</param>
        /// <param name="tilesetName">The name of the tileset extracted from the filename</param>
        /// <param name="fileName">The filename without the extension</param>
        /// <returns>Whether the path is valid</returns>
        protected static bool CheckPathEndsWith(string path, string extension, out string tilesetName, out string fileName) {
            string[] parts = path.Split(Path.DirectorySeparatorChar);
            if (parts.Length == 3 && parts[0] == Folder && parts[2].EndsWith(extension)) {
                tilesetName = parts[1];
                fileName = parts[2].Substring(0, parts[2].Length - extension.Length);
                return true;
            }
            tilesetName = null;
            fileName = null;
            return false;
        }

        /// <summary>Creates all of the directories necessary to store the TilesetAsset in the project</summary>
        /// <param name="projectPath">The root project path</param>
        /// <param name="filename">The ending filename of the asset</param>
        /// <returns>The full path to the file</returns>
        protected string CreateDirectories(string projectPath, string filename) {
            string path = Path.Combine(projectPath, Folder);
            if (!Directory.Exists(path)) {
                Directory.CreateDirectory(path);
            }
            path = Path.Combine(path, tilesetName);
            if (!Directory.Exists(path)) {
                Directory.CreateDirectory(path);
            }
            return Path.Combine(path, filename);
        }
    }
}
