using SkiaSharp;
using Xamarin.Essentials;

namespace TailwindTraders.Mobile.Features.Scanning.AR
{
    public static class BoundingBoxDrawingHelper
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

        internal static void Draw(
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

            canvas.Clear();
            canvas.DrawRoundRect(rect, boundingBoxCornerRadius, boundingBoxShadowPaint);
            canvas.DrawRoundRect(rect, boundingBoxCornerRadius, boundingBoxPaint);
        }
    }
}
