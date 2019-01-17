using System;
using System.Linq;
using System.Runtime.InteropServices;
using AVFoundation;
using CoreFoundation;
using CoreGraphics;
using CoreVideo;
using Foundation;
using TailwindTraders.Mobile.Features.Scanning;
using TailwindTraders.Mobile.Helpers;
using UIKit;
using Xamarin.Forms;

namespace TailwindTraders.Mobile.IOS.ThirdParties.Camera
{
    public enum CameraState
    {
        Ready,
        AccessDenied,
        NoDeviceFound,
        NotDetermined,
    }

    public enum CameraDevice
    {
        Front,
        Back,
    }

    public enum CameraFlashMode : int
    {
        Off = 0,
        On = 1,
        Auto = 2,
    }

    public class CameraManager : NSObject, IAVCaptureFileOutputRecordingDelegate
    {
        // MARK: - Public properties

        /// Capture session to customize camera settings.
        public AVCaptureSession captureSession;

        private readonly DispatchQueue queue = new DispatchQueue("videoQueue");
        private readonly TensorflowLiteService tensorflowLiteService;

        /// Property to determine if the manager should show the error for the user. If you want to show the errors
        /// yourself set this to false. If you want to add custom error UI set showErrorBlock property. Default value is
        /// false.
        public bool showErrorsToUsers = false;

        /// Property to determine if the manager should show the camera permission popup immediatly when it's needed or
        /// you want to show it manually. Default value is true. Be carful cause using the camera requires permission,
        /// if you set this value to false and don't ask manually you won't be able to use the camera.
        public bool showAccessPermissionPopupAutomatically = true;

        public bool cameraIsReady
        {
            get
            {
                return cameraIsSetup;
            }
        }

        public bool hasFrontCamera
        {
            get
            {
                var devices = AVCaptureDevice.DevicesWithMediaType(AVMediaType.Video);
                return devices.Any(x => x.Position == AVCaptureDevicePosition.Front);
            }
        }

        public bool hasFlash
        {
            get
            {
                var devices = AVCaptureDevice.DevicesWithMediaType(AVMediaType.Video);
                var back = devices.First(x => x.Position == AVCaptureDevicePosition.Back);
                return back.HasFlash;
            }
        }

        private CameraDevice cameraDevice = CameraDevice.Back;

        public CameraDevice CameraDevice
        {
            get
            {
                return cameraDevice;
            }

            set
            {
                if (cameraIsSetup)
                {
                    if (cameraDevice != value)
                    {
                        cameraDevice = value;
                        _updateCameraDevice(cameraDevice);
                    }
                }
            }
        }

        private CameraFlashMode flashMode = CameraFlashMode.Off;

        public CameraFlashMode FlashMode
        {
            get
            {
                return flashMode;
            }

            set
            {
                if (cameraIsSetup)
                {
                    if (flashMode != value)
                    {
                        flashMode = value;
                        _updateFlasMode(value);
                    }
                }
            }
        }

        private UIView embeddingView;

        private AVCaptureDevice frontCameraDevice
        {
            get
            {
                var devices = AVCaptureDevice.DevicesWithMediaType(AVMediaType.Video);
                return devices.FirstOrDefault(x => x.Position == AVCaptureDevicePosition.Front);
            }
        }

        private AVCaptureDevice backCameraDevice
        {
            get
            {
                var devices = AVCaptureDevice.DevicesWithMediaType(AVMediaType.Video);
                return devices.FirstOrDefault(x => x.Position == AVCaptureDevicePosition.Back);
            }
        }

        private AVCaptureStillImageOutput stillImageOutput;

        private AVCaptureVideoPreviewLayer previewLayer;

        private bool cameraIsSetup = false;
        private bool tensorflowAnalysis;
        private int[] colors;
        private VideoCaptureDelegate captureDelegate;
        private AVCaptureVideoDataOutput videoOutput;

        public CameraState addPreviewLayerToView(UIView view, Action completion, bool tensorflowAnalysis)
        {
            this.tensorflowAnalysis = tensorflowAnalysis;

            if (_canLoadCamera())
            {
                if (embeddingView != null)
                {
                    if (previewLayer != null)
                    {
                        previewLayer.RemoveFromSuperLayer();
                    }
                }

                if (cameraIsSetup)
                {
                    _addPreviewLayerToView(view);
                    completion?.Invoke();
                }
                else
                {
                    _setupCamera(
                        () =>
                        {
                            this._addPreviewLayerToView(view);
                            completion?.Invoke();
                        });
                }
            }

            return _checkIfCameraIsAvailable();
        }

