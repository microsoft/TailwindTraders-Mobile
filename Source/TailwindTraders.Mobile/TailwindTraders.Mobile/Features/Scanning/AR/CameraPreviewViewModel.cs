using Plugin.Permissions.Abstractions;
using System;
using System.Threading.Tasks;
using TailwindTraders.Mobile.Framework;
using Xamarin.Forms;

namespace TailwindTraders.Mobile.Features.Scanning.AR
{
    public class CameraPreviewViewModel : BaseViewModel
    {
        public const string AddCameraControlMessage = nameof(AddCameraControlMessage);

        private readonly PhotoService photoService;

        private Random rnd = new Random();

        public CameraPreviewViewModel()
        {
            photoService = DependencyService.Get<PhotoService>();
        }

        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();

            var cameraHasInitializedAndAdded = await photoService.CheckPermissionsAsync(
                Permission.Storage,
                Permission.Camera);
            if (!cameraHasInitializedAndAdded)
            {
                await App.NavigateBackAsync();
                return;
            }

            MessagingCenter.Send(this, AddCameraControlMessage);
        }
    }
}
