using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Necrofy
{
    /// <summary>
    /// Contains pointers to important data in the ZAMN ROM.
    /// Also holds several extension methods for streams related to reading/writing pointers.
    /// </summary>
    static class ROMPointers
    {
        /// <summary>Pointer to the table of level pointers</summary>
        public const int LevelPointers = 0xf8000;
        /// <summary>Pointer to the table that indicates which bonus level follows each level</summary>
        public const int BonusLevelNums = 0x1517e;

        // NOTE: in these functions I am using "address" to refer to the lower 16 bits of the full 24 bit LoROM address.

        /// <summary>Reads a 4-byte SNES LoROM pointer from the stream.</summary>
        /// <param name="s">The stream</param>
        /// <returns>The pointer as a PC address or -1 if the end of the stream was reached or there was an invalid pointer at the current position.</returns>
        public static int ReadPointer(this Stream s) {
            int address = s.ReadInt16();
            int bank = s.ReadByte();
            s.ReadByte(); // The fourth byte is useless, but in ZAMN it is always included as far as I know
            if (address < 0x8000 || bank < 0x80)
                return -1;
            return (bank - 0x80) * 0x8000 + address - 0x8000;
        }

        /// <summary>Reads a 2-byte SNES LoROM pointer from the stream as if it was from the given bank.</summary>
        /// <param name="s">The stream</param>
        /// <param name="bank">The LoROM bank of the pointer. This should be between 0x80 and 0xff.</param>
        /// <returns>The pointer as a PC address or -1 if the end of the stream was reached or there was an invalid pointer at the current position.</returns>
        public static int ReadRelativePointer(this Stream s, int bank) {
            int address = s.ReadInt16();
            if (address < 0x8000 | bank < 0x80)
                return -1;
            return (bank - 0x80) * 0x8000 + address - 0x8000;
        }

        /// <summary>Moves the stream position to the address specified in the 4-byte SNES LoROM pointer at the current position.</summary>
        /// <param name="s">The stream</param>
        /// <exception cref="System.Exception">if the end of the stream is reached while trying to read the pointer or an invalid pointer is at the current position.</exception>
        public static void GoToPointer(this Stream s) {
            int pointer = s.ReadPointer();
            if (pointer < 0)
                throw new Exception(string.Format("Cannot move stream to invalid pointer. Found at {0:X}", s.Position - 4));
            s.Seek(pointer, SeekOrigin.Begin);
        }

        /// <summary>Moves the stream position to the address specified in the 2-byte SNES LoROM pointer relative to the given bank.</summary>
        /// <param name="s">The stream</param>
        /// <param name="bank">The LoROM bank of the pointer. This should be between 0x80 and 0xff.</param>
        /// <exception cref="System.Exception">if the end of the stream is reached while trying to read the pointer or an invalid pointer is at the current position.</exception>
        public static void GoToRelativePointer(this Stream s, int bank) {
            int pointer = s.ReadRelativePointer(bank);
            if (pointer < 0)
                throw new Exception(string.Format("Cannot move stream to invalid pointer. Found at {0:X}", s.Position - 2));
            s.Seek(pointer, SeekOrigin.Begin);
        }

        /// <summary>Moves the stream position to the address specified in the 2-byte SNES LoROM pointer relative to the bank the stream is positioned in.</summary>
        /// <param name="s">The stream</param>
        /// <exception cref="System.Exception">if the end of the stream is reached while trying to read the pointer or an invalid pointer is at the current position.</exception>
        public static void GoToRelativePointer(this Stream s) {
            int bank = (int)s.Position / 0x8000 + 0x80;
            s.GoToRelativePointer(bank);
        }

        /// <summary>Moves the stream position to the address specified in the 4-byte SNES LoROM pointer at the current position and pushes the position after the pointer.</summary>
        /// <param name="s">The stream</param>
        /// <exception cref="System.Exception">if the end of the stream is reached while trying to read the pointer or an invalid pointer is at the current position.</exception>
        public static void GoToPointerPush(this NStream s) {
            int pointer = s.ReadPointer();
            if (pointer < 0)
                throw new Exception(string.Format("Cannot move stream to invalid pointer. Found at {0:X}", s.Position - 4));
            s.PushPosition();
            s.Seek(pointer, SeekOrigin.Begin);
        }

        /// <summary>Moves the stream position to the address specified in the 2-byte SNES LoROM pointer relative to the given bank and pushes the position after the pointer.</summary>
        /// <param name="s">The stream</param>
        /// <param name="bank">The LoROM bank of the pointer. This should be between 0x80 and 0xff.</param>
        /// <exception cref="System.Exception">if the end of the stream is reached while trying to read the pointer or an invalid pointer is at the current position.</exception>
        public static void GoToRelativePointerPush(this NStream s, int bank) {
            int pointer = s.ReadRelativePointer(bank);
            if (pointer < 0)
                throw new Exception(string.Format("Cannot move stream to invalid pointer. Found at {0:X}", s.Position - 2));
            s.PushPosition();
            s.Seek(pointer, SeekOrigin.Begin);
        }

        /// <summary>Moves the stream position to the address specified in the 2-byte SNES LoROM pointer relative to the bank the stream is positioned in and pushes the position after the pointer.</summary>
        /// <param name="s">The stream</param>
        /// <exception cref="System.Exception">if the end of the stream is reached while trying to read the pointer or an invalid pointer is at the current position.</exception>
        public static void GoToRelativePointerPush(this NStream s) {
            int bank = (int)s.Position / 0x8000 + 0x80;
            s.GoToRelativePointerPush(bank);
        }

        /// <summary>Writes a 4-byte SNES LoROM pointer into the stream at the current position.</summary>
        /// <param name="s">The stream</param>
        /// <param name="pointer">The PC address of the pointer to write. This should be between 0 and 0x3fffff.</param>
        /// <exception cref="System.Exception">if the given pointer does not correspond to a valid LoROM address.</exception>
        public static void WritePointer(this Stream s, int pointer) {
            if (pointer < 0 || pointer >= 0x400000)
                throw new Exception(string.Format("{0:X} is not a valid PC address", pointer));
            int address = pointer % 0x8000 + 0x8000;
            int bank = pointer / 0x8000 + 0x80;
            s.WriteByte((byte)(address % 0x100));
            s.WriteByte((byte)(address / 0x100));
            s.WriteByte((byte)bank);
            s.WriteByte((byte)0); // Pointers in ZAMN are always 4 bytes even though the fourth byte is useless.
        }

        /// <summary>Writes a 2-byte SNES LoROM pointer into the stream at the current position.</summary>
        /// <param name="s">The stream</param>
        /// <param name="pointer">The PC address of the pointer to write. This should be between 0 and 0x3fffff.</param>
        /// <exception cref="System.Exception">if the given pointer does not correspond to a valid LoROM address.</exception>
        public static void WriteRelativePointer(this Stream s, int pointer) {
            if (pointer < 0 || pointer >= 0x400000)
                throw new Exception(string.Format("{0:X} is not a valid PC address", pointer));
            int address = pointer % 0x8000 + 0x8000;
            s.WriteByte((byte)(address % 0x100));
            s.WriteByte((byte)(address / 0x100));
        }

        /// <summary>Reads an unsigned little-endian 16-bit integer from the stream.</summary>
        /// <param name="s">The stream</param>
        /// <returns>The integer or -1 if the stream reached the end</returns>
        public static ushort ReadInt16(this Stream s) {
            int byte1 = s.ReadByte();
            int byte2 = s.ReadByte();
            if (byte2 < 0)
                throw new EndOfStreamException();
            return (ushort)(byte1 | byte2 << 8);
        }

        /// <summary>Reads an unsigned little-endian 16-bit integer from the stream without advancing the position.</summary>
        /// <param name="s">The stream</param>
        /// <returns>The integer or -1 if the stream reached the end</returns>
        public static ushort PeekInt16(this Stream s) {
            ushort value = s.ReadInt16();
            s.Seek(-2, SeekOrigin.Current);
            return value;
        }

        /// <summary>Reads a 4-byte SNES LoROM pointer from the stream without advancing the position.</summary>
        /// <param name="s">The stream</param>
        /// <returns>The pointer as a PC address or -1 if the end of the stream was reached or there was an invalid pointer at the current position.</returns>
        public static int PeekPointer(this Stream s) {
            int value = s.ReadPointer();
            s.Seek(-4, SeekOrigin.Current);
            return value;
        }
    }
}
