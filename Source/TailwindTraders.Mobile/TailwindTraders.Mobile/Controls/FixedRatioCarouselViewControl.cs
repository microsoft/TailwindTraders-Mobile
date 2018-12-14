using CarouselView.FormsPlugin.Abstractions;
using Xamarin.Forms;

namespace TailwindTraders.Mobile.Controls
{
    public class FixedRatioCarouselViewControl : CarouselViewControl
    {
        protected override SizeRequest OnMeasure(double widthConstraint, double heightConstraint) =>
            new SizeRequest(new Size(widthConstraint, widthConstraint * 1.27f));
    }
}