        public void askUserForCameraPermissions(Action<bool> completion)
        {
            AVCaptureDevice.RequestAccessForMediaType(AVMediaType.Video, allowed =>
            {
                completion(allowed);
            });
        }

        public void stopCaptureSession()
        {
            if (captureSession != null)
            {
                captureSession.StopRunning();
            }
        }

        public void stopAndRemoveCaptureSession()
        {
            stopCaptureSession();
            cameraDevice = CameraDevice.Back;
            cameraIsSetup = false;
            previewLayer = null;
            captureSession = null;
            stillImageOutput = null;
            embeddingView = null;
        }

        public void CapturePicture(Action<UIImage, NSError> completion)
        {
            if (!cameraIsSetup)
            {
                return;
            }

            NSData imageData;
            _getStillImageOutput().CaptureStillImageAsynchronously(
                stillImageOutput.ConnectionFromMediaType(AVMediaType.Video),
                (imageDataSampleBuffer, error) =>
                {
                    if (imageDataSampleBuffer == null)
                    {
                        return;
                    }

                    imageData = AVCaptureStillImageOutput.JpegStillToNSData(imageDataSampleBuffer);

                    var image = new UIImage(imageData);
                    image.SaveToPhotosAlbum((img, err) =>
                    {
                        completion(img, err);
                    });
                });
        }

        public CameraState currentCameraStatus()
        {
            return _checkIfCameraIsAvailable();
        }

        private AVCaptureStillImageOutput _getStillImageOutput()
        {
            var shouldReinitializeStillImageOutput = stillImageOutput == null;
            if (!shouldReinitializeStillImageOutput)
            {
                var connection = stillImageOutput.ConnectionFromMediaType(AVMediaType.Video);
                if (connection != null)
                {
                    shouldReinitializeStillImageOutput = shouldReinitializeStillImageOutput || !connection.Active;
                }
            }

            if (shouldReinitializeStillImageOutput)
            {
                stillImageOutput = new AVCaptureStillImageOutput();

                captureSession.BeginConfiguration();
                captureSession.AddOutput(stillImageOutput);
                captureSession.CommitConfiguration();
            }

            return stillImageOutput;
        }

        private AVCaptureVideoOrientation _currentVideoOrientation()
        {
            switch (UIDevice.CurrentDevice.Orientation)
            {
                case UIDeviceOrientation.LandscapeLeft:
                    return AVCaptureVideoOrientation.LandscapeRight;
                case UIDeviceOrientation.LandscapeRight:
                    return AVCaptureVideoOrientation.LandscapeLeft;
                default:
                    return AVCaptureVideoOrientation.Portrait;
            }
        }

        private bool _canLoadCamera()
        {
            var currentCameraState = _checkIfCameraIsAvailable();
            return currentCameraState == CameraState.Ready ||
                (currentCameraState == CameraState.NotDetermined && showAccessPermissionPopupAutomatically);
        }

        private void _setupCamera(Action completion)
        {
            captureSession = new AVCaptureSession();

            DispatchQueue.MainQueue.DispatchAsync(() =>
            {
                var validCaptureSession = this.captureSession;
                if (validCaptureSession != null)
                {
                    validCaptureSession.BeginConfiguration();
                    validCaptureSession.SessionPreset = AVCaptureSession.PresetHigh;
                    this._updateCameraDevice(this.CameraDevice);
                    this._setupOutputs();
                    this._setupPreviewLayer();
                    validCaptureSession.CommitConfiguration();
                    this._updateFlasMode(this.FlashMode);
                    validCaptureSession.StartRunning();
                    this.cameraIsSetup = true;

                    completion();
                }
            });
        }

        private void _addPreviewLayerToView(UIView view)
        {
            embeddingView = view;

            DispatchQueue.MainQueue.DispatchAsync(() =>
            {
                if (this.previewLayer == null)
                {
                    return;
                }

                this.previewLayer.Frame = view.Layer.Bounds;
                view.ClipsToBounds = false;
                view.Layer.AddSublayer(this.previewLayer);
            });
        }

