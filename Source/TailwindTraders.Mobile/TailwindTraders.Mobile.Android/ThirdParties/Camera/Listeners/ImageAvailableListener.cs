using Android.Graphics;
using Android.Media;
using Java.IO;
using System;
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
                var orientation = owner.GetOrientation();
                System.Threading.Tasks.Task.Run(() =>
                {
                    tensorflowLiteService.Recognize(bytes, orientation);
                }).ConfigureAwait(false);
            }
            else if (captureStillImage)
            {
                captureStillImage = false;

                CaptureImage(bytes);
            }
        }

        private void CaptureImage(byte[] bytes)
        {
            var path = System.IO.Path.Combine(
            Android.App.Application.Context.GetExternalFilesDir(null).AbsolutePath,
            $"photo_{Guid.NewGuid().ToString()}.jpg");

            using (var mFile = new File(path))
            {
                using (var output = new FileOutputStream(mFile))
                {
                    try
                    {
                        output.Write(bytes);
                    }
                    catch (IOException e)
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