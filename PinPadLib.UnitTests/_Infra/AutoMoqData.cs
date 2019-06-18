using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Xunit2;

namespace PinPadLib.UnitTests
{
    public class AutoMoqDataAttribute : AutoDataAttribute
    {
        public AutoMoqDataAttribute() : base(FixtureFactory) { }

        private static IFixture FixtureFactory()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            return fixture;
        }
    }
}
