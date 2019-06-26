using PinPadLib.Raw;
using System;
using System.Collections.Generic;
using System.Text;
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
            var bytes = new List<byte>();
            bytes.AddRange(Encoding.ASCII.GetBytes(message.Command.acronym));
            foreach (var parameter in message.Parameters)
            {
                bytes.AddRange(Encoding.ASCII.GetBytes(parameter.Length.ToString("000")));
                bytes.AddRange(Encoding.ASCII.GetBytes(parameter));
            }
            return this.rawPinPad.SendRawMessageAsync(bytes.ToArray());
        }
    }
}
