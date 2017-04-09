using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Necrofy
{
    /// <summary>
    /// Holds data that contains pointers relative to its starting location. This temporary storage is used to determine the size of data before inserting it into the ROM.
    /// </summary>
    class MovableData
    {
        public List<byte> data { get; private set; }
        private List<PointedData> pointers;

        public MovableData() {
            data = new List<byte>();
            pointers = new List<PointedData>();
        }

        /// <summary>Adds a pointer to data</summary>
        /// <param name="pointerSize">The size of the pointer</param>
        /// <returns>The pointer holder</returns>
        public void AddPointer(PointerSize pointerSize, MovableData refData) {
            PointedData pointer = new PointedData(pointerSize, data.Count, refData);
            data.AddRange(new byte[(int)pointerSize]);
            pointers.Add(pointer);
        }

        /// <summary>Combines all of the data into a single byte array given it will be placed at the given address.</summary>
        /// <param name="baseAddress">The address at which the data will be placed</param>
        public byte[] Build(int baseAddress) {
            List<byte> output = new List<byte>(data);
            foreach (PointedData pointer in pointers) {
                int address = baseAddress + output.Count;
                byte[] refData = pointer.refData.Build(address);
                output.AddRange(refData);
                if (pointer.pointerSize == PointerSize.TwoBytes) {
                    output.WriteRelativePointer(pointer.pointerIndex, address);
                } else if (pointer.pointerSize == PointerSize.FourBytes) {
                    output.WritePointer(pointer.pointerIndex, address);
                }
            }
            return output.ToArray();
        }

        /// <summary>Gets the total size of the data</summary>
        /// <returns>The size</returns>
        public int GetSize() {
            int size = data.Count;
            foreach (PointedData pointer in pointers) {
                size += pointer.refData.GetSize();
            }
            return size;
        }

        public enum PointerSize : int
        {
            TwoBytes = 2,
            FourBytes = 4
        }

        /// <summary>
        /// Hold data that will be referred to by a pointer in MovableData
        /// </summary>
        private class PointedData
        {
            /// <summary>The size of the pointer</summary>
            public PointerSize pointerSize { get; private set; }
            /// <summary>The index in the data where the pointer will be placed</summary>
            public int pointerIndex { get; private set; }
            /// <summary>The data that will be pointed to</summary>
            public MovableData refData { get; private set; }

            public PointedData(PointerSize pointerSize, int pointerIndex, MovableData refData) {
                this.pointerSize = pointerSize;
                this.pointerIndex = pointerIndex;
                this.refData = refData;
            }
        }
    }
}
