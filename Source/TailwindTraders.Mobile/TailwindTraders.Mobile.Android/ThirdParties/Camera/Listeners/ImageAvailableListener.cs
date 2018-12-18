using Android.Media;
using Android.Util;
using Android.Views;
using Java.IO;
using System;

namespace TailwindTraders.Mobile.Droid.ThirdParties.Camera.Listeners
{
    public class ImageAvailableListener : Java.Lang.Object, ImageReader.IOnImageAvailableListener
    {
        private bool captureStillImage = false;
        private bool tensorflowProcess = false;

        private readonly ICamera owner;

        public ImageAvailableListener(ICamera fragment)
        {
            if (fragment == null)
            {
                throw new System.ArgumentNullException("fragment");
            }

            owner = fragment;
        }

        public void OnImageAvailable(ImageReader reader)
        {
            Image image = null;
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

        private void HandleImage(Image image)
        {
            if (tensorflowProcess)
            {
            }
            else if (captureStillImage)
            {
                captureStillImage = false;

                CaptureImage(image);
            }
        }

        private void CaptureImage(Image image)
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