using LanguageExt;
using PinPadLib.HighLevel.Errors;

namespace PinPadLib.HighLevel.Commands
{
    public struct PinPadResponse<T> : Either<PinPadError, T>
    {
    }
}
