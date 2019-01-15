using System;
using System.Threading.Tasks;
using CoreGraphics;
using TailwindTraders.Mobile.Features.Scanning;
using TailwindTraders.Mobile.IOS.Features.Scanning;
using TailwindTraders.Mobile.IOS.ThirdParties.Camera;
using UIKit;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(CameraPreview), typeof(CameraPreviewRenderer))]
namespace TailwindTraders.Mobile.IOS.Features.Scanning
{
    public class CameraPreviewRenderer : ViewRenderer<CameraPreview, UIView>
    {
        private CameraManager cameraManager = new CameraManager();
        private UIView cameraPreview;
        private TaskCompletionSource<string> captureTcs;
        private CameraPreview element;

        public static void Initialize()
        {
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            if (Xamarin.Forms.DesignMode.IsDesignModeEnabled)
            {
                return;
            }

            if (cameraManager != null)
            {
                InitManager();
            }
        }

        protected override void OnElementChanged(ElementChangedEventArgs<CameraPreview> e)
        {
            base.OnElementChanged(e);

            if (Xamarin.Forms.DesignMode.IsDesignModeEnabled)
            {
                return;
            }

            element = e.NewElement;

            if (element == null)
            {
                return;
            }

            element.TakePicture = () => { return TakePictureAsync(); };

            if (Control == null)
            {
                cameraPreview = new UIView(new CGRect());
                cameraPreview.BackgroundColor = UIColor.White;
                SetNativeControl(cameraPreview);

                this.BackgroundColor = UIColor.Cyan;
            }

            if (e.OldElement != null)
            {
                // Unsubscribe
            }

            if (e.NewElement != null)
            {
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (Control != null)
                {
                    Control.Dispose();
                }
            }

            base.Dispose(disposing);
        }

        public Task<string> TakePictureAsync()
        {
            if (captureTcs != null)
            {
                captureTcs.TrySetCanceled();
            }

            captureTcs = new TaskCompletionSource<string>();

            cameraManager.CapturePicture((img, err) =>
            {
                var file = $"photo_{Guid.NewGuid().ToString()}.jpg";
                var jpgFilename = System.IO.Path.Combine(FileSystem.AppDataDirectory, file);
                var imgData = img.AsJPEG();
                if (!imgData.Save(jpgFilename, false, out var error))
                {
                    Console.WriteLine("NOT saved as " + jpgFilename + " because" + error.LocalizedDescription);
                    return;
                }

                Console.WriteLine("saved as " + jpgFilename);
                FinalizeSave(jpgFilename);
            });

            return captureTcs.Task;
        }

        private void FinalizeSave(string absoluteString)
        {
            captureTcs.TrySetResult(absoluteString);

            cameraManager.Dispose();
            cameraManager = null;
            InitManager();
        }

        private void InitManager()
        {
            if (cameraManager == null)
            {
                cameraManager = new CameraManager();
            }

            cameraManager.addPreviewLayerToView(Control, OnCameraReady, element.EnableTensorflowAnalysis);
        }

        private void OnCameraReady()
        {
        }

        public void OnDispose()
        {
            Dispose(true);
        }
    }
}