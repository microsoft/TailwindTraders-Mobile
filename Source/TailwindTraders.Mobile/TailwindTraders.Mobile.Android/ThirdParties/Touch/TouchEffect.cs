using System;
using System.Linq;
using Android.Support.V4.View;
using Android.Views;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportEffect(typeof(TouchTracking.Droid.TouchEffect), "TouchEffect")]

namespace TouchTracking.Droid
{
    public class TouchEffect : PlatformEffect
    {
        private GestureDetectorCompat gestureRecognizer;
        private InternalGestureDetector tapDetector;

        private Android.Views.View view;
        private Element formsElement;
        private TouchTracking.TouchEffect libTouchEffect;
        private Func<double, double> fromPixels;
        private int[] twoIntArray = new int[2];

        protected override void OnAttached()
        {
            // Get the Android View corresponding to the Element that the effect is attached to
            view = Control == null ? Container : Control;

            // Get access to the TouchEffect class in the .NET Standard library
            TouchTracking.TouchEffect touchEffect =
                (TouchTracking.TouchEffect)Element.Effects.
                    FirstOrDefault(e => e is TouchTracking.TouchEffect);

            if (touchEffect != null && view != null)
            {
                formsElement = Element;

                libTouchEffect = touchEffect;

                // Save fromPixels function
                fromPixels = view.Context.FromPixels;

                tapDetector = new InternalGestureDetector
                {
                    TapAction = motionEvent =>
                    {
                        libTouchEffect.OnTapAction(formsElement);
                    },
                };

                gestureRecognizer = new GestureDetectorCompat(view.Context, tapDetector);

                // Set event handler on View
                view.Touch += OnTouch;
            }
        }

        protected override void OnDetached()
        {
            view.Touch -= OnTouch;
        }

        private void OnTouch(object sender, Android.Views.View.TouchEventArgs args)
        {
            gestureRecognizer?.OnTouchEvent(args.Event);

            // Two object common to all the events
            Android.Views.View senderView = sender as Android.Views.View;
            MotionEvent motionEvent = args.Event;

            // Get the pointer index
            int pointerIndex = motionEvent.ActionIndex;

            // Get the id that identifies a finger over the course of its progress
            int id = motionEvent.GetPointerId(pointerIndex);

            senderView.GetLocationOnScreen(twoIntArray);
            Point screenPointerCoords = new Point(
                twoIntArray[0] + motionEvent.GetX(pointerIndex),
                twoIntArray[1] + motionEvent.GetY(pointerIndex));

            // Use ActionMasked here rather than Action to reduce the number of possibilities
            switch (args.Event.ActionMasked)
            {
                case MotionEventActions.Down:
                case MotionEventActions.PointerDown:
                    FireEvent(this, id, TouchActionType.Pressed, screenPointerCoords, true);
                    break;

                case MotionEventActions.Up:
                case MotionEventActions.Pointer1Up:
                    FireEvent(this, id, TouchActionType.Released, screenPointerCoords, false);
                    break;

                case MotionEventActions.Cancel:
                    FireEvent(this, id, TouchActionType.Cancelled, screenPointerCoords, false);
                    break;
            }
        }

        private void FireEvent(
            TouchEffect touchEffect,
            int id, 
            TouchActionType actionType,
            Point pointerLocation,
            bool isInContact)
        {
            // Get the method to call for firing events
            Action<Element, TouchActionEventArgs> onTouchAction = touchEffect.libTouchEffect.OnTouchAction;

            // Get the location of the pointer within the view
            touchEffect.view.GetLocationOnScreen(twoIntArray);
            double x = pointerLocation.X - twoIntArray[0];
            double y = pointerLocation.Y - twoIntArray[1];
            Point point = new Point(fromPixels(x), fromPixels(y));

            // Call the method
            onTouchAction(
                touchEffect.formsElement,
                new TouchActionEventArgs(id, actionType, point, isInContact));
        }

        private sealed class InternalGestureDetector : GestureDetector.SimpleOnGestureListener
        {
            public Action<MotionEvent> TapAction { get; set; }

            public override bool OnSingleTapUp(MotionEvent e)
            {
                TapAction?.Invoke(e);
                return true;
            }
        }
    }
}
