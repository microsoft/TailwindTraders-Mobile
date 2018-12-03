using System.Threading.Tasks;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.AppCenter.Distribute;
using TailwindTraders.Mobile.Features.Common;
using TailwindTraders.Mobile.Features.LogIn;
using TailwindTraders.Mobile.Features.Scanning.Photo;
using TailwindTraders.Mobile.Features.Settings;
using TailwindTraders.Mobile.Features.Shell;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]

namespace TailwindTraders.Mobile
{
    public partial class App
    {
        private static NavigableElement navigationRoot;

        public App()
        {
            InitializeComponent();

            RegisterServicesAndProviders();

#if DEBUG
            Resources.Add(nameof(DebugConverter), new DebugConverter());
#endif

            MainPage = new TheShell();
        }

        public static NavigableElement NavigationRoot
        {
            get => GetShellSection(navigationRoot) ?? navigationRoot;
            set => navigationRoot = value;
        }

        public static TheShell Shell => Current.MainPage as TheShell;

        protected override void OnStart()
        {
            base.OnStart();

#if !DEBUG
            InitializeAppCenter();
#endif
        }

        // It provides a navigatable section for elements which aren't explicitly defined within the Shell. For example,
        // ProductCategoryPage: it's accessed from the fly-out through a MenuItem but it doesn't belong to any section
        internal static ShellSection GetShellSection(Element element)
        {
            if (element == null)
            {
                return null;
            }

            var parent = element;
            var parentSection = parent as ShellSection;

            while (parentSection == null && parent != null)
            {
                parent = parent.Parent;
                parentSection = parent as ShellSection;
            }

            return parentSection;
        }

        internal static async Task NavigateBackAsync() => await NavigationRoot.Navigation.PopAsync();

        internal static async Task NavigateToAsync(Page page, bool closeFlyout = false)
        {
            if (closeFlyout)
            {
                await Shell.CloseFlyoutAsync();
            }

            await NavigationRoot.Navigation.PushAsync(page).ConfigureAwait(false);
        }

        internal static async Task NavigateModallyToAsync(Page page, bool animated = true)
        {
            await Shell.CloseFlyoutAsync();
            await NavigationRoot.Navigation.PushModalAsync(page, animated).ConfigureAwait(false);
        }

        private void RegisterServicesAndProviders()
        {
#if DEBUG
            DependencyService.Register<DebugLoggingService>();
#else
            DependencyService.Register<AppCenterLoggingService>();
#endif

            if (Settings.UseFakeAPIs)
            {
                DependencyService.Register<FakeRestPoolService>();
            }
            else
            {
                DependencyService.Register<RestPoolService>();
            }

            if (Settings.UseFakeAuthentication)
            {
                DependencyService.Register<FakeAuthenticationService>();
            }
            else
            {
                DependencyService.Register<AuthenticationService>();
            }
        }

        private static void InitializeAppCenter()
        {
            AppCenter.LogLevel = LogLevel.Verbose;
            AppCenter.Start(
                $"ios={Settings.AppCenteriOSSecret}​;android={Settings.AppCenterAndroidSecret}",
                typeof(Analytics),
                typeof(Crashes),
                typeof(Distribute));
        }
    }
}
