using System;
using Android.App;
using Android.Content.PM;
using Android.OS;
using CarouselView.FormsPlugin.Android;
using Plugin.CurrentActivity;
using Plugin.XSnack;
using Sharpnado.Presentation.Forms.Droid;
using TailwindTraders.Mobile.Droid.Features.Scanning.Photo;
using TailwindTraders.Mobile.Droid.Helpers;
using TailwindTraders.Mobile.Features.Scanning;
using TailwindTraders.Mobile.Features.Scanning.Photo;
using Xamarin.Forms;

namespace TailwindTraders.Mobile.Droid
{
    [Activity(
        Label = "@string/appName",
        Icon = "@mipmap/icon",
        Theme = "@style/MainTheme",
        ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            InitRenderersAndServices(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            Forms.SetFlags(new[] { "CollectionView_Experimental", "Shell_Experimental", "Visual_Experimental" });
            Forms.Init(this, savedInstanceState);

            RegisterPlatformServices();

            InitTensorflowService();

            LoadApplication(new App());
        }

        public override void OnRequestPermissionsResult(
            int requestCode,
            string[] permissions,
            Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            Plugin.Permissions.PermissionsImplementation.Current.OnRequestPermissionsResult(
                requestCode,
                permissions,
                grantResults);
        }

        private void InitRenderersAndServices(Bundle savedInstanceState)
        {
            CrossCurrentActivity.Current.Init(this, savedInstanceState);
            CarouselViewRenderer.Init();
            SharpnadoInitializer.Initialize();
        }

        private void RegisterPlatformServices()
        {
            DependencyService.Register<IXSnack, XSnackImplementation>();
            DependencyService.Register<IPlatformService, PlatformService>();
        }

        private void InitTensorflowService()
        {
            var tensorflowLiteService = DependencyService.Get<TensorflowLiteService>();

            var labelPath = PathHelper.CopyToFilesDirAndGetPath(tensorflowLiteService.LabelFilename);
            var modelPath = PathHelper.CopyToFilesDirAndGetPath(tensorflowLiteService.ModelFilename);

            tensorflowLiteService.Initialize(labelPath, modelPath);
        }
    }
}