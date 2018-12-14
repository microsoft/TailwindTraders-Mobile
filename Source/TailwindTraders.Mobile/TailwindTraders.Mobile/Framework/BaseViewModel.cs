using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using OperationResult;
using Plugin.XSnack;
using TailwindTraders.Mobile.Features.Localization;
using TailwindTraders.Mobile.Features.Logging;
using TailwindTraders.Mobile.Features.LogIn;
using Xamarin.Forms;

namespace TailwindTraders.Mobile.Framework
{
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        protected static readonly IAuthenticationService AuthenticationService;
        protected static readonly ILoggingService LoggingService;
        protected static readonly IXSnack XSnackService;

        private bool isBusy;

        static BaseViewModel()
        {
            AuthenticationService = DependencyService.Get<IAuthenticationService>();

            XSnackService = DependencyService.Get<IXSnack>();
            LoggingService = DependencyService.Get<ILoggingService>();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsBusy
        {
            get => isBusy;
            set => SetAndRaisePropertyChanged(ref isBusy, value);
        }

        public ICommand FeatureNotAvailableCommand { get; } = new AsyncCommand(ShowFeatureNotAvailableAsync);

        public virtual Task InitializeAsync() => Task.CompletedTask;

        public virtual Task UninitializeAsync() => Task.CompletedTask;

        protected static async Task ShowFeatureNotAvailableAsync()
        {
            await Application.Current.MainPage.DisplayAlert(
                Resources.Alert_Title_FeatureNotAvailable,
                Resources.Alert_Message_DemoApp,
                Resources.Alert_OK);
        }

        protected async Task<Status> TryExecuteWithLoadingIndicatorsAsync(
            Task operation,
            Func<Exception, Task<bool>> onError = null) =>
            await TaskHelper.Create()
                .WhenStarting(() => IsBusy = true)
                .WhenFinished(() => IsBusy = false)
                .TryWithErrorHandlingAsync(operation, onError);

        protected async Task<Result<T>> TryExecuteWithLoadingIndicatorsAsync<T>(
            Task<T> operation,
            Func<Exception, Task<bool>> onError = null) =>
            await TaskHelper.Create()
                .WhenStarting(() => IsBusy = true)
                .WhenFinished(() => IsBusy = false)
                .TryWithErrorHandlingAsync(operation, onError);

        protected void SetAndRaisePropertyChanged<TRef>(
            ref TRef field, TRef value, [CallerMemberName] string propertyName = null)
        {
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void SetAndRaisePropertyChangedIfDifferentValues<TRef>(
            ref TRef field, TRef value, [CallerMemberName] string propertyName = null)
            where TRef : class
        {
            if (field == null || !field.Equals(value))
            {
                SetAndRaisePropertyChanged(ref field, value, propertyName);
            }
        }
    }
}
