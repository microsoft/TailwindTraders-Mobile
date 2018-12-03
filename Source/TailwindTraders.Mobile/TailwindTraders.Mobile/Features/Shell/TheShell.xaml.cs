using System;
using System.Threading.Tasks;

namespace TailwindTraders.Mobile.Features.Shell
{
    public partial class TheShell
    {
        public static readonly TimeSpan TimeFlyoutCloses = TimeSpan.FromSeconds(0.5f);

        public TheShell()
        {
            InitializeComponent();

            BindingContext = new TheShellViewModel();
        }

        internal async Task CloseFlyoutAsync()
        {
            FlyoutIsPresented = false;
            await Task.Delay(TimeFlyoutCloses);
        }
    }
}
