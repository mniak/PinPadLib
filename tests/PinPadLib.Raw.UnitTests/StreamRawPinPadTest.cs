using PinPadLib.Raw.UnitTests._Infra;
using PinPadLib.Raw.UnitTests._Infra.Stubs;
using PinPadLib.Utils;
using Shouldly;
using System;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PinPadLib.Raw.UnitTests
{
    public class StreamRawPinPadTest
    {
        private const int TimeoutMilliseconds = 2000;
        private const int ToleranceMilliseconds = 50;

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
        public void Send_WhenReceiveACK_ShouldStopSending()
        {
            using (var stream = new RWStream())
            using (var sut = new StreamRawPinPad(stream))
            {
                var payload = Encoding.ASCII.GetBytes("OPN000");
                var expectedBytes = new ByteArrayBuilder()
                    .Add(Bytes.SYN).Add(payload).Add(Bytes.ETB).Add(0x77, 0x5e)
                    .ToArray();

                Should.CompleteIn(async () =>
                {
                    var taskSend = sut.SendRawMessageAsync(new RawRequestMessage(payload));
                    await Task.Delay(100);
                    stream.PushByteToRead(Bytes.ACK);
                    await taskSend;
                }, TimeSpan.FromMilliseconds(100 + ToleranceMilliseconds));

                stream.GetBytesWritten().ShouldBe(expectedBytes);
            }
        }

        [Fact]
        public void Send_WhenDoesNotReceiveReplyAndTimeout_ShouldAddCANandAbort()
        {
            using (var stream = new RWStream())
            using (var sut = new StreamRawPinPad(stream))
            {
                var payload = Encoding.ASCII.GetBytes("OPN000");
                var expectedBytes = new ByteArrayBuilder()
                    .Add(Bytes.SYN).Add(payload).Add(Bytes.ETB).Add(0x77, 0x5e)
                    .Add(Bytes.CAN)
                    .ToArray();

                Should.CompleteIn(async () =>
                {
                    await sut.SendRawMessageAsync(new RawRequestMessage(payload));
                }, TimeSpan.FromMilliseconds(TimeoutMilliseconds + ToleranceMilliseconds));

                stream.GetBytesWritten().ShouldBe(expectedBytes);
            }
        }

        [Fact]
        public void Send_WhenReceiveNAK_1stTime_ShouldTryAgain()
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

                Should.CompleteIn(async () =>
                 {
                     var taskSend = sut.SendRawMessageAsync(new RawRequestMessage(payload));
                     await Task.Delay(100);
                     stream.PushByteToRead(Bytes.NAK);
                     await taskSend;
                 }, TimeSpan.FromMilliseconds(100 + TimeoutMilliseconds + ToleranceMilliseconds));

                stream.GetBytesWritten().ShouldBe(expectedBytes);
            }
        }

        [Fact]
        public void Send_WhenReceiveNAK_2ndTime_ShouldTryAgain()
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

                Should.CompleteIn(async () =>
                {
                    var taskSend = sut.SendRawMessageAsync(new RawRequestMessage(payload));
                    await Task.Delay(100);
                    stream.PushByteToRead(Bytes.NAK);
                    await Task.Delay(100);
                    stream.PushByteToRead(Bytes.NAK);
                    await taskSend;
                }, TimeSpan.FromMilliseconds(100 + 100 + TimeoutMilliseconds + ToleranceMilliseconds));

                stream.GetBytesWritten().ShouldBe(expectedBytes);
            }
        }

        [Fact]
        public void Send_WhenReceiveNAK_3rdTime_ShouldAddCANandAbort()
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

                Should.CompleteIn(async () =>
                {
                    var taskSend = sut.SendRawMessageAsync(new RawRequestMessage(payload));
                    await Task.Delay(100);
                    stream.PushByteToRead(Bytes.NAK);
                    await Task.Delay(100);
                    stream.PushByteToRead(Bytes.NAK);
                    await Task.Delay(100);
                    stream.PushByteToRead(Bytes.NAK);
                    await taskSend;
                }, TimeSpan.FromMilliseconds(100 + 100 + 100 + ToleranceMilliseconds));

                stream.GetBytesWritten().ShouldBe(expectedBytes);
            }
        }

        public void Receive
    }
}