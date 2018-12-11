using Plugin.Media;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TailwindTraders.Mobile.Features.Localization;
using TailwindTraders.Mobile.Features.Scanning;
using Xamarin.Forms;

[assembly: Dependency(typeof(PhotoService))]

namespace TailwindTraders.Mobile.Features.Scanning
{
    public class PhotoService
    {
        public async Task<bool> CheckPermissionsAsync(params Permission[] permissions)
        {
            var isInitialized = await InitializeCameraAsync();
            if (!isInitialized)
            {
                await Application.Current.MainPage.DisplayAlert(
                   Resources.Alert_Title_NoCameraSupport,
                   Resources.Alert_Message_NoCameraSupport,
                   Resources.Alert_OK);

                return false;
            }

            var statuses = await CheckAndRequestPermissionsAsync(permissions);
            foreach (var status in statuses)
            {
                if (status != PermissionStatus.Granted)
                {
                    await Application.Current.MainPage.DisplayAlert(
                       Resources.Alert_Title_PermissionsNotGranted,
                       Resources.Alert_Message_PermissionsNotGranted,
                       Resources.Alert_OK);

                    return false;
                }
            }

            return true;
        }

        private async Task<bool> InitializeCameraAsync()
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

        private async Task<PermissionStatus[]> CheckAndRequestPermissionsAsync(Permission[] permissions)
        {
            var statuses = new Dictionary<Permission, PermissionStatus>();
            foreach (var permission in permissions)
            {
                var status = await CrossPermissions.Current.CheckPermissionStatusAsync(permission);

                statuses[permission] = status;
            }

            var notGrandedPermissions = statuses.Where(x => x.Value != PermissionStatus.Granted)
                .Select(x => x.Key)
                .ToArray();

            var results = await CrossPermissions.Current.RequestPermissionsAsync(notGrandedPermissions);

            return results.Values.ToArray();
        }
    }
}
