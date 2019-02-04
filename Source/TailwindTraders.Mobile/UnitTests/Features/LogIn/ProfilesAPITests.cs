using System.Threading.Tasks;
using NUnit.Framework;
using Refit;
using TailwindTraders.Mobile.Features.LogIn;
using TailwindTraders.Mobile.Features.Settings;
using TailwindTraders.Mobile.Helpers;

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
            var profileAPI = RestService.For<IProfilesAPI>(HttpClientFactory.Create(DefaultSettings.ProfilesApiUrl));
            var profiles = await profileAPI.GetAsync(DefaultSettings.AnonymousToken);

            Assert.IsNotEmpty(profiles);
        }
    }
}
