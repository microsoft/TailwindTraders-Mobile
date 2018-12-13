using System;
using System.IO;
using System.Threading.Tasks;
using Android.Graphics;
using Android.Media;
using Android.Views;
using Plugin.CurrentActivity;
using TailwindTraders.Mobile.Features.Logging;
using TailwindTraders.Mobile.Features.Scanning.Photo;

namespace TailwindTraders.Mobile.Droid.Features.Scanning.Photo
{
    public class PlatformService : IPlatformService
    {
        private readonly ILoggingService loggingService;

        public PlatformService()
        {
            loggingService = Xamarin.Forms.DependencyService.Get<ILoggingService>();
        }

        public void KeyboardClick()
        {
            var root = CrossCurrentActivity.Current.Activity.FindViewById<View>(Android.Resource.Id.Content);
            root.PlaySoundEffect(SoundEffects.Click);
        }

        public Task<bool> ResizeImageAsync(string filePath, PhotoSize photoSize, int quality)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                return Task.FromResult(false);
            }

            try
            {
                return Task.Run(() =>
                {
                    return InternalResize(filePath, photoSize, quality);
                });
            }
            catch (Exception ex)
            {
                loggingService.Error(ex);

                return Task.FromResult(false);
            }
        }

        private bool InternalResize(string filePath, PhotoSize photoSize, int quality)
        {
            try
            {
                if (photoSize == PhotoSize.Full)
                {
                    return false;
                }

                var percent = CalculateResizePercent(photoSize);

                var exif = new ExifInterface(filePath);
                var rotation = GetRotation(exif);

                var options = new BitmapFactory.Options
                {
                    InJustDecodeBounds = true,
                };

                BitmapFactory.DecodeFile(filePath, options);

                var finalWidth = (int)(options.OutWidth * percent);
                var finalHeight = (int)(options.OutHeight * percent);

                if (rotation % 180 == 90)
                {
                    var tmpSwap = finalWidth;
                    finalWidth = finalHeight;
                    finalHeight = tmpSwap;
                }

                exif.SetAttribute("PixelXDimension", Java.Lang.Integer.ToString(finalWidth));
                exif.SetAttribute("PixelYDimension", Java.Lang.Integer.ToString(finalHeight));

                options.InSampleSize = CalculateInSampleSize(options, finalWidth, finalHeight);
                options.InJustDecodeBounds = false;

                PerformResizeWithRotation(filePath, quality, exif, rotation, options);

                GC.Collect();

                return true;
            }
            catch (Exception ex)
            {
                loggingService.Error(ex);

                return false;
            }
        }

        private void PerformResizeWithRotation(
            string filePath,
            int quality,
            ExifInterface exif,
            int rotation,
            BitmapFactory.Options options)
        {
            using (var originalImage = BitmapFactory.DecodeFile(filePath, options))
            {
                if (rotation != 0)
                {
                    var matrix = new Matrix();
                    matrix.PostRotate(rotation);
                    using (var rotatedImage = Bitmap.CreateBitmap(
                        originalImage,
                        0,
                        0,
                        originalImage.Width,
                        originalImage.Height,
                        matrix,
                        true))
                    {
                        using (var stream = File.Open(filePath, FileMode.Create, FileAccess.ReadWrite))
                        {
                            rotatedImage.Compress(Bitmap.CompressFormat.Jpeg, quality, stream);
                            stream.Close();
                        }

                        rotatedImage.Recycle();
                    }

                    exif.SetAttribute(
                        ExifInterface.TagOrientation,
                        Java.Lang.Integer.ToString((int)Orientation.Normal));
                }
                else
                {
                    using (var stream = File.Open(filePath, FileMode.Create, FileAccess.ReadWrite))
                    {
                        originalImage.Compress(
                            Bitmap.CompressFormat.Jpeg,
                            quality,
                            stream);
                        stream.Close();
                    }
                }
            }
        }

        private float CalculateResizePercent(PhotoSize photoSize)
        {
            var percent = 1.0f;
            switch (photoSize)
            {
                case PhotoSize.Large:
                    percent = .75f;
                    break;
                case PhotoSize.Medium:
                    percent = .5f;
                    break;
                case PhotoSize.Small:
                    percent = .25f;
                    break;
            }

            return percent;
        }

        private int GetRotation(ExifInterface exif)
        {
            if (exif == null)
            {
                return 0;
            }

            try
            {
                var orientation = (Orientation)exif.GetAttributeInt(
                    ExifInterface.TagOrientation,
                    (int)Orientation.Normal);

                switch (orientation)
                {
                    case Orientation.Rotate90:
                        return 90;
                    case Orientation.Rotate180:
                        return 180;
                    case Orientation.Rotate270:
                        return 270;
                    default:
                        return 0;
                }
            }
            catch (Exception ex)
            {
                loggingService.Error(ex);

                return 0;
            }
        }

        private int CalculateInSampleSize(BitmapFactory.Options options, int reqWidth, int reqHeight)
        {
            // Raw height and width of image
            var height = options.OutHeight;
            var width = options.OutWidth;
            var inSampleSize = 1;

            if (height > reqHeight || width > reqWidth)
            {
                var halfHeight = height / 2;
                var halfWidth = width / 2;

                // Calculate the largest inSampleSize value that is a power of 2 and keeps both
                // height and width larger than the requested height and width.
                while ((halfHeight / inSampleSize) >= reqHeight
                        && (halfWidth / inSampleSize) >= reqWidth)
                {
                    inSampleSize *= 2;
                }
            }

            return inSampleSize;
        }
    }
}