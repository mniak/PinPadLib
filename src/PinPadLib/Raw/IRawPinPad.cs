using System.Threading.Tasks;

namespace PinPadLib.Raw
{
    public interface IRawPinPad
    {
        Task<bool> SendRawMessageAsync(RawRequestMessage rawMessage);
        Task<RawResponseMessage> ReceiveRawMessageAsync();
    }
}
