using System.Threading.Tasks;
using Refit;
using TailwindTraders.Mobile.Features.Common;
using TailwindTraders.Mobile.Features.Product;

namespace TailwindTraders.Mobile.Features.Home
{
    public class FakeHomeAPI : IHomeAPI
    {
        public Task<LandingDTO> GetAsync([Header(Settings.Settings.ApiAuthorizationHeader)] string authorizationHeader)
            => FakeNetwork.ReturnAsync(new LandingDTO { PopularProducts = FakeProducts.Fakes, });
    }
}
