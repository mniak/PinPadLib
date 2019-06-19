using System.Threading.Tasks;

namespace PinPadLib.LowLevel
{
    public interface ILowLevelPinPad
    {
        Task SendCommandAsync(LowLevelRequestMessage message);
        Task<LowLevelResponseMessage> ReceiveResponseAsync();
    }
}
