using Android.Hardware.Camera2;
using Java.Lang;

namespace TailwindTraders.Mobile.Droid.ThirdParties.Camera.Listeners
{
    public class CameraCaptureListener : CameraCaptureSession.CaptureCallback
    {
        private readonly ICamera owner;

        public CameraCaptureListener(ICamera owner)
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
            Process(result);
        }

        public override void OnCaptureProgressed(
            CameraCaptureSession session,
            CaptureRequest request,
            CaptureResult partialResult)
        {
            Process(partialResult);
        }

        private void Process(CaptureResult result)
        {
            switch (owner.mState)
            {
                case CameraState.STATE_WAITING_LOCK:
                    {
                        Integer afState = (Integer)result.Get(CaptureResult.ControlAfState);
                        if (afState == null)
                        {
                            owner.CaptureStillPicture();
                        }
                        else if ((afState.IntValue() == ((int)ControlAFState.FocusedLocked)) ||
                                   (afState.IntValue() == ((int)ControlAFState.NotFocusedLocked)))
                        {
                            // ControlAeState can be null on some devices
                            Integer aeState = (Integer)result.Get(CaptureResult.ControlAeState);
                            if (aeState == null ||
                                    aeState.IntValue() == ((int)ControlAEState.Converged))
                            {
                                owner.mState = CameraState.STATE_PICTURE_TAKEN;
                                owner.CaptureStillPicture();
                            }
                            else
                            {
                                owner.RunPrecaptureSequence();
                            }
                        }

                        break;
                    }

                case CameraState.STATE_WAITING_PRECAPTURE:
                    {
                        // ControlAeState can be null on some devices
                        Integer aeState = (Integer)result.Get(CaptureResult.ControlAeState);
                        if (aeState == null ||
                                aeState.IntValue() == ((int)ControlAEState.Precapture) ||
                                aeState.IntValue() == ((int)ControlAEState.FlashRequired))
                        {
                            owner.mState = CameraState.STATE_WAITING_NON_PRECAPTURE;
                        }

                        break;
                    }

                case CameraState.STATE_WAITING_NON_PRECAPTURE:
                    {
                        // ControlAeState can be null on some devices
                        Integer aeState = (Integer)result.Get(CaptureResult.ControlAeState);
                        if (aeState == null || aeState.IntValue() != ((int)ControlAEState.Precapture))
                        {
                            owner.mState = CameraState.STATE_PICTURE_TAKEN;
                            owner.CaptureStillPicture();
                        }

                        break;
                    }

                case CameraState.STATE_PICTURE_TAKEN:
                    {
                        System.Diagnostics.Debug.WriteLine("STATE_PICTURE_TAKEN");
                        break;
                    }
            }
        }
    }
}