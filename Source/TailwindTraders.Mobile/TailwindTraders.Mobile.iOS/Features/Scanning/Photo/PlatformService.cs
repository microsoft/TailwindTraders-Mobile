using CoreFoundation;
using CoreGraphics;
using CoreImage;
using Foundation;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using TailwindTraders.Mobile.Features.Logging;
using TailwindTraders.Mobile.Features.Scanning.Photo;
using UIKit;

namespace TailwindTraders.Mobile.IOS.Features.Scanning.Photo
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
            UIDevice.CurrentDevice.PlayInputClick();
        }

        public bool ResizeImage(string filePath, PhotoSize photoSize, int quality)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                return false;
            }

            try
            {
                return InternalResize(filePath, photoSize, quality);
            }
            catch (Exception ex)
            {
                loggingService.Error(ex);

                return false;
            }
        }

        public string CopyToFilesAndGetPath(string path)
        {
            return path;
        }

        public void ReadImageFileToTensor(
            byte[] imageData,
            IntPtr dest,
            int rotation)
        {
            using (var image = new UIImage(NSData.FromArray(imageData)))
            {
                Debug.Assert(image.Size.Width == 300);
                Debug.Assert(image.Size.Height == 300);

                var rotatedImage = UIImage.FromImage(image.CGImage, 1, UIImageOrientation.Right);

                ////SaveImage(rotatedImage);

                CopyColors(dest, rotatedImage);
            }
        }

        private void SaveImage(UIImage image)
        {
            DispatchQueue.MainQueue.DispatchAsync(() =>
            {
                image.SaveToPhotosAlbum((a, b) =>
                {
                });
            });
        }

        private void CopyColors(IntPtr dest, UIImage image)
        {
            var intValues = new int[(int)(image.Size.Width * image.Size.Height)];
            var byteValues = new byte[(int)(image.Size.Width * image.Size.Height * 3)];
            System.Runtime.InteropServices.GCHandle handle = System.Runtime.InteropServices.GCHandle.Alloc(
                intValues,
                System.Runtime.InteropServices.GCHandleType.Pinned);
            using (CGImage cgimage = image.CGImage)
            using (CGColorSpace cspace = CGColorSpace.CreateDeviceRGB())
            using (CGBitmapContext context = new CGBitmapContext(
                handle.AddrOfPinnedObject(),
                (nint)image.Size.Width,
                (nint)image.Size.Height,
                8,
                (nint)image.Size.Width * 4,
                cspace,
                CGImageAlphaInfo.PremultipliedLast))
            {
                context.DrawImage(new CGRect(new CGPoint(), image.Size), cgimage);
            }

            handle.Free();

            for (int i = 0; i < intValues.Length; ++i)
            {
                int val = intValues[i];
                byteValues[(i * 3) + 0] = (byte)((val >> 16) & 0xFF);
                byteValues[(i * 3) + 1] = (byte)((val >> 8) & 0xFF);
                byteValues[(i * 3) + 2] = (byte)(val & 0xFF);
            }

            System.Runtime.InteropServices.Marshal.Copy(byteValues, 0, dest, byteValues.Length);
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

                var originalImage = new UIImage(filePath);

                var originalWidth = originalImage.Size.Width;
                var originalHeight = originalImage.Size.Height;

                var finalWidth = (int)(originalWidth * percent);
                var finalHeight = (int)(originalHeight * percent);

                using (var resizedImage = InternalResizeToUIImage(originalImage, finalWidth, finalHeight))
                {
                    using (var stream = File.Open(filePath, FileMode.Create, FileAccess.ReadWrite))
                    {
                        var bytesImage = resizedImage.AsJPEG(quality / 100.0f).ToArray();

                        stream.Write(bytesImage, 0, bytesImage.Length);

                        stream.Close();
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                loggingService.Error(ex);

                return false;
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

        private UIImage InternalResizeToUIImage(UIImage originalImage, int finalWidth, int finalHeight)
        {
            UIGraphics.BeginImageContext(new SizeF(finalWidth, finalHeight));
            originalImage.Draw(new RectangleF(0, 0, finalWidth, finalHeight));
            var resizedImage = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();

            return resizedImage;
        }
    }
}