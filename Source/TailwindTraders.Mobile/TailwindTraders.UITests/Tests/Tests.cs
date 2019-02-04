using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Xamarin.UITest;
using Xamarin.UITest.Queries;

namespace TailwindTraders.UITests
{
    public class Tests : BaseTestFixture
    {
        public Tests(Platform platform)
            : base(platform)
        {
        }

        [Test]
        public void AddToCartTest()
        {
            new HomePage()
                    .SelectMenuOption("Home Appliances");

            new HomeAppliancesListPage()
                    .SelectFirstItem();

            new ApplianceDetailPage()
                .AddToCart();
        }

        ///[Test]
        public void Repl()
        {
            app.Repl();
        }
    }
}
