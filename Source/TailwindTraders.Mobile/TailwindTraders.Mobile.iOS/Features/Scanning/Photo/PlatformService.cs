using System;
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