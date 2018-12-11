using Android.Media;
using Java.IO;
using Java.Lang;
using Java.Nio;
using System;

namespace TailwindTraders.Mobile.Droid.ThirdParties.Camera.Listeners
{
    public class ImageAvailableListener : Java.Lang.Object, ImageReader.IOnImageAvailableListener
    {
        public ImageAvailableListener(ICamera fragment)
        {
            if (fragment == null)
            {
                throw new System.ArgumentNullException("fragment");
            }

            owner = fragment;
        }

        private readonly ICamera owner;

        public void OnImageAvailable(ImageReader reader)
        {
            Image image = reader.AcquireNextImage();
            ImageSaver r = new ImageSaver(image, owner);
            owner.mBackgroundHandler.Post(r);
        }

        // Saves a JPEG {@link Image} into the specified {@link File}.
        private class ImageSaver : Java.Lang.Object, IRunnable
        {
            // The JPEG image
            private Image mImage;

            private ICamera mOwner;

            public ImageSaver(Image image, ICamera owner)
            {
                if (image == null)
                {
                    throw new System.ArgumentNullException("image");
                }

                mImage = image;
                mOwner = owner;
            }

            public void Run()
            {
                var path = System.IO.Path.Combine(
                    global::Android.App.Application.Context.GetExternalFilesDir(null).AbsolutePath,
                    $"photo_{Guid.NewGuid().ToString()}.jpg");
                var mFile = new File(path);

                ByteBuffer buffer = mImage.GetPlanes()[0].Buffer;
                byte[] bytes = new byte[buffer.Remaining()];
                buffer.Get(bytes);
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
                        mImage.Close();
                        mOwner.OnCaptureComplete(path);
                    }
                }
            }
        }
    }
}