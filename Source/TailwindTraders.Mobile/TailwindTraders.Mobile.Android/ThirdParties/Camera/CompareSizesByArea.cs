using Android.Util;
using Java.Lang;
using Java.Util;

namespace TailwindTraders.Mobile.Droid.ThirdParties.Camera
{
    public class CompareSizesByArea : Java.Lang.Object, IComparator
    {
        public int Compare(Object lhs, Object rhs)
        {
            var lhsSize = (Size)lhs;
            var rhsSize = (Size)rhs;

            // We cast here to ensure the multiplications won't overflow
            return Long.Signum(((long)lhsSize.Width * lhsSize.Height) - ((long)rhsSize.Width * rhsSize.Height));
        }
    }
}