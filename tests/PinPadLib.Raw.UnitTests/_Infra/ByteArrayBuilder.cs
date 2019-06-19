using System.Collections.Generic;
using System.Text;

namespace PinPadLib.Raw.UnitTests._Infra
{
    internal class ByteArrayBuilder
    {
        private readonly List<byte> bytes;

        public ByteArrayBuilder()
        {
            this.bytes = new List<byte>();
        }
        public Encoding Encoding { get; set; } = Encoding.ASCII;

        public void Add(params byte[] bytes)
        {
            foreach (var b in bytes)
            {
                this.bytes.Add(b);
            }
        }

        public void Add(string str)
        {
            this.bytes.AddRange(Encoding.GetBytes(str));
        }

        public byte[] ToArray()
        {
            return this.bytes.ToArray();
        }
    }
}
