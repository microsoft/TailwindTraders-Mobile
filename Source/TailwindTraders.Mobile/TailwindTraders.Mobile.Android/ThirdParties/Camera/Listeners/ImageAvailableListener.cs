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
        private readonly int NumFramesToDiscard = 6;
        private readonly bool TFAnalysisInBackground = true;

        private int[] colorArray;
        private bool captureStillImage = false;
        private bool tensorflowAnalysis = false;

        private readonly ICamera owner;
        private readonly TensorflowLiteService tensorflowLiteService;
        private readonly ILoggingService loggingService;

        private int frameCount = 0;

        public ImageAvailableListener(ICamera fragment)
        {
            if (fragment == null)
            {
                throw new System.ArgumentNullException("fragment");
            }

            colorArray = new int[TensorflowLiteService.ModelInputSize * TensorflowLiteService.ModelInputSize];

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
                frameCount++;

                if (frameCount >= NumFramesToDiscard)
                {
                    frameCount = 0;

                    AnalyzeFrame(bytes);
                }
            }
            else if (captureStillImage)
            {
                captureStillImage = false;

                CaptureImage(bytes);
            }
        }

        private void AnalyzeFrame(byte[] bytes)
        {
            using (var bmp = BitmapFactory.DecodeByteArray(bytes, 0, bytes.Length))
            {
                using (var scaledImage = ScaleImage(bmp))
                {
                    var orientation = owner.GetOrientation();

                    using (var rotatedImage = RotateImage(scaledImage, orientation))
                    {
                        //// SaveImg(rotatedImage);

                        CopyColors(rotatedImage);

                        if (TFAnalysisInBackground)
                        {
                            System.Threading.Tasks.Task.Run(() =>
                            {
                                tensorflowLiteService.Recognize(colorArray);
                            }).ConfigureAwait(false);
                        }
                        else
                        {
                            tensorflowLiteService.Recognize(colorArray);
                        }
                    }
                }
            }
        }

        private Bitmap ScaleImage(Bitmap bmp)
        {
            var width = TensorflowLiteService.ModelInputSize;
            var height = TensorflowLiteService.ModelInputSize;

            return Bitmap.CreateScaledBitmap(bmp, width, height, false);
        }

        private Bitmap RotateImage(Bitmap image, int orientation)
        {
            var matrix = new Matrix();
            matrix.PostRotate(orientation);

            var rotatedImage = Bitmap.CreateBitmap(image, 0, 0, image.Width, image.Height, matrix, true);
            return rotatedImage;
        }

        private void CopyColors(Bitmap bmp)
        {
            bmp.GetPixels(colorArray, 0, bmp.Width, 0, 0, bmp.Width, bmp.Height);

            for (int i = 0; i < colorArray.Length; i++)
            {
                var color = new ColorUnion((uint)colorArray[i]);
                var swappedColor = new ColorUnion(color.B, color.G, color.R, color.A);

                colorArray[i] = (int)swappedColor.Value;
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