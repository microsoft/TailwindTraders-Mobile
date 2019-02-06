// Aliases Func<AppQuery, AppQuery> with Query
using Query = System.Func<Xamarin.UITest.Queries.AppQuery, Xamarin.UITest.Queries.AppQuery>;

namespace TailwindTraders.UITests
{
    public class LoginPage : BasePage
    {
        private readonly Query userNameField;
        private readonly Query passwordField;
        private readonly Query loginButton;

        protected override PlatformQuery Trait => new PlatformQuery
        {
            Android = x => x.Marked("main.scrollview"),
            iOS = x => x.Marked("logo_horizontal_b.png"),
        };

        public LoginPage()
        {
            userNameField = x => x.Marked("usernameField");
            passwordField = x => x.Marked("passwordField");
            loginButton = x => x.Marked("loginButton");
        }

        public LoginPage EnterCredentials(string userName, string password)
        {
            app.WaitForElement(userNameField);
            app.Tap(userNameField);
            app.EnterText(userName);
            app.DismissKeyboard();

            app.Tap(passwordField);
            app.EnterText(password);
            app.DismissKeyboard();

            app.Screenshot("Credentials Entered");

            return this;
        }

        public void SignIn()
        { 
            app.Tap(loginButton);
        }
    }
}
