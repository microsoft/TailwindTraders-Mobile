using System;

// Aliases Func<AppQuery, AppQuery> with Query
using Query = System.Func<Xamarin.UITest.Queries.AppQuery, Xamarin.UITest.Queries.AppQuery>;

namespace TailwindTraders.UITests
{
    public class HomeAppliancesListPage : BasePage
    {
        readonly Query firstListItem;

        protected override PlatformQuery Trait => new PlatformQuery
        {
            Android = x => x.Marked("Home Appliances"),
            iOS = x => x.Marked("Home Appliances")
        };

        public HomeAppliancesListPage()
        {
            firstListItem = x => x.Marked("Microwave 0.9 Cu. Ft. 900 W longlonglonglonglonglonglonglong product name");

            if(OnAndroid)
            {
            }

            if(OniOS)
            {

            }
        }

        public void SelectFirstItem()
        {
            app.WaitForElement(firstListItem);
            app.Tap(firstListItem);
        }
    }
}
