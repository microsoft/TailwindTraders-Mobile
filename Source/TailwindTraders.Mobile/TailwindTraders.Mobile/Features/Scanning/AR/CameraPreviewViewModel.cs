using Plugin.Permissions.Abstractions;
using System;
using System.Threading.Tasks;
using TailwindTraders.Mobile.Features.Common;
using Xamarin.Forms;

namespace TailwindTraders.Mobile.Features.Scanning.AR
{
    public class CameraPreviewViewModel : BaseViewModel
    {
        public const string AddCameraControlMessage = nameof(AddCameraControlMessage);
        public const string DrawBoundingBoxMessage = nameof(DrawBoundingBoxMessage);

        private readonly PhotoService photoService;

        private Random rnd = new Random();
        private bool shouldRecognize;

        public CameraPreviewViewModel()
        {
            photoService = DependencyService.Get<PhotoService>();
        }

        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();

            shouldRecognize = true;

            var cameraHasInitializedAndAdded = await photoService.CheckPermissionsAsync(
                Permission.Storage,
                Permission.Camera);
            if (!cameraHasInitializedAndAdded)
            {
                await App.NavigateBackAsync();
                return;
            }
            else
            {
                MessagingCenter.Send(this, AddCameraControlMessage);
            }

            await AddFakeBoundingBoxesAsync();
        }

        public override async Task UninitializeAsync()
        {
            await base.UninitializeAsync();

            shouldRecognize = false;
        }

        private async Task AddFakeBoundingBoxesAsync()
        {
            while (shouldRecognize)
            {
                var xmin = (float)rnd.NextDouble();
                var ymin = (float)rnd.NextDouble();
                var xmax = MathHelper.Clamp(xmin + (float)rnd.NextDouble(), 0.0f, 1.0f);
                var ymax = MathHelper.Clamp(ymin + (float)rnd.NextDouble(), 0.0f, 1.0f);

                MessagingCenter.Send(this, DrawBoundingBoxMessage, new BoundingBoxMessageArgs()
                {
                    Xmin = xmin,
                    Ymin = xmin,
                    Xmax = xmax,
                    Ymax = ymax,
                });

                await Task.Delay(TimeSpan.FromSeconds(2));
            }
        }
    }
}
