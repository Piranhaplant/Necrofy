using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Necrofy
{
    /// <summary>
    /// The category of an asset. This is used to group the same type of asset for converting pointers to names and vice-versa.
    /// This is also the order that the assets will be inserted into a ROM. Levels are dependent on the other assets, so they need to be last.
    /// </summary>
    public enum AssetCategory
    {
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
        /// <param name="projectDir">The project directory to write to</param>
        public abstract void WriteFile(string projectDir);
        /// <summary>Inserts the asset into the given ROM</summary>
        /// <param name="rom">The ROM</param>
        /// <param name="romInfo">The ROM info</param>
        public abstract void Insert(NStream rom, ROMInfo romInfo);
        /// <summary>Reserves any space that will be needed by this asset before inserting into a ROM.</summary>
        /// <param name="freespace">The freespace</param>
        public virtual void ReserveSpace(Freespace freespace) { }
        /// <summary>Gets the order that this asset should be inserted</summary>
        public abstract AssetCategory Category { get; }

        /// <summary>Delegate for creating an asset from a file</summary>
        /// <param name="projectDir">The directory of the project</param>
        /// <param name="path">The path to the file. This is relative to <paramref name="projectDir"/></param>
        /// <returns>The asset if it is applicable to the given file, null otherwise</returns>
        protected delegate Asset FromFileDelegate(string projectDir, string path);
        /// <summary>Delegate for creating all of the assets of a given type that exist in the base ROM</summary>
        protected delegate void AddDefaultsDelegate(NStream romStream, ROMInfo romInfo);
        private static List<FromFileDelegate> fileLoaders = new List<FromFileDelegate>();
        private static List<AddDefaultsDelegate> defaultAdders = new List<AddDefaultsDelegate>();

        protected static void AddLoader(FromFileDelegate loader, AddDefaultsDelegate defaultAdder) {
            fileLoaders.Add(loader);
            defaultAdders.Add(defaultAdder);
        }

        /// <summary>Gets the asset loaded from the given file</summary>
        /// <param name="projectDir">The base directory of the project</param>
        /// <param name="filename">The filename of the asset within the project</param>
        /// <returns>The asset, or null if no asset could be created from the file</returns>
        public static Asset FromFile(string projectDir, string filename) {
            foreach (FromFileDelegate fileLoader in fileLoaders) {
                Asset asset = fileLoader(projectDir, filename);
                if (asset != null) {
                    return asset;
                }
            }
            // TODO: record error or something when this happens
            return null;
        }

        /// <summary>Creates the default assets for all asset types</summary>
        /// <param name="romStream">The ROM stream</param>
        /// <param name="romInfo">The ROM info</param>
        public static void AddAllDefaults(NStream romStream, ROMInfo romInfo) {
            foreach (AddDefaultsDelegate defaultAdder in defaultAdders) {
                defaultAdder(romStream, romInfo);
            }
        }

        // Have to init all of the loaders here since the static constructors of the subclasses won't be called
        static Asset() {
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
    }
}
