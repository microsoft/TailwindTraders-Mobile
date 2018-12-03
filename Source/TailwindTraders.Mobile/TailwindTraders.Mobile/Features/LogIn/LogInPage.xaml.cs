using Xamarin.Forms;

namespace TailwindTraders.Mobile.Features.LogIn
{
    public partial class LogInPage
    {
        public LogInPage()
        {
            NavigationPage.SetHasNavigationBar(this, false);

            InitializeComponent();

            BindingContext = new LoginViewModel();
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }
    }
}
