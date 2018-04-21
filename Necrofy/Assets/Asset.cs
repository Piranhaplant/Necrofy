using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace Necrofy
{
    /// <summary>
    /// The category of an asset. This is used to group the same type of asset for converting pointers to names and vice-versa.
    /// This is also the order that the assets will be inserted into a ROM. Levels are dependent on the other assets, so they need to be last.
    /// </summary>
    public enum AssetCategory
    {
        Editor,
        Collision,
        Graphics,
        Palette,
        Tilemap,
        Level
    }

    /// <summary>An asset that is part of a project. For example, levels, tilesets, palettes...</summary>
    abstract class Asset : IComparable<Asset>
    {
        /// <summary>Writes a file containing the asset into the given project directory</summary>
        /// <param name="project">The project</param>
        public abstract void WriteFile(Project project);
        /// <summary>Reserves any space that will be needed by this asset before inserting into a ROM.</summary>
        /// <param name="freespace">The freespace</param>
        public virtual void ReserveSpace(Freespace freespace) {
            if (FixedPointer != null) {
                // This will only ever be done on assets that use a byte array inserter, so it's okay to pass null here
                // and it's cheap to create a second inserter when we actually go to insert the asset.
                Inserter inserter = GetInserter(null);
                freespace.Reserve(FixedPointer.Value, inserter.GetSize());
            }
        }
        /// <summary>Gets the order that this asset should be inserted</summary>
        protected abstract AssetCategory Category { get; }
        /// <summary>Gets the name of this asset</summary>
        protected abstract string Name { get; }
        /// <summary>Gets the fixed pointer at which this asset should be inserted to the ROM, if one exists</summary>
        protected virtual int? FixedPointer {
            get { return null; }
        }
        /// <summary>Inserts the asset into the given ROM</summary>
        /// <param name="rom">The ROM</param>
        /// <param name="romInfo">The ROM info</param>
        public void Insert(NStream rom, ROMInfo romInfo) {
            Inserter inserter = GetInserter(romInfo);
            rom.PushPosition();

            int pointer = romInfo.Freespace.Claim(inserter.GetSize());
            byte[] data = inserter.GetData(pointer);

            rom.Seek(pointer, SeekOrigin.Begin);
            rom.Write(data, 0, data.Length);
            inserter.InsertExtras(pointer, rom);
            romInfo.AddAssetPointer(Category, Name, pointer);

            rom.PopPosition();
        }
        /// <summary>Gets the inserter that will be used to insert the asset into the ROM</summary>
        /// <param name="romInfo">The ROM info</param>
        /// <returns>The inserter</returns>
        protected abstract Inserter GetInserter(ROMInfo romInfo);

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
                NameInfo nameInfo = creator.GetNameInfo(pathParts);
                if (nameInfo != null) {
                    return creator.FromFile(nameInfo, Path.Combine(project.path, filename));
                }
            }
            // TODO: record error or something when this happens
            return null;
        }
        /// <summary>Adds the default assets for all asset types to romInfo</summary>
        /// <param name="romStream">The ROM stream</param>
        /// <param name="romInfo">The ROM info</param>
        public static void AddAllDefaults(NStream romStream, ROMInfo romInfo) {
            foreach (Creator creator in creators) {
                foreach (DefaultParams defaultParams in creator.GetDefaults()) {
                    CreateAsset(romStream, romInfo, creator, defaultParams.nameInfo, defaultParams.pointer, defaultParams.size);
                }
            }
        }

        /// <summary>Gets the name of the asset at the given pointer, or creates one if it does not yet exist</summary>
        /// <param name="romStream">The ROM stream</param>
        /// <param name="romInfo">The ROM info</param>
        /// <param name="pointer">The pointer to the data</param>
        /// <param name="creator">The creator that will be used to create a new asset if one does not yet exist</param>
        /// <returns>The name of the asset</returns>
        protected static string GetAssetName(NStream romStream, ROMInfo romInfo, int pointer, Creator creator) {
            string name = romInfo.GetAssetName(creator.GetCategory(), pointer);
            if (name == null) {
                string hexName = pointer.ToString("X6");
                NameInfo nameInfo = creator.GetNameInfoForName(hexName);
                CreateAsset(romStream, romInfo, creator, nameInfo, pointer, null);
                name = nameInfo.Name;
            }
            return name;
        }

        // Creates an asset from a ROM
        private static void CreateAsset(NStream romStream, ROMInfo romInfo, Creator creator, NameInfo nameInfo, int pointer, int? size) {
            romStream.PushPosition();

            romStream.Seek(pointer, SeekOrigin.Begin);
            long startPos = romStream.Position;
            Asset asset = creator.FromRom(nameInfo, romStream, size);
            // We can't just use data.Length here because the size itself counts towards the length for compressed data
            romInfo.Freespace.AddSize(pointer, (int)(romStream.Position - startPos));

            romInfo.assets.Add(asset);
            romInfo.AddAssetName(creator.GetCategory(), pointer, nameInfo.Name);

            romStream.PopPosition();
        }

        // Have to init all of the loaders here since the static constructors of the subclasses won't be called
        static Asset() {
            EditorAsset.RegisterLoader();
            GraphicsAsset.RegisterLoader();
            LevelAsset.RegisterLoader();
            PaletteAsset.RegisterLoader();
            TilesetCollisionAsset.RegisterLoader();
            TilesetGraphicsAsset.RegisterLoader();
            TilesetPaletteAsset.RegisterLoader();
            TilesetTilemapAsset.RegisterLoader();
        }

        public int CompareTo(Asset other) {
            return Category.CompareTo(other.Category);
        }

        /// <summary>Holds information about an asset's name</summary>
        protected abstract class NameInfo
        {
            /// <summary>Condences the NameInfo down to a single string that will be used to reference the asset</summary>
            public abstract string Name { get; }

            /// <summary>Parses the individual parts out of an asset path</summary>
            public static PathParts ParsePath(string path) {
                PathParts pathParts = new PathParts();

                string[] parts = path.Split(Path.DirectorySeparatorChar);
                if (parts.Length < 2 || parts.Length > 3) {
                    return null;
                }
                pathParts.topFolder = parts[0];
                if (parts.Length == 3) {
                    pathParts.subFolder = parts[1];
                }
                string fileName = parts[parts.Length - 1];
                int dotIndex = fileName.LastIndexOf('.');
                if (dotIndex >= 0) {
                    pathParts.fileExtension = fileName.Substring(dotIndex + 1);
                    fileName = fileName.Substring(0, dotIndex);
                }
                Match m = Regex.Match(fileName, "^(.*)@([0-9A-Fa-f]{6})$");
                if (m.Success) {
                    fileName = m.Groups[1].Value;
                    pathParts.pointer = Convert.ToInt32(m.Groups[2].Value, 16);
                }
                pathParts.name = fileName;

                return pathParts;
            }

            /// <summary>Gets the components of the name that will be used in the filename of the asset</summary>
            protected abstract PathParts GetPathParts();

            /// <summary>Gets the filename for the asset and creates any intermediate directories if they don't exist</summary>
            public string GetFilename(string projectDir) {
                PathParts parts = GetPathParts();
                string directory = projectDir;
                if (parts.topFolder != null) {
                    directory = Path.Combine(directory, parts.topFolder);
                }
                if (parts.subFolder != null) {
                    directory = Path.Combine(directory, parts.subFolder);
                }
                if (!Directory.Exists(directory)) {
                    Directory.CreateDirectory(directory);
                }
                string filename = parts.name;
                if (parts.pointer != null) {
                    filename += "@" + parts.pointer.Value.ToString("X6");
                }
                if (parts.fileExtension != null) {
                    filename += "." + parts.fileExtension;
                }
                return Path.Combine(directory, filename);
            }

            /// <summary>Finds the name of an existing file that matches this name with any pointer.</summary>
            public string FindFilename(string projectDir) {
                PathParts parts = GetPathParts();
                string directory = projectDir;
                if (parts.topFolder != null) {
                    directory = Path.Combine(directory, parts.topFolder);
                }
                if (parts.subFolder != null) {
                    directory = Path.Combine(directory, parts.subFolder);
                }
                string regex = "^" + Regex.Escape(parts.name) + "(@[0-9A-Fa-f]{6})?";
                if (parts.fileExtension != null) {
                    regex += "\\." + Regex.Escape(parts.fileExtension);
                }
                regex += "$";
                foreach (string file in Directory.GetFiles(directory)) {
                    if (Regex.IsMatch(Path.GetFileName(file), regex)) {
                        return file;
                    }
                }
                throw new IOException("Could not find asset " + parts);
            }

            /// <summary>Different parts of an asset path that are used to convert to and from filenames</summary>
            public class PathParts
            {
                public string topFolder;
                public string subFolder;
                public string name;
                public string fileExtension;
                public int? pointer;

                public PathParts() { }
                public PathParts(string topFolder, string subFolder, string name, string fileExtension, int? pointer) {
                    this.topFolder = topFolder;
                    this.subFolder = subFolder;
                    this.name = name;
                    this.fileExtension = fileExtension;
                    this.pointer = pointer;
                }

                public override string ToString() {
                    StringBuilder builder = new StringBuilder();
                    if (topFolder != null) {
                        builder.Append(topFolder);
                        builder.Append("/");
                    }
                    if (subFolder != null) {
                        builder.Append(subFolder);
                        builder.Append("/");
                    }
                    builder.Append(name);
                    if (pointer != null) {
                        builder.Append("@");
                        builder.Append(pointer.Value.ToString("X6"));
                    }
                    if (fileExtension != null) {
                        builder.Append(".");
                        builder.Append(fileExtension);
                    }
                    return builder.ToString();
                }
            }
        }

        /// <summary>An object containing methods for creating assets from files and ROMs</summary>
        protected abstract class Creator
        {
            // Methods needed for reading from files

            /// <summary>Gets the NameInfo of an asset created from the given path</summary>
            /// <param name="path">The path relative to the root directory of the project</param>
            public abstract NameInfo GetNameInfo(NameInfo.PathParts pathParts);
            /// <summary>Creates a new asset from the given file</summary>
            /// <param name="nameInfo">The name of the asset. This will be the object returned from GetNameInfo</param>
            /// <param name="filename">The full filename of the asset</param>
            public abstract Asset FromFile(NameInfo nameInfo, string filename);
            /// <summary>Gets the category of assets that will be created</summary>
            public abstract AssetCategory GetCategory();

            // Methods needed for reading from ROMs
            // These don't have to be implemented since some assets can't be read from a ROM

            /// <summary>Gets a list of assets that exist in a clean ROM for the asset type</summary>
            public virtual List<DefaultParams> GetDefaults() { return new List<DefaultParams>(); }
            /// <summary>Creates an asset from the given ROM</summary>
            /// <param name="nameInfo">The NameInfo for the asset. This will be the value from either GetDefaults or GetNameInfoForName</param>
            /// <param name="romStream">The rom stream, positioned at the start of the asset</param>
            public virtual Asset FromRom(NameInfo nameInfo, NStream romStream, int? size) { return null; }
            /// <summary>Gets a NameInfo from the given name string. This is used to create assets that do not exist in the defaults.</summary>
            public virtual NameInfo GetNameInfoForName(string name) { return null; }
        }

        /// <summary>Information about an asset that exists in a clean ROM</summary>
        protected class DefaultParams
        {
            public readonly int pointer;
            public readonly NameInfo nameInfo;
            public readonly int? size;

            public DefaultParams(int pointer, NameInfo nameInfo, int? size) {
                this.pointer = pointer;
                this.nameInfo = nameInfo;
                this.size = size;
            }

            public DefaultParams(int pointer, NameInfo nameInfo)
                : this(pointer, nameInfo, null) {
            }
        }

        /// <summary>An object that will be used to insert an asset into a ROM</summary>
        protected abstract class Inserter
        {
            /// <summary>Gets the number of bytes of the asset</summary>
            public abstract int GetSize();
            /// <summary>Gets the data to be inserted</summary>
            /// <param name="pointer">The location the asset will be inserted at</param>
            public abstract byte[] GetData(int pointer);
            /// <summary>Insets anything separate from the main data</summary>
            /// <param name="pointer">The location the main data was inserted at</param>
            /// <param name="romStream">The rom stream</param>
            public virtual void InsertExtras(int pointer, NStream romStream) { }
        }

        /// <summary>An inserter for a byte array</summary>
        protected class ByteArrayInserter : Inserter
        {
            private readonly byte[] data;

            public ByteArrayInserter(byte[] data) {
                this.data = data;
            }

            public override int GetSize() {
                return data.Length;
            }

            public override byte[] GetData(int pointer) {
                return data;
            }
        }
    }
}
