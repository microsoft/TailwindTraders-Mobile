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
        public void SuccessSignInTest()
        {  
            new LoginPage()
                .EnterCredentials(TestsSettings.TestUsername, TestsSettings.TestPassword)
                .SignIn();

            new HomePage();
        }

        [Test]
        public void EmptySignInTest()
        {
            new LoginPage()
                .EnterCredentials(string.Empty, string.Empty)
                .SignIn();
        }

        [Test]
        public void AddToCartTest()
        {
            SuccessSignInTest();

            new HomePage()
                    .SelectMenuOption("Home Appliances");

            new HomeAppliancesListPage()
                    .SelectFirstItem();

            new ApplianceDetailPage()
                .AddToCart();
        }

        [Test]
        public void SaveSettingsTest()
        {
            SuccessSignInTest();

            new HomePage()
                .SelectMenuOption("Settings");

            new SettingsPage()
                .SaveSettings();
        }
    }
}
