using System;
using System.Threading;
using Xamarin.Forms;

namespace TailwindTraders.Mobile.Features.Scanning.AR
{
    public partial class CameraPreviewPage
    {
        private const int AnimationLengthMilliseconds = 125;

        private static readonly TimeSpan boundingBoxPersistanceTime = TimeSpan.FromSeconds(3);

        private readonly Animation framingAnimation;
        private readonly Animation fadeOutAnimation;

        private enum BoundingBoxState
        {
            Initial,
            Missing,
            Framing,
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
            framingAnimation = new Animation(InvalidateCanvasSurface, easing: Easing.SinOut);
            fadeOutAnimation = new Animation(InvalidateCanvasSurface, start: 1, end: 0, easing: Easing.CubicIn);

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

            MessagingCenter.Instance.Subscribe<TensorflowLiteService, DetectionMessage>(
                this,
                TensorflowLiteService.ObjectDetectedMessage,
                (_, message) => UpdateBoundingBoxState(BoundingBoxState.Framing, message));

            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            MessagingCenter.Unsubscribe<CameraPreviewViewModel>(this, CameraPreviewViewModel.AddCameraControlMessage);

            MessagingCenter.Instance.Unsubscribe<TensorflowLiteService, DetectionMessage>(
                this,
                TensorflowLiteService.ObjectDetectedMessage);

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
            canvas.Clear();

            if (currentBoundingBoxState == BoundingBoxState.Missing)
            {
                return;
            }

            var width = canvasView.CanvasSize.Width;
            var height = canvasView.CanvasSize.Height;

            if (currentBoundingBoxState == BoundingBoxState.Framing)
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
                DrawingHelper.DrawElapsedTime(elapsedTimeSinceLastDetection, canvas, height, boundingBox);
            }
        }

        private void CommitFadeOutAnimation()
        {
            UpdateBoundingBoxState(BoundingBoxState.Disappearing);

            fadeOutAnimation.Commit(
                this, 
                nameof(fadeOutAnimation), 
                length: AnimationLengthMilliseconds,
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
            fadeOutTimer = new Timer(_ => CommitFadeOutAnimation());
            fadeOutTimer.Change(boundingBoxPersistanceTime, TimeSpan.Zero);
        }

        private void InvalidateCanvasSurface(double ticks)
        {
            currentAnimationTicks = ticks;
            canvasView.InvalidateSurface();
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
                case BoundingBoxState.Framing:
                    DisposeFadeOutTimer();

                    elapsedTimeSinceLastDetection = DateTime.UtcNow - lastDetectionDate;
                    lastDetectionDate = DateTime.UtcNow;

                    previousBoundingBox = boundingBox;
                    boundingBox = newBoundingBox;

                    if (!this.AnimationIsRunning(nameof(framingAnimation)))
                    {
                        framingAnimation.Commit(
                            this,
                            nameof(framingAnimation),
                            length: AnimationLengthMilliseconds,
                            finished: (_, __) => InitializeFadeOutTimer());
                    }

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
