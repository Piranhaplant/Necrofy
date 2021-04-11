using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Globalization;

namespace Necrofy
{
    /// <summary>
    /// Contains pointers to important data in the ZAMN ROM.
    /// Also holds several helper methods related to reading/writing pointers.
    /// </summary>
    static class ROMPointers
    {
        /// <summary>Pointer to the table of level pointers</summary>
        public const int LevelPointers = 0xf8000;
        /// <summary>Pointer to the table that indicates which bonus level follows each level</summary>
        public const int BonusLevelNums = 0x1517e;
        /// <summary>Pointer to the table that indicates what code to execute for the secret bonus on each level</summary>
        public const int SecretBonusCodePointers = 0x151ee;
        /// <summary>Pointer to the ROM size in the SNES header</summary>
        public const int ROMSize = 0x7fd7;
        /// <summary>Pointer to the first chunk of sprite data</summary>
        public const int SpriteData1 = 0x7d300;
        /// <summary>Pointer to the second chunk of sprite data</summary>
        public const int SpriteData2 = 0x80000;
        /// <summary>Pointer to the first two characters in the level 0 password</summary>
        public const int Level0Password1 = 0x1301c;
        /// <summary>Pointer to the last two characters in the level 0 password</summary>
        public const int Level0Password2 = 0x13024;

        // NOTE: in these functions I am using "address" to refer to the lower 16 bits of the full 24 bit LoROM address.

        /// <summary>Reads a 4-byte SNES LoROM pointer from the stream.</summary>
        /// <param name="s">The stream</param>
        /// <returns>The pointer as a PC address or -1 if the end of the stream was reached or there was an invalid pointer at the current position.</returns>
        public static int ReadPointer(this Stream s) {
            int address = s.ReadInt16();
            int bank = s.ReadInt16();
            if (address < 0x8000 || bank < 0x80 || bank > 0xFF)
                return -1;
            return (bank - 0x80) * 0x8000 + address - 0x8000;
        }

        /// <summary>Reads a 2-byte SNES LoROM pointer from the stream as if it was from the given bank.</summary>
        /// <param name="s">The stream</param>
        /// <param name="bank">The LoROM bank of the pointer. This should be between 0x80 and 0xff.</param>
        /// <returns>The pointer as a PC address or -1 if the end of the stream was reached or there was an invalid pointer at the current position.</returns>
        public static int ReadRelativePointer(this Stream s, byte bank) {
            int address = s.ReadInt16();
            if (address < 0x8000 || bank < 0x80)
                return -1;
            return (bank - 0x80) * 0x8000 + address - 0x8000;
        }

        /// <summary>Moves the stream position to the address specified in the 4-byte SNES LoROM pointer at the current position.</summary>
        /// <param name="s">The stream</param>
        public static void GoToPointer(this Stream s) {
            int pointer = s.ReadPointer();
            if (pointer < 0)
                throw new Exception(string.Format("Cannot move stream to invalid pointer. Found at {0:X}", s.Position - 4));
            s.Seek(pointer);
        }

        /// <summary>Moves the stream position to the address specified in the 2-byte SNES LoROM pointer relative to the given bank.</summary>
        /// <param name="s">The stream</param>
        /// <param name="bank">The LoROM bank of the pointer. This should be between 0x80 and 0xff.</param>
        public static void GoToRelativePointer(this Stream s, byte bank) {
            int pointer = s.ReadRelativePointer(bank);
            if (pointer < 0)
                throw new Exception(string.Format("Cannot move stream to invalid pointer. Found at {0:X}", s.Position - 2));
            s.Seek(pointer);
        }

        /// <summary>Moves the stream position to the address specified in the 2-byte SNES LoROM pointer relative to the bank the stream is positioned in.</summary>
        /// <param name="s">The stream</param>
        public static void GoToRelativePointer(this Stream s) {
            byte bank = (byte)(s.Position / 0x8000 + 0x80);
            s.GoToRelativePointer(bank);
        }

        /// <summary>Moves the stream position to the address specified in the 4-byte SNES LoROM pointer at the current position and pushes the position after the pointer.</summary>
        /// <param name="s">The stream</param>
        public static void GoToPointerPush(this NStream s) {
            int pointer = s.ReadPointer();
            if (pointer < 0)
                throw new Exception(string.Format("Cannot move stream to invalid pointer. Found at {0:X}", s.Position - 4));
            s.PushPosition();
            s.Seek(pointer);
        }

