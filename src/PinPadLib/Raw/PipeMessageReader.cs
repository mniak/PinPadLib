using PinPadLib.Utils;
using System;
using System.Buffers;
using System.IO.Pipelines;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PinPadLib.Raw
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
                    await ReadSynAsync();
                    var payload = await ReadPayloadAsync();
                    var crc = await ReadCrcAsync();
                    var crcIsValid = ValidateCrc(payload, crc);
                    if (crcIsValid)
                    {
                        return payload;
                    }
                    else
                    {
                        return ResponseInterruption.InvalidCrc;
                    }
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
        public async Task<AcknowledgmentResponseInterruption> ReadAckOrNakAsync()
        {
            try
            {
                var result = await this.reader.ReadAsync(new CancellationTokenSource(DelayMs).Token);
                ThrowIfHasCan(result);

                var b0 = result.Buffer.Slice(0, 1).ToArray()[0];
                this.reader.AdvanceTo(result.Buffer.GetPosition(1));
                switch (b0)
                {
                    case Bytes.ACK:
                        return AcknowledgmentResponseInterruption.Acknowledgment;
                    case Bytes.NAK:
                        return AcknowledgmentResponseInterruption.NegativeAcknowledgment;
                }
                return AcknowledgmentResponseInterruption.Abort;
            }
            catch (OperationCanceledException)
            {
                return AcknowledgmentResponseInterruption.Abort;
            }
        }

        private const byte ByteSyn = 0x16;
        private const byte ByteEtb = 0x17;
        private const byte ByteCan = 0x18;
        private const int MaxLength = 1024;

        private async Task ReadSynAsync()
        {
            var result = await this.reader.ReadAsync(new CancellationTokenSource(DelayMs).Token);
            ThrowIfHasCan(result);

            var b0 = result.Buffer.Slice(0, 1).ToArray()[0];
            this.reader.AdvanceTo(result.Buffer.GetPosition(1));
            if (b0 != ByteSyn)
                throw new InvalidMessageException();
        }
        private async Task<byte[]> ReadPayloadAsync()
        {
            ReadResult result;
            do
            {
                result = await this.reader.ReadAsync(new CancellationTokenSource(DelayMs).Token);
                ThrowIfHasCan(result);

                var etbPos = result.Buffer.PositionOf(ByteEtb);
                if (etbPos.HasValue)
                {
                    this.reader.AdvanceTo(result.Buffer.GetPosition(1, etbPos.Value));

                    var slice = result.Buffer.Slice(0, etbPos.Value);
                    if (slice.Length > MaxLength)
                        break;

                    var buffer = slice.ToArray();
                    if (buffer.Any(b => b < 0x20 || b > 0x7f))
                        break;

                    return buffer;
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
        private async Task<ushort> ReadCrcAsync()
        {
            ReadResult result;
            do
            {
                result = await this.reader.ReadAsync(new CancellationTokenSource(DelayMs).Token);
                ThrowIfHasCan(result);
                if (result.Buffer.Length >= 2)
                {
                    var slice = result.Buffer.Slice(0, 2);
                    var bytes = slice.ToArray();
                    this.reader.AdvanceTo(slice.End);
                    return (ushort)(bytes[0] * 256 + bytes[1]);
                }
                else
                {
                    this.reader.AdvanceTo(result.Buffer.Start, result.Buffer.End);
                }
            } while (!result.IsCanceled && !result.IsCompleted);
            throw new InvalidMessageException();
        }

        private bool ValidateCrc(byte[] payload, ushort crc)
        {
            var payloadWithEtb = payload.Concat(new byte[] { ByteEtb });
            var computedCrc = Crc16.Compute(payloadWithEtb);
            return computedCrc == crc;
        }

        private void ThrowIfHasCan(ReadResult result)
        {
            var canPos = result.Buffer.PositionOf(ByteCan);
            if (canPos.HasValue)
            {
                this.reader.AdvanceTo(result.Buffer.GetPosition(1, canPos.Value));
                throw new AbortedException();
            }
        }
    }
}