        private CameraState _checkIfCameraIsAvailable()
        {
            var deviceHasCamera = UIImagePickerController.IsCameraDeviceAvailable(
                UIImagePickerControllerCameraDevice.Rear)
                || UIImagePickerController.IsCameraDeviceAvailable(UIImagePickerControllerCameraDevice.Front);
            if (deviceHasCamera)
            {
                var authorizationStatus = AVCaptureDevice.GetAuthorizationStatus(AVMediaType.Video);
                var userAgreedToUseIt = authorizationStatus == AVAuthorizationStatus.Authorized;
                if (userAgreedToUseIt)
                {
                    return CameraState.Ready;
                }
                else if (authorizationStatus == AVAuthorizationStatus.NotDetermined)
                {
                    return CameraState.NotDetermined;
                }
                else
                {
                    // _show(NSLocalizedString("Camera access denied", comment:""), message:NSLocalizedString("You need
                    // to go to settings app and grant acces to the camera device to use it.", comment:""))
                    return CameraState.AccessDenied;
                }
            }
            else
            {
                // _show(NSLocalizedString("Camera unavailable", comment:""), message:NSLocalizedString("The device does
                // not have a camera.", comment:""))
                return CameraState.NoDeviceFound;
            }
        }

        private void _setupOutputs()
        {
            captureSession.BeginConfiguration();

            if (this.tensorflowAnalysis)
            {
                colors = new int[TensorflowLiteService.ModelInputSize * TensorflowLiteService.ModelInputSize];

                this.captureDelegate = new VideoCaptureDelegate(OnFrameCaptured);

                this.videoOutput = new AVCaptureVideoDataOutput();

                var settings = new CVPixelBufferAttributes
                {
                    PixelFormatType = CVPixelFormatType.CV32BGRA,
                };
                videoOutput.WeakVideoSettings = settings.Dictionary;
                videoOutput.AlwaysDiscardsLateVideoFrames = true;
                videoOutput.SetSampleBufferDelegateQueue(captureDelegate, queue);

                captureSession.AddOutput(videoOutput);
            }
            else
            {
                stillImageOutput = new AVCaptureStillImageOutput();
                captureSession.AddOutput(stillImageOutput);
            }

            captureSession.CommitConfiguration();

            if (this.tensorflowAnalysis)
            {
                _updateCameraQualityMode(AVCaptureSession.Preset352x288);
            }
            else
            {
                _updateCameraQualityMode(AVCaptureSession.PresetHigh);
            }
        }

        private void _setupPreviewLayer()
        {
            if (captureSession != null)
            {
                previewLayer = new AVCaptureVideoPreviewLayer(captureSession);
                previewLayer.VideoGravity = AVLayerVideoGravity.ResizeAspectFill;
            }
        }

        private void _updateCameraDevice(CameraDevice deviceType)
        {
            var validCaptureSession = captureSession;
            if (validCaptureSession != null)
            {
                validCaptureSession.BeginConfiguration();
                var inputs = validCaptureSession.Inputs;
                foreach (var input in inputs)
                {
                    if (input != null)
                    {
                        var deviceInput = input as AVCaptureDeviceInput;
                        if (deviceInput.Device == backCameraDevice && CameraDevice == CameraDevice.Front)
                        {
                            validCaptureSession.RemoveInput(deviceInput);
                            break;
                        }
                        else if (deviceInput.Device == frontCameraDevice && CameraDevice == CameraDevice.Back)
                        {
                            validCaptureSession.RemoveInput(deviceInput);
                            break;
                        }
                    }
                }

                switch (CameraDevice)
                {
                    case CameraDevice.Front:
                        if (hasFrontCamera)
                        {
                            var validFrontDevice = _deviceInputFromDevice(frontCameraDevice);
                            if (validFrontDevice != null)
                            {
                                if (!inputs.Contains(validFrontDevice))
                                {
                                    validCaptureSession.AddInput(validFrontDevice);
                                }
                            }
                        }

                        break;
                    case CameraDevice.Back:
                        var validBackDevice = _deviceInputFromDevice(backCameraDevice);
                        if (validBackDevice != null)
                        {
                            if (!inputs.Contains(validBackDevice))
                            {
                                validCaptureSession.AddInput(validBackDevice);
                            }
                        }

                        break;
                    default:
                        break;
                }

                validCaptureSession.CommitConfiguration();
            }
        }

