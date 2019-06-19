using PinPadLib.Raw.UnitTests._Infra;
using PinPadLib.Serial;
using Shouldly;
using System.IO.Pipelines;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PinPadLib.Raw.UnitTests
{
    public class PipeMessageReaderTest
    {
        [Fact]
        public async Task TestMessageOpn000()
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

            msg.Match(intr => false, data => true).ShouldBe(true, "Should return data");
            msg.Do(_ => { }, data =>
            {
                data.Length.ShouldBe(6);
                Encoding.ASCII.GetString(data).ShouldBe("OPN000");
            });
        }
    }
}
