using Yammer.Chat.Core.Test.Mocks;

namespace Yammer.Chat.Core.Test
{
    public class TestBase
    {
        public TestBase()
        {
            Analytics.Default = new ApplicationInsightsWrapperMock();
        }
    }
}
