using PinPadLib.Raw.UnitTests._Infra;
using PinPadLib.Serial;
using System.IO.Pipelines;
using System.Threading.Tasks;
using Xunit;

namespace PinPadLib.Raw.UnitTests
{
    public class PipeMessageReaderTest
    {
        [Fact]
        public async Task PPGet()
        {
            var bytes = new ByteArrayBuilder();
            //bytes.Add(Ascii.ACK);
            bytes.Add(Bytes.SYN);
            bytes.Add("OPN000");
            bytes.Add(Bytes.ETB);
            bytes.Add(0x77, 0x5e);

            var pipe = new Pipe();
            await pipe.Writer.WriteAsync(bytes.ToArray());

            var sut = new PipeMessageReader(pipe.Reader);
            var msg = await sut.ReadMessageAsync();
        }
    }
}
