using System.Threading.Tasks;
using NUnit.Framework;
using Refit;
using TailwindTraders.Mobile.Features.Common;
using TailwindTraders.Mobile.Features.LogIn;
using TailwindTraders.Mobile.Features.Settings;

namespace UnitTests.Features.LogIn
{
#if !DEBUG
    [Ignore(Constants.IgnoreReason)]
#endif
    public class ProfilesAPITests
    {
        [Test]
        public async Task GetAsync()
        {
            var profileAPI = RestService.For<IProfilesAPI>(HttpClientFactory.Create(Settings.ProfilesApiUrl));
            var profiles = await profileAPI.GetAsync(Settings.AnonymousToken);

            Assert.IsNotEmpty(profiles);
        }
    }
}
