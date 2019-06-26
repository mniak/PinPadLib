using AutoFixture.Xunit2;
using Moq;
using PinPadLib.LowLevel.UnitTests._Infra;
using PinPadLib.Raw;
using Xunit;

namespace PinPadLib.LowLevel.UnitTests
{
    public class LowLevelPinpadTest
    {
        [Theory]
        [AutoMoqData]
        public void WhenSendMessageWithoutParameters_ShouldAdd000(
            [Frozen]Mock<IRawPinPad> rawPinPadMock,
            LowLevelPinPad sut)
        {
            rawPinPadMock.Setup(x => x.SendRawMessageAsync(It.IsAny<RawRequestMessage>()));

            sut.SendCommandAsync(new LowLevelRequestMessage(CommandName.Open));

            var rawMsg = new ByteArrayBuilder()
                .Add(Bytes.SYN)
                .Add("OPN000")
                .Add(Bytes.ETB)
                .Add(0x77, 0x5e)
                .ToArray();
            rawPinPadMock.Verify(x => x.SendRawMessageAsync(It.Is<RawRequestMessage>(msg => msg.Equals(rawMsg))), Times.Once);
        }
    }
}
