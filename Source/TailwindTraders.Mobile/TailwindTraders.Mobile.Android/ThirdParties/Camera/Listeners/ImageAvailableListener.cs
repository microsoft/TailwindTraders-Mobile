using Android.Graphics;
using Android.Media;
using Java.IO;
using System;
using System.IO;
using TailwindTraders.Mobile.Features.Logging;
using TailwindTraders.Mobile.Features.Scanning;
using Xamarin.Forms;

namespace TailwindTraders.Mobile.Droid.ThirdParties.Camera.Listeners
{
    public class ImageAvailableListener : Java.Lang.Object, ImageReader.IOnImageAvailableListener
    {
        private bool captureStillImage = false;
        private bool tensorflowAnalysis = false;

        private readonly ICamera owner;
        private readonly TensorflowLiteService tensorflowLiteService;
        private readonly ILoggingService loggingService;

        public ImageAvailableListener(ICamera fragment)
        {
            if (fragment == null)
            {
                throw new System.ArgumentNullException("fragment");
            }

            captureStillImage = false;
            tensorflowAnalysis = false;

            owner = fragment;

            tensorflowLiteService = DependencyService.Get<TensorflowLiteService>();
            loggingService = DependencyService.Get<ILoggingService>();
        }

        public void OnImageAvailable(ImageReader reader)
        {
            Android.Media.Image image = null;
            byte[] bytes = null;

            try
            {
                image = reader.AcquireNextImage();

                var buffer = image.GetPlanes()[0].Buffer;
                var length = buffer.Remaining();

                bytes = new byte[length];

                buffer.Get(bytes);

                HandleImage(bytes);
            }
            finally
            {
                if (image != null)
                {
                    image.Close();
                }
            }
        }

        private void HandleImage(byte[] bytes)
        {
            if (tensorflowAnalysis)
            {
                using (var bmp = BitmapFactory.DecodeByteArray(bytes, 0, bytes.Length))
                {
                    var width = TensorflowLiteService.ModelInputSize;
                    var height = TensorflowLiteService.ModelInputSize;

                    using (var resized = Bitmap.CreateScaledBitmap(bmp, width, height, false))
                    {
                        var orientation = owner.GetOrientation();

                        var matrix = new Matrix();
                        matrix.PostRotate(orientation);
                        using (var rotatedImage = Bitmap.CreateBitmap(
                            resized,
                            0,
                            0,
                            resized.Width,
                            resized.Height,
                            matrix,
                            true))
                        {
                            //// SaveImg(rotatedImage);
                            var colors = GetColorsFromBmp(rotatedImage);

                            System.Threading.Tasks.Task.Run(() =>
                            {
                                tensorflowLiteService.Recognize(colors);
                            }).ConfigureAwait(false);
                        }
                    }
                }
            }
            else if (captureStillImage)
            {
                captureStillImage = false;

                CaptureImage(bytes);
            }
        }

        private void SaveImg(Bitmap resized)
        {
            var path = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
            var filePath = System.IO.Path.Combine(path, "test.png");
            var stream = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite);
            resized.Compress(Bitmap.CompressFormat.Png, 100, stream);
            stream.Close();
        }

        private int[] GetColorsFromBmp(Bitmap bmp)
        {
            var size = bmp.Width * bmp.Height;
            var intValues = new int[size];

            bmp.GetPixels(intValues, 0, bmp.Width, 0, 0, bmp.Width, bmp.Height);
            return intValues;
        }

        private void CaptureImage(byte[] bytes)
        {
            var path = System.IO.Path.Combine(
            Android.App.Application.Context.GetExternalFilesDir(null).AbsolutePath,
            $"photo_{Guid.NewGuid().ToString()}.jpg");

            using (var mFile = new Java.IO.File(path))
            {
                using (var output = new FileOutputStream(mFile))
                {
                    try
                    {
                        output.Write(bytes);
                    }
                    catch (Java.IO.IOException e)
                    {
                        e.PrintStackTrace();
                    }
                    finally
                    {
                        owner.OnCaptureComplete(path);
                    }
                }
            }

            try
            {
                var orientation = GetExifOrientation(owner.GetOrientation());

                var exif = new ExifInterface(path);
                exif.SetAttribute(
                    ExifInterface.TagOrientation,
                    Java.Lang.Integer.ToString((int)orientation));

                exif.SaveAttributes();
            }
            catch
            {
            }
        }

        private Orientation GetExifOrientation(int rotation)
        {
                switch (rotation)
                {
                    case 90:
                        return Orientation.Rotate90;
                    case 180:
                        return Orientation.Rotate180;
                    case 270:
                        return Orientation.Rotate270;
                    default:
                        return 0;
                }
        }

        public void AllowCaptureStillImageShot()
        {
            captureStillImage = true;
        }

        public void EnableTensorflowAnalysis()
        {
            tensorflowAnalysis = true;
        }
    }
}