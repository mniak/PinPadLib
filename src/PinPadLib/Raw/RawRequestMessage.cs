using PinPadLib.Utils;
using System;
using System.Linq;

namespace PinPadLib.Raw
{
    public class RawRequestMessage : RawMessage<RequestInterruption>
    {
        public RawRequestMessage(RequestInterruption interruption) : base(interruption)
        {
        }
        public RawRequestMessage(byte[] bytes) : base(bytes)
        {
        }

        public static implicit operator RawRequestMessage(RequestInterruption interruption)
        {
            return new RawRequestMessage(interruption);
        }
        public static implicit operator RawRequestMessage(byte[] data)
        {
            return new RawRequestMessage(data);
        }

        public byte[] RawBytesToSend()
        {
            return Match(i =>
            {
                switch (i)
                {
                    case RequestInterruption.Abort:
                        return new[] { Bytes.CAN };
                    default:
                        return Array.Empty<byte>();
                }
            }, data =>
            {
                var crc = Crc16.Compute(data.Concat(new[] { Bytes.ETB }));
                var crcByte1 = (byte)(crc / 256);
                var crcByte2 = (byte)(crc % 256);
                var bytes = new ByteArrayBuilder()
                    .Add(Bytes.SYN)
                    .Add(data)
                    .Add(Bytes.ETB)
                    .Add(crcByte1, crcByte2);
                return bytes.ToArray();
            });
        }
    }
}
