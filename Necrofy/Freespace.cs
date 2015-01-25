using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Necrofy
{
    /// <summary>Manages a set of space in a ROM that is available for writing.</summary>
    class Freespace
    {
        /// <summary>The size of a bank in bytes. Blocks will not be allowed to cross banks.</summary>
        public const int bankSize = 0x8000;

        private List<FreeBlock> blocks;
        // If there isn't enough freespace in the ROM, the data will be put at the end, so we need to know how big the ROM is.
        private int romSize;

        /// <summary>Creates a new empty Freespace for a ROM with the given size.</summary>
        /// <param name="romSize">The size of the ROM</param>
        public Freespace(int romSize) {
            blocks = new List<FreeBlock>();
            this.romSize = romSize;
        }

        /// <summary>Adds a block of freespace with the specified start and end locations.</summary>
        /// <param name="start">The inclusive start of the block</param>
        /// <param name="end">The exclusive end of the block</param>
        public void Add(int start, int end) {
            // If the given chunk crosses a bank boundary, split it into two.
            if (start / bankSize != (end - 1) / bankSize) {
                int splitPoint = (end / bankSize) * bankSize;
                Add(start, splitPoint);
                start = splitPoint;
            }
            FreeBlock block = new FreeBlock(start, end);
            blocks.Insert(0, block);
            // Merge any blocks that intersect
            for (int i = 1; i < blocks.Count; i++) {
                FreeBlock b = blocks[i];
                if (b.IntersectsWith(block)) {
                    b.Merge(block);
                    blocks.Remove(block);
                    block = b;
                    i--;
                }
            }
        }

        /// <summary>Adds a block of freespace with the specified start and size.</summary>
        /// <param name="start">The inclusive start of the block</param>
        /// <param name="size">The size of the block</param>
        public void AddSize(int start, int size) {
            Add(start, start + size);
        }

        /// <summary>Claims a block of <paramref name="size"/> bytes from the freespace pool. If there aren't enough bytes, claims space at the end of the ROM.</summary>
        /// <param name="size">The number of bytes required</param>
        /// <returns>The address of the claimed space</returns>
        public int Claim(int size) {
            // Find the smallest block that will hold the required bytes. Hopefully this will use the freespace fairly efficiently.
            FreeBlock bestBlock = null;
            foreach (FreeBlock b in blocks) {
                if (b.Size >= size && (bestBlock == null || b.Size < bestBlock.Size))
                    bestBlock = b;
            }
            if (bestBlock == null) {
                // Couldn't find a block big enough, so claim space at the end of the ROM
                int address = romSize;
                romSize += size;
                return address;
            } else {
                // Found a block, so make that block smaller and return its original start
                int address = bestBlock.Start;
                bestBlock.Subtract(size);
                // And remove the block if there's no space left in it. Not necessary, but I think it's a probably a good idea
                if (bestBlock.Size == 0)
                    blocks.Remove(bestBlock);
                return address;
            }
        }

        /// <summary>Reserves the specified block so that it can't be claimed.</summary>
        /// <param name="start">The start of the block</param>
        /// <param name="size">The size of the block</param>
        public void Reserve(int start, int size) {
            FreeBlock block = new FreeBlock(start, start + size);
            for (int i = 0; i < blocks.Count; i++) {
                FreeBlock b = blocks[i];
                if (b.IntersectsWith(block)) {
                    FreeBlock splitBlock = b.Subtract(block);
                    if (splitBlock != null)
                        blocks.Add(splitBlock);
                    if (b.Size <= 0) {
                        blocks.RemoveAt(i);
                        i--;
                    }
                }
            }
        }

        /// <summary>Sorts the free blocks by their start address. Used only for getting a human readable output.</summary>
        public void Sort() {
            blocks.Sort();
        }

        public override string ToString() {
            StringBuilder builder = new StringBuilder();
            foreach (FreeBlock b in blocks)
                builder.AppendLine(b.ToString());
            return builder.ToString();
        }

        /// <summary>Represents one continuous block of free space</summary>
        private class FreeBlock : IComparable<FreeBlock>
        {
            /// <summary>The inclusive start position of the block.</summary>
            public int Start;
            /// <summary>The exclusive end position of the block.</summary>
            public int End;

            /// <summary>Gets the number of bytes held in this block.</summary>
            public int Size {
                get { return End - Start; }
            }

            /// <summary>Creates a new block with the specified start and end.</summary>
            /// <param name="start">The start of the block</param>
            /// <param name="end">The end of the block</param>
            /// <exception cref="System.ArgumentException">if end is less than or equal to start</exception>
            public FreeBlock(int start, int end) {
                if (end <= start)
                    throw new ArgumentException("end must be greater than start");
                this.Start = start;
                this.End = end;
            }

            /// <summary>Gets whether this block could be combined with the specified block.</summary>
            /// <param name="block">The other block</param>
            /// <returns>true if the blocks intersect, false if they don't</returns>
            public bool IntersectsWith(FreeBlock block) {
                return (block.Start <= End && Start <= block.End)
                    && (Start / bankSize == block.Start / bankSize); // Don't merge blocks across banks
            }

            /// <summary>Merges this block with the one specified. Assumes that the blocks intersect.</summary>
            /// <param name="block">The block with which to merge</param>
            public void Merge(FreeBlock block) {
                if (block.Start < Start)
                    Start = block.Start;
                if (block.End > End)
                    End = block.End;
            }

            /// <summary>Removes <paramref name="size"/> bytes from the beginning of this block. Assumes that this block is at least that big.</summary>
            /// <param name="size">The number of bytes to remove</param>
            public void Subtract(int size) {
                Start += size;
            }

            /// <summary>Removes the specified block from this block. Assumes the blocks intersect.</summary>
            /// <param name="block">The block to subtract</param>
            /// <returns>A new block if subtracting the block split this block in half, null otherwise</returns>
            public FreeBlock Subtract(FreeBlock block) {
                // The block is inside of this block, so we need to split it into two
                if (block.Start > Start && block.End < End) {
                    End = block.Start;
                    return new FreeBlock(block.End, End);
                }
                // The block intersects the front of this block
                if (block.End > Start) {
                    Start = block.End;
                }
                // The block intersects the end of this block
                if (block.Start < End) {
                    End = block.Start;
                }
                return null;
            }

            public override string ToString() {
                return string.Format("{0:X6}-{1:X6} ({2:X6})", Start, End, Size);
            }

            public int CompareTo(FreeBlock other) {
                return Start.CompareTo(other.Start);
            }
        }
    }
}
