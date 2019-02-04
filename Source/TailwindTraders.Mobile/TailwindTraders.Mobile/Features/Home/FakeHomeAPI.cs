using System.Threading.Tasks;
using Refit;
using TailwindTraders.Mobile.Features.Product;
using TailwindTraders.Mobile.Features.Settings;
using TailwindTraders.Mobile.Helpers;

namespace TailwindTraders.Mobile.Features.Home
{
    public class FakeHomeAPI : IHomeAPI
    {
        public Task<LandingDTO> GetAsync([Header(DefaultSettings.ApiAuthorizationHeader)] string authorizationHeader)
            => FakeNetwork.ReturnAsync(new LandingDTO { PopularProducts = FakeProducts.Fakes, });
    }
}
