using System;
using System.Linq;

namespace PinPadLib.Raw
{
    public abstract class RawMessage<TInterruption> where TInterruption : Enum
    {
        private readonly bool isInterruption;
        private readonly byte[] data;
        private readonly TInterruption interruption;

        public RawMessage(TInterruption interruption)
        {
            this.isInterruption = true;
            this.interruption = interruption;
            if (!Enum.IsDefined(typeof(TInterruption), interruption))
                throw new ArgumentException("Invalid interrupt", nameof(interruption));
        }

        public RawMessage(byte[] bytes)
        {
            this.isInterruption = false;
            this.data = bytes ?? throw new ArgumentNullException(nameof(bytes));
        }

        public T Match<T>(Func<TInterruption, T> whenIsInterruption, Func<byte[], T> whenIsData)
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
        public void Do<T>(Action<TInterruption> whenIsInterruption, Action<byte[]> whenIsData)
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

        public bool DataEquals(byte[] data)
        {
            return Match(i => false, d => d.SequenceEqual(data));
        }

        public bool InterruptionEquals(TInterruption interruption)
        {
            return Match(i => ReferenceEquals(i, interruption), d => false);
        }
    }
}
