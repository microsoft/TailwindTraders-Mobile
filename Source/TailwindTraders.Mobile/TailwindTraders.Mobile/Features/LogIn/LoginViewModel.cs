using System;
using System.Threading.Tasks;
using System.Windows.Input;
using TailwindTraders.Mobile.Features.Localization;
using TailwindTraders.Mobile.Features.Settings;
using TailwindTraders.Mobile.Framework;
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

#pragma warning disable CS0162
        public override async Task InitializeAsync()
        {
            if (DefaultSettings.ForceAutomaticLogin)
            {
                IsBusy = true;

                // We simulate someone typing her credentials
                await Task.Delay(TimeSpan.FromSeconds(0.5f));
                Email = "foo";
                Password = "bar";
                LogInCommand.Execute(null);
            }

            await base.InitializeAsync();
        }
#pragma warning restore CS0162

        public ICommand MicrosoftLogInCommand => FeatureNotAvailableCommand;

        public override async Task UninitializeAsync()
        {
            await base.UninitializeAsync();

            MessagingCenter.Send(this, LogInFinishedMessage);
        }

        private async Task LogInAsync()
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                XSnackService.ShowMessage(Resources.Snack_Message_InvalidUsernameOrPassword);
                return;
            }

            var result = await TryExecuteWithLoadingIndicatorsAsync(AuthenticationService.LogInAsync(email, password));

            if (result)
            {
                await App.NavigateModallyBackAsync();
            }
        }
    }
}
