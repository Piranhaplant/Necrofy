﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Runtime.CompilerServices;

namespace Necrofy
{
    /// <summary>
    /// The category of an asset. This is used to group the same type of asset for converting pointers to names and vice-versa.
    /// This is also the order that the assets will be inserted into a ROM. Levels are dependent on the other assets, so they need to be last.
    /// </summary>
    public enum AssetCategory
    {
        Editor,
        Data,
        Passwords,
        Sprites,
        Collision,
        Graphics,
        Palette,
        Tilemap,
        Level,
    }
    
    /// <summary>An asset that is part of a project. For example, levels, tilesets, palettes...</summary>
    abstract class Asset : IComparable<Asset>
    {
        public const char FolderSeparator = '/';

        public const string LevelTitleFolder = "Level Title";
        public const string TitleScreenFolder = "Title Screen";
        public const string SpritesFolder = "Sprites";
        public const string TilesetFolder = "Tilesets";
        public const string MiscFolder = "Misc";

        protected const string Castle = "Castle";
        protected const string Grass = "Grass";
        protected const string Sand = "Sand";
        protected const string Office = "Office+Cave";
        protected const string Mall = "Mall+Factory";

        /// <summary>Writes a file containing the asset into the given project directory</summary>
        /// <param name="project">The project</param>
        public abstract void WriteFile(Project project);
        /// <summary>Gets the order that this asset should be inserted</summary>
        protected abstract AssetCategory Category { get; }
        /// <summary>Gets the name of this asset</summary>
        protected abstract string Name { get; }
        /// <summary>Reserves any space that will be needed by this asset before inserting into a ROM.</summary>
        /// <param name="freespace">The freespace</param>
        public virtual void ReserveSpace(Freespace freespace) { }
        /// <summary>Inserts the asset into the given ROM</summary>
        /// <param name="rom">The ROM</param>
        /// <param name="romInfo">The ROM info</param>
        /// <param name="project">The project</param>
        public abstract void Insert(NStream rom, ROMInfo romInfo, Project project);
        protected void InsertByteArray(NStream rom, ROMInfo romInfo, byte[] data, int? pointer = null) {
            if (pointer == null) {
                pointer = romInfo.Freespace.Claim(data.Length);
            }
            rom.Seek((int)pointer);
            rom.Write(data, 0, data.Length);
            romInfo.AddAssetPointer(Category, Name, (int)pointer);
        }
        protected void InsertCompressedByteArray(NStream rom, ROMInfo romInfo, byte[] data, string filename, int? pointer = null) {
            string compressedFilename = filename + ".nfyz";
            byte[] compressedData;
            if (!File.Exists(compressedFilename) || File.GetLastWriteTimeUtc(filename) > File.GetLastWriteTimeUtc(compressedFilename)) {
                compressedData = ZAMNCompress.Compress(data);
                File.WriteAllBytes(compressedFilename, compressedData);
            } else {
                compressedData = File.ReadAllBytes(compressedFilename);
            }
            
            int newPointer = ZAMNCompress.Insert(rom, romInfo.Freespace, compressedData, pointer);
            romInfo.AddAssetPointer(Category, Name, newPointer);
        }

