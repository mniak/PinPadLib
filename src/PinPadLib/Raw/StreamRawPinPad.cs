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

        private readonly Stream outputStream;

        private readonly Pipe inputPipe;
        private readonly PipeMessageReader msgReader;
        internal readonly CancellationTokenSource pipeFillCancellation;

        public StreamRawPinPad(Stream inputStream, Stream outputStream)
        {
            if (inputStream is null)
                throw new ArgumentNullException(nameof(inputStream));
            this.outputStream = outputStream ?? throw new ArgumentNullException(nameof(outputStream));

            this.inputPipe = new Pipe();
            this.msgReader = new PipeMessageReader(this.inputPipe.Reader);

            this.pipeFillCancellation = new CancellationTokenSource();
            _ = KeepFillingTheInputPipe(inputStream, this.inputPipe.Writer, this.pipeFillCancellation.Token);
        }

        public void Dispose()
        {
            this.pipeFillCancellation.Cancel();
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
                await this.outputStream.WriteAsync(bytes, 0, bytes.Length);
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
            this.outputStream.Write(new[] { Bytes.CAN }, 0, 1);
            return false;
        }

        public Task<RawResponseMessage> ReceiveRawMessageAsync()
        {
            throw new NotImplementedException();
        }
    }
}
