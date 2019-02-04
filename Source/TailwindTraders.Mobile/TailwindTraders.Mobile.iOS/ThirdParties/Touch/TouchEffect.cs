using System.Linq;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportEffect(typeof(TouchTracking.iOS.TouchEffect), "TouchEffect")]

namespace TouchTracking.iOS
{
    public class TouchEffect : PlatformEffect
    {
        private UIView view;
        private TouchRecognizer touchRecognizer;
        private UITapGestureRecognizer tapDetector;

        protected override void OnAttached()
        {
            // Get the iOS UIView corresponding to the Element that the effect is attached to
            view = Control == null ? Container : Control;

            // Get access to the TouchEffect class in the .NET Standard library
            TouchTracking.TouchEffect effect = (TouchTracking.TouchEffect)Element.Effects.FirstOrDefault(
                e => e is TouchTracking.TouchEffect);

            if (effect != null && view != null)
            {
                // Create a TouchRecognizer for this UIView
                touchRecognizer = new TouchRecognizer(Element, view, effect)
                {
                    ShouldRecognizeSimultaneously = (recognizer, gestureRecognizer) => true,
                };
                view.AddGestureRecognizer(touchRecognizer);
                touchRecognizer.Enabled = true;

                tapDetector = new UITapGestureRecognizer(() =>
                {
                    effect.OnTapAction(Element);
                })
                {
                    ShouldRecognizeSimultaneously = (recognizer, gestureRecognizer) => true,
                };
                view.AddGestureRecognizer(tapDetector);
                tapDetector.Enabled = true;
            }
        }

        protected override void OnDetached()
        {
            if (touchRecognizer != null)
            {
                touchRecognizer.Enabled = false;
                view.RemoveGestureRecognizer(touchRecognizer);

                tapDetector.Enabled = false;
                view.RemoveGestureRecognizer(tapDetector);
            }
        }
    }
}