using System;
using System.Threading.Tasks;
using System.Windows.Input;
using TailwindTraders.Mobile.Features.Common;
using TailwindTraders.Mobile.Features.Localization;
using Xamarin.Forms;

namespace TailwindTraders.Mobile.Features.LogIn
{
    public class LoginViewModel : BaseViewModel
    {
        public const string LogInFinishedMessage = nameof(LogInFinishedMessage);

        private string email;
        private string password;

        public string Email
        {
            get => email;
            set => SetAndRaisePropertyChangedIfDifferentValues(ref email, value);
        }

        public string Password
        {
            get => password;
            set => SetAndRaisePropertyChangedIfDifferentValues(ref password, value);
        }

        public ICommand LogInCommand => new AsyncCommand(LogInAsync);

#if DEBUG
        public override async Task InitializeAsync()
        {
            IsBusy = true;

            await base.InitializeAsync();

            // We simulate someone typing her credentials
            await Task.Delay(TimeSpan.FromSeconds(0.5f));
            Email = "foo";
            Password = "bar";
            LogInCommand.Execute(null);
        }
#endif

        public ICommand MicrosoftLogInCommand => FeatureNotAvailableCommand;

        public override async Task UninitializeAsync()
        {
            await base.UninitializeAsync();

            MessagingCenter.Send(this, LogInFinishedMessage);
        }

        private async Task CloseLogInAsync()
        {
            var navigation = Application.Current.MainPage.Navigation;
            await navigation.PopModalAsync();
        }

        private async Task LogInAsync()
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                XSnackService.ShowMessage(Resources.Snack_Message_InvalidUsernameOrPassword);
                return;
            }

            var result = await ExecuteWithLoadingIndicatorsAsync(
                () => AuthenticationService.LogInAsync(email, password));

            if (result.IsSucceded)
            {
                await CloseLogInAsync();
            }
        }
    }
}
