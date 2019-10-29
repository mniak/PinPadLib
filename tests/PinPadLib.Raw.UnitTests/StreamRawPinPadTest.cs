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
            using (var inputStream = new DisposableStream())
            using (var outputStream = new DisposableStream())
            {
                StreamRawPinPad pinpad;
                using (pinpad = new StreamRawPinPad(inputStream, outputStream))
                {
                    pinpad.pipeFillCancellation.IsCancellationRequested.ShouldBeFalse();
                }
                inputStream.WasDisposed.ShouldBeFalse();
                outputStream.WasDisposed.ShouldBeFalse();
                pinpad.pipeFillCancellation.IsCancellationRequested.ShouldBeTrue();
            }
        }

        [Fact]
        public void Send_WhenReceiveACK_ShouldStopSending()
        {
            using (var inputStream = new RWStream(RWStream.Mode.Read))
            using (var outputStream = new RWStream(RWStream.Mode.Write))
            using (var sut = new StreamRawPinPad(inputStream, outputStream))
            {
                var payload = Encoding.ASCII.GetBytes("OPN000");
                var expectedBytes = new ByteArrayBuilder()
                    .Add(Bytes.SYN).Add(payload).Add(Bytes.ETB).Add(0x77, 0x5e)
                    .ToArray();

                Should.CompleteIn(async () =>
                {
                    var taskSend = sut.SendRawMessageAsync(new RawRequestMessage(payload));
                    await Task.Delay(100);
                    inputStream.PushByteToRead(Bytes.ACK);
                    await taskSend;
                }, TimeSpan.FromMilliseconds(100 + ToleranceMilliseconds));

                outputStream.GetBytesWritten().ShouldBe(expectedBytes);
            }
        }

        [Fact]
        public void Send_WhenDoesNotReceiveReplyAndTimeout_ShouldAddCANandAbort()
        {
            using (var inputStream = new RWStream(RWStream.Mode.Read))
            using (var outputStream = new RWStream(RWStream.Mode.Write))
            using (var sut = new StreamRawPinPad(inputStream, outputStream))
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

                outputStream.GetBytesWritten().ShouldBe(expectedBytes);
            }
        }

        [Fact]
        public void Send_WhenReceiveNAK_1stTime_ShouldTryAgain()
        {
            using (var inputStream = new RWStream(RWStream.Mode.Read))
            using (var outputStream = new RWStream(RWStream.Mode.Write))
            using (var sut = new StreamRawPinPad(inputStream, outputStream))
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
                     inputStream.PushByteToRead(Bytes.NAK);
                     await taskSend;
                 }, TimeSpan.FromMilliseconds(100 + TimeoutMilliseconds + ToleranceMilliseconds));

                outputStream.GetBytesWritten().ShouldBe(expectedBytes);
            }
        }

        [Fact]
        public void Send_WhenReceiveNAK_2ndTime_ShouldTryAgain()
        {
            using (var inputStream = new RWStream(RWStream.Mode.Read))
            using (var outputStream = new RWStream(RWStream.Mode.Write))
            using (var sut = new StreamRawPinPad(inputStream, outputStream))
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
                    inputStream.PushByteToRead(Bytes.NAK);
                    await Task.Delay(100);
                    inputStream.PushByteToRead(Bytes.NAK);
                    await taskSend;
                }, TimeSpan.FromMilliseconds(100 + 100 + TimeoutMilliseconds + ToleranceMilliseconds));

                outputStream.GetBytesWritten().ShouldBe(expectedBytes);
            }
        }

        [Fact]
        public void Send_WhenReceiveNAK_3rdTime_ShouldAddCANandAbort()
        {
            using (var inputStream = new RWStream(RWStream.Mode.Read))
            using (var outputStream = new RWStream(RWStream.Mode.Write))
            using (var sut = new StreamRawPinPad(inputStream, outputStream))
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
                    inputStream.PushByteToRead(Bytes.NAK);
                    await Task.Delay(100);
                    inputStream.PushByteToRead(Bytes.NAK);
                    await Task.Delay(100);
                    inputStream.PushByteToRead(Bytes.NAK);
                    await taskSend;
                }, TimeSpan.FromMilliseconds(100 + 100 + 100 + ToleranceMilliseconds));

                outputStream.GetBytesWritten().ShouldBe(expectedBytes);
            }
        }
    }
}