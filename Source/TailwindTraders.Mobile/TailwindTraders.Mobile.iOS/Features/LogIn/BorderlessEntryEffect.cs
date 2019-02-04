using CoreGraphics;
using TailwindTraders.Mobile.Effects;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportEffect(
    typeof(TailwindTraders.Mobile.IOS.Features.LogIn.BorderlessEntryEffect), nameof(BorderlessEntryEffect))]

namespace TailwindTraders.Mobile.IOS.Features.LogIn
{
    public class BorderlessEntryEffect : PlatformEffect
    {
        protected override void OnAttached()
        {
            var textField = Control as UITextField;
            textField.BorderStyle = UITextBorderStyle.None;

            var layer = textField.Layer;
            var primaryColor = (Color)App.Current.Resources["PrimaryColor"];
            layer.BackgroundColor = primaryColor.ToCGColor();
            var accentColor = (Color)App.Current.Resources["AccentColor"];
            layer.ShadowColor = accentColor.ToCGColor();
            layer.ShadowOffset = new CGSize(0, 2);
            layer.ShadowOpacity = 1;
            layer.ShadowRadius = 0;
        }

        protected override void OnDetached()
        {
        }
    }
}