        private static List<Creator> creators = new List<Creator>();
        /// <summary>Adds a creator that will be used to load assets from files and ROMs</summary>
        /// <param name="creator">The creator</param>
        protected static void AddCreator(Creator creator) {
            creators.Add(creator);
        }
        /// <summary>Loads an asset from the given file</summary>
        /// <param name="project">The project</param>
        /// <param name="filename">The filename of the asset within the project</param>
        /// <returns>The asset, or null if no asset could be created from the file</returns>
        public static Asset FromFile(Project project, string filename) {
            NameInfo.PathParts pathParts = NameInfo.ParsePath(filename);
            foreach (Creator creator in creators) {
                NameInfo nameInfo = creator.GetNameInfo(pathParts, project);
                if (nameInfo != null) {
                    return creator.FromFile(nameInfo, Path.Combine(project.path, filename));
                }
            }
            return null;
        }
        public static NameInfo GetInfo(Project project, string filename) {
            NameInfo.PathParts pathParts = NameInfo.ParsePath(filename);
            foreach (Creator creator in creators) {
                NameInfo nameInfo = creator.GetNameInfo(pathParts, project);
                if (nameInfo != null) {
                    return nameInfo;
                }
            }
            return null;
        }
        /// <summary>Adds the default assets for all asset types to romInfo</summary>
        /// <param name="romStream">The ROM stream</param>
        /// <param name="romInfo">The ROM info</param>
        public static void AddAllDefaults(NStream romStream, ROMInfo romInfo) {
            foreach (Creator creator in creators) {
                foreach (DefaultParams defaultParams in creator.GetDefaults()) {
                    if (defaultParams.extractFromNecrofyROM || !romInfo.NecrofyROM) {
                        CreateAsset(romStream, romInfo, creator, defaultParams.nameInfo, defaultParams.pointer, defaultParams.size);
                    }
                }
            }
        }

        /// <summary>Gets the name of the asset at the given pointer, or creates one if it does not yet exist</summary>
        /// <param name="romStream">The ROM stream</param>
        /// <param name="romInfo">The ROM info</param>
        /// <param name="pointer">The pointer to the data</param>
        /// <param name="creator">The creator that will be used to create a new asset if one does not yet exist</param>
        /// <returns>The name of the asset</returns>
        protected static string GetAssetName(NStream romStream, ROMInfo romInfo, Creator creator, AssetCategory category, int pointer, string group = null) {
            string name = romInfo.GetAssetName(category, pointer);
            if (name == null) {
                string hexName = pointer.ToString("X6");
                NameInfo nameInfo = creator.GetNameInfoForName(hexName, group ?? hexName);
                CreateAsset(romStream, romInfo, creator, nameInfo, pointer, null);
                name = nameInfo.Name;
            }
            return name;
        }

        // Creates an asset from a ROM
        private static void CreateAsset(NStream romStream, ROMInfo romInfo, Creator creator, NameInfo nameInfo, int pointer, int? size) {
            romStream.PushPosition();

            romStream.Seek(pointer);
            long startPos = romStream.Position;
            Asset asset = creator.FromRom(nameInfo, romStream, romInfo, size, out bool trackFreespace);
            if (trackFreespace) {
                romInfo.Freespace.AddSize(pointer, (int)(romStream.Position - startPos));
            }

            romInfo.assets.Add(asset);
            romInfo.AddAssetName(nameInfo.Category, pointer, nameInfo.Name);

            romStream.PopPosition();
        }

        protected static string GetTilesetFolder(string tileset) {
            return TilesetFolder + FolderSeparator + tileset;
        }
        
