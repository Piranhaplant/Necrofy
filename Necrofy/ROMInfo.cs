using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Necrofy
{
    /// <summary>Holds information about a ROM that will be used to create a new project and compile a project</summary>
    class ROMInfo
    {
        /// <summary>A set of pointers to the levels found in the ROM</summary>
        public HashSet<int> LevelPointers = new HashSet<int>();
        /// <summary>A set of pointers to the tilemaps found in the ROM</summary>
        public HashSet<int> TilemapPointers = new HashSet<int>();
        /// <summary>A set of pointers to the tileset collisions found in the ROM</summary>
        public HashSet<int> TilesetCollisionPointers = new HashSet<int>();
        /// <summary>A set of pointers to the tileset graphics found in the ROM</summary>
        public HashSet<int> TilesetGraphicsPointers = new HashSet<int>();
        /// <summary>A set of pointers to the tileset palettes found in the ROM</summary>
        public HashSet<int> TilesetPalettePointers = new HashSet<int>();
        /// <summary>A set of pointers to the sprite palettes found in the ROM</summary>
        public HashSet<int> SpritePalettePointers = new HashSet<int>();
        /// <summary>A log of what happened during the loading of the ROMInfo</summary>
        public Log Log = new Log();
        /// <summary>The freespace that was found in the ROM</summary>
        public Freespace Freespace;

        /// <summary>Loads the ROMInfo data from an already opened stream.</summary>
        /// <param name="s">A stream to a ROM file</param>
        public ROMInfo(Stream s) {
            Freespace = new Necrofy.Freespace((int)s.Length);

            // First get a list of all the level pointers
            s.Seek(ROMPointers.LevelPointers, SeekOrigin.Begin);
            int levelCount = s.ReadInt16();
            s.Seek(ROMPointers.BonusLevelNums, SeekOrigin.Begin);
            int maxBonusLevel = levelCount;
            // I am going to assume that there aren't any skipped level numbers
            for (int i = 0; i <= levelCount; i++) {
                int bonusLevelNum = s.ReadInt16();
                if (bonusLevelNum > maxBonusLevel) maxBonusLevel = bonusLevelNum;
            }
            s.Seek(ROMPointers.LevelPointers + 2, SeekOrigin.Begin); // Skip past the first 2 bytes which indicate how many levels there are
            for (int i = 0; i <= maxBonusLevel; i++) {
                // TODO: need to be able to detect if the ROM is using 4-byte level pointers
                LevelPointers.Add(s.ReadRelativePointer(0x9f));
            }

            // Then load the tilesets, etc. that are used in each level
            foreach (int levelPtr in LevelPointers) {
                s.Seek(levelPtr, SeekOrigin.Begin);
                AddPointer(TilemapPointers, s);
                int levelTilesPtr = s.ReadPointer(); // Will be needed later to claim freespace once we know the level width/height
                AddPointer(TilesetCollisionPointers, s);
                AddPointer(TilesetGraphicsPointers, s);
                AddPointer(TilesetPalettePointers, s);
                AddPointer(SpritePalettePointers, s);
                s.Seek(4, SeekOrigin.Current); // Skip the palette animation pointer
                // Now we need to determine the total space used by the level. To simplify things significantly, I'm assuming the level is in one continuous block
                int maxAddress = 0;
                FindMaxAddress(s, 10, ref maxAddress); // Monsters
                FindMaxAddress(s, 12, ref maxAddress); // Victims/One-time monsters
                FindMaxAddress(s, 5, ref maxAddress); // Items
                int width = s.ReadInt16();
                int height = s.ReadInt16();
                // Skipping the level titles even though they should probably be checked. This won't ever cause problems with a standard ZAMN ROM, but could with customs
                s.Seek(20, SeekOrigin.Current); // Skip a bunch of stuff so we're at the bonuses
                try { // It's possible that the level has no bonus pointer
                    FindMaxAddress(s, 2, ref maxAddress); // Bonuses
                } catch (Exception) { };
                // Also skipping level monsters because it'd be a pain to compute their size

                // Add the freespace for this level
                Freespace.Add(levelPtr, maxAddress);
                Freespace.AddSize(levelTilesPtr, width * height * 2);
            }

            // Add the freespace for everything else
            foreach (int ptr in TilemapPointers) {
                // They're compressed, so the first two bytes of the data determines the size
                s.Seek(ptr, SeekOrigin.Begin);
                Freespace.AddSize(ptr, s.ReadInt16() + 2); // +2 for the size bytes themselves
            }
            foreach (int ptr in TilesetCollisionPointers) {
                Freespace.AddSize(ptr, 0x400);
            }
            foreach (int ptr in TilesetGraphicsPointers) {
                Freespace.AddSize(ptr, 0x4000);
            }
            foreach (int ptr in TilesetPalettePointers) {
                Freespace.AddSize(ptr, 0x100);
            }
            foreach (int ptr in SpritePalettePointers) {
                Freespace.AddSize(ptr, 0x100);
            }
        }

        // Reads a 4-byte pointer from the stream and adds it to the set if it was valid
        private void AddPointer(HashSet<int> set, Stream s) {
            int ptr = s.ReadPointer();
            if (ptr != -1) {
                set.Add(ptr);
            }
        }

        // Finds the max address of the data that the stream currently points to.
        // Reads the first two bytes of each chunk until they are both zero
        private void FindMaxAddress(Stream s, int chunkSize, ref int maxAddress) {
            long pos = s.Position;
            s.GoToRelativePointer();
            while (true) {
                int value = s.ReadInt16();
                if (value == 0) {
                    maxAddress = Math.Max((int)s.Position, maxAddress);
                    s.Seek(pos + 2, SeekOrigin.Begin);
                    return;
                }
                s.Seek(chunkSize - 2, SeekOrigin.Current);
            }
        }
    }
}
