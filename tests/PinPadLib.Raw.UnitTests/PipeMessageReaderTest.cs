using PinPadLib.Utils;
using Shouldly;
using System;
using System.Threading.Tasks;
using Xunit;

namespace PinPadLib.Raw.UnitTests
{
    public class PipeMessageReaderTest
    {
        [Theory]
        [InlineData("OPN000", 0x77, 0x5e)]
        [InlineData("AAAAAAAA", 0x9a, 0x63)]
        public async Task ReadMessageAsync_WhenReceiveWellFormattedMessage_ShouldReturnData(string payload, byte crcByte1, byte crcByte2)
        {
            var pipe = await new ByteArrayBuilder()
                .Add(Bytes.SYN)
                .Add(payload)
                .Add(Bytes.ETB)
                .Add(crcByte1, crcByte2)
                .BuildPipeAsync();

            var sut = new PipeMessageReader(pipe.Reader);
            var msg = await sut.ReadMessageAsync();

            var data = msg.ShouldBeData();
            data.Length.ShouldBe(payload.Length);
            data.ShouldBeInAscii(payload);
        }

        [Theory]
        [InlineData("OPN000", 0x77, 0x5e)]
        [InlineData("AAAAAAAA", 0x9a, 0x63)]
        public async Task ReadMessageAsync_WhenReceiveWellFormattedMessage_WithCANInTheBeginning_ShouldReturnData(string payload, byte crcByte1, byte crcByte2)
        {
            var pipe = await new ByteArrayBuilder()
                .Add(Bytes.CAN)
                .Add(Bytes.SYN)
                .Add(payload)
                .Add(Bytes.ETB)
                .Add(crcByte1, crcByte2)
                .BuildPipeAsync();

            var sut = new PipeMessageReader(pipe.Reader);
            var msg = await sut.ReadMessageAsync();

            var data = msg.ShouldBeData();
            data.Length.ShouldBe(payload.Length);
            data.ShouldBeInAscii(payload);
        }

        [Theory]
        [InlineData("OPN000", 0x77, 0x5e)]
        [InlineData("AAAAAAAA", 0x9a, 0x63)]
        public async Task ReadMessageAsync_WhenReceiveWellFormattedMessage_WithCANInTheMiddle_ShouldReturnData(string payload, byte crcByte1, byte crcByte2)
        {
            var pipe = await new ByteArrayBuilder()
                .Add(Bytes.SYN)
                .Add("ABCDEFG")
                .Add(Bytes.CAN)
                .Add(Bytes.SYN)
                .Add(payload)
                .Add(Bytes.ETB)
                .Add(crcByte1, crcByte2)
                .BuildPipeAsync();

            var sut = new PipeMessageReader(pipe.Reader);
            var msg = await sut.ReadMessageAsync();

            var data = msg.ShouldBeData();
            data.Length.ShouldBe(payload.Length);
            data.ShouldBeInAscii(payload);
        }

        [Fact]
        public async Task ReadMessageAsync_WhenReceiveWrongCRC_ShouldReturnInterruptionInvalidCrc()
        {
            var pipe = await new ByteArrayBuilder()
                .Add(Bytes.SYN)
                .Add("ABCDEFG")
                .Add(Bytes.ETB)
                .Add(0x11, 0x22)
                .BuildPipeAsync();

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
        public async Task ReadMessageAsync_WhenReceiveMessage_WithByteOutOfRange_ShouldReturnInterruptInvalidMessage(byte byteOutOfRange)
        {
            var pipe = await new ByteArrayBuilder()
                .Add(Bytes.SYN)
                .Add("ABCD")
                .Add(byteOutOfRange)
                .Add("EFGH")
                .Add(Bytes.ETB)
                .Add(0x11, 0x22)
                .BuildPipeAsync();

            var sut = new PipeMessageReader(pipe.Reader);
            var msg = await sut.ReadMessageAsync();

            var @int = msg.ShouldBeInterruption();
            @int.ShouldBe(ResponseInterruption.InvalidMessage);
        }

        [Fact]
        public async Task ReadMessageAsync_WhenReceiveMessage_WithLengthGreaterThan1024_ShouldReturnInterruptInvalidMessage()
        {
            var pipe = await new ByteArrayBuilder()
                .Add(Bytes.SYN)
                .Add(new string('a', 1025))
                .Add(Bytes.ETB)
                .Add(0x11, 0x22)
                .BuildPipeAsync();

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
        public async Task ReadMessageAsync_WhenReceiveMessage_WithLengthAtMost1024_ShouldReturnInterruptInvalidCrc(int length)
        {
            var pipe = await new ByteArrayBuilder()
                .Add(Bytes.SYN)
                .Add(new string('a', length))
                .Add(Bytes.ETB)
                .Add(0x11, 0x22)
                .BuildPipeAsync();

            var sut = new PipeMessageReader(pipe.Reader);
            var msg = await sut.ReadMessageAsync();

            var @int = msg.ShouldBeInterruption();
            @int.ShouldBe(ResponseInterruption.InvalidCrc);
        }

        [Theory]
        [InlineData(Bytes.ACK, AcknowledgmentResponseInterruption.Acknowledgment)]
        [InlineData(Bytes.NAK, AcknowledgmentResponseInterruption.NegativeAcknowledgment)]
        [InlineData('A', AcknowledgmentResponseInterruption.Abort)]
        [InlineData('1', AcknowledgmentResponseInterruption.Abort)]
        [InlineData(Bytes.ETB, AcknowledgmentResponseInterruption.Abort)]
        public async Task ReadAckOrNakAsync_WhenReceiveByte_ShouldReturnAccordingly(byte @byte, AcknowledgmentResponseInterruption expected)
        {
            var pipe = await new ByteArrayBuilder()
                .Add(@byte)
                .BuildPipeAsync();

            var sut = new PipeMessageReader(pipe.Reader);
            var @int = await sut.ReadAckOrNakAsync();

            @int.ShouldBe(expected);
        }

        [Fact]
        public async Task ReadAckOrNakAsync_WhenDoesNotReceiveByte_ShouldReturnAbort()
        {
            var pipe = await new ByteArrayBuilder().BuildPipeAsync();

            var sut = new PipeMessageReader(pipe.Reader);
            var @int = await sut.ReadAckOrNakAsync();

            @int.ShouldBe(AcknowledgmentResponseInterruption.Abort);
        }
    }
}
