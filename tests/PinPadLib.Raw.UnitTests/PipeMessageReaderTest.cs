using PinPadLib.Raw.UnitTests._Infra;
using PinPadLib.Serial;
using Shouldly;
using System.IO.Pipelines;
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
            bytes.Add(Bytes.SYN);
            bytes.Add("OPN000");
            bytes.Add(Bytes.ETB);
            bytes.Add(0x77, 0x5e);

            var pipe = new Pipe();
            await pipe.Writer.WriteAsync(bytes.ToArray());

            var sut = new PipeMessageReader(pipe.Reader);
            var msg = await sut.ReadMessageAsync();

            var data = msg.ShouldBeData();
            data.Length.ShouldBe(6);
            data.ShouldBeInAscii("OPN000");
        }

        [Fact]
        public async Task TestMessageOpn000_WithCanInTheMiddle()
        {
            var bytes = new ByteArrayBuilder();
            bytes.Add(Bytes.SYN);
            bytes.Add("ABCDEFG");
            bytes.Add(Bytes.CAN);
            bytes.Add(Bytes.SYN);
            bytes.Add("OPN000");
            bytes.Add(Bytes.ETB);
            bytes.Add(0x77, 0x5e);

            var pipe = new Pipe();
            await pipe.Writer.WriteAsync(bytes.ToArray());

            var sut = new PipeMessageReader(pipe.Reader);
            var msg = await sut.ReadMessageAsync();

            var data = msg.ShouldBeData();
            data.Length.ShouldBe(6);
            data.ShouldBeInAscii("OPN000");
        }


        [Fact]
        public async Task TestMessageOpn000_WithWrongCrc()
        {
            var bytes = new ByteArrayBuilder();
            bytes.Add(Bytes.SYN);
            bytes.Add("OPN000");
            bytes.Add(Bytes.ETB);
            bytes.Add(0x88, 0xee);

            var pipe = new Pipe();
            await pipe.Writer.WriteAsync(bytes.ToArray());

            var sut = new PipeMessageReader(pipe.Reader);
            var msg = await sut.ReadMessageAsync();

            var @int = msg.ShouldBeInterruption();
            @int.ShouldBe(ResponseInterruption.InvalidCrc);
        }
    }
}
