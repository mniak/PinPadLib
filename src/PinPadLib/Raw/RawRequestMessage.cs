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

        public T Match<T>(Func<RequestInterruption, T> whenIsInterruption, Func<byte[], T> whenIsData)
        {
            if (whenIsInterruption == null)
                throw new ArgumentNullException(nameof(whenIsInterruption));
            if (whenIsData == null)
                throw new ArgumentNullException(nameof(whenIsData));

            if (this.isInterruption)
            {
                return whenIsInterruption(this.interruption);
            }
            else
            {
                return whenIsData(this.data);
            }
        }
        public void Do<T>(Action<RequestInterruption> whenIsInterruption, Action<byte[]> whenIsData)
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

        public static implicit operator RawRequestMessage(RequestInterruption interruption)
        {
            return new RawRequestMessage(interruption);
        }
        public static implicit operator RawRequestMessage(byte[] data)
        {
            return new RawRequestMessage(data);
        }
    }
}
