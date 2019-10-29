using PinPadLib.Utils;
using System.IO.Pipelines;
using System.Threading.Tasks;

namespace System
{
    internal static class ByteArrayBuilderExtensions
    {
        public static async Task<Pipe> BuildPipeAsync(this ByteArrayBuilder bytes)
        {
            var pipe = new Pipe();
            await pipe.Writer.WriteAsync(bytes.ToArray());
            return pipe;
        }
    }
}
