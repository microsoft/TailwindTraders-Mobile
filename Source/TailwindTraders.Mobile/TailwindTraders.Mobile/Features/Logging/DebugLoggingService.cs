using System;

namespace TailwindTraders.Mobile.Features.Logging
{
    public class DebugLoggingService : ILoggingService
    {
        public void Debug(string message)
        {
            System.Diagnostics.Debug.WriteLine(message);
        }

        public void Warning(string message)
        {
            Debug($"# {nameof(Warning)}");
            Debug(message);
        }

        public void Error(Exception exception)
        {
            Debug($"# {nameof(Error)}");
            Debug(exception.ToString());
        }
    }
}
