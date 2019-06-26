namespace PinPadLib.Raw
{
    public class RawRequestMessage : RawMessage<RequestInterruption>
    {
        public RawRequestMessage(RequestInterruption interruption) : base(interruption)
        {
        }
        public RawRequestMessage(byte[] bytes) : base(bytes)
        {
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
