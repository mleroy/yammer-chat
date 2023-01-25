using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yammer.Chat.Core.Test.Mocks
{
    public class ApplicationInsightsWrapperMock : IApplicationInsightsWrapper
    {
        public void LogEvent(string eventName)
        {
        }

        public void LogEvent(string eventName, IDictionary<string, object> properties)
        {
        }

        public void LogPageView(string pagePath)
        {
        }

        public ITimedAnalyticsEvent StartTimedEvent(string eventName)
        {
            return new TimedAnalyticsEventMock();
        }

        public ITimedAnalyticsEvent StartTimedEvent(string eventName, IDictionary<string, object> properties)
        {
            return new TimedAnalyticsEventMock();
        }

        public string AppUserId
        {
            get;
            set;
        }
    }

    public class TimedAnalyticsEventMock : ITimedAnalyticsEvent
    {
        public TimedAnalyticsEventMock()
        {
            this.Name = "";
            this.Properties = new Dictionary<string, object>();
        }

        public string Name
        {
            get;
            set;
        }

        public IDictionary<string, object> Properties
        {
            get;
            set;
        }

        public void End()
        {
            
        }

        public void Cancel()
        {
        }

        public void Dispose()
        {
        }
    }

}
