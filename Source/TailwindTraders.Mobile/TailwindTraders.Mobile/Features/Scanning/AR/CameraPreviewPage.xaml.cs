using System;
using System.Threading;
using PubSub.Extension;
using SkiaSharp;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace TailwindTraders.Mobile.Features.Scanning.AR
{
    public partial class CameraPreviewPage
    {
        private static readonly TimeSpan boundingBoxPersistanceTime = TimeSpan.FromSeconds(1.5f);
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

        private readonly Animation fadeInAnimation;
        private readonly Animation fadeOutAnimation;

        private DetectionMessage boundingBoxArgs;
        private DetectionMessage previousBoundingBoxArgs;
        private double currentAnimationTicks;
        private Timer fadeOutTimer;

        public CameraPreviewPage()
        {
            Action<double> callback = ticks =>
            {
                currentAnimationTicks = ticks;
                canvasView.InvalidateSurface();
            };
            fadeInAnimation = new Animation(callback, easing: Easing.CubicOut);
            fadeOutAnimation = new Animation(callback, start: 1, end: 0, easing: Easing.CubicIn);

            InitializeComponent();

            BindingContext = new CameraPreviewViewModel();
        }

        protected override void OnAppearing()
        {
            MessagingCenter.Subscribe<CameraPreviewViewModel>(
                this,
                CameraPreviewViewModel.AddCameraControlMessage,
                sender => AddCameraControl());

            this.Subscribe<DetectionMessage>(args =>
            {
                if (fadeOutTimer != null)
                {
                    DisposeFadeOutTimer();
                }

                previousBoundingBoxArgs = boundingBoxArgs;
                boundingBoxArgs = args;

                fadeInAnimation.Commit(this, nameof(fadeInAnimation), finished: (_, __) =>
                {
                    fadeOutTimer = new Timer(CommitFadeOutAnimation);
                    fadeOutTimer.Change(boundingBoxPersistanceTime, TimeSpan.Zero);
                });
            });

            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            MessagingCenter.Unsubscribe<CameraPreviewViewModel>(this, CameraPreviewViewModel.AddCameraControlMessage);

            this.Unsubscribe<DetectionMessage>();

            base.OnDisappearing();
        }

        private void CanvasView_PaintSurface(object sender, SkiaSharp.Views.Forms.SKPaintSurfaceEventArgs e)
        {
            if (boundingBoxArgs == null)
            {
                return;
            }

            var canvas = e.Surface.Canvas;
            var width = canvasView.CanvasSize.Width;
            var height = canvasView.CanvasSize.Height;

            if (previousBoundingBoxArgs == null)
            {
                DrawBoundingBox(
                    canvas,
                    width,
                    height,
                    boundingBoxArgs.Xmin,
                    boundingBoxArgs.Ymin,
                    boundingBoxArgs.Xmax,
                    boundingBoxArgs.Ymax,
                    applyAlpha: true);
            }
            else
            {
                var ticks = (float)currentAnimationTicks;
                var xmin = previousBoundingBoxArgs.Xmin + 
                    (Math.Abs(previousBoundingBoxArgs.Xmin - boundingBoxArgs.Xmin) * ticks);
                var ymin = previousBoundingBoxArgs.Ymin + 
                    (Math.Abs(previousBoundingBoxArgs.Ymin - boundingBoxArgs.Ymin) * ticks);
                var xmax = previousBoundingBoxArgs.Xmax + 
                    (Math.Abs(previousBoundingBoxArgs.Xmax - boundingBoxArgs.Xmax) * ticks);
                var ymax = previousBoundingBoxArgs.Ymax +
                    (Math.Abs(previousBoundingBoxArgs.Ymax - boundingBoxArgs.Ymax) * ticks);
                DrawBoundingBox(
                    canvas,
                    width,
                    height,
                    xmin,
                    ymin,
                    xmax,
                    ymax);
            }
        }

        private void AddCameraControl()
        {
            if (cameraControl.Content != null)
            {
                return;
            }

            cameraControl.Content = new CameraPreview
            {
                EnableTensorflowAnalysis = true,
            };
        }

        private void CommitFadeOutAnimation(object state)
        {
            fadeOutAnimation.Commit(this, nameof(fadeOutAnimation), finished: (_, __) => boundingBoxArgs = null);

            DisposeFadeOutTimer();
        }

        private void DisposeFadeOutTimer()
        {
            if (fadeOutTimer != null)
            {
                fadeOutTimer.Dispose();
                fadeOutTimer = null;
            }
        }

        private void DrawBoundingBox(
            SKCanvas canvas,
            float width,
            float height,
            float xmin,
            float ymin,
            float xmax,
            float ymax,
            bool applyAlpha = false)
        {
            var top = xmin * height;
            var left = ymin * width;
            var bottom = xmax * height;
            var right = ymax * width;

            if (applyAlpha)
            {
                var alpha = (byte)(currentAnimationTicks * byte.MaxValue);
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
