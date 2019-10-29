using System.Text;

namespace Shouldly
{
    public static class BytesShouldExtensions
    {
        public static void ShouldBeInAscii(this byte[] actual, string expected, string customMessage = null)
        {
            Encoding.ASCII.GetString(actual).ShouldBe(expected, customMessage ?? $"The value in ascii should be '{expected}'");
        }
    }
}