        // Have to init all of the loaders here since the static constructors of the subclasses won't be called
        static Asset() {
            foreach (Type subclass in typeof(Asset).Assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(Asset)))) {
                RuntimeHelpers.RunClassConstructor(subclass.TypeHandle);
            }
        }

        public int CompareTo(Asset other) {
            return Category.CompareTo(other.Category);
        }

        /// <summary>Holds information about an asset's name</summary>
        public abstract class NameInfo
        {
            /// <summary>Condences the NameInfo down to a single string that will be used to reference the asset</summary>
            public string Name { get; private set;  }
            public ParsedName ParsedName { get; private set; }
            /// <summary>The name that will be displayed to the user in the project browser</summary>
            public abstract string DisplayName { get; }
            /// <summary>Gets the order that this asset should be inserted</summary>
            public abstract AssetCategory Category { get; }
            
            protected NameInfo(string name) {
                Name = name;
                ParsedName = new ParsedName(name);
            }
            protected NameInfo(string folder, string finalName) : this(folder + FolderSeparator + finalName) { }

            /// <summary>Gets the components of the name that will be used in the filename of the asset</summary>
            protected abstract PathParts GetPathParts();

            /// <summary>Gets whether the asset has an editor for it</summary>
            public virtual bool Editable => false;

            /// <summary>Gets the component that will be used to edit the asset</summary>
            /// <param name="project">The project</param>
            /// <returns>The editor, or null if there is none</returns>
            public virtual EditorWindow GetEditor(Project project) {
                return null;
            }

            /// <summary>Called when the file contents located at this name have changed</summary>
            public virtual void Refresh() { }

            public override bool Equals(object obj) {
                if (obj is NameInfo nameInfo) {
                    return nameInfo.GetPathParts().Equals(GetPathParts());
                }
                return false;
            }

            public override int GetHashCode() {
                return GetPathParts().GetHashCode();
            }

            /// <summary>Parses the individual parts out of an asset path</summary>
            public static PathParts ParsePath(string path) {
                PathParts pathParts = new PathParts();

                string[] parts = path.Split(Path.DirectorySeparatorChar);
                pathParts.folder = string.Join(FolderSeparator.ToString(), parts, 0, parts.Length - 1);

                string fileName = parts[parts.Length - 1];
                int dotIndex = fileName.LastIndexOf('.');
                if (dotIndex >= 0) {
                    pathParts.fileExtension = fileName.Substring(dotIndex + 1);
                    fileName = fileName.Substring(0, dotIndex);
                }
                Match m = Regex.Match(fileName, "^([^@#]*)(@[0-9A-Fa-f]{6})?(#)?$");
                if (m.Success) {
                    fileName = m.Groups[1].Value;
                    if (m.Groups[2].Length > 0) {
                        pathParts.pointer = Convert.ToInt32(m.Groups[2].Value.Substring(1), 16);
                    }
                    if (m.Groups[3].Length > 0) {
                        pathParts.compressed = true;
                    }
                }
                pathParts.name = fileName;

                return pathParts;
            }

            /// <summary>Gets the filename for the asset, optionally creating any intermediate directories if they don't exist</summary>
            public string GetFilename(string projectDir, bool createDirectories = false) {
                PathParts pathParts = GetPathParts();
                string directory = Path.Combine(projectDir, Path.Combine(pathParts.folder.Split(FolderSeparator)));
                if (createDirectories && !Directory.Exists(directory)) {
                    Directory.CreateDirectory(directory);
                }
                string filename = pathParts.name;
                if (pathParts.pointer != null) {
                    filename += "@" + pathParts.pointer.Value.ToString("X6");
                }
                if (pathParts.compressed) {
                    filename += "#";
                }
                if (pathParts.fileExtension != null) {
                    filename += "." + pathParts.fileExtension;
                }
                return Path.Combine(directory, filename);
            }

            /// <summary>Finds the name of an existing file that matches this name with any pointer.</summary>
            public string FindFilename(string projectDir) {
                PathParts pathParts = GetPathParts();
                string directory = Path.Combine(projectDir, Path.Combine(pathParts.folder.Split(FolderSeparator)));
                string regex = "^" + Regex.Escape(pathParts.name) + "(@[0-9A-Fa-f]{6})?#?";
                if (pathParts.fileExtension != null) {
                    regex += Regex.Escape("." + pathParts.fileExtension);
                }
                regex += "$";
                foreach (string file in Directory.GetFiles(directory)) {
                    if (Regex.IsMatch(Path.GetFileName(file), regex)) {
                        return file;
                    }
                }
                throw new IOException("Could not find asset " + pathParts.folder + FolderSeparator + pathParts.name);
            }

            /// <summary>Different parts of an asset path that are used to convert to and from filenames</summary>
            public class PathParts
            {
                public string folder;
                public string name;
                public string fileExtension;
                public int? pointer;
                public bool compressed;

                public PathParts() { }
                public PathParts(string folder, string name, string fileExtension, int? pointer, bool compressed) {
                    this.folder = folder;
                    this.name = name;
                    this.fileExtension = fileExtension;
                    this.pointer = pointer;
                    this.compressed = compressed;
                }

                public override bool Equals(object obj) {
                    var parts = obj as PathParts;
                    return parts != null &&
                           folder == parts.folder &&
                           name == parts.name &&
                           fileExtension == parts.fileExtension &&
                           EqualityComparer<int?>.Default.Equals(pointer, parts.pointer) &&
                           compressed == parts.compressed;
                }

                public override int GetHashCode() {
                    var hashCode = 1762610141;
                    hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(folder);
                    hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(name);
                    hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(fileExtension);
                    hashCode = hashCode * -1521134295 + EqualityComparer<int?>.Default.GetHashCode(pointer);
                    hashCode = hashCode * -1521134295 + compressed.GetHashCode();
                    return hashCode;
                }
            }
        }

        public class ParsedName
        {
            public string Tileset { get; private set; }
            public string Folder { get; private set; }
            public string FinalName { get; private set; }

            public ParsedName(string name) {
                string[] parts = name.Split(FolderSeparator);
                if (parts.Length == 3 && parts[0] == TilesetFolder) {
                    Tileset = parts[1];
                }
                Folder = string.Join(FolderSeparator.ToString(), parts, 0, parts.Length - 1);
                FinalName = parts[parts.Length - 1];
            }
        }

        /// <summary>An object containing methods for creating assets from files and ROMs</summary>
        protected abstract class Creator
        {
            // Methods needed for reading from files

            /// <summary>Gets the NameInfo of an asset created from the given path</summary>
            /// <param name="pathParts">The path parts</param>
            /// <param name="project">The project</param>
            public abstract NameInfo GetNameInfo(NameInfo.PathParts pathParts, Project project);
            /// <summary>Creates a new asset from the given file</summary>
            /// <param name="nameInfo">The name of the asset. This will be the object returned from GetNameInfo</param>
            /// <param name="filename">The full filename of the asset</param>
            public abstract Asset FromFile(NameInfo nameInfo, string filename);

            // Methods needed for reading from ROMs
            // These don't have to be implemented since some assets can't be read from a ROM

            /// <summary>Gets a list of assets that exist in a clean ROM for the asset type</summary>
            public virtual List<DefaultParams> GetDefaults() { return new List<DefaultParams>(); }
            /// <summary>Creates an asset from the given ROM</summary>
            /// <param name="nameInfo">The NameInfo for the asset. This will be the value from either GetDefaults or GetNameInfoForName</param>
            /// <param name="romStream">The rom stream, positioned at the start of the asset</param>
            public virtual Asset FromRom(NameInfo nameInfo, NStream romStream, ROMInfo romInfo, int? size, out bool trackFreespace) {
                trackFreespace = false;
                return null;
            }
            /// <summary>Gets a NameInfo from the given name string. This is used to create assets that do not exist in the defaults.</summary>
            /// <param name="name">The desired name of the asset</param>
            /// <param name="group">The group the asset will belong to. This is used to group assets into a signle tileset. May be null</param>
            public virtual NameInfo GetNameInfoForName(string name, string group) { return null; }
        }

        /// <summary>Information about an asset that exists in a clean ROM</summary>
        protected class DefaultParams
        {
            public readonly int pointer;
            public readonly NameInfo nameInfo;
            public readonly int? size;
            public readonly bool extractFromNecrofyROM;

            public DefaultParams(int pointer, NameInfo nameInfo, int? size = null, bool extractFromNecrofyROM = false) {
                this.pointer = pointer;
                this.nameInfo = nameInfo;
                this.size = size;
                this.extractFromNecrofyROM = extractFromNecrofyROM;
            }
        }
    }
}
