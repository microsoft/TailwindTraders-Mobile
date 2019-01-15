using System;
using SkiaSharp;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace TailwindTraders.Mobile.Features.Scanning.AR
{
    public static class DrawingHelper
    {
        private static readonly SKSize boundingBoxCornerRadius = new SKSize(23, 23);
        private static readonly SKColor boundingBoxColor = SKColors.White;
        private static readonly SKPaint boundingBoxPaint = new SKPaint
        {
            StrokeWidth = (float)(1.5f * DeviceDisplay.MainDisplayInfo.Density),
            Style = SKPaintStyle.Stroke,
            IsAntialias = true,
        };

        private static readonly SKPaint boundingBoxShadowPaint = new SKPaint
        {
            StrokeWidth = boundingBoxPaint.StrokeWidth,
            Style = SKPaintStyle.Stroke,
            MaskFilter = SKMaskFilter.CreateBlur(
                SKBlurStyle.Normal,
                (float)(2 * DeviceDisplay.MainDisplayInfo.Density)),
            IsAntialias = true,
        };

        private static readonly float elapsedTimeMargin = 5 * (float)DeviceDisplay.MainDisplayInfo.Density;
        private static readonly SKPaint elapsedTimePaint = new SKPaint
        {
            Color = SKColors.White,
            IsAntialias = true,
            Style = SKPaintStyle.Fill,
            TextSize = (float)(Device.GetNamedSize(NamedSize.Small, typeof(Label)) *
                DeviceDisplay.MainDisplayInfo.Density),
        };

        private static readonly float elapsedTimeHeight;

        static DrawingHelper()
        {
            var textBounds = new SKRect();
            elapsedTimePaint.MeasureText("123", ref textBounds);
            elapsedTimeHeight = textBounds.Height;
        }

        internal static void DrawBoundingBox(
            SKCanvas canvas,
            float width,
            float height,
            float xmin,
            float ymin,
            float xmax,
            float ymax,
            double ticks,
            bool applyAlpha = false)
        {
            if (applyAlpha && ticks < 0.1d)
            {
                canvas.Clear();
                return;
            }

            var top = xmin * height;
            var left = ymin * width;
            var bottom = xmax * height;
            var right = ymax * width;

            if (applyAlpha)
            {
                var alpha = (byte)(ticks * byte.MaxValue);
                boundingBoxPaint.Color = boundingBoxColor.WithAlpha(alpha);
            }
            else
            {
                boundingBoxPaint.Color = boundingBoxColor;
            }

            boundingBoxShadowPaint.Color = boundingBoxPaint.Color;

            var rect = new SKRect(left, top, right, bottom);

            canvas.DrawRoundRect(rect, boundingBoxCornerRadius, boundingBoxShadowPaint);
            canvas.DrawRoundRect(rect, boundingBoxCornerRadius, boundingBoxPaint);
        }

        internal static void DrawElapsedTime(
            TimeSpan elapsedTime,
            SKCanvas canvas, 
            float height, 
            DetectionMessage detectionMessage)
        {
            canvas.DrawText(
                $"{elapsedTime.TotalMilliseconds.ToString("#")} ms - {detectionMessage.Score} - {detectionMessage.Label}",
                elapsedTimeMargin,
                height - (elapsedTimeHeight / 2) - elapsedTimeMargin,
                elapsedTimePaint);
        }
    }
}
