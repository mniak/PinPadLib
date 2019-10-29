using System;
using System.IO;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;

namespace PinPadLib.Raw
{
    internal class StreamRawPinPad : IRawPinPad, IDisposable
    {
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
            this.taskWrite = WriteToPipeAsync(stream, this.pipe.Writer, this.writeCancellation.Token);
        }

        private async Task WriteToPipeAsync(Stream stream, PipeWriter writer, CancellationToken cancellationToken)
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

        public void Dispose()
        {
            this.writeCancellation.Cancel();
        }

        public Task<RawResponseMessage> ReceiveRawMessageAsync()
        {
            throw new System.NotImplementedException();
        }

        public async Task SendRawMessageAsync(RawRequestMessage rawMessage)
        {
            var bytes = rawMessage.RawBytesToSend();
            for (var i = 0; i < 3; i++)
            {
                await this.stream.WriteAsync(bytes, 0, bytes.Length);
                var isAck = await this.msgReader.ReadAckOrNakAsync();
            }
        }
    }
}
