using System;

namespace TailwindTraders.Mobile.Features.Common
{
    public interface ILoggingService
    {
        void Debug(string message);

        void Warning(string message);

        void Error(Exception exception);
    }
}