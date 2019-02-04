using TailwindTraders.Mobile.Effects;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportEffect(
    typeof(TailwindTraders.Mobile.IOS.Effects.MultilineButtonEffect), nameof(MultilineButtonEffect))]

namespace TailwindTraders.Mobile.IOS.Effects
{
    public class MultilineButtonEffect : PlatformEffect
    {
        protected override void OnAttached()
        {
            var button = Control as UIButton;
            button.TitleLabel.LineBreakMode = UILineBreakMode.WordWrap;
        }

        protected override void OnDetached()
        {
        }
    }
}
