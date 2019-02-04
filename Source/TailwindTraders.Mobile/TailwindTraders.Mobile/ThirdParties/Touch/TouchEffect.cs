using System;
using Xamarin.Forms;

namespace TouchTracking
{
    public class TouchEffect : RoutingEffect
    {
        public event TouchActionEventHandler TouchAction;

        public event EventHandler TapAction;

        public TouchEffect()
            : base("TailwindTraders.TouchEffect")
        {
        }

        public void OnTouchAction(Element element, TouchActionEventArgs args)
        {
            TouchAction?.Invoke(element, args);
        }

        public void OnTapAction(Element element)
        {
            TapAction?.Invoke(element, new EventArgs());
        }
    }
}
