using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yammer.Chat.Core;
using Microsoft.ApplicationInsights.Telemetry.WindowsStore;

namespace Yammer.Chat.WP
{
    public class ApplicationInsightsWrapper : IApplicationInsightsWrapper
    {
        public ApplicationInsightsWrapper(string applicationInsightsId)
        {
            ClientAnalyticsSession.Default.Start(applicationInsightsId);
        }

        public void LogEvent(string eventName)
        {
            ClientAnalyticsChannel.Default.LogEvent(eventName);
        }

        public void LogEvent(string eventName, IDictionary<string, object> properties)
        {
            ClientAnalyticsChannel.Default.LogEvent(eventName, properties);
        }

        public void LogPageView(string pagePath)
        {
            ClientAnalyticsChannel.Default.LogPageView(pagePath);
        }

        public ITimedAnalyticsEvent StartTimedEvent(string eventName)
        {
            return new TimedAnalyticsEventWrapper(ClientAnalyticsChannel.Default.StartTimedEvent(eventName));
        }

        public ITimedAnalyticsEvent StartTimedEvent(string eventName, IDictionary<string, object> properties)
        {
            return new TimedAnalyticsEventWrapper(ClientAnalyticsChannel.Default.StartTimedEvent(eventName, properties));
        }


        public string AppUserId
        {
            get
            {
                return ClientAnalyticsSession.Default.AppUserId;
            }
            set
            {
                ClientAnalyticsSession.Default.AppUserId = value;
            }
        }
    }

    public class TimedAnalyticsEventWrapper : ITimedAnalyticsEvent
    {
        public TimedAnalyticsEvent TimedEvent { get; set; }

        public TimedAnalyticsEventWrapper(TimedAnalyticsEvent timedAnalyticsEvent)
        {
            TimedEvent = timedAnalyticsEvent;
        }

        public string Name { get { return TimedEvent.Name; } }

        /// <summary>Properties of the timed analytics event 
        /// in which <key, value> pairs can be added.</summary>
        public IDictionary<string, object> Properties { get { return TimedEvent.Properties; } }

        public void Dispose()
        {
            TimedEvent.Dispose();
        }

        /// <summary>Ends the timed analytics event and 
        /// logs its total duration.</summary>
        public void End()
        {
            TimedEvent.End();
        }

        /// <summary>Cancels timed analytics event and 
        /// prevent data logging on End/Dispose.</summary>
        public void Cancel()
        {
            TimedEvent.Cancel();
        }
    }
}
