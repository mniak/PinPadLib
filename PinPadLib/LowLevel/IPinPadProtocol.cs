using System.Threading.Tasks;

namespace PinPadLib.LowLevel
{
    internal interface IPinPadProtocol
    {
        Task SendCommandAsync(string v1, string v2);
    }
}
