using PinPadLib.Raw;
using System;
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
            throw new NotImplementedException();
        }
    }
}