        /// <summary>Moves the stream position to the address specified in the 2-byte SNES LoROM pointer relative to the given bank and pushes the position after the pointer.</summary>
        /// <param name="s">The stream</param>
        /// <param name="bank">The LoROM bank of the pointer. This should be between 0x80 and 0xff.</param>
        public static void GoToRelativePointerPush(this NStream s, byte bank) {
            int pointer = s.ReadRelativePointer(bank);
            if (pointer < 0)
                throw new Exception(string.Format("Cannot move stream to invalid pointer. Found at {0:X}", s.Position - 2));
            s.PushPosition();
            s.Seek(pointer);
        }

        /// <summary>Moves the stream position to the address specified in the 2-byte SNES LoROM pointer relative to the bank the stream is positioned in and pushes the position after the pointer.</summary>
        /// <param name="s">The stream</param>
        public static void GoToRelativePointerPush(this NStream s) {
            byte bank = (byte)(s.Position / 0x8000 + 0x80);
            s.GoToRelativePointerPush(bank);
        }

        /// <summary>Adds a 4-byte SNES LoROM pointer to the list.</summary>
        /// <param name="list">The list</param>
        /// <param name="pointer">The PC address of the pointer to write. This should be between 0 and 0x3fffff.</param>
        public static void AddPointer(this IList<byte> list, int pointer) {
            byte[] pointerBytes = new byte[4];
            WritePointer(pointerBytes, 0, pointer);
            for (int i = 0; i < pointerBytes.Length; i++) {
                list.Add(pointerBytes[i]);
            }
        }

        /// <summary>Writes a 4-byte SNES LoROM pointer into the stream at the current position.</summary>
        /// <param name="s">The stream</param>
        /// <param name="pointer">The PC address of the pointer to write. This should be between 0 and 0x3fffff.</param>
        public static void WritePointer(this Stream s, int pointer) {
            byte[] pointerBytes = new byte[4];
            WritePointer(pointerBytes, 0, pointer);
            s.Write(pointerBytes, 0, pointerBytes.Length);
        }

        /// <summary>Writes a 4-byte SNES LoROM pointer into the list at the given index.</summary>
        /// <param name="list">The list</param>
        /// <param name="index">The index at which to write the pointer</param>
        /// <param name="pointer">The PC address of the pointer to write. This should be between 0 and 0x3fffff.</param>
        public static void WritePointer(this IList<byte> list, int index, int pointer) {
            if (pointer < 0 || pointer >= 0x8000 * 0x80)
                throw new Exception(string.Format("{0:X} is not a valid PC address", pointer));
            int address = pointer % 0x8000 + 0x8000;
            int bank = pointer / 0x8000 + 0x80;
            list[index + 0] = (byte)(address % 0x100);
            list[index + 1] = (byte)(address / 0x100);
            list[index + 2] = (byte)bank;
            list[index + 3] = (byte)0; // Pointers in ZAMN are always 4 bytes even though the fourth byte is useless.
        }
        
        /// <summary>Adds a 2-byte SNES LoROM pointer to the list.</summary>
        /// <param name="list">The list</param>
        /// <param name="pointer">The PC address of the pointer to write. This should be between 0 and 0x3fffff.</param>
        public static void AddRelativePointer(this IList<byte> list, int pointer) {
            byte[] pointerBytes = new byte[2];
            WriteRelativePointer(pointerBytes, 0, pointer);
            for (int i = 0; i < pointerBytes.Length; i++) {
                list.Add(pointerBytes[i]);
            }
        }

        /// <summary>Writes a 2-byte SNES LoROM pointer into the stream at the current position.</summary>
        /// <param name="s">The stream</param>
        /// <param name="pointer">The PC address of the pointer to write. This should be between 0 and 0x3fffff.</param>
        public static void WriteRelativePointer(this Stream s, int pointer) {
            byte[] pointerBytes = new byte[2];
            WriteRelativePointer(pointerBytes, 0, pointer);
            s.Write(pointerBytes, 0, pointerBytes.Length);
        }

