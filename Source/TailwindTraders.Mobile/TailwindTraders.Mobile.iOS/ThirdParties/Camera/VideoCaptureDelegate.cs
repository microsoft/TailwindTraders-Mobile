using System;
using AVFoundation;
using CoreGraphics;
using CoreImage;
using CoreMedia;
using CoreVideo;
using Foundation;
using TailwindTraders.Mobile.Helpers;
using UIKit;

namespace TailwindTraders.Mobile.IOS.ThirdParties.Camera
{
    public class VideoCaptureDelegate : NSObject, IAVCaptureVideoDataOutputSampleBufferDelegate
    {
        private DateTime lastAnalysis = DateTime.UtcNow;  // controlling the pace of the machine vision analysis
        private TimeSpan pace = new TimeSpan(0, 0, 0, 0, 333); // in milliseconds

        /// <summary>
        /// Keep a single context around, to avoid per-frame allocation
        /// </summary>
        private CIContext context = CIContext.Create();

        public event EventHandler<EventArgsT<UIImage>> FrameCaptured = (sender, e) => { };

        public VideoCaptureDelegate(EventHandler<EventArgsT<UIImage>> callback)
        {
            this.FrameCaptured = callback;
        }

        [Export("captureOutput:didOutputSampleBuffer:fromConnection:")]
        public void DidOutputSampleBuffer(
            AVCaptureOutput captureOutput, 
            CMSampleBuffer sampleBuffer, 
            AVCaptureConnection connection)
        {
            try
            {
                var currentDate = DateTime.UtcNow;

                // control the pace of the machine vision to protect battery life
                if (currentDate - lastAnalysis >= pace)
                {
                    lastAnalysis = currentDate;
                }
                else
                {
                    return; // don't run the classifier more often than we need
                }

                // Crop and resize the image data.
                // Note, this uses a Core Image pipeline that could be appended with other pre-processing.
                // If we don't want to do anything custom, we can remove this step and let the Vision framework handle
                // crop and resize as long as we are careful to pass the orientation properly.
                using (var uiImage = GetUIImage(sampleBuffer))
                {
                    if (uiImage == null)
                    {
                        return;
                    }

                    FrameCaptured(this, new EventArgsT<UIImage>(uiImage));
                }
            }
            finally
            {
                sampleBuffer.Dispose();
            }
        }

        public UIImage GetUIImage(CMSampleBuffer sampleBuffer)
        {
            var imageBuffer = (CVPixelBuffer)sampleBuffer.GetImageBuffer();
            if (imageBuffer == null)
            {
                throw new ArgumentException("Cannot convert to CVPixelBuffer");
            }

            var image = ImageBufferToUIImage(imageBuffer);

            return image;
        }

        public static UIImage ImageBufferToUIImage(CVPixelBuffer imageBuffer)
        {
            imageBuffer.Lock(CVPixelBufferLock.None);

            var baseAddress = imageBuffer.BaseAddress;
            var bytesPerRow = imageBuffer.BytesPerRow;

            var width = imageBuffer.Width;
            var height = imageBuffer.Height;

            var colorSpace = CGColorSpace.CreateDeviceRGB();
            var bitmapInfo = (uint)CGImageAlphaInfo.NoneSkipFirst | (uint)CGBitmapFlags.ByteOrder32Little;

            using (var context = new CGBitmapContext(
                baseAddress,
                width, 
                height,
                8, 
                bytesPerRow,
                colorSpace,
                (CGImageAlphaInfo)bitmapInfo))
            {
                var quartzImage = context?.ToImage();
                imageBuffer.Unlock(CVPixelBufferLock.None);

                var image = new UIImage(quartzImage, 1.0f, UIImageOrientation.Up);

                return image;
            }
        }

        private bool disposed = false;

        protected override void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            disposed = true;
            if (context != null)
            {
                context.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}