// Aliases Func<AppQuery, AppQuery> with Query
using Query = System.Func<Xamarin.UITest.Queries.AppQuery, Xamarin.UITest.Queries.AppQuery>;

namespace TailwindTraders.UITests
{
    public class SettingsPage : BasePage
    {
        private readonly Query saveButton;
        private readonly Query settingsSavedMessage;

        protected override PlatformQuery Trait => new PlatformQuery
        {
            Android = x => x.Marked("Settings"),
            iOS = x => x.Marked("Settings"),
        };

        public SettingsPage()
        {
            saveButton = x => x.Marked("SAVE");
            settingsSavedMessage = x => x.Text("Settings saved!");
        }

        public void SaveSettings()
        {
            app.WaitForElement(saveButton);
            app.Tap(saveButton);
            app.WaitForElement(settingsSavedMessage);
        }
    }
}
