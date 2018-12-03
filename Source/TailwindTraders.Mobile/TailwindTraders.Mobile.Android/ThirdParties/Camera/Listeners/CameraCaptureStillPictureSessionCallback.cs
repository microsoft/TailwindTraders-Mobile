using Android.Hardware.Camera2;

namespace TailwindTraders.Mobile.Droid.ThirdParties.Camera.Listeners
{
    public class CameraCaptureStillPictureSessionCallback : CameraCaptureSession.CaptureCallback
    {
        private readonly ICamera owner;

        public CameraCaptureStillPictureSessionCallback(ICamera owner)
        {
            if (owner == null)
            {
                throw new System.ArgumentNullException("owner");
            }

            this.owner = owner;
        }

        public override void OnCaptureCompleted(
            CameraCaptureSession session,
            CaptureRequest request,
            TotalCaptureResult result)
        {
            owner.UnlockFocus();
        }
    }
}
