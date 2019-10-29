using System;
using System.Collections.Generic;
using System.IO;

namespace PinPadLib.Raw.UnitTests._Infra
{
    public class RWStream : Stream
    {
        private readonly Queue<byte> bytesToRead;
        private readonly Queue<byte> bytesWritten;

        public RWStream()
        {
            this.bytesToRead = new Queue<byte>();
            this.bytesWritten = new Queue<byte>();
        }

        public override bool CanRead => true;

        public override bool CanSeek => false;

        public override bool CanWrite => true;

        public override long Length => throw new InvalidOperationException();

        public override long Position { get => throw new InvalidOperationException(); set => throw new InvalidOperationException(); }

        public override void Flush() { }

        public override int Read(byte[] buffer, int offset, int count)
        {
            for (var i = 0; i < count; i++)
            {
                if (this.bytesToRead.TryDequeue(out var b))
                {
                    buffer[offset + i] = b;
                }
                else
                {
                    return i;
                }
            }
            return count;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new InvalidOperationException();
        }

        public override void SetLength(long value)
        {
            throw new InvalidOperationException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            for (var i = 0; i < count; i++)
            {
                var b = buffer[offset + i];
                this.bytesWritten.Enqueue(b);
            }
        }

        public byte[] GetBytesWritten()
        {
            return this.bytesWritten.ToArray();
        }

        public void PushByteToRead(byte b)
        {
            this.bytesToRead.Enqueue(b);
        }
        public void PushBytesToRead(IEnumerable<byte> bytes)
        {
            foreach (var b in bytes)
            {
                this.bytesToRead.Enqueue(b);
            }
        }
    }
}
