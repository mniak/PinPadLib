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
        [Theory]
        [InlineData("OPN000", 0x77, 0x5e)]
        [InlineData("AAAAAAAA", 0x9a, 0x63)]
        public async Task WhenReceiveWellFormattedMessage_ShouldReturnData(string payload, byte crcByte1, byte crcByte2)
        {
            var bytes = new ByteArrayBuilder();
            bytes.Add(Bytes.SYN);
            bytes.Add(payload);
            bytes.Add(Bytes.ETB);
            bytes.Add(crcByte1, crcByte2);

            var pipe = new Pipe();
            await pipe.Writer.WriteAsync(bytes.ToArray());

            var sut = new PipeMessageReader(pipe.Reader);
            var msg = await sut.ReadMessageAsync();

            var data = msg.ShouldBeData();
            data.Length.ShouldBe(payload.Length);
            data.ShouldBeInAscii(payload);
        }

        [Theory]
        [InlineData("OPN000", 0x77, 0x5e)]
        [InlineData("AAAAAAAA", 0x9a, 0x63)]
        public async Task WhenReceiveWellFormattedMessage_WithCANInTheBeginning_ShouldReturnData(string payload, byte crcByte1, byte crcByte2)
        {
            var bytes = new ByteArrayBuilder();
            bytes.Add(Bytes.CAN);
            bytes.Add(Bytes.SYN);
            bytes.Add(payload);
            bytes.Add(Bytes.ETB);
            bytes.Add(crcByte1, crcByte2);

            var pipe = new Pipe();
            await pipe.Writer.WriteAsync(bytes.ToArray());

            var sut = new PipeMessageReader(pipe.Reader);
            var msg = await sut.ReadMessageAsync();

            var data = msg.ShouldBeData();
            data.Length.ShouldBe(payload.Length);
            data.ShouldBeInAscii(payload);
        }

        [Theory]
        [InlineData("OPN000", 0x77, 0x5e)]
        [InlineData("AAAAAAAA", 0x9a, 0x63)]
        public async Task WhenReceiveWellFormattedMessage_WithCANInTheMiddle_ShouldReturnData(string payload, byte crcByte1, byte crcByte2)
        {
            var bytes = new ByteArrayBuilder();
            bytes.Add(Bytes.SYN);
            bytes.Add("ABCDEFG");
            bytes.Add(Bytes.CAN);
            bytes.Add(Bytes.SYN);
            bytes.Add(payload);
            bytes.Add(Bytes.ETB);
            bytes.Add(crcByte1, crcByte2);

            var pipe = new Pipe();
            await pipe.Writer.WriteAsync(bytes.ToArray());

            var sut = new PipeMessageReader(pipe.Reader);
            var msg = await sut.ReadMessageAsync();

            var data = msg.ShouldBeData();
            data.Length.ShouldBe(payload.Length);
            data.ShouldBeInAscii(payload);
        }

        [Fact]
        public async Task WhenReceiveWrongCRC_ShouldReturnInterruptionInvalidCrc()
        {
            var bytes = new ByteArrayBuilder();
            bytes.Add(Bytes.SYN);
            bytes.Add("ABCDEFG");
            bytes.Add(Bytes.ETB);
            bytes.Add(0x11, 0x22);

            var pipe = new Pipe();
            await pipe.Writer.WriteAsync(bytes.ToArray());

            var sut = new PipeMessageReader(pipe.Reader);
            var msg = await sut.ReadMessageAsync();

            var @int = msg.ShouldBeInterruption();
            @int.ShouldBe(ResponseInterruption.InvalidCrc);
        }

        [Theory]
        [InlineData(0x00)]
        [InlineData(0x11)]
        [InlineData(0x19)]
        [InlineData(0x90)]
        [InlineData(0xA0)]
        [InlineData(0xF0)]
        public async Task WhenReceiveMessage_WithByteOutOfRange_ShouldReturnInterruptInvalidMessage(byte byteOutOfRange)
        {
            var bytes = new ByteArrayBuilder();
            bytes.Add(Bytes.SYN);
            bytes.Add("ABCD");
            bytes.Add(byteOutOfRange);
            bytes.Add("EFGH");
            bytes.Add(Bytes.ETB);
            bytes.Add(0x11, 0x22);

            var pipe = new Pipe();
            await pipe.Writer.WriteAsync(bytes.ToArray());

            var sut = new PipeMessageReader(pipe.Reader);
            var msg = await sut.ReadMessageAsync();

            var @int = msg.ShouldBeInterruption();
            @int.ShouldBe(ResponseInterruption.InvalidMessage);
        }

        [Fact]
        public async Task WhenReceiveMessage_WithLengthGreaterThan1024_ShouldReturnInterruptInvalidMessage()
        {
            var bytes = new ByteArrayBuilder();
            bytes.Add(Bytes.SYN);
            bytes.Add(new string('a', 1025));
            bytes.Add(Bytes.ETB);
            bytes.Add(0x11, 0x22);
            var pipe = new Pipe();
            await pipe.Writer.WriteAsync(bytes.ToArray());

            var sut = new PipeMessageReader(pipe.Reader);
            var msg = await sut.ReadMessageAsync();

            var @int = msg.ShouldBeInterruption();
            @int.ShouldBe(ResponseInterruption.InvalidMessage);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(102)]
        [InlineData(1024)]
        public async Task WhenReceiveMessage_WithLengthAtMost1024_ShouldReturnInterruptInvalidCrcNotInvalidMessage(int length)
        {
            var bytes = new ByteArrayBuilder();
            bytes.Add(Bytes.SYN);
            bytes.Add(new string('a', length));
            bytes.Add(Bytes.ETB);
            bytes.Add(0x11, 0x22);
            var pipe = new Pipe();
            await pipe.Writer.WriteAsync(bytes.ToArray());

            var sut = new PipeMessageReader(pipe.Reader);
            var msg = await sut.ReadMessageAsync();

            var @int = msg.ShouldBeInterruption();
            @int.ShouldBe(ResponseInterruption.InvalidCrc);
        }
    }
}
