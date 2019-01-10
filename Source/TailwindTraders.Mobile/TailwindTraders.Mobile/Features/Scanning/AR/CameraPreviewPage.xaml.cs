using System;
using System.Threading;
using PubSub.Extension;
using Xamarin.Forms;

namespace TailwindTraders.Mobile.Features.Scanning.AR
{
    public partial class CameraPreviewPage
    {
        private static readonly TimeSpan boundingBoxPersistanceTime = TimeSpan.FromSeconds(1.5f);

        private readonly Animation fadeInAnimation;
        private readonly Animation fadeOutAnimation;

        private enum BoundingBoxState
        {
            Initial,
            Missing,
            Showing,
            Disappearing,
        }

        private BoundingBoxState currentBoundingBoxState = BoundingBoxState.Initial;
        private DetectionMessage boundingBox;
        private DetectionMessage previousBoundingBox;
        private DateTime lastDetectionDate;
        private TimeSpan elapsedTimeSinceLastDetection;
        private double currentAnimationTicks;
        private Timer fadeOutTimer;

        public CameraPreviewPage()
        {
            void callback(double ticks)
            {
                currentAnimationTicks = ticks;
                canvasView.InvalidateSurface();
            }

            fadeInAnimation = new Animation(callback, easing: Easing.SinOut);
            fadeOutAnimation = new Animation(callback, start: 1, end: 0, easing: Easing.CubicIn);

            InitializeComponent();

            BindingContext = new CameraPreviewViewModel();

            UpdateBoundingBoxState(BoundingBoxState.Missing);
        }

        protected override void OnAppearing()
        {
            MessagingCenter.Subscribe<CameraPreviewViewModel>(
                this,
                CameraPreviewViewModel.AddCameraControlMessage,
                _ => AddCameraControl());

            this.Subscribe<DetectionMessage>(message => UpdateBoundingBoxState(BoundingBoxState.Showing, message));

            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            MessagingCenter.Unsubscribe<CameraPreviewViewModel>(this, CameraPreviewViewModel.AddCameraControlMessage);

            this.Unsubscribe<DetectionMessage>();

            base.OnDisappearing();
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

        private void CanvasView_PaintSurface(object sender, SkiaSharp.Views.Forms.SKPaintSurfaceEventArgs e)
        {
            if (currentBoundingBoxState == BoundingBoxState.Initial)
            {
                return;
            }

            var canvas = e.Surface.Canvas;

            if (currentBoundingBoxState == BoundingBoxState.Missing)
            {
                canvas.Clear();
                return;
            }

            var width = canvasView.CanvasSize.Width;
            var height = canvasView.CanvasSize.Height;

            if (currentBoundingBoxState == BoundingBoxState.Showing)
            {
                var ticks = (float)currentAnimationTicks;
                var xmin = previousBoundingBox.Xmin +
                    ((boundingBox.Xmin - previousBoundingBox.Xmin) * ticks);
                var ymin = previousBoundingBox.Ymin +
                    ((boundingBox.Ymin - previousBoundingBox.Ymin) * ticks);
                var xmax = previousBoundingBox.Xmax +
                    ((boundingBox.Xmax - previousBoundingBox.Xmax) * ticks);
                var ymax = previousBoundingBox.Ymax +
                    ((boundingBox.Ymax - previousBoundingBox.Ymax) * ticks);
                DrawingHelper.DrawBoundingBox(canvas, width, height, xmin, ymin, xmax, ymax, currentAnimationTicks);
            }
            else if (currentBoundingBoxState == BoundingBoxState.Disappearing)
            {
                DrawingHelper.DrawBoundingBox(
                    canvas,
                    width,
                    height,
                    boundingBox.Xmin,
                    boundingBox.Ymin,
                    boundingBox.Xmax,
                    boundingBox.Ymax,
                    currentAnimationTicks,
                    applyAlpha: true);
            }

            if (Settings.Settings.DebugMode)
            {
                DrawingHelper.DrawElapsedTime(elapsedTimeSinceLastDetection, canvas, height);
            }
        }

        private void CommitFadeOutAnimation(object state)
        {
            UpdateBoundingBoxState(BoundingBoxState.Disappearing);

            fadeOutAnimation.Commit(
                this, 
                nameof(fadeOutAnimation), 
                finished: (_, __) => UpdateBoundingBoxState(BoundingBoxState.Missing));

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

        private void InitializeFadeOutTimer()
        {
            fadeOutTimer = new Timer(CommitFadeOutAnimation);
            fadeOutTimer.Change(boundingBoxPersistanceTime, TimeSpan.Zero);
        }

        private void UpdateBoundingBoxState(BoundingBoxState newState, DetectionMessage newBoundingBox = null)
        {
            switch (newState)
            {
                case BoundingBoxState.Initial:
                    throw new InvalidOperationException(
                        $"{nameof(BoundingBoxState.Initial)} state can be set just at the beginning.");
                case BoundingBoxState.Missing:
                    canvasView.InvalidateSurface();
                    boundingBox = DetectionMessage.FullScreen;
                    break;
                case BoundingBoxState.Showing:
                    elapsedTimeSinceLastDetection = DateTime.UtcNow - lastDetectionDate;
                    lastDetectionDate = DateTime.UtcNow;

                    previousBoundingBox = boundingBox;
                    boundingBox = newBoundingBox;

                    if (fadeOutTimer != null)
                    {
                        DisposeFadeOutTimer();
                    }

                    fadeInAnimation.Commit(
                        this, 
                        nameof(fadeInAnimation), 
                        finished: (_, __) => InitializeFadeOutTimer());
                    break;
                case BoundingBoxState.Disappearing:
                    break;
                default:
                    throw new InvalidOperationException();
            }

            currentBoundingBoxState = newState;
        }
    }
}
