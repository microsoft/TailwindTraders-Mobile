using System.Collections.Generic;
using System.Threading.Tasks;
using Foundation;
using Photos;
using TailwindTraders.Mobile.Features.Scanning.Photo;
using TailwindTraders.Mobile.IOS.Features.Scanning.Photo;
using Xamarin.Forms;

[assembly: Dependency(typeof(GalleryService))]

namespace TailwindTraders.Mobile.IOS.Features.Scanning.Photo
{
    public class GalleryService : IGalleryService
    {
        public async Task<List<string>> GetGalleryPhotosAsync(int photoCount)
        {
            var photoAssets = new List<string>();

            var fetchOptions = new PHFetchOptions
            {
                SortDescriptors = new NSSortDescriptor[] { new NSSortDescriptor("creationDate", false) },
            };

            var fetchAssetsResult = PHAsset.FetchAssets(PHAssetMediaType.Image, fetchOptions);

            var phAssetIndex = 0;
            var numPhotos = fetchAssetsResult.Count;

            while (phAssetIndex < numPhotos && phAssetIndex < photoCount)
            {
                var phAsset = (PHAsset)fetchAssetsResult.ObjectAt(phAssetIndex);

                var filePath = await GetFilePathFromAssetAsync(phAsset);
                photoAssets.Add(filePath);

                phAssetIndex++;
            }

            return photoAssets;
        }

        private Task<string> GetFilePathFromAssetAsync(PHAsset phAsset)
        {
            var tcs = new TaskCompletionSource<string>();

            phAsset.RequestContentEditingInput(
                new PHContentEditingInputRequestOptions(),
                (input, v) =>
                {
                    var filePath = input.FullSizeImageUrl.Path;
                    tcs.TrySetResult(filePath);
                });
            return tcs.Task;
        }
    }
}