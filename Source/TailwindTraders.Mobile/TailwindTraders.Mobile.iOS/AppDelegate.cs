using System;
using Foundation;
using Microsoft.AppCenter.Distribute;
using Plugin.XSnack;
using Sharpnado.Presentation.Forms.iOS;
using TailwindTraders.Mobile.Features.Scanning;
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

            RegisterPlatformServices();

            InitTensorflowService();

            Distribute.DontCheckForUpdatesInDebug();
#if ENABLE_TEST_CLOUD
            Xamarin.Calabash.Start();
#endif
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
            TensorflowLiteService.DoNotStripMe();
        }

        private void RegisterPlatformServices()
        {
            DependencyService.Register<IXSnack, XSnackImplementation>();
            DependencyService.Register<IPlatformService, PlatformService>();
            DependencyService.Register<TensorflowLiteService, TensorflowLiteService>();
        }

        private void InitTensorflowService()
        {
            var tensorflowLiteService = DependencyService.Get<TensorflowLiteService>();
            tensorflowLiteService.Initialize(tensorflowLiteService.LabelFilename, tensorflowLiteService.ModelFilename);
        }
    }
}
