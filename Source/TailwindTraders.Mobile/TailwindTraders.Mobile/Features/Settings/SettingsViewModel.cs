using TailwindTraders.Mobile.Framework;
using Xamarin.Forms;

namespace TailwindTraders.Mobile.Features.Settings
{
    public class SettingsViewModel : BaseViewModel
    {
        private string rootApiUrl;

        public string RootApiUrl
        {
            get => rootApiUrl;
            set => SetAndRaisePropertyChangedIfDifferentValues(ref rootApiUrl, value);
        }

        public Command SaveCommand { get; }

        public SettingsViewModel()
        {
            rootApiUrl = Settings.RootApiUrl;

            SaveCommand = new Command(Save);
        }

        private void Save()
        {
            Settings.RootApiUrl = rootApiUrl;

            RestPoolService.UpdateApiUrl(rootApiUrl);

            XSnackService.ShowMessage("Settings saved");
        }
    }
}
