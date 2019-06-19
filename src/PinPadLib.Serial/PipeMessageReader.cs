using PinPadLib.Raw;
using System;
using System.Buffers;
using System.IO.Pipelines;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PinPadLib.Serial
{
    internal class PipeMessageReader
    {
        private const int DelayMs = 2000;
        private readonly PipeReader reader;
        public PipeMessageReader(PipeReader pipeReader)
        {
            this.reader = pipeReader ?? throw new ArgumentNullException(nameof(pipeReader));
        }

        public async Task<RawResponseMessage> ReadMessageAsync()
        {
            while (true)
            {
                try
                {
                    await ReadSyn();
                    var payload = ReadPayload();
                }
                catch (InvalidMessageException)
                {
                    break;
                }
                catch (AbortedException)
                {
                    continue;
                }
            }
            return ResponseInterruption.InvalidMessage;
        }

        private const byte ByteSyn = 0x16;
        private const byte ByteEtb = 0x17;
        private const byte ByteCan = 0x18;
        private const int MaxLength = 1024;
        private async Task ReadSyn()
        {
            var result = await this.reader.ReadAsync(new CancellationTokenSource(DelayMs).Token);
            ThrowIfHasCan(result);

            var b0 = result.Buffer.Slice(0, 1).ToArray()[0];
            this.reader.AdvanceTo(result.Buffer.GetPosition(1));
            if (b0 != ByteSyn)
                throw new InvalidMessageException();
        }

        private async Task<byte[]> ReadPayload()
        {
            ReadResult result;
            do
            {
                result = await this.reader.ReadAsync(new CancellationTokenSource(DelayMs).Token);
                ThrowIfHasCan(result);

                var etbPos = result.Buffer.PositionOf(ByteEtb);
                if (etbPos.HasValue)
                {
                    var slice = result.Buffer.Slice(0, etbPos.Value);
                    if (slice.Length > MaxLength)
                        break;

                    var buffer = slice.ToArray();
                    if (buffer.Any(b => b < 0x20 || b > 0x7f))
                        break;


                }
                else
                {
                    if (result.Buffer.Length > MaxLength)
                        break;
                    this.reader.AdvanceTo(result.Buffer.Start, result.Buffer.End);
                }
            } while (!result.IsCanceled && !result.IsCompleted);
            throw new InvalidMessageException();
        }

        private void ThrowIfHasCan(ReadResult result)
        {
            var canPos = result.Buffer.PositionOf(ByteCan);
            if (canPos.HasValue)
            {
                this.reader.AdvanceTo(canPos.Value);
                throw new AbortedException();
            }
        }
    }
}
