using LanguageExt;
using PinPadLib.HighLevel.Commands.GetInfo;
using PinPadLib.HighLevel.Errors;
using System;
using System.Threading.Tasks;

namespace PinPadLib.HighLevel
{
    internal class PinPadOverProtocol
    {
        internal Task<Either<GeneralInfo, PinPadError>> GetGeneralInfoAsync()
        {
            throw new NotImplementedException();
        }
    }
}
