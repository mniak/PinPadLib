using System;

namespace PinPadLib.Raw
{
    public class RawResponseMessage
    {
        private readonly bool isInterruption;
        private readonly byte[] data;
        private readonly ResponseInterruption interruption;

        public RawResponseMessage(ResponseInterruption interruption)
        {
            this.isInterruption = true;
            this.interruption = interruption;
            if (!Enum.IsDefined(typeof(ResponseInterruption), interruption))
                throw new ArgumentException("Invalid interrupt", nameof(interruption));
        }

        public RawResponseMessage(byte[] bytes)
        {
            this.isInterruption = false;
            this.data = bytes ?? throw new ArgumentNullException(nameof(bytes));
        }

        public T Match<T>(Func<ResponseInterruption, T> whenIsInterruption, Func<byte[], T> whenIsData)
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
        public void Do(Action<ResponseInterruption> whenIsInterruption, Action<byte[]> whenIsData)
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

        public static implicit operator RawResponseMessage(ResponseInterruption interruption)
        {
            return new RawResponseMessage(interruption);
        }
        public static implicit operator RawResponseMessage(byte[] data)
        {
            return new RawResponseMessage(data);
        }
    }
}
