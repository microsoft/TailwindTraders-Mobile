using TailwindTraders.Mobile.Features.Common;
using TailwindTraders.Mobile.Framework;
using Xamarin.Forms;

namespace TailwindTraders.Mobile.Features.Settings
{
    public class SettingsViewModel : BaseViewModel
    {
        private readonly IRestPoolService restPoolService;

        private string rootApiUrl;

        public string RootApiUrl
        {
            get => rootApiUrl;
            set => SetAndRaisePropertyChangedIfDifferentValues(ref rootApiUrl, value);
        }

        public Command SaveCommand { get; }

        public SettingsViewModel()
        {
            restPoolService = DependencyService.Get<IRestPoolService>();

            rootApiUrl = Settings.RootApiUrl;

            SaveCommand = new Command(Save);
        }

        private void Save()
        {
            Settings.RootApiUrl = rootApiUrl;

            restPoolService.UpdateApiUrl(rootApiUrl);

            XSnackService.ShowMessage("Settings saved");
        }
    }
}
