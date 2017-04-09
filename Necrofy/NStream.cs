using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Necrofy
{
    /// <summary>A stream wrapper that provides some extra features</summary>
    class NStream : Stream
    {
        private Stream inner;
        private Stack<long> posStack = new Stack<long>();
        private long readBlockStart = -1;
        private long readBlockEnd = -1;

        public NStream(Stream inner) {
            this.inner = inner;
        }

        // New stuff

        public void PushPosition() {
            posStack.Push(Position);
        }

        public void PopPosition() {
            Position = posStack.Pop();
        }

        /// <summary>Starts tracking the block of data read from for the purpose of adding it to freespace</summary>
        public void StartBlock() {
            readBlockStart = Position;
            readBlockEnd = readBlockStart + 1;
        }

        /// <summary>Stops tracking the block of data read from and adds it to the given freespace</summary>
        /// <param name="freespace">The freespace to add the block to.</param>
        public void EndBlock(Freespace freespace) {
            if (readBlockStart < 0)
                throw new InvalidOperationException("StartBlock must be called before EndBlock");
            freespace.Add((int)readBlockStart, (int)readBlockEnd);
            readBlockStart = -1;
            readBlockEnd = -1;
        }

        // Base implementation

        public override bool CanRead {
            get { return inner.CanRead; }
        }

        public override bool CanSeek {
            get { return inner.CanSeek; }
        }

        public override bool CanWrite {
            get { return inner.CanWrite; }
        }

        public override void Close() {
            inner.Close();
        }

        public override void Flush() {
            inner.Flush();
        }

        public override long Length {
            get { return inner.Length; }
        }

        public override long Position {
            get {
                return inner.Position;
            }
            set {
                inner.Position = value;
            }
        }

        public override int Read(byte[] buffer, int offset, int count) {
            if (readBlockStart >= 0) {
                readBlockStart = Math.Min(readBlockStart, Position);
                readBlockEnd = Math.Max(readBlockEnd, Position + count);
            }
            return inner.Read(buffer, offset, count);
        }

        public override int ReadByte() {
            if (readBlockStart >= 0) {
                readBlockStart = Math.Min(readBlockStart, Position);
                readBlockEnd = Math.Max(readBlockEnd, Position + 1);
            }
            return inner.ReadByte();
        }

        public override long Seek(long offset, SeekOrigin origin) {
            return inner.Seek(offset, origin);
        }

        public override void SetLength(long value) {
            inner.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count) {
            inner.Write(buffer, offset, count);
        }

        public override void WriteByte(byte value) {
            inner.WriteByte(value);
        }
    }
}
