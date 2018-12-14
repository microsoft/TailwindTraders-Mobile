using Foundation;
using Microsoft.AppCenter.Distribute;
using Plugin.XSnack;
using Sharpnado.Presentation.Forms.iOS;
using TailwindTraders.Mobile.Features.Scanning.Photo;
using TailwindTraders.Mobile.IOS.Features.Scanning;
using TailwindTraders.Mobile.IOS.Features.Scanning.Photo;
using TouchTracking.iOS;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ResolutionGroupName(nameof(TailwindTraders))]

namespace TailwindTraders.Mobile.IOS
{
    [Register("AppDelegate")]
    public class AppDelegate : FormsApplicationDelegate
    {
        public override bool FinishedLaunching(UIApplication uiApplication, NSDictionary launchOptions)
        {
            InitRenderersAndServices();

            Forms.SetFlags(new[] { "CollectionView_Experimental", "Shell_Experimental", "Visual_Experimental" });
            Forms.Init();

            RegisterServices();

            Distribute.DontCheckForUpdatesInDebug();

            LoadApplication(new App());

            CustomizeAppearance();

            return base.FinishedLaunching(uiApplication, launchOptions);
        }

        public override bool OpenUrl(
            UIApplication application,
            NSUrl url,
            string sourceApplication,
            NSObject annotation)
        {
            Distribute.OpenUrl(url);

            return true;
        }

        private void CustomizeAppearance()
        {
            var accentColor = (Color)App.Current.Resources["AccentColor"];
            UITextField.Appearance.TintColor = accentColor.ToUIColor();
        }

        private void InitRenderersAndServices()
        {
            CameraPreviewRenderer.Initialize();
            CarouselView.FormsPlugin.iOS.CarouselViewRenderer.Init();
            SharpnadoInitializer.Initialize();
            TouchRecognizer.Initialize();
            TensorflowLite.DoNotStripMe();
        }

        private void RegisterServices()
        {
            DependencyService.Register<IXSnack, XSnackImplementation>();
            DependencyService.Register<IPlatformService, PlatformService>();
        }
    }
}
