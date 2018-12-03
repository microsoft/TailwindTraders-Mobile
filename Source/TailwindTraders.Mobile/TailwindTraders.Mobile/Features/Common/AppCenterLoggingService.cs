using System;
using System.Collections.Generic;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;

namespace TailwindTraders.Mobile.Features.Common
{
    public class AppCenterLoggingService : ILoggingService
    {
        public void Debug(string message)
        {
            Analytics.TrackEvent(nameof(Debug), new Dictionary<string, string> { { nameof(message), message } });
        }

        public void Warning(string message)
        {
            Analytics.TrackEvent(nameof(Warning), new Dictionary<string, string> { { nameof(message), message } });
        }

        public void Error(Exception exception)
        {
            Crashes.TrackError(exception);
        }
    }
}
