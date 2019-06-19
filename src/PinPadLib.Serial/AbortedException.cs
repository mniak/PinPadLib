using System;

namespace PinPadLib.Serial
{
    [Serializable]
    internal class AbortedException : Exception
    {
        public AbortedException() { }
        public AbortedException(string message) : base(message) { }
        public AbortedException(string message, Exception inner) : base(message, inner) { }
        protected AbortedException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
