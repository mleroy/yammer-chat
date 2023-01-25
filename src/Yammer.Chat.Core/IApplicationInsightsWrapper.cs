using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yammer.Chat.Core
{
    public interface IApplicationInsightsWrapper
    {
        void LogEvent(string eventName);
        void LogEvent(string eventName, IDictionary<string, object> properties);
        void LogPageView(string pagePath);
        ITimedAnalyticsEvent StartTimedEvent(string eventName);
        ITimedAnalyticsEvent StartTimedEvent(string eventName, IDictionary<string, object> properties);
        
        string AppUserId { get; set; }
    }

    public interface ITimedAnalyticsEvent : IDisposable
    {
        string Name { get; }
        IDictionary<string, object> Properties { get; }
        void End();
        void Cancel();
    }
}
