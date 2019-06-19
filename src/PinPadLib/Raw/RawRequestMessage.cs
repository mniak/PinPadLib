using System;

namespace PinPadLib.Raw
{
    public class RawRequestMessage
    {
        private readonly bool isInterruption;
        private readonly byte[] data;
        private readonly RequestInterruption interruption;

        public RawRequestMessage(RequestInterruption interruption)
        {
            this.isInterruption = true;
            this.interruption = interruption;
            if (!Enum.IsDefined(typeof(RequestInterruption), interruption))
                throw new ArgumentException("Invalid interrupt", nameof(interruption));
        }

        public RawRequestMessage(byte[] bytes)
        {
            this.isInterruption = false;
            this.data = bytes ?? throw new ArgumentNullException(nameof(bytes));
        }

        public void Match(Action<RequestInterruption> whenIsInterruption, Action<byte[]> whenIsData)
        {
            if (this.isInterruption)
            {
                whenIsInterruption?.Invoke(this.interruption);
            }
            else
            {
                whenIsData?.Invoke(this.data);
            }
        }
    }
}
