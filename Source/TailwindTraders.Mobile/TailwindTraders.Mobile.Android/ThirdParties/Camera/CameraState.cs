namespace TailwindTraders.Mobile.Droid.ThirdParties.Camera
{
    public enum CameraState : int
    {
        // Camera state: Showing camera preview.
        STATE_PREVIEW = 0,

        // Camera state: Waiting for the focus to be locked.
        STATE_WAITING_LOCK = 1,

        // Camera state: Waiting for the exposure to be precapture state.
        STATE_WAITING_PRECAPTURE = 2,

        // Camera state: Waiting for the exposure state to be something other than precapture.
        STATE_WAITING_NON_PRECAPTURE = 3,

        // Camera state: Picture was taken.
        STATE_PICTURE_TAKEN = 4,
    }
}