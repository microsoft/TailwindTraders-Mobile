using System;
using AVFoundation;
using CoreGraphics;
using CoreMedia;
using CoreVideo;
using TailwindTraders.Mobile.Helpers;
using UIKit;

namespace TailwindTraders.Mobile.IOS.ThirdParties.Camera
{
    public class VideoCaptureDelegate : AVCaptureVideoDataOutputSampleBufferDelegate
    {
        private bool disposed = false;

        public event EventHandler<EventArgsT<UIImage>> FrameCaptured = (sender, e) => { };

        public VideoCaptureDelegate(EventHandler<EventArgsT<UIImage>> callback)
        {
            this.FrameCaptured = callback;
        }

        public override void DidOutputSampleBuffer(
            AVCaptureOutput captureOutput, 
            CMSampleBuffer sampleBuffer, 
            AVCaptureConnection connection)
        {
            try
            {
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

        protected override void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            disposed = true;

            base.Dispose(disposing);
        }

        private UIImage GetUIImage(CMSampleBuffer sampleBuffer)
        {
            using (var imageBuffer = (CVPixelBuffer)sampleBuffer.GetImageBuffer())
            {
                imageBuffer.Lock(CVPixelBufferLock.None);

                var bitmapInfo = (CGImageAlphaInfo)((uint)CGBitmapFlags.ByteOrder32Little |
                    (uint)CGImageAlphaInfo.PremultipliedFirst);
                var colorSpace = CGColorSpace.CreateDeviceRGB();
                var context = new CGBitmapContext(
                    imageBuffer.BaseAddress,
                    imageBuffer.Width,
                    imageBuffer.Height,
                    8,
                    imageBuffer.BytesPerRow, 
                    colorSpace, 
                    bitmapInfo);
                var cgImage = context.ToImage();

                imageBuffer.Unlock(CVPixelBufferLock.None);

                var uiImage = new UIImage(cgImage);
                
                return uiImage;
            }
        }
    }
}