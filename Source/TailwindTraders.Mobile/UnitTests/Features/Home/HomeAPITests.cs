using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NUnit.Framework;
using Refit;
using TailwindTraders.Mobile.Features.Home;
using TailwindTraders.Mobile.Features.Settings;
using TailwindTraders.Mobile.Helpers;
using UnitTests.Framework;

namespace UnitTests.Features.Home
{
    public class HomeAPITests : BaseAPITest
    {
#if !DEBUG
        [Ignore(Constants.IgnoreReason)]
#endif
        [Test]
        public async Task GetProductsAsync()
        {
            var productsAPI = RestService.For<IHomeAPI>(HttpClientFactory.Create(DefaultSettings.RootApiUrl));
            var home = await this.PreauthenticateAsync(() => productsAPI.GetAsync("Bearer " + DefaultSettings.AccessToken));

            Assert.IsNotEmpty(home.PopularProducts);
        }

        [Test]
        public void NullProductsPayload()
        {
            Assert.Throws<ArgumentNullException>(() => JsonConvert.DeserializeObject<LandingDTO>(null));
        }

        [Test]
        public void EmptyProductsPayload()
        {
            var dto = JsonConvert.DeserializeObject<LandingDTO>(string.Empty);

            Assert.Null(dto);
        }
    }
}
