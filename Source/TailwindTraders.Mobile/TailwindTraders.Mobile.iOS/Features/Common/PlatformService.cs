using CoreGraphics;
using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using TailwindTraders.Mobile.Features.Common;
using UIKit;

namespace TailwindTraders.Mobile.IOS.Features.Common
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

        public string GetContent(string v)
        {
            return File.ReadAllText(v);
        }

        public string CopyToFilesAndGetPath(string v)
        {
            return v;
        }

        public void ReadImageFileToTensor(
            string fileName,
            bool quantized,
            IntPtr dest,
            int inputHeight = -1,
            int inputWidth = -1)
        {
            using (var image = new UIImage(fileName))
            {
                using (var resized = image.Scale(new CGSize(inputWidth, inputHeight)))
                {
                    int[] intValues = new int[(int)(resized.Size.Width * resized.Size.Height)];
                    var byteValues = new byte[(int)(resized.Size.Width * resized.Size.Height * 3)];
                    System.Runtime.InteropServices.GCHandle handle = System.Runtime.InteropServices.GCHandle.Alloc(
                        intValues, 
                        System.Runtime.InteropServices.GCHandleType.Pinned);
                    using (CGImage cgimage = resized.CGImage)
                    using (CGColorSpace cspace = CGColorSpace.CreateDeviceRGB())
                    using (CGBitmapContext context = new CGBitmapContext(
                        handle.AddrOfPinnedObject(),
                        (nint)resized.Size.Width,
                        (nint)resized.Size.Height,
                        8,
                        (nint)resized.Size.Width * 4,
                        cspace,
                        CGImageAlphaInfo.PremultipliedLast))
                    {
                        context.DrawImage(new CGRect(new CGPoint(), resized.Size), cgimage);
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
            catch (System.Exception ex)
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