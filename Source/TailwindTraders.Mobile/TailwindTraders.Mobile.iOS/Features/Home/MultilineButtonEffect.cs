using TailwindTraders.Mobile.Features.Home;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportEffect(
    typeof(TailwindTraders.Mobile.IOS.Features.Home.MultilineButtonEffect), nameof(MultilineButtonEffect))]

namespace TailwindTraders.Mobile.IOS.Features.Home
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
