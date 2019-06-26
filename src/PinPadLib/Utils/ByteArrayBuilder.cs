using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace PinPadLib.Utils
{
    internal class ByteArrayBuilder : IEnumerable<byte>
    {
        private readonly List<byte> bytes;

        public ByteArrayBuilder()
        {
            this.bytes = new List<byte>();
        }
        public Encoding Encoding { get; set; } = Encoding.ASCII;

        public ByteArrayBuilder Add(params byte[] bytes)
        {
            foreach (var b in bytes)
            {
                this.bytes.Add(b);
            }
            return this;
        }

        public ByteArrayBuilder Add(string str)
        {
            this.bytes.AddRange(Encoding.GetBytes(str));
            return this;
        }

        public IEnumerator<byte> GetEnumerator()
        {
            return this.bytes.GetEnumerator();
        }

        public byte[] ToArray()
        {
            return this.bytes.ToArray();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.bytes.GetEnumerator();
        }
    }
}
