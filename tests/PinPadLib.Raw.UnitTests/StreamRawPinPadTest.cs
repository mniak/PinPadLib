using PinPadLib.Raw.UnitTests._Infra;
using PinPadLib.Raw.UnitTests._Infra.Stubs;
using PinPadLib.Utils;
using Shouldly;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PinPadLib.Raw.UnitTests
{
    public class StreamRawPinPadTest
    {
        [Fact]
        public void Dispose_ShouldNotDisposeStream()
        {
            using (var stream = new DisposableStream())
            {
                using (new StreamRawPinPad(stream)) { }
                stream.WasDisposed.ShouldBeFalse();
            }
        }

        [Fact]
        public async Task Send_WhenReceiveACK_ShouldStopSending()
        {
            using (var stream = new RWStream())
            using (var sut = new StreamRawPinPad(stream))
            {
                var payload = Encoding.ASCII.GetBytes("OPN000");
                var expectedBytes = new ByteArrayBuilder()
                    .Add(Bytes.SYN)
                    .Add(payload)
                    .Add(Bytes.ETB)
                    .Add(0x77, 0x5e)
                    .ToArray();

                var taskSend = sut.SendRawMessageAsync(new RawRequestMessage(payload));
                await Task.Delay(100);
                stream.PushByteToRead(Bytes.ACK);
                await taskSend;

                stream.GetBytesWritten().ShouldBe(expectedBytes);
            }
        }

        [Fact]
        public async Task Send_WhenReceiveNAKForTheFirstTime_ShouldTryAgain()
        {
            using (var stream = new RWStream())
            using (var sut = new StreamRawPinPad(stream))
            {
                var payload = Encoding.ASCII.GetBytes("OPN000");
                var expectedBytes = new ByteArrayBuilder()
                    .Add(Bytes.SYN).Add(payload).Add(Bytes.ETB).Add(0x77, 0x5e)
                    .Add(Bytes.SYN).Add(payload).Add(Bytes.ETB).Add(0x77, 0x5e)
                    .Add(Bytes.CAN)
                    .ToArray();

                var taskSend = sut.SendRawMessageAsync(new RawRequestMessage(payload));
                await Task.Delay(100);
                stream.PushByteToRead(Bytes.NAK);
                await taskSend;

                stream.GetBytesWritten().ShouldBe(expectedBytes);
            }
        }

        [Fact]
        public async Task Send_WhenReceiveNAKForTheSecondTime_ShouldTryAgain()
        {
            using (var stream = new RWStream())
            using (var sut = new StreamRawPinPad(stream))
            {
                var payload = Encoding.ASCII.GetBytes("OPN000");
                var expectedBytes = new ByteArrayBuilder()
                    .Add(Bytes.SYN).Add(payload).Add(Bytes.ETB).Add(0x77, 0x5e)
                    .Add(Bytes.SYN).Add(payload).Add(Bytes.ETB).Add(0x77, 0x5e)
                    .Add(Bytes.SYN).Add(payload).Add(Bytes.ETB).Add(0x77, 0x5e)
                    .Add(Bytes.CAN)
                    .ToArray();

                var taskSend = sut.SendRawMessageAsync(new RawRequestMessage(payload));
                await Task.Delay(100);
                stream.PushByteToRead(Bytes.NAK);
                await Task.Delay(100);
                stream.PushByteToRead(Bytes.NAK);
                await taskSend;

                stream.GetBytesWritten().ShouldBe(expectedBytes);
            }
        }

        [Fact]
        public async Task Send_WhenReceiveNAKForTheThirdTime_ShouldAddCanAndAbort()
        {
            using (var stream = new RWStream())
            using (var sut = new StreamRawPinPad(stream))
            {
                var payload = Encoding.ASCII.GetBytes("OPN000");
                var expectedBytes = new ByteArrayBuilder()
                    .Add(Bytes.SYN).Add(payload).Add(Bytes.ETB).Add(0x77, 0x5e)
                    .Add(Bytes.SYN).Add(payload).Add(Bytes.ETB).Add(0x77, 0x5e)
                    .Add(Bytes.SYN).Add(payload).Add(Bytes.ETB).Add(0x77, 0x5e)
                    .Add(Bytes.CAN)
                    .ToArray();

                var taskSend = sut.SendRawMessageAsync(new RawRequestMessage(payload));
                await Task.Delay(100);
                stream.PushByteToRead(Bytes.NAK);
                await Task.Delay(100);
                stream.PushByteToRead(Bytes.NAK);
                await Task.Delay(100);
                stream.PushByteToRead(Bytes.NAK);
                await Task.Delay(2100);
                await taskSend;

                stream.GetBytesWritten().ShouldBe(expectedBytes);
            }
        }

        [Fact]
        public async Task Send_WhenDoesNotReceiveReplyAndTimeout_ShouldAddCanAndAbort()
        {
            using (var stream = new RWStream())
            using (var sut = new StreamRawPinPad(stream))
            {
                var payload = Encoding.ASCII.GetBytes("OPN000");
                var expectedBytes = new ByteArrayBuilder()
                    .Add(Bytes.SYN)
                    .Add(payload)
                    .Add(Bytes.ETB)
                    .Add(0x77, 0x5e)
                    .Add(Bytes.CAN)
                    .ToArray();

                var taskSend = sut.SendRawMessageAsync(new RawRequestMessage(payload));
                await Task.Delay(2100);
                await taskSend;

                stream.GetBytesWritten().ShouldBe(expectedBytes);
            }
        }
    }
}