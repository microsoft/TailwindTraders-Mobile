using Plugin.Media;
using Plugin.Media.Abstractions;
using System.Threading.Tasks;
using TailwindTraders.Mobile.Features.Common;
using Xamarin.Forms;

[assembly: Dependency(typeof(PhotoService))]

namespace TailwindTraders.Mobile.Features.Common
{
    public class PhotoService
    {
        public async Task<bool> InitializeCameraAsync()
        {
            var result = await CrossMedia.Current.Initialize();

            // [Android] Context.PackageManager.HasSystemFeature(PackageManager.FeatureCamera)
            // [iOS] UIImagePickerController.IsCameraDeviceAvailable()
            var deviceHasCameraAndIsAvailable = CrossMedia.Current.IsCameraAvailable &&
                                                CrossMedia.Current.IsTakePhotoSupported;
            if (!deviceHasCameraAndIsAvailable)
            {
                return false;
            }

            return result;
        }

        public async Task<WrapResult<string>> TakePhotoAsync()
        {
            var options = new StoreCameraMediaOptions
            {
                Directory = "Captures",
                Name = "capture.jpg",
                PhotoSize = PhotoSize.Small,
            };

            var mediaFile = default(MediaFile);

            try
            {
                mediaFile = await CrossMedia.Current.TakePhotoAsync(options);
            }
            catch (MediaPermissionException)
            {
                return WrapResult<string>.Failed;
            }

            return new WrapResult<string>(mediaFile?.Path, true);
        }
    }
}
