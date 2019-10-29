using PinPadLib.Utils;
using System;
using System.IO;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;

namespace PinPadLib.Raw
{
    internal class StreamRawPinPad : IRawPinPad, IDisposable
    {
        private const int NumberOfAttempts = 3;

        private readonly Stream stream;
        private readonly Pipe pipe;
        private readonly PipeMessageReader msgReader;
        private readonly CancellationTokenSource writeCancellation;
        private readonly Task taskWrite;

        public StreamRawPinPad(Stream stream)
        {
            this.stream = stream ?? throw new ArgumentNullException(nameof(stream));
            this.pipe = new Pipe();
            this.msgReader = new PipeMessageReader(this.pipe.Reader);

            this.writeCancellation = new CancellationTokenSource();
            this.taskWrite = KeepFillingTheInputPipe(stream, this.pipe.Writer, this.writeCancellation.Token);
        }

        public void Dispose()
        {
            this.writeCancellation.Cancel();
        }

        private async Task KeepFillingTheInputPipe(Stream stream, PipeWriter writer, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var buffer = new byte[1024];
                var bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
                if (bytesRead > 0)
                {
                    await writer.WriteAsync(new Memory<byte>(buffer, 0, bytesRead), cancellationToken);
                }
            }
            writer.Complete();
        }
        public async Task<bool> SendRawMessageAsync(RawRequestMessage rawMessage)
        {
            var bytes = rawMessage.RawBytesToSend();
            for (var i = 0; i < NumberOfAttempts; i++)
            {
                await this.stream.WriteAsync(bytes, 0, bytes.Length);
                var @int = await this.msgReader.ReadAckOrNakAsync();
                switch (@int)
                {
                    case AcknowledgmentResponseInterruption.Acknowledgment:
                        return true;
                    case AcknowledgmentResponseInterruption.NegativeAcknowledgment:
                        continue;

                    default:
                    case AcknowledgmentResponseInterruption.Abort:
                        break;
                }
                break;
            }
            this.stream.Write(new[] { Bytes.CAN }, 0, 1);
            return false;
        }

        public Task<RawResponseMessage> ReceiveRawMessageAsync()
        {
            throw new NotImplementedException();
        }
    }
}
