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
        private BoundingBoxMessageArgs boundingBoxArgs;
        private BoundingBoxMessageArgs previousBoundingBoxArgs;
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

            this.Subscribe<BoundingBoxMessageArgs>(args => UpdateBoundingBoxState(BoundingBoxState.Showing, args));

            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            MessagingCenter.Unsubscribe<CameraPreviewViewModel>(this, CameraPreviewViewModel.AddCameraControlMessage);

            this.Unsubscribe<BoundingBoxMessageArgs>();

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
                var xmin = previousBoundingBoxArgs.Xmin +
                    ((boundingBoxArgs.Xmin - previousBoundingBoxArgs.Xmin) * ticks);
                var ymin = previousBoundingBoxArgs.Ymin +
                    ((boundingBoxArgs.Ymin - previousBoundingBoxArgs.Ymin) * ticks);
                var xmax = previousBoundingBoxArgs.Xmax +
                    ((boundingBoxArgs.Xmax - previousBoundingBoxArgs.Xmax) * ticks);
                var ymax = previousBoundingBoxArgs.Ymax +
                    ((boundingBoxArgs.Ymax - previousBoundingBoxArgs.Ymax) * ticks);
                BoundingBoxDrawingHelper.Draw(canvas, width, height, xmin, ymin, xmax, ymax, currentAnimationTicks);
            }
            else if (currentBoundingBoxState == BoundingBoxState.Disappearing)
            {
                BoundingBoxDrawingHelper.Draw(
                    canvas,
                    width,
                    height,
                    boundingBoxArgs.Xmin,
                    boundingBoxArgs.Ymin,
                    boundingBoxArgs.Xmax,
                    boundingBoxArgs.Ymax,
                    currentAnimationTicks,
                    applyAlpha: true);
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

        private void UpdateBoundingBoxState(BoundingBoxState newState, BoundingBoxMessageArgs newBoundingBox = null)
        {
            switch (newState)
            {
                case BoundingBoxState.Initial:
                    throw new InvalidOperationException(
                        $"{nameof(BoundingBoxState.Initial)} state can be set just at the beginning.");
                case BoundingBoxState.Missing:
                    canvasView.InvalidateSurface();
                    boundingBoxArgs = BoundingBoxMessageArgs.FullScreen;
                    break;
                case BoundingBoxState.Showing:
                    previousBoundingBoxArgs = boundingBoxArgs;
                    boundingBoxArgs = newBoundingBox;

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
