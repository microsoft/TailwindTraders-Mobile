using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Plugin.Permissions.Abstractions;
using TailwindTraders.Mobile.Framework;
using Xamarin.Forms;

namespace TailwindTraders.Mobile.Features.Scanning.Photo
{
    public class CameraPreviewTakePhotoViewModel : BaseViewModel
    {
        public const string AddCameraControlMessage = nameof(AddCameraControlMessage);
        public const string TakePictureCameraControlMessage = nameof(TakePictureCameraControlMessage);

        private readonly PhotoService photoService;
        private readonly IGalleryService galleryService;

        private IEnumerable<GalleryImageViewModel> galleryPhotos;

        public ICommand TakePhotoCommand { get; }

        public Command<string> PhotoTakenCommand { get; } = new Command<string>(
            async mediaPath => await App.NavigateModallyToAsync(new CameraPage(mediaPath)));

        public IEnumerable<GalleryImageViewModel> GalleryPhotos
        {
            get => galleryPhotos;
            set => SetAndRaisePropertyChanged(ref galleryPhotos, value);
        }

        public CameraPreviewTakePhotoViewModel()
        {
            photoService = DependencyService.Get<PhotoService>();
            galleryService = DependencyService.Get<IGalleryService>();

            TakePhotoCommand = new Command(TakePhotoCommandHandle);
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

            await ReloadGalleryAsync();
            MessagingCenter.Send(this, AddCameraControlMessage);
        }

        public async Task ReloadGalleryAsync()
        {
            IsBusy = true;

            var imageSources = await galleryService.GetGalleryPhotosAsync();
            GalleryPhotos = imageSources
                .Select(source => new GalleryImageViewModel(source, PhotoTakenCommand))
                .ToList();

            IsBusy = false;
        }

        private void TakePhotoCommandHandle()
        {
            MessagingCenter.Send(this, TakePictureCameraControlMessage);
        }
    }
}
