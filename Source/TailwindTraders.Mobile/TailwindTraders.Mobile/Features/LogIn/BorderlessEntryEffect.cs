using Xamarin.Forms;

namespace TailwindTraders.Mobile.Features.LogIn
{
    public class BorderlessEntryEffect : RoutingEffect
    {
        public BorderlessEntryEffect()
            : base($"{nameof(TailwindTraders)}.{nameof(BorderlessEntryEffect)}")
        {
        }
    }
}
