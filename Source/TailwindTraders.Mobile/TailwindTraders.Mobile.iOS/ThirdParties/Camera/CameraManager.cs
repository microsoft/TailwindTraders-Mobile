using System;
using System.Linq;
using AVFoundation;
using CoreFoundation;
using Foundation;
using UIKit;

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

    public enum CameraOutputMode
    {
        StillImage,
    }

    public enum CameraOutputQuality : int
    {
        Low = 0,
        Medium = 1,
        High = 2,
    }

    public class CameraManager : NSObject
    {
        // MARK: - Public properties

        /// Capture session to customize camera settings.
        public AVCaptureSession captureSession;

        /// Property to determine if the manager should show the camera permission popup immediatly when it's needed or
        /// you want to show it manually. Default value is true. Be carful cause using the camera requires permission,
        /// if you set this value to false and don't ask manually you won't be able to use the camera.
        public bool showAccessPermissionPopupAutomatically = true;

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

        private CameraOutputQuality cameraOutputQuality = CameraOutputQuality.High;

        public CameraOutputQuality CameraOutputQuality
        {
            get
            {
                return cameraOutputQuality;
            }

            set
            {
                if (cameraIsSetup)
                {
                    if (cameraOutputQuality != value)
                    {
                        cameraOutputQuality = value;
                        _updateCameraQualityMode(cameraOutputQuality);
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

        public CameraState addPreviewLayerToView(UIView view, CameraOutputMode newCameraOutputMode, Action completion)
        {
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

        private void _orientationChanged()
        {
            var currentConnection = stillImageOutput.ConnectionFromMediaType(AVMediaType.Video);

            var validPreviewLayer = previewLayer;
            if (validPreviewLayer != null)
            {
                var validPreviewLayerConnection = validPreviewLayer.Connection;
                if (validPreviewLayerConnection != null)
                {
                    if (validPreviewLayerConnection.SupportsVideoOrientation)
                    {
                        validPreviewLayerConnection.VideoOrientation = _currentVideoOrientation();
                    }
                }

                var validOutputLayerConnection = currentConnection;
                if (validOutputLayerConnection != null)
                {
                    if (validOutputLayerConnection.SupportsVideoOrientation)
                    {
                        validOutputLayerConnection.VideoOrientation = _currentVideoOrientation();
                    }
                }

                DispatchQueue.MainQueue.DispatchAsync(() =>
                {
                    var validEmbeddingView = this.embeddingView;
                    if (validEmbeddingView != null)
                    {
                        validPreviewLayer.Frame = validEmbeddingView.Bounds;
                    }
                });
            }
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
                    this._setupOutputMode();
                    this._setupPreviewLayer();
                    validCaptureSession.CommitConfiguration();
                    this._updateFlasMode(this.FlashMode);
                    this._updateCameraQualityMode(this.CameraOutputQuality);
                    validCaptureSession.StartRunning();
                    this.cameraIsSetup = true;
                    this._orientationChanged();

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

        private void _setupOutputMode()
        {
            captureSession.BeginConfiguration();

            if (stillImageOutput == null)
            {
                _setupOutputs();
            }

            if (stillImageOutput != null)
            {
                captureSession.AddOutput(stillImageOutput);
            }

            captureSession.CommitConfiguration();
            _updateCameraQualityMode(CameraOutputQuality);
            _orientationChanged();
        }

        private void _setupOutputs()
        {
            if (stillImageOutput == null)
            {
                stillImageOutput = new AVCaptureStillImageOutput();
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

        private void _updateCameraQualityMode(CameraOutputQuality newCameraOutputQuality)
        {
            var validCaptureSession = captureSession;
            if (validCaptureSession != null)
            {
                var sessionPreset = AVCaptureSession.PresetLow;
                switch (newCameraOutputQuality)
                {
                    case CameraOutputQuality.Low:
                        sessionPreset = AVCaptureSession.PresetLow;
                        break;
                    case CameraOutputQuality.Medium:
                        sessionPreset = AVCaptureSession.PresetMedium;
                        break;
                    case CameraOutputQuality.High:
                        sessionPreset = AVCaptureSession.PresetPhoto;
                        break;
                    default:
                        break;
                }

                if (validCaptureSession.CanSetSessionPreset(sessionPreset))
                {
                    validCaptureSession.BeginConfiguration();
                    validCaptureSession.SessionPreset = sessionPreset;
                    validCaptureSession.CommitConfiguration();
                }
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
    }
}
