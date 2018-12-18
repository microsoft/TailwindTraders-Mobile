using NUnit.Framework;
using TailwindTraders.Mobile.Features.Common;
using TailwindTraders.Mobile.Features.Settings;
using System;

namespace UnitTests.Features.Common
{
    public class RestPoolServiceTests
    {
        [Test]
        public void HomeApiIsRecreated()
        {
            Settings.RootApiUrl = "http://github.com";

            var service = new RestPoolService();

            Assert.False(service.HomeAPI.IsValueCreated);

            try
            {
                var useless = service.HomeAPI.Value;
            }
            catch (TypeLoadException)
            {
            }

            Assert.True(service.HomeAPI.IsValueCreated);

            service.UpdateApiUrl("foo");

            Assert.False(service.HomeAPI.IsValueCreated);
        }
    }
}
