using NUnit.Framework;
using TailwindTraders.Mobile.Features.Common;

namespace UnitTests.Features.Common
{
    public class RestPoolServiceTests
    {
        private const string NewApiUrl = "http://foo.bar";

        private RestPoolService service;

        [SetUp]
        public void SetUp()
        {
            service = new RestPoolService();
        }

        [Test]
        public void HomeApiIsRecreated()
        {
            var initialHash = service.HomeAPI.GetHashCode();

            service.UpdateApiUrl(NewApiUrl);

            Assert.AreNotEqual(service.HomeAPI.GetHashCode(), initialHash);
        }

        [Test]
        public void ProductsApiIsRecreated()
        {
            var initialHash = service.ProductsAPI.GetHashCode();

            service.UpdateApiUrl(NewApiUrl);

            Assert.AreNotEqual(service.ProductsAPI.GetHashCode(), initialHash);
        }

        [Test]
        public void ProfilesApiIsRecreated()
        {
            var initialHash = service.ProfilesAPI.GetHashCode();

            service.UpdateApiUrl(NewApiUrl);

            Assert.AreNotEqual(service.ProfilesAPI.GetHashCode(), initialHash);
        }
    }
}
