using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Necrofy
{
    /// <summary>
    /// "Big" type monsters that are active during the entire level (big baby, UFO, etc.)
    /// Also includes special sprites that affect the level (palette fade and tile animation)
    /// </summary>
    abstract class LevelMonster
    {
        public int type { get; set; }

        /// <summary>Delegate for loading a LevelMonster from a ROM file</summary>
        protected delegate LevelMonster ROMDelegate(ROMInfo r, NStream s);
        /// <summary>Delegate for creating a blank LevelMonster (for loading from JSON)</summary>
        protected delegate LevelMonster BlankDelegate();
        private static Dictionary<int, ROMDelegate> romLoaders = new Dictionary<int, ROMDelegate>();
        private static Dictionary<int, BlankDelegate> blankLoaders = new Dictionary<int, BlankDelegate>();

        /// <summary>Adds a level monster loader so that a subclass can be instantiated when loading a level.</summary>
        /// <param name="type">The type of the level monster that the loader is for</param>
        /// <param name="loader">The function that creates the new level monster from a ROM file.</param>
        /// <param name="converter">The function that create a blank level monster (for reading from JSON).</param>
        protected static void AddLoader(int type, ROMDelegate romLoader, BlankDelegate blankLoader) {
            romLoaders.Add(type, romLoader);
            blankLoaders.Add(type, blankLoader);
        }

        /// <summary>Creates a level monster of the correct type from the given ROM stream</summary>
        /// <param name="r">ROMInfo needed to get palette names for the <see cref="PaletteFadeLevelMonster"/></param>
        /// <param name="s">The stream to create the level monster from</param>
        /// <returns>The level monster</returns>
        public static LevelMonster FromROM(ROMInfo r, NStream s) {
            int type = s.ReadPointer();
            if (romLoaders.ContainsKey(type)) {
                return romLoaders[type](r, s);
            }
            return new PositionLevelMonster(type, s);
        }

        /// <summary>Creates a blank level monster of the correct type for loading from JSON</summary>
        /// <param name="type">The type of the level monster</param>
        /// <returns>The level monster</returns>
        public static LevelMonster GetBlank(int type) {
            if (blankLoaders.ContainsKey(type)) {
                return blankLoaders[type]();
            }
            return new PositionLevelMonster(0, 0, 0);
        }

        // Have to init all of the loaders here since the static constructors of the subclasses won't be called
        static LevelMonster() {
            PaletteFadeLevelMonster.RegisterLoader();
            TileAnimLevelMonster.RegisterLoader();
        }

        [JsonConstructor]
        public LevelMonster(int type) {
            this.type = type;
        }

        /// <summary>Builds the LevelMonster for inserting into a ROM.</summary>
        /// <param name="data">The data to build into</param>
        /// <param name="rom">The ROM used for building. This is required for the PaletteFadeLevelMonster.</param>
        public abstract void Build(MovableData data, ROMInfo rom);
    }
}
