using System;
using System.Collections.Generic;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.AppCenter.Distribute;
using TailwindTraders.Mobile.Features.Settings;

namespace TailwindTraders.Mobile.Features.Logging
{
    public class AppCenterLoggingService : ILoggingService
    {
        public AppCenterLoggingService()
        {
            AppCenter.LogLevel = LogLevel.Verbose;
            AppCenter.Start(
                $"ios={DefaultSettings.AppCenteriOSSecret}​;android={DefaultSettings.AppCenterAndroidSecret}",
                typeof(Analytics),
                typeof(Crashes),
                typeof(Distribute));
        }

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
