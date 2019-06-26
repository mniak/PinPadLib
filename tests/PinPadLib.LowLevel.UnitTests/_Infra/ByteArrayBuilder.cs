using System.Collections.Generic;
using System.Text;

namespace PinPadLib.LowLevel.UnitTests._Infra
{
    internal class ByteArrayBuilder
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

        public byte[] ToArray()
        {
            return this.bytes.ToArray();
        }
    }
}
