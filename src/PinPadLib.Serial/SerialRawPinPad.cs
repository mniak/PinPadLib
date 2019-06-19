using PinPadLib.Raw;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading.Tasks;

namespace PinPadLib.Serial
{
    internal class SerialRawPinPad : IRawPinPad
    {
        private readonly SerialPort serialPort;
        private readonly Queue<ResponseInterruption> interruptionQueue = new Queue<ResponseInterruption>();

        public SerialRawPinPad(SerialPort serialPort)
        {
            this.serialPort = serialPort ?? throw new ArgumentNullException(nameof(serialPort));
        }

        public Task SendRawMessageAsync(RawRequestMessage rawMessage)
        {
            var bytes = rawMessage.Match(
                intr => FormatInterruption(intr),
                data => FormatData(data));

            return Task.Run(() =>
            {
                try
                {
                    this.serialPort.Write(bytes, 0, bytes.Length);
                }
                catch (Exception)
                {
                    this.interruptionQueue.Enqueue(ResponseInterruption.CommunicationError);
                }
            });
        }

        public Task<RawResponseMessage> ReceiveRawMessageAsync()
        {
            this.serialPort.ReadExisting(
        }

        private byte[] FormatInterruption(RequestInterruption interruption)
        {
            switch (interruption)
            {
                case RequestInterruption.Abort:
                    return new byte[] { 0x18 };
                default:
                    return Array.Empty<byte>();
            }
        }

        private byte[] FormatData(byte[] data)
        {
            var tmp = new List<byte>();
            tmp.Add(0x16);
            tmp.AddRange(data);
            tmp.Add(0x17);

            var crcBytes = CalcCrc16(tmp);
            tmp.AddRange(crcBytes);

            return tmp.ToArray();
        }

        private byte[] CalcCrc16(IEnumerable<byte> data)
        {
            ushort CRC_MASK = 0x1021;
            ushort wCRC = 0;
            foreach (var b in data)
            {
                var wData = b << 8;
                for (byte i = 0; i < 8; i++)
                {
                    if (((wCRC ^ wData) & 0x8000) != 0)
                    {
                        wCRC <<= 1;
                        wCRC ^= CRC_MASK;
                    }
                    else
                    {
                        wCRC <<= 1;
                    }

                    wData <<= 1;
                }
            }
            var result = new byte[] {
                (byte)(wCRC / 256),
                (byte)(wCRC % 256),
            };
            return result;
        }
    }
}
