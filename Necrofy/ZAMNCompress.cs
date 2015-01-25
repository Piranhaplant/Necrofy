using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Necrofy
{
    class ZAMNCompress
    {
        /// <summary>Decompresses the file that the stream is positioned at</summary>
        /// <param name="s">The stream holding the compressed data</param>
        /// <returns>The decompressed data</returns>
        public static byte[] Decompress(Stream s) {
            List<byte> result = new List<byte>();
            byte[] dict = new byte[0x1000];
            // Fill the entire dictionary with 0x20 bytes. This is done in the game's decompressor, so we need to match it.
            for (int i = 0; i < dict.Length; i++) {
                dict[i] = 0x20;
            }
            int bytesLeft = s.ReadInt16();
            int writeDictPos = 0xfee; // The dictionary starts at 0xfee for some reason
            int bitsLeft = 0;
            byte formatByte = 0; // The byte that will specify how to read from the file
            byte temp = 0; // Used for temporarily holding data

            while (true) {
                if (bitsLeft == 0) { // When all the bits are used
                    bitsLeft = 8; // Reset to start from the beginning of the byte
                    if (ReadNext(s, ref formatByte, ref bytesLeft)) break; // Get a new byte
                }
                bitsLeft--;
                if ((formatByte & 1) == 1) { // If the current bit is a one
                    if (ReadNext(s, ref temp, ref bytesLeft)) break; // Simply read a byte from the file
                    result.Add(temp); // Put it in the result
                    dict[writeDictPos] = temp; // And the dictionary
                    writeDictPos = (writeDictPos + 1) & 0xfff; // Increment the dictionary position, but not over 0xfff
                } else { // If the current bit is zero
                    byte temp2 = 0;
                    if (ReadNext(s, ref temp, ref bytesLeft) || ReadNext(s, ref temp2, ref bytesLeft)) break;
                    int readDictPos = temp | ((temp2 & 0xf0) << 4); // These 3 nybbles specify the position to read from the dictionary
                    int byteCount = (temp2 & 0xf) + 2; // The last nybble is how many bytes to read, but we actually need 2 more bytes than that
                    // Transfer that many bytes from the dictionary to the result, also writing them in the dictionary
                    for (int i = 0; i < byteCount; i++) {
                        temp = dict[readDictPos];
                        dict[writeDictPos] = temp;
                        result.Add(temp);
                        writeDictPos = (writeDictPos + 1) & 0xfff;
                        readDictPos = (readDictPos + 1) & 0xfff;
                    }
                }
                formatByte >>= 1; // Move to the next bit
            }
            return result.ToArray();
        }
        
        private static bool ReadNext(Stream s, ref byte result, ref int bytesLeft) {
            if (bytesLeft == 0) return true;
            int b = s.ReadByte();
            if (b < 0) return true;
            bytesLeft--;
            result = (byte)b;
            return false;
        }
        
        public static byte[] Compress(byte[] data) {
            List<byte> result = new List<byte>(data.Length / 2); // Make the default size half that of the original data
            byte[] dict = new byte[0x1000];
            // Fill the entire dictionary with 0x20 bytes. This is done in the game's decompressor, so we need to match it.
            for (int i = 0; i < dict.Length; i++) {
                dict[i] = 0x20;
            }
            int dictIndex = 0xfee;
            int dataIndex = 0;
            byte formatByte = 0;
            int formatBitCount = 0;
            int formatByteIndex = 2; // The first format byte will be after the file size

            int findDictPos, findDictLen;
            int findRepSize, findRepLen;

            result.Add(0); // The final size will be put here
            result.Add(0);
            result.Add(0); // And one more to hold the first format byte
            while (dataIndex < data.Length) {
                formatByte >>= 1;
                formatBitCount++;
                FindInDict(dict, data, dataIndex, out findDictPos, out findDictLen); // Search for a dictionary match
                FindRepeat(data, dataIndex, out findRepSize, out findRepLen); // Search for a repeating pattern
                if (findRepLen - findRepSize > findDictLen && findRepLen >= 3) { // Most efficient to insert a data repeat
                    // Write the data to be repeated
                    for (int i = 0; i < findRepSize; i++) {
                        result.Add(data[dataIndex + 1]);
                        formatByte |= 0x80; // Set the bit in the format byte
                        if (formatBitCount == 8) { // If the format byte is full
                            result[formatByteIndex] = formatByte;
                            formatByte = 0;
                            formatBitCount = 0;
                            formatByteIndex = result.Count;
                            result.Add(0); // Placeholder for next format byte
                        }
                        formatByte >>= 1;
                        formatBitCount++;
                    }
                    // Tell it to repeat
                    result.Add((byte)(dictIndex & 0xff)); // Write the dictionary position
                    result.Add((byte)(((dictIndex & 0xf00) >> 4) | (findRepLen - 3))); // Write the rest of the position and the length
                    for (int i = 0; i < findRepLen + findRepSize; i++) { // Insert the new data to the dictionary
                        dict[(dictIndex + i) & 0xfff] = data[dictIndex + (i % findRepSize)];
                    }
                    dictIndex = (dictIndex + findRepLen + findRepSize) & 0xfff;
                    dataIndex += findRepLen + findRepSize;
                } else if (findDictPos > -1) { // Insert a read from the dictionary
                    for (int i = 0; i < findDictLen; i++) { // Write the new bytes to the dictionary
                        dict[dictIndex] = dict[(findDictPos + i) & 0xfff];
                        dictIndex = (dictIndex + 1) & 0xfff;
                    }
                    dataIndex += findDictLen;
                    result.Add((byte)(findDictPos & 0xff));
                    result.Add((byte)(((findDictPos & 0xf00) >> 4) | (findDictLen - 3)));
                } else { // Just insert the byte by itself
                    result.Add(data[dataIndex]);
                    dict[dictIndex] = data[dataIndex];
                    dictIndex = (dictIndex + 1) & 0xfff;
                    dataIndex++;
                    formatByte |= 0x80;
                }
                if (formatBitCount == 8 || dataIndex == data.Length) { // If the format byte is full or the file has ended
                    formatByte >>= (8 - formatBitCount); // If the file has ended, but there is still part of a format byte left
                    result[formatByteIndex] = formatByte;
                    formatByte = 0;
                    formatBitCount = 0;
                    formatByteIndex = result.Count;
                    result.Add(0); // Make a placeholder for the next one
                }
            }

            int len = result.Count - 2; // Don't count the first two bytes themselves
            result[0] = (byte)(len & 0xff);
            result[1] = (byte)((len & 0xff00) >> 8);
            return result.ToArray();
        }
                
        // This will find the longest match in the dictionary
        private static void FindInDict(byte[] dict, byte[] data, int index, out int pos, out int len) {
            int maxMatchCount = 0;
            int maxMatchPos = -1;
            for (int idict = 0; idict < dict.Length; idict++) {
                if (dict[idict] == data[index]) {
                    int i2;
                    for (i2 = 0; i2 < 18; i2++) {
                        if (index + i2 >= data.Length) break;
                        if (dict[(idict + i2) & 0xfff] != data[index + i2]) break;
                    }
                    if (i2 > maxMatchCount && i2 > 2) {
                        maxMatchCount = i2;
                        maxMatchPos = idict;
                    }
                }
            }
            pos = maxMatchPos;
            len = maxMatchCount;
        }

        // This will find the longest repeating sequence
        private static void FindRepeat(byte[] data, int index, out int dataSize, out int totalSize) {
            int maxSize = 1;
            int maxDataSize = 0;
            for (int dsize = 1; dsize < 9; dsize++) {
                int tsize;
                for (tsize = dsize; tsize < dsize + 18; tsize++) {
                    if (index + tsize >= data.Length) break;
                    if (data[index + tsize] != data[index + (tsize % dsize)]) break;
                }
                if (tsize - dsize > maxDataSize) {
                    maxDataSize = tsize - dsize;
                }
                maxSize = dsize;
            }
            dataSize = maxSize;
            totalSize = maxDataSize;
        }
    }
}
