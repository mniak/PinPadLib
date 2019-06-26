namespace PinPadLib.Raw
{
    public class RawResponseMessage : RawMessage<ResponseInterruption>
    {
        public RawResponseMessage(ResponseInterruption interruption) : base(interruption)
        {
        }
        public RawResponseMessage(byte[] bytes) : base(bytes)
        {
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
