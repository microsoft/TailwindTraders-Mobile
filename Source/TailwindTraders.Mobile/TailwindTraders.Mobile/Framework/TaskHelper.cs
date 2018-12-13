using System;
using System.Net.Http;
using System.Threading.Tasks;
using TailwindTraders.Mobile.Features.Localization;
using TailwindTraders.Mobile.Features.Logging;
using Xamarin.Forms;

namespace TailwindTraders.Mobile.Framework
{
    public class TaskHelper
    {
        private readonly IConnectivityService connectivityService;
        private readonly ILoggingService loggingService;

        private Action whenStarting;
        private Action whenFinished;

        public TaskHelper()
        {
            connectivityService = DependencyService.Get<IConnectivityService>();
            loggingService = DependencyService.Get<ILoggingService>();
        }

        public static TaskHelper Create()
        {
            return new TaskHelper();
        }

        public TaskHelper WhenStarting(Action action)
        {
            whenStarting = action;

            return this;
        }

        public TaskHelper WhenFinished(Action action)
        {
            whenFinished = action;

            return this;
        }

        public async Task<WrapResult<object>> TryWithErrorHandlingAsync(
            Func<Task> task,
            Func<Exception, Task<bool>> customErrorHandler = null)
        {
            Func<Task<object>> taskWrapper = new Func<Task<object>>(async () =>
                {
                    await task();

                    return new object();
                });

            WrapResult<object> result = await TryWithErrorHandlingAsync(
                new Func<Task<object>>(taskWrapper),
                customErrorHandler);

            return result;
        }

        public async Task<WrapResult<T>> TryWithErrorHandlingAsync<T>(
            Func<Task<T>> task,
            Func<Exception, Task<bool>> customErrorHandler = null)
        {
            var result = WrapResult<T>.Failed;
            bool keepTrying;

            whenStarting?.Invoke();

            if (!connectivityService.IsThereInternet)
            {
                loggingService?.Warning("There's no Internet access");
                return result;
            }

            do
            {
                keepTrying = false;

                try
                {
                    T actualResult = await task();
                    result = new WrapResult<T>(actualResult, true);
                }
                catch (HttpRequestException exception)
                {
                    loggingService?.Warning($"{exception}");

                    if (customErrorHandler == null || !await customErrorHandler?.Invoke(exception))
                    {
                        await Application.Current.MainPage.DisplayAlert(
                            Resources.Alert_Title_UnexpectedError,
                            Resources.Alert_Message_InternetError,
                            Resources.Alert_OK_OKEllipsis);
                    }
                }
                catch (TaskCanceledException exception)
                {
                    loggingService?.Debug($"{exception}");
                }
                catch (Exception exception)
                {
                    loggingService?.Error(exception);

                    if (customErrorHandler == null || !await customErrorHandler?.Invoke(exception))
                    {
                        await Application.Current.MainPage.DisplayAlert(
                            Resources.Alert_Title_UnexpectedError,
                            Resources.Alert_Message_InternetError,
                            Resources.Alert_OK_OKEllipsis);
                    }
                }
            }
            while (keepTrying);

            whenFinished?.Invoke();

            return result;
        }
    }
}
