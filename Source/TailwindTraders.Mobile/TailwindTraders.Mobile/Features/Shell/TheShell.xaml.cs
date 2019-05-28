using System;
using System.Threading.Tasks;
using TailwindTraders.Mobile.Features.Product.Category;
using Xamarin.Forms;

namespace TailwindTraders.Mobile.Features.Shell
{
    public partial class TheShell
    {
        internal static string ProductCategoryRoute = nameof(ProductCategoryRoute);

        public static readonly TimeSpan TimeFlyoutCloses = TimeSpan.FromSeconds(0.5f);

        public TheShell()
        {   
            InitializeComponent();
            BindingContext = new TheShellViewModel();

            Routing.RegisterRoute(ProductCategoryRoute, typeof(ProductCategoryPage));
        }

        internal async Task CloseFlyoutAsync()
        {
            FlyoutIsPresented = false;
            await Task.Delay(TimeFlyoutCloses);
        }
    }
}
