using Android.App;
using Android.Hardware.Camera2;

namespace TailwindTraders.Mobile.Droid.ThirdParties.Camera.Listeners
{
    public class CameraStateListener : CameraDevice.StateCallback
    {
        private readonly ICamera owner;

        public CameraStateListener(ICamera owner)
        {
            if (owner == null)
            {
                throw new System.ArgumentNullException("owner");
            }

            this.owner = owner;
        }

        public override void OnOpened(CameraDevice cameraDevice)
        {
            // This method is called when the camera is opened.  We start camera preview here.
            owner.mCameraOpenCloseLock.Release();
            owner.mCameraDevice = cameraDevice;
            owner.CreateCameraPreviewSession();
        }

        public override void OnDisconnected(CameraDevice cameraDevice)
        {
            owner.mCameraOpenCloseLock.Release();
            cameraDevice.Close();
            owner.mCameraDevice = null;
        }

        public override void OnError(CameraDevice cameraDevice, CameraError error)
        {
            owner.mCameraOpenCloseLock.Release();
            cameraDevice.Close();
            owner.mCameraDevice = null;
            if (owner == null)
            {
                return;
            }

            Activity activity = owner.Activity;
            if (activity != null)
            {
                activity.Finish();
            }
        }
    }
}