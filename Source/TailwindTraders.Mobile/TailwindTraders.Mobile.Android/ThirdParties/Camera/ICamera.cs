using Android.App;
using Android.Hardware.Camera2;
using Android.OS;
using Java.Util.Concurrent;
using TailwindTraders.Mobile.Droid.ThirdParties.Camera.Listeners;

namespace TailwindTraders.Mobile.Droid.ThirdParties.Camera
{
    public interface ICamera
    {
        Semaphore mCameraOpenCloseLock { get; set; }

        CameraDevice mCameraDevice { get; set; }

        Activity Activity { get; set; }

        CameraState mState { get; set; }

        Handler mBackgroundHandler { get; set; }

        CameraCaptureSession mCaptureSession { get; set; }

        CaptureRequest.Builder mPreviewRequestBuilder { get; set; }

        CaptureRequest mPreviewRequest { get; set; }

        CameraCaptureListener mCaptureCallback { get; set; }

        void CreateCameraPreviewSession();

        void CaptureStillPicture();

        void RunPrecaptureSequence();

        void OpenCamera(int width, int height);

        void ConfigureTransform(int width, int height);

        void OnCaptureComplete(string path);
    }
}