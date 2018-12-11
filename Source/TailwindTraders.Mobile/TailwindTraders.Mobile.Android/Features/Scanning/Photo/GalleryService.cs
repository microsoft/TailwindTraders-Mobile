using System.Collections.Generic;
using System.Threading.Tasks;
using Android.Provider;
using Plugin.CurrentActivity;
using TailwindTraders.Mobile.Droid.Features.Scanning.Photo;
using TailwindTraders.Mobile.Features.Scanning.Photo;
using Xamarin.Forms;

[assembly: Dependency(typeof(GalleryService))]

namespace TailwindTraders.Mobile.Droid.Features.Scanning.Photo
{
    public class GalleryService : IGalleryService
    {
        public Task<List<string>> GetGalleryPhotosAsync(int photoCount)
        {
            var galleryList = new List<string>();

            var projection = new[] { MediaStore.Images.Media.InterfaceConsts.Data };
            var orderBy = MediaStore.Images.Media.InterfaceConsts.DateModified + " DESC";

            var cursor = CrossCurrentActivity.Current.Activity.ContentResolver.Query(
                MediaStore.Images.Media.ExternalContentUri,
                projection,
                null, 
                null,
                orderBy);
            if (cursor == null || !cursor.MoveToFirst())
            {
                return Task.FromResult(new List<string>());
            }

            var photoIndex = 0;
            do
            {
                var path = cursor.GetString(cursor.GetColumnIndex(MediaStore.Images.Media.InterfaceConsts.Data));
                if (!string.IsNullOrEmpty(path))
                {
                    galleryList.Add(path);
                }

                photoIndex++;
            }
            while (cursor.MoveToNext() && photoIndex < photoCount);

            return Task.FromResult(galleryList);
        }
    }
}