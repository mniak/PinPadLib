//using AutoFixture.Xunit2;
//using Bogus;
//using Moq;
//using PinPadLib.HighLevel;
//using PinPadLib.LowLevel;
//using System.Text;
//using System.Threading.Tasks;
//using Xunit;

//namespace PinPadLib.HighLevel.UnitTests.HighLevel
//{
//    public class PinPadOverProtocolTest
//    {
//        private readonly Faker faker;

//        public PinPadOverProtocolTest()
//        {
//            this.faker = new Faker();
//        }
//        [Theory, AutoMoqData]
//        public async Task GetInfoGeneralInfo(
//            [Frozen] Mock<IPinPadProtocol> protocol,
//            PinPadOverProtocol sut)
//        {
//            var manufacturer = this.faker.Random.AlphaNumeric(20);
//            var modelAndMemory = this.faker.Random.AlphaNumeric(19);
//            var supportsChip = this.faker.PickRandom("C", " ");
//            var firmwareVersion = this.faker.Random.AlphaNumeric(20);
//            var specVersionMajor = this.faker.Random.AlphaNumeric(4);
//            var appVersion = this.faker.Random.AlphaNumeric(16);
//            var serialNumber = this.faker.Random.AlphaNumeric(20);

//            protocol.Setup(x => x.SendCommandAsync("GIN", "00"))
//                .ReturnsAsync(new StringBuilder()
//                    .Append(manufacturer)
//                    .Append(modelAndMemory)
//                    .Append(supportsChip)
//                    .Append(firmwareVersion)
//                    .Append(specVersionMajor)
//                    .Append(appVersion)
//                    .Append(serialNumber)
//                    .ToString());

//            var result = await sut.GetGeneralInfoAsync();
//            result.ShouldBeOfType<GeneralInfo>();
//        }
//    }
//}
