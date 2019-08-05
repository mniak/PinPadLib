using AutoFixture.Xunit2;
using Moq;
using PinPadLib.LowLevel.UnitTests._Infra;
using PinPadLib.Raw;
using PinPadLib.Utils;
using System.Threading.Tasks;
using Xunit;

namespace PinPadLib.LowLevel.UnitTests
{
    public class LowLevelPinpadTest
    {
        [Theory]
        [AutoMoqData]
        public async Task WhenSendMessageWithOneEmptyParaqmeter_ShouldAdd000(
           [Frozen]Mock<IRawPinPad> rawPinPadMock,
           LowLevelPinPad sut)
        {
            await sut.SendCommandAsync(new LowLevelRequestMessage(CommandName.Open, string.Empty));

            var rawMsg = new ByteArrayBuilder()
                .Add(Bytes.SYN)
                .Add("OPN000")
                .Add(Bytes.ETB)
                .Add(0x77, 0x5e)
                .ToArray();
            rawPinPadMock.Verify(x => x.SendRawMessageAsync(It.IsAny<RawRequestMessage>()), Times.Once);
            rawPinPadMock.Verify(x => x.SendRawMessageAsync(It.Is<RawRequestMessage>(
                msg => msg.DataEquals(rawMsg)
            )), Times.Once);
        }

        [Theory]
        [AutoMoqData]
        public async Task WhenSendMessageWithoutParameters_ShouldAdd000(
            [Frozen]Mock<IRawPinPad> rawPinPadMock,
            LowLevelPinPad sut)
        {
            await sut.SendCommandAsync(new LowLevelRequestMessage(CommandName.Open));

            var rawMsg = new ByteArrayBuilder()
                .Add(Bytes.SYN)
                .Add("OPN000")
                .Add(Bytes.ETB)
                .Add(0x77, 0x5e)
                .ToArray();
            rawPinPadMock.Verify(x => x.SendRawMessageAsync(It.IsAny<RawRequestMessage>()), Times.Once);
            rawPinPadMock.Verify(x => x.SendRawMessageAsync(It.Is<RawRequestMessage>(
                msg => msg.DataEquals(rawMsg)
            )), Times.Once);
        }
    }
}
