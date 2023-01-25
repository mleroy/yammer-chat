using Microsoft.Phone.Scheduler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yammer.Chat.Core.Resources;

namespace Yammer.Chat.WP
{
    public static class BackgroundAgentManager
    {
        public static void Setup()
        {
            var periodicTaskName = "YammerNowUnreadCountUpdater";
            var periodicTask = ScheduledActionService.Find(periodicTaskName) as PeriodicTask;

            if (periodicTask != null)
            {
                try
                {
                    ScheduledActionService.Remove(periodicTaskName);
                }
                catch (Exception) { }
            }

            periodicTask = new PeriodicTask(periodicTaskName);
            periodicTask.Description = AppResources.BackgroundAgentDescriptionText;

            try
            {
                ScheduledActionService.Add(periodicTask);
            }
            catch (InvalidOperationException exception)
            {
                if (exception.Message.Contains("BNS Error: The action is disabled"))
                {
                    // Background agents for this application have been disabled by the user.
                }

                if (exception.Message.Contains("BNS Error: The maximum number of ScheduledActions of this type have already been added."))
                {
                    // No user action required. The system prompts the user when the hard limit of periodic tasks has been reached.
                }

            }
            catch (SchedulerServiceException)
            {
                // No user action required.  
            }
        }
    }
}
