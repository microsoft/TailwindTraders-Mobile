// Aliases Func<AppQuery, AppQuery> with Query
using System;
using Query = System.Func<Xamarin.UITest.Queries.AppQuery, Xamarin.UITest.Queries.AppQuery>;

namespace TailwindTraders.UITests
{
    public class LoginPage : BasePage
    {
        private readonly Query emailField;
        private readonly Query passwordField;
        private readonly Query loginButton;

        protected override PlatformQuery Trait => new PlatformQuery
        {
            Android = x => x.Marked("main.scrollview"),
            iOS = x => x.Marked("header.jpg"),
        };

        public LoginPage()
        {
            emailField = x => x.Marked("username");
            passwordField = x => x.Marked("password");
            loginButton = x => x.Marked("LOG IN");
        }

        public LoginPage EnterCredentials(string userName, string password)
        {
            app.WaitForElement(emailField);
            app.Tap(emailField);
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