        private void _updateFlasMode(CameraFlashMode flashMode)
        {
            captureSession.BeginConfiguration();
            var devices = AVCaptureDevice.DevicesWithMediaType(AVMediaType.Video);
            foreach (var device in devices)
            {
                if (device.Position == AVCaptureDevicePosition.Back)
                {
                    var avFlashMode = (AVCaptureFlashMode)Enum.ToObject(typeof(AVCaptureFlashMode), flashMode);

                    if (device.IsFlashModeSupported(avFlashMode))
                    {
                        try
                        {
                            NSError err;
                            device.LockForConfiguration(out err);
                        }
                        catch
                        {
                            return;
                        }

                        device.FlashMode = avFlashMode;
                        device.UnlockForConfiguration();
                    }
                }
            }

            captureSession.CommitConfiguration();
        }

        private void _updateCameraQualityMode(NSString sessionPreset)
        {
            var validCaptureSession = captureSession;

            if (validCaptureSession.CanSetSessionPreset(sessionPreset))
            {
                validCaptureSession.BeginConfiguration();
                validCaptureSession.SessionPreset = sessionPreset;
                validCaptureSession.CommitConfiguration();
            }
        }

        private AVCaptureDeviceInput _deviceInputFromDevice(AVCaptureDevice device = null)
        {
            if (device == null)
            {
                return null;
            }

            try
            {
                NSError err;
                return new AVCaptureDeviceInput(device, out err);
            }
            catch
            {
                return null;
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            stopAndRemoveCaptureSession();
        }

        public CameraManager()
        {
            tensorflowLiteService = DependencyService.Get<TensorflowLiteService>();
        }

        public void FinishedRecording(
            AVCaptureFileOutput captureOutput, NSUrl outputFileUrl, NSObject[] connections, NSError error)
        {
        }

        private void OnFrameCaptured(object sender, EventArgsT<UIImage> args)
        {
            var image = args.Value;

            using (var scaledImage = CreateScaledImage(image))
            {
                using (var rotatedImage = CreateRotatedImage(scaledImage, 90))
                {
                    CopyColorsFromImage(rotatedImage);

                    tensorflowLiteService.Recognize(colors);
                }
            }
        }

        private UIImage CreateRotatedImage(UIImage image, float degree)
        {
            float radians = degree * (float)Math.PI / 180;

            UIGraphics.BeginImageContext(image.Size);

            UIImage rotatedImage;
            using (var bitmap = UIGraphics.GetCurrentContext())
            {
                bitmap.TranslateCTM(image.Size.Width / 2, image.Size.Height / 2);
                bitmap.RotateCTM(radians);
                bitmap.ScaleCTM(1.0f, -1.0f);

                bitmap.DrawImage(
                    new CGRect(-image.Size.Width / 2, -image.Size.Height / 2, image.Size.Width, image.Size.Height),
                    image.CGImage);

                rotatedImage = UIGraphics.GetImageFromCurrentImageContext();
            }

            UIGraphics.EndImageContext();

            return rotatedImage;
        }

        private UIImage CreateScaledImage(UIImage image)
        {
            var width = TensorflowLiteService.ModelInputSize;
            var height = TensorflowLiteService.ModelInputSize;

            var scaledImage = image.Scale(new CGSize(width, height));

            return scaledImage;
        }

        private void SaveImage(UIImage image)
        {
            DispatchQueue.MainQueue.DispatchAsync(() =>
            {
                image.SaveToPhotosAlbum((a, b) =>
                {
                });
            });
        }

        private void CopyColorsFromImage(UIImage image)
        {
            var handle = GCHandle.Alloc(colors, GCHandleType.Pinned);

            using (CGImage cgimage = image.CGImage)
            using (CGColorSpace cspace = CGColorSpace.CreateDeviceRGB())
            using (CGBitmapContext context = new CGBitmapContext(
                handle.AddrOfPinnedObject(),
                (nint)image.Size.Width,
                (nint)image.Size.Height,
                8,
                (nint)image.Size.Width * 4,
                cspace,
                CGImageAlphaInfo.PremultipliedLast))
            {
                context.DrawImage(new CGRect(new CGPoint(), image.Size), cgimage);
            }

            handle.Free();
        }
    }
}
