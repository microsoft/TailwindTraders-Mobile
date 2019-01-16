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
            Console.WriteLine("holaaa");

            sampleBuffer.Dispose();

            //try
            //{
            //    using (var uiImage = GetUIImage(sampleBuffer))
            //    {
            //        if (uiImage == null)
            //        {
            //            return;
            //        }

            //        FrameCaptured(this, new EventArgsT<UIImage>(uiImage));
            //    }
            //}
            //finally
            //{
            //    sampleBuffer.Dispose();
            //}
        }

        public UIImage GetUIImage(CMSampleBuffer sampleBuffer)
        {
            using (var imageBuffer = sampleBuffer.GetImageBuffer())
            {
                var pixelBuffer = (CVPixelBuffer)imageBuffer;

                var image = ImageBufferToUIImage(pixelBuffer);
                return image;
            }
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
    }
}