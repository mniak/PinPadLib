using PinPadLib.Raw;
using PinPadLib.Utils;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PinPadLib.LowLevel
{
    public class LowLevelPinPad : ILowLevelPinPad
    {
        private readonly IRawPinPad rawPinPad;

        public LowLevelPinPad(IRawPinPad rawPinPad)
        {
            this.rawPinPad = rawPinPad ?? throw new ArgumentNullException(nameof(rawPinPad));
        }
        public Task<LowLevelResponseMessage> ReceiveResponseAsync()
        {
            throw new NotImplementedException();
        }

        public Task SendCommandAsync(LowLevelRequestMessage message)
        {
            var bytes = new ByteArrayBuilder()
                .Add(Bytes.SYN)
                .Add(message.Command.acronym);
            if (!message.Parameters.Any())
            {
                message = message.With(parameters: new[] { string.Empty });
            }
            foreach (var parameter in message.Parameters)
            {
                bytes
                    .Add(parameter.Length.ToString("000"))
                    .Add(parameter);
            }
            bytes.Add(Bytes.ETB);
            var crc = Crc16.Compute(bytes.Skip(1));
            bytes.Add((byte)(crc / 256), (byte)(crc % 256));
            return this.rawPinPad.SendRawMessageAsync(bytes.ToArray());
        }
    }
}
