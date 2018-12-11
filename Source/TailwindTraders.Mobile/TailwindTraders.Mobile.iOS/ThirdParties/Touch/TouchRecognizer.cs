using System;
using System.Linq;
using CoreGraphics;
using Foundation;
using UIKit;
using Xamarin.Forms;

namespace TouchTracking.iOS
{
    public class TouchRecognizer : UIGestureRecognizer
    {
        private readonly Element element;        // Forms element for firing events
        private readonly UIView view;            // iOS UIView 
        private readonly TouchTracking.TouchEffect touchEffect;

        public TouchRecognizer(Element element, UIView view, TouchTracking.TouchEffect touchEffect)
        {
            this.element = element;
            this.view = view;
            this.touchEffect = touchEffect;
        }

        public static void Initialize()
        {
        }

        // touches = touches of interest; evt = all touches of type UITouch
        public override void TouchesBegan(NSSet touches, UIEvent evt)
        {
            base.TouchesBegan(touches, evt);

            foreach (UITouch touch in touches.Cast<UITouch>())
            {
                long id = touch.Handle.ToInt64();
                FireEvent(this, id, TouchActionType.Pressed, touch, true);
            }
        }

        public override void TouchesEnded(NSSet touches, UIEvent evt)
        {
            base.TouchesEnded(touches, evt);

            foreach (UITouch touch in touches.Cast<UITouch>())
            {
                long id = touch.Handle.ToInt64();

                FireEvent(this, id, TouchActionType.Released, touch, false);
            }
        }

        public override void TouchesCancelled(NSSet touches, UIEvent evt)
        {
            base.TouchesCancelled(touches, evt);

            foreach (UITouch touch in touches.Cast<UITouch>())
            {
                long id = touch.Handle.ToInt64();

                FireEvent(this, id, TouchActionType.Cancelled, touch, false);
            }
        }

        private void FireEvent(
            TouchRecognizer recognizer, 
            long id, 
            TouchActionType actionType,
            UITouch touch, 
            bool isInContact)
        {
            // Convert touch location to Xamarin.Forms Point value
            CGPoint cgPoint = touch.LocationInView(recognizer.View);
            Point xfPoint = new Point(cgPoint.X, cgPoint.Y);

            // Get the method to call for firing events
            Action<Element, TouchActionEventArgs> onTouchAction = recognizer.touchEffect.OnTouchAction;

            // Call that method
            onTouchAction(
                recognizer.element,
                new TouchActionEventArgs(id, actionType, xfPoint, isInContact));
        }
    }
}