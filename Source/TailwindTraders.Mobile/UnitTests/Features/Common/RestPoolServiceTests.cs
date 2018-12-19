using NUnit.Framework;
using TailwindTraders.Mobile.Features.Common;
using TailwindTraders.Mobile.Features.Settings;

namespace UnitTests.Features.Common
{
    public class RestPoolServiceTests
    {
        private const string NewApiUrl = "foo";

        private RestPoolService service;

        public RestPoolServiceTests()
        {
            DefaultSettings.RootApiUrl = "http://foo.bar";
        }

        [SetUp]
        public void SetUp()
        {
            service = new RestPoolService();
        }

        [Test]
        public void HomeApiIsRecreated()
        {
            var useless = service.HomeAPI.Value;

            Assert.True(service.HomeAPI.IsValueCreated);

            service.UpdateApiUrl(NewApiUrl);

            Assert.False(service.HomeAPI.IsValueCreated);
        }

        [Test]
        public void ProductsApiIsRecreated()
        {
            var useless = service.ProductsAPI.Value;

            Assert.True(service.ProductsAPI.IsValueCreated);

            service.UpdateApiUrl(NewApiUrl);

            Assert.False(service.ProductsAPI.IsValueCreated);
        }

        [Test]
        public void ProfilesApiIsRecreated()
        {
            var useless = service.ProfilesAPI.Value;

            Assert.True(service.ProfilesAPI.IsValueCreated);

            service.UpdateApiUrl(NewApiUrl);

            Assert.False(service.ProfilesAPI.IsValueCreated);
        }
    }
}
