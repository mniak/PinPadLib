using System.Threading.Tasks;

namespace PinPadLib.Raw
{
    public interface IRawPinPad
    {
        Task SendRawMessageAsync(RawRequestMessage rawMessage);
        Task<RawResponseMessage> ReceiveRawMessageAsync();
    }
}
