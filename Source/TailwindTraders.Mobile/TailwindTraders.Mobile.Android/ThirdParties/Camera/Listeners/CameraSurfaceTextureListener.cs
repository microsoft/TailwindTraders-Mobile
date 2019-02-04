using Android.Views;

namespace TailwindTraders.Mobile.Droid.ThirdParties.Camera.Listeners
{
    public class CameraSurfaceTextureListener : Java.Lang.Object, TextureView.ISurfaceTextureListener
    {
        private readonly ICamera owner;

        public CameraSurfaceTextureListener(ICamera owner)
        {
            if (owner == null)
            {
                throw new System.ArgumentNullException("owner");
            }

            this.owner = owner;
        }

        public void OnSurfaceTextureAvailable(Android.Graphics.SurfaceTexture surface, int width, int height)
        {
            owner.OpenCamera(width, height);
        }

        public bool OnSurfaceTextureDestroyed(Android.Graphics.SurfaceTexture surface)
        {
            return true;
        }

        public void OnSurfaceTextureSizeChanged(Android.Graphics.SurfaceTexture surface, int width, int height)
        {
        }

        public void OnSurfaceTextureUpdated(Android.Graphics.SurfaceTexture surface)
        {
        }
    }
}