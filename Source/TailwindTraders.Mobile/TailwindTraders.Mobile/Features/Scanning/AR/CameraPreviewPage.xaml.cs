using SkiaSharp;
using Xamarin.Forms;

namespace TailwindTraders.Mobile.Features.Scanning.AR
{
    public partial class CameraPreviewPage
    {
        private BoundingBoxMessageArgs boundingBoxArgs;
        private double currentMilliseconds;

        public CameraPreviewPage()
        {
            InitializeComponent();

            BindingContext = new CameraPreviewViewModel();
        }

        protected override void OnAppearing()
        {
            MessagingCenter.Subscribe<CameraPreviewViewModel>(
                this,
                CameraPreviewViewModel.AddCameraControlMessage,
                (sender) =>
                {
                    AddCameraControl();
                });

            MessagingCenter.Subscribe<CameraPreviewViewModel, BoundingBoxMessageArgs>(
                this,
                CameraPreviewViewModel.DrawBoundingBoxMessage,
                (sender, args) =>
                {
                    boundingBoxArgs = args;

                    var fadeInAnimation = new Animation(
                        milliseconds =>
                        {
                            currentMilliseconds = milliseconds;
                            canvasView.InvalidateSurface();
                        },
                        easing: Easing.CubicIn);
                    fadeInAnimation.Commit(this, nameof(fadeInAnimation));
                });

            base.OnAppearing();
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

            DrawBoundingBox(
                canvas,
                width,
                height,
                boundingBoxArgs.Xmin,
                boundingBoxArgs.Ymin,
                boundingBoxArgs.Xmax,
                boundingBoxArgs.Ymax);
        }

        protected override void OnDisappearing()
        {
            MessagingCenter.Unsubscribe<CameraPreviewViewModel>(this, CameraPreviewViewModel.AddCameraControlMessage);
            MessagingCenter.Unsubscribe<CameraPreviewViewModel>(this, CameraPreviewViewModel.DrawBoundingBoxMessage);

            base.OnDisappearing();
        }

        private void AddCameraControl()
        {
            if (cameraControl.Content != null)
            {
                return;
            }

            cameraControl.Content = new CameraPreview();
        }

        private void DrawBoundingBox(
            SKCanvas canvas,
            float width,
            float height,
            float xmin,
            float ymin,
            float xmax,
            float ymax)
        {
            var left = xmin * width;
            var top = ymin * height;
            var right = xmax * width;
            var bottom = ymax * height;

            var alpha = (byte)(currentMilliseconds * byte.MaxValue);
            var paint = new SKPaint
            {
                StrokeWidth = 3,
                Color = SKColors.White.WithAlpha(alpha),
                Style = SKPaintStyle.Stroke,
            };

            var cornerRadius = new SKSize(23, 23);
            var rect = new SKRect(left, top, right, bottom);

            canvas.Clear();
            canvas.DrawRoundRect(rect, cornerRadius, paint);
        }
    }
}
