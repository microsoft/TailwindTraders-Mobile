using System.IO;

namespace TailwindTraders.Mobile.Droid.Helpers
{
    public static class PathHelper
    {
        public static string CopyToFilesDirAndGetPath(string path)
        {
            var cleanPath = path.Replace("/", "_");

            // https://kimsereyblog.blogspot.com/2016/11/differences-between-internal-and.html
            var absoluteFilePath = System.IO.Path.Combine(
                Android.App.Application.Context.FilesDir.AbsolutePath,
                cleanPath);

            var assets = Android.App.Application.Context.Assets;
            using (var f = assets.Open(path))
            {
                using (var dest = new FileStream(absoluteFilePath, FileMode.OpenOrCreate))
                {
                    f.CopyTo(dest);
                }
            }

            return absoluteFilePath;
        }
    }
}