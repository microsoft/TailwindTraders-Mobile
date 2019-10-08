using Refit;
using TailwindTraders.Mobile.Features.Home;
using TailwindTraders.Mobile.Features.LogIn;
using TailwindTraders.Mobile.Features.Product;
using TailwindTraders.Mobile.Features.Product.Cart;
using TailwindTraders.Mobile.Features.Settings;
using TailwindTraders.Mobile.Helpers;

namespace TailwindTraders.Mobile.Features.Common
{
    public class RestPoolService : IRestPoolService
    {
        public IProfilesAPI ProfilesAPI { get; private set; }

        public IHomeAPI HomeAPI { get; private set; }

        public IProductsAPI ProductsAPI { get; private set; }

        public ILoginAPI LoginAPI { get; private set; }

        public ISimilarProductsAPI SimilarProductsAPI { get; private set; }

        // There is no real API for products cart, meanwhile must use a faked one
        public IProductCartAPI ProductCartAPI => new FakeProductCartAPI();

        public RestPoolService()
        {
            UpdateApiUrl(DefaultSettings.RootApiUrl);
        }

        public void UpdateApiUrl(string newApiUrl)
        {
            LoginAPI = RestService.For<ILoginAPI>(
                HttpClientFactory.Create(newApiUrl));
            ProfilesAPI = RestService.For<IProfilesAPI>(
                HttpClientFactory.Create(DefaultSettings.RootWebApiUrl));
            HomeAPI = RestService.For<IHomeAPI>(
                HttpClientFactory.Create(newApiUrl));
            ProductsAPI = RestService.For<IProductsAPI>(
                HttpClientFactory.Create(newApiUrl));
            SimilarProductsAPI = RestService.For<ISimilarProductsAPI>(
                HttpClientFactory.Create(newApiUrl));
        }
    }
}
