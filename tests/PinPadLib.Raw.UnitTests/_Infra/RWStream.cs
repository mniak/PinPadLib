using System;
using System.Collections.Generic;
using System.IO;

namespace PinPadLib.Raw.UnitTests._Infra
{
    public class RWStream : Stream
    {
        [Flags]
        public enum Mode
        {
            Read = 1,
            Write = 2,
            ReadWrite = 3,
        }

        private readonly Queue<byte> bytesToRead;
        private readonly Queue<byte> bytesWritten;
        private readonly Mode mode;

        public RWStream(Mode mode = Mode.ReadWrite)
        {
            this.mode = mode;
            this.bytesToRead = new Queue<byte>();
            this.bytesWritten = new Queue<byte>();
        }

        public override bool CanRead => this.mode.HasFlag(Mode.Read);

        public override bool CanSeek => false;

        public override bool CanWrite => this.mode.HasFlag(Mode.Write);

        public override long Length => throw new InvalidOperationException();

        public override long Position { get => throw new InvalidOperationException(); set => throw new InvalidOperationException(); }

        public override void Flush() { }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (!CanRead)
                throw new InvalidOperationException("The stream is not readable");

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
            if (!CanWrite)
                throw new InvalidOperationException("The stream is not writeable");

            for (var i = 0; i < count; i++)
            {
                var b = buffer[offset + i];
                this.bytesWritten.Enqueue(b);
            }
        }

        public byte[] GetBytesWritten()
        {
            if (!CanWrite)
                throw new InvalidOperationException("The stream is not writeable");

            return this.bytesWritten.ToArray();
        }

        public void PushByteToRead(byte b)
        {
            if (!CanRead)
                throw new InvalidOperationException("The stream is not readable");

            this.bytesToRead.Enqueue(b);
        }
        public void PushBytesToRead(IEnumerable<byte> bytes)
        {
            if (!CanRead)
                throw new InvalidOperationException("The stream is not readable");

            foreach (var b in bytes)
            {
                this.bytesToRead.Enqueue(b);
            }
        }
    }
}
