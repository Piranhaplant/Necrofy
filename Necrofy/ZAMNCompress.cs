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
                    int byteCount = (temp2 & 0xf) + 3; // The last nybble is how many bytes to read, but we actually need 3 more bytes than that
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

            result.Add(0); // The final size will be put here
            result.Add(0);
            result.Add(0); // And one more to hold the first format byte
            while (dataIndex < data.Length) {
                formatByte >>= 1;
                formatBitCount++;
                FindInDict(dict, dictIndex, data, dataIndex, out findDictPos, out findDictLen); // Search for a dictionary match
                if (findDictPos > -1) { // Insert a read from the dictionary
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
					// if (dataIndex < data.Length) // I think this should be here, but it needs to be tested
                    result.Add(0); // Make a placeholder for the next one
                }
            }

            int len = result.Count - 2; // Don't count the first two bytes themselves
            result[0] = (byte)(len & 0xff);
            result[1] = (byte)((len & 0xff00) >> 8);
            return result.ToArray();
        }
                
        // This will find the longest match in the dictionary
        private static void FindInDict(byte[] dict, int dictWriteIndex, byte[] data, int index, out int pos, out int len) {
            int maxMatchCt = 0;
            int maxMatchPos = -1;
            for (int i = 0; i < dict.Length; i++) {
                int curMatchCt = 0;
                while (curMatchCt < 18 && index + curMatchCt < data.Length
                        && GetDictByte(dict, dictWriteIndex, i, data, index, curMatchCt) == data[index + curMatchCt]) {
                    curMatchCt++;
                }
                if (curMatchCt > maxMatchCt && curMatchCt >= 3) {
                    maxMatchCt = curMatchCt;
                    maxMatchPos = i;
                }
            }
            pos = maxMatchPos;
            len = maxMatchCt;
        }

        // Gets the byte that will be in the dictionary at dictReadIndex given that matchLen bytes from data will have been written at dictWriteIndex
        private static byte GetDictByte(byte[] dict, int dictWriteIndex, int dictReadIndex, byte[] data, int index, int matchLen) {
            // Make dictWriteIndex larger so we can ignore the wrapping
            if (dictWriteIndex < dictReadIndex)
                dictWriteIndex += dict.Length;
            // The actual position we are reading from the dictionary
            int readIndex = dictReadIndex + matchLen;
            // If the byte we are reading has been overwritten from data
            if (readIndex >= dictWriteIndex && readIndex < dictWriteIndex + matchLen) {
                int offset = readIndex - dictWriteIndex;
                return data[index + offset];
            }
            return dict[dictReadIndex % dict.Length];
        }
    }
}
