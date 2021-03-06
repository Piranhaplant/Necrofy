﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Necrofy
{
    /// <summary>Manages a set of space in a ROM that is available for writing.</summary>
    public class Freespace
    {
        /// <summary>The size of a bank in bytes. Blocks will not be allowed to cross banks.</summary>
        public const int BankSize = 0x8000;

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
            if (start >= end) {
                return;
            }
            // If the given chunk crosses a bank boundary, split it into two.
            if (start / BankSize != (end - 1) / BankSize) {
                int splitPoint = (start / BankSize + 1) * BankSize;
                Add(splitPoint, end);
                end = splitPoint;
            }
            FreeBlock block = new FreeBlock(start, end);
            blocks.Add(block);
            // Merge any blocks that intersect
            for (int i = 0; i < blocks.Count - 1; i++) {
                FreeBlock b = blocks[i];
                // Don't merge blocks across banks
                if (block.IntersectsWith(b) && block.Start / BankSize == b.Start / BankSize) {
                    block.Merge(b);
                    blocks.Remove(b);
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
            if (size > BankSize) {
                throw new ArgumentException("size cannot be bigger than bankSize");
            }
            // Find the first block that will hold the required bytes
            FreeBlock foundBlock = null;
            foreach (FreeBlock b in blocks) {
                if (b.Size >= size) {
                    foundBlock = b;
                    break;
                }
            }
            if (foundBlock == null) {
                // Couldn't find a block big enough, so add some new space at the end of the ROM
                if (romSize >= BankSize * 0x80) {
                    throw new Exception("Data has exceeded maximum ROM size");
                }
                AddSize(romSize, BankSize);
                romSize += BankSize;
                return Claim(size);
            } else {
                // Found a block, so make that block smaller and return its original start
                int address = foundBlock.Start;
                foundBlock.Subtract(size);
                // And remove the block if there's no space left in it. Not necessary, but I think it's a probably a good idea
                if (foundBlock.Size == 0) {
                    blocks.Remove(foundBlock);
                }
                return address;
            }
        }

        /// <summary>Reserves the specified block so that it can't be claimed.</summary>
        /// <param name="start">The start of the block</param>
        /// <param name="size">The size of the block</param>
        public void Reserve(int start, int size) {
            FreeBlock reservedBlock = new FreeBlock(start, start + size);
            for (int i = 0; i < blocks.Count; i++) {
                FreeBlock b = blocks[i];
                if (b.IntersectsWith(reservedBlock)) {
                    FreeBlock splitBlock = b.Subtract(reservedBlock);
                    if (splitBlock != null) {
                        blocks.Add(splitBlock);
                    }
                    if (b.Size <= 0) {
                        blocks.RemoveAt(i);
                        i--;
                    }
                }
            }
        }

        /// <summary>Fills all freespace in the given stream with the given byte value</summary>
        /// <param name="s">The stream</param>
        /// <param name="value">The value to fill with</param>
        public void Fill(Stream s, byte value) {
            foreach (FreeBlock block in blocks) {
                s.Seek(block.Start);
                for (int i = block.Start; i < block.End; i++) {
                    s.WriteByte(value);
                }
            }
        }

        /// <summary>Sorts the free blocks by their start address. Used only for getting a human readable output.</summary>
        public void Sort() {
            blocks.Sort();
        }

        public override string ToString() {
            StringBuilder builder = new StringBuilder();
            foreach (FreeBlock b in blocks) {
                builder.AppendLine(b.ToString());
            }
            return builder.ToString();
        }

        /// <summary>Represents one continuous block of free space</summary>
        public class FreeBlock : IComparable<FreeBlock>
        {
            /// <summary>The inclusive start position of the block.</summary>
            public int Start;
            /// <summary>The exclusive end position of the block.</summary>
            public int End;

            /// <summary>Gets the number of bytes held in this block.</summary>
            public int Size => End - Start;

            /// <summary>Creates a new block with the specified start and end.</summary>
            /// <param name="start">The start of the block</param>
            /// <param name="end">The end of the block</param>
            /// <exception cref="System.ArgumentException">if end is less than or equal to start</exception>
            public FreeBlock(int start, int end) {
                if (end <= start) {
                    throw new ArgumentException("end must be greater than start");
                }
                Start = start;
                End = end;
            }

            /// <summary>Gets whether this block intersects with or is directly adjacent to the specified block.</summary>
            /// <param name="block">The other block</param>
            /// <returns>true if the blocks intersect, false if they don't</returns>
            public bool IntersectsWith(FreeBlock block) {
                return block.Start <= End && Start <= block.End;
            }

            /// <summary>Merges this block with the one specified. Assumes that the blocks intersect.</summary>
            /// <param name="block">The block with which to merge</param>
            public void Merge(FreeBlock block) {
                if (block.Start < Start) {
                    Start = block.Start;
                }
                if (block.End > End) {
                    End = block.End;
                }
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
                    int originalEnd = End;
                    End = block.Start;
                    return new FreeBlock(block.End, originalEnd);
                }
                // Chop off the end of this block
                if (block.End >= End) {
                    End = Math.Max(Start, block.Start);
                }
                // Chop off the start of this block
                if (block.Start <= Start) {
                    Start = Math.Min(End, block.End);
                }
                return null;
            }

            public override string ToString() {
                return string.Format("{0:X6}-{1:X6} ({2:X6})", Start, End, Size);
            }

            public int CompareTo(FreeBlock other) {
                if (Start == other.Start) {
                    return End.CompareTo(other.End);
                }
                return Start.CompareTo(other.Start);
            }
        }
    }
}
