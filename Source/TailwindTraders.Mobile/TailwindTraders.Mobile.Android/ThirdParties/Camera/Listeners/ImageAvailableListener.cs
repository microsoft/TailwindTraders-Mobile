using Android.Media;
using Java.IO;
using System;
using System.Threading.Tasks;
using TailwindTraders.Mobile.Features.Scanning;
using Xamarin.Forms;

namespace TailwindTraders.Mobile.Droid.ThirdParties.Camera.Listeners
{
    public class ImageAvailableListener : Java.Lang.Object, ImageReader.IOnImageAvailableListener
    {
        private bool captureStillImage = false;
        private bool tensorflowProcess = true;

        private int imageCount;

        private readonly ICamera owner;

        private readonly TensorflowLiteService tensorflowLiteService;

        public ImageAvailableListener(ICamera fragment)
        {
            if (fragment == null)
            {
                throw new System.ArgumentNullException("fragment");
            }

            owner = fragment;

            tensorflowLiteService = DependencyService.Get<TensorflowLiteService>();
        }

        public void OnImageAvailable(ImageReader reader)
        {
            Android.Media.Image image = null;
            try
            {
                image = reader.AcquireNextImage();

                HandleImage(image);
            }
            finally
            {
                if (image != null)
                {
                    image.Close();
                }
            }
        }

        private void HandleImage(Android.Media.Image image)
        {
            if (tensorflowProcess)
            {
                imageCount++;

                if (imageCount == 10)
                {
                    imageCount = 0;

                    var buffer = image.GetPlanes()[0].Buffer;
                    var bytes = new byte[buffer.Remaining()];
                    buffer.Get(bytes);

                    Task.Run(() => tensorflowLiteService.Recognize(bytes));
                }
            }
            else if (captureStillImage)
            {
                captureStillImage = false;

                CaptureImage(image);
            }
        }

        private void CaptureImage(Android.Media.Image image)
        {
            var path = System.IO.Path.Combine(
            global::Android.App.Application.Context.GetExternalFilesDir(null).AbsolutePath,
            $"photo_{Guid.NewGuid().ToString()}.jpg");

            var buffer = image.GetPlanes()[0].Buffer;
            var bytes = new byte[buffer.Remaining()];
            buffer.Get(bytes);

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

        public void SetCaptureStillImage()
        {
            captureStillImage = true;
        }

        public void SetTensorflowProcess()
        {
            tensorflowProcess = true;
        }
    }
}