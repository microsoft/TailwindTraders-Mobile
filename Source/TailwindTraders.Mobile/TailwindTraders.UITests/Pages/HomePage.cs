using System;

// Aliases Func<AppQuery, AppQuery> with Query
using Query = System.Func<Xamarin.UITest.Queries.AppQuery, Xamarin.UITest.Queries.AppQuery>;

namespace TailwindTraders.UITests
{
    public class HomePage : BasePage
    {
        private readonly Query menuButton;
        private readonly Func<string, Query> menuOptions;

        protected override PlatformQuery Trait => new PlatformQuery
        {
            Android = x => x.Marked("main.scrollview"),
            iOS = x => x.Marked("header.jpg"),
        };

        public HomePage()
        {
            menuOptions = optionName => x => x.Marked(optionName);

            if (OnAndroid)
            {
                menuButton = x => x.Marked("OK");
            }

            if (OniOS)
            {
                menuButton = x => x.Marked("3bar");
            }
        }

        public void SelectMenuOption(string option)
        {
            app.WaitForElement(menuButton);
            app.Tap(menuButton);

            app.WaitForElement(menuOptions(option));
            app.Tap(menuOptions(option));
        }
    }
}
