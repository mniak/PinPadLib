using System;

namespace PinPadLib.Raw
{
    [Serializable]
    internal class InvalidMessageException : Exception
    {
        public InvalidMessageException() { }
        public InvalidMessageException(string message) : base(message) { }
        public InvalidMessageException(string message, Exception inner) : base(message, inner) { }
        protected InvalidMessageException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
