using System;
using System.Net.Http;
using System.Threading.Tasks;
using OperationResult;
using TailwindTraders.Mobile.Features.Localization;
using TailwindTraders.Mobile.Features.Logging;
using Xamarin.Forms;
using static OperationResult.Helpers;

namespace TailwindTraders.Mobile.Framework
{
    public class TaskHelper
    {
        private readonly IConnectivityService connectivityService;
        private readonly ILoggingService loggingService;

        private Action whenStarting;
        private Action whenFinished;

        private TaskHelper()
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

        public async Task<Status> TryWithErrorHandlingAsync(
            Task task,
            Func<Exception, Task<bool>> customErrorHandler = null)
        {
            var taskWrapper = new Func<Task<object>>(() => WrapTaskAsync(task));
            var result = await TryWithErrorHandlingAsync(taskWrapper(), customErrorHandler);

            if (result)
            {
                return Ok();
            }
                
            return Error();
        }

        public async Task<Result<T>> TryWithErrorHandlingAsync<T>(
            Task<T> task,
            Func<Exception, Task<bool>> customErrorHandler = null)
        {
            whenStarting?.Invoke();

            if (!connectivityService.IsThereInternet)
            {
                loggingService?.Warning("There's no Internet access");
                return Error();
            }

            try
            {
                T actualResult = await task;
                return Ok(actualResult);
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

            whenFinished?.Invoke();

            return Error();
        }

        private async Task<object> WrapTaskAsync(Task innerTask)
        {
            await innerTask;

            return new object();
        }
    }
}
