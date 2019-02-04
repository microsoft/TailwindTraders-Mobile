using System;
using System.Threading.Tasks;
using OperationResult;
using TailwindTraders.Mobile.Features.Logging;
using TouchTracking;
using Xamarin.Forms;
using static OperationResult.Helpers;

namespace TailwindTraders.Mobile.Features.Scanning.Photo
{
    public partial class CameraPreviewTakePhotoPage
    {
        private readonly IPlatformService platformService;
        private readonly ILoggingService loggingService;

        private CameraPreview cameraPreview;
        private Task<bool> scaleInTask;
        private Task<bool> scaleOutTask;

        public CameraPreviewTakePhotoPage()
        {
            InitializeComponent();

            platformService = DependencyService.Get<IPlatformService>();
            loggingService = DependencyService.Get<ILoggingService>();

            BindingContext = new CameraPreviewTakePhotoViewModel();
        }

        protected override void OnAppearing()
        {
            MessagingCenter.Subscribe<CameraPreviewTakePhotoViewModel>(
                this,
                CameraPreviewTakePhotoViewModel.AddCameraControlMessage,
                (sender) =>
                {
                    AddCameraControl();
                });

            MessagingCenter.Subscribe<CameraPreviewTakePhotoViewModel>(
                this,
                CameraPreviewTakePhotoViewModel.TakePictureCameraControlMessage,
                async (sender) =>
                {
                    var result = await TryCapturePictureAsync();
                    if (result)
                    {
                        ViewModel.PhotoTakenCommand.Execute(result.Value);
                    }
                });

            /* This view model message serves as a workaround for an issue with our current navigation. 
             * (OnAppearing method doesn't get invoke when navigating back.
             * This has to do with another workaround (NavigationProxy.Inner)).
             */
            MessagingCenter.Subscribe<CameraViewModel>(
                this,
                CameraViewModel.ReloadGalleryMessage,
                async (sender) =>
                {
                    await ViewModel.ReloadGalleryAsync();
                });

            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            MessagingCenter.Unsubscribe<CameraPreviewTakePhotoViewModel>(
                this, 
                CameraPreviewTakePhotoViewModel.AddCameraControlMessage);
            MessagingCenter.Unsubscribe<CameraPreviewTakePhotoViewModel>(
                this, 
                CameraPreviewTakePhotoViewModel.TakePictureCameraControlMessage);
            MessagingCenter.Unsubscribe<CameraViewModel>(
                this, 
                CameraViewModel.ReloadGalleryMessage);

            base.OnDisappearing();
        }

        private async Task<Result<string>> TryCapturePictureAsync()
        {
            ViewModel.IsBusy = true;
            try
            {
                var mediaPath = await cameraPreview.TakePicture();
                return Ok(mediaPath);
            }
            catch (OperationCanceledException ex)
            {
                loggingService.Error(ex);
            }
            finally
            {
                ViewModel.IsBusy = false;
            }

            return Error();
        }

        private void AddCameraControl()
        {
            if (cameraControl.Content != null)
            {
                return;
            }

            cameraPreview = new CameraPreview();
            cameraControl.Content = cameraPreview;
        }

        private async void OnTouchEffectActionAsync(object sender, TouchActionEventArgs args)
        {
            if (args.Type == TouchActionType.Pressed)
            {
                scaleInTask = captureButton.ScaleTo(0.9f, 150, Easing.BounceIn);
            }
            else if (args.Type == TouchActionType.Released)
            {
                if (scaleInTask != null)
                {
                    await scaleInTask;
                    scaleInTask = null;
                }

                scaleOutTask = captureButton.ScaleTo(1.0f, 150, Easing.BounceOut);
            }
        }

        private async void OnTapEffectActionAsync(object sender, EventArgs e)
        {
            if (Device.RuntimePlatform == Device.Android)
            {
                platformService.KeyboardClick();
            }

            if (scaleOutTask != null)
            {
                await scaleOutTask;
                scaleOutTask = null;
            }

            ViewModel.TakePhotoCommand.Execute(null);
        }
    }
}
