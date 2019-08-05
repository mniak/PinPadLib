//using PinPadLib.Raw;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.IO.Pipelines;
//using System.Linq;
//using System.Threading.Tasks;

//namespace PinPadLib.Serial
//{
//    internal class PipeStreamPinPad : IRawPinPad
//    {
//        private readonly PipeReader inputReader;
//        private readonly Stream outputStream;
//        private readonly Queue<ResponseInterruption> interruptionQueue = new Queue<ResponseInterruption>();

//        public PipeStreamPinPad(PipeReader inputReader, Stream outputStream)
//        {
//            this.inputReader = inputReader ?? throw new System.ArgumentNullException(nameof(inputReader));
//            this.outputStream = outputStream ?? throw new System.ArgumentNullException(nameof(outputStream));
//        }

//        public Task<RawResponseMessage> ReceiveRawMessageAsync()
//        {
//            if (this.interruptionQueue.Any())
//            {
//                var interruption = this.interruptionQueue.Dequeue();
//                return Task.FromResult(new RawResponseMessage(interruption));
//            }
//            return ReadMessageFromPipeReader();
//        }

//        private async Task<RawResponseMessage> ReadMessageFromPipeReader()
//        {
//            byte b = await ReadByteAsync();
//        }

//        public Task SendRawMessageAsync(RawRequestMessage rawMessage)
//        {
//            var bytes = rawMessage.Match(
//                intr => FormatInterruption(intr),
//                data => FormatData(data));

//            return Task.Run(() =>
//            {
//                try
//                {
//                    this.outputStream.Write(bytes, 0, bytes.Length);
//                }
//                catch (Exception)
//                {
//                    this.interruptionQueue.Enqueue(ResponseInterruption.CommunicationError);
//                }
//            });
//        }

//        private byte[] FormatInterruption(RequestInterruption interruption)
//        {
//            switch (interruption)
//            {
//                case RequestInterruption.Abort:
//                    return new byte[] { 0x18 };
//                default:
//                    return Array.Empty<byte>();
//            }
//        }

//        private byte[] FormatData(byte[] data)
//        {
//            var tmp = new List<byte>();
//            tmp.Add(0x16);
//            tmp.AddRange(data);
//            tmp.Add(0x17);

//            var crcBytes = CalcCrc16(tmp);
//            tmp.AddRange(crcBytes);

//            return tmp.ToArray();
//        }
//    }
//}
