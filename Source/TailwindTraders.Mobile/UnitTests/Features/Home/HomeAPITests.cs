using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NUnit.Framework;
using Refit;
using TailwindTraders.Mobile.Features.Common;
using TailwindTraders.Mobile.Features.Home;
using TailwindTraders.Mobile.Features.Settings;

namespace UnitTests.Features.Home
{
    public class HomeAPITests
    {
#if !DEBUG
        [Ignore(Constants.IgnoreReason)]
#endif
        [Test]
        public async Task GetProductsAsync()
        {
            var productsAPI = RestService.For<IHomeAPI>(HttpClientFactory.Create(Settings.ProductApiUrl));
            var home = await productsAPI.GetAsync(Settings.AnonymousToken);

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