        /// <summary>Writes a 2-byte SNES LoROM pointer into the list at the given index.</summary>
        /// <param name="list">The list</param>
        /// <param name="index">The index at which to write the pointer</param>
        /// <param name="pointer">The PC address of the pointer to write. This should be between 0 and 0x3fffff.</param>
        public static void WriteRelativePointer(this IList<byte> list, int index, int pointer) {
            if (pointer < 0 || pointer >= 0x8000 * 0x80)
                throw new Exception(string.Format("{0:X} is not a valid PC address", pointer));
            int address = pointer % 0x8000 + 0x8000;
            list[index + 0] = (byte)(address % 0x100);
            list[index + 1] = (byte)(address / 0x100);
        }

        /// <summary>Adds an unsigned little-endian 16-bit integer to the list</summary>
        /// <param name="list">The list</param>
        /// <param name="value">The value to add</param>
        public static void AddInt16(this IList<byte> list, ushort value) {
            list.Add((byte)(value & 0xff));
            list.Add((byte)(value >> 8));
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

        /// <summary>Writes an unsigned little-endian 16-bit integer to the stream</summary>
        /// <param name="s">The stream</param>
        /// <param name="value">The value to write</param>
        public static void WriteInt16(this Stream s, ushort value) {
            s.WriteByte((byte)(value & 0xff));
            s.WriteByte((byte)(value >> 8));
        }

        /// <summary>Writes all of the bytes in the given array to the stream</summary>
        /// <param name="s">The stream</param>
        /// <param name="bytes">The bytes</param>
        public static void Write(this Stream s, byte[] bytes) {
            s.Write(bytes, 0, bytes.Length);
        }

        /// <summary>Reads the specified number of bytes from the stream.</summary>
        /// <param name="s">The stream</param>
        /// <param name="count">The number of bytes to read</param>
        /// <returns>The bytes</returns>
        public static byte[] ReadBytes(this Stream s, int count) {
            byte[] data = new byte[count];
            s.Read(data, 0, count);
            return data;
        }

        /// <summary>Reads a byte from the stream without advancing the position</summary>
        /// <param name="s">The stream</param>
        /// <returns>The byte</returns>
        public static byte PeekByte(this Stream s) {
            int value = s.ReadByte();
            if (value < 0)
                throw new EndOfStreamException();
            s.Seek(-1, SeekOrigin.Current);
            return (byte)value;
        }

        /// <summary>Reads an unsigned little-endian 16-bit integer from the stream without advancing the position.</summary>
        /// <param name="s">The stream</param>
        /// <returns>The integer</returns>
        public static ushort PeekInt16(this Stream s) {
            ushort value = s.ReadInt16();
            s.Seek(-2, SeekOrigin.Current);
            return value;
        }

        /// <summary>Reads a 4-byte SNES LoROM pointer from the stream without advancing the position.</summary>
        /// <param name="s">The stream</param>
        /// <returns>The pointer as a PC address or -1 if there was an invalid pointer at the current position.</returns>
        public static int PeekPointer(this Stream s) {
            int value = s.ReadPointer();
            s.Seek(-4, SeekOrigin.Current);
            return value;
        }

        /// <summary>Tries to parse the given string as either a hexadecimal or SNES LoROM pointer</summary>
        /// <param name="value">The value to parse</param>
        /// <param name="pointer">The parsed pointer</param>
        /// <returns>true if the pointer was parsed successfully</returns>
        public static bool ParsePointer(string value, out int pointer) {
            if (value.StartsWith("$")) {
                string strippedValue = value.Replace(":", "").Replace("$", "");
                if (int.TryParse(strippedValue, NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out int parsedValue)) {
                    int bank = parsedValue / 0x10000;
                    int address = parsedValue % 0x10000;
                    if (bank >= 0x80 && bank < 0x100 && address >= 0x8000) {
                        pointer = (bank - 0x80) * 0x8000 + address - 0x8000;
                        return true;
                    }
                }
            } else {
                string strippedValue = value.StartsWith("0x") ? value.Substring(2) : value;
                if (int.TryParse(strippedValue, NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out int parsedValue)) {
                    if (parsedValue < 0x80 * 0x8000) {
                        pointer = parsedValue;
                        return true;
                    }
                }
            }
            pointer = -1;
            return false;
        }
    }
}
