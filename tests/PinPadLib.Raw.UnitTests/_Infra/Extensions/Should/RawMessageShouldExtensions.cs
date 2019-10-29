using PinPadLib.Raw;

namespace Shouldly
{
    public static class RawMessageShouldExtensions
    {
        public static ResponseInterruption ShouldBeInterruption(this RawResponseMessage actual, string customMessage = null)
        {
            actual.Match(i => true, d => false).ShouldBeTrue(customMessage ?? "The raw response message should be an interruption");
            return actual.Match(i => i, d => default);
        }
        public static byte[] ShouldBeData(this RawResponseMessage actual, string customMessage = null)
        {
            actual.Match(i => false, d => true).ShouldBeTrue(customMessage ?? "The raw response message should be data");
            return actual.Match(i => default, d => d);
        }
    }
}
