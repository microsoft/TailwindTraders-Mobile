using System;
using Refit;
using TailwindTraders.Mobile.Features.Home;
using TailwindTraders.Mobile.Features.LogIn;
using TailwindTraders.Mobile.Features.Product;
using TailwindTraders.Mobile.Helpers;

namespace TailwindTraders.Mobile.Features.Common
{
    public class RestPoolService : IRestPoolService
    {
        public Lazy<IProfilesAPI> ProfilesAPI { get; private set; }

        public Lazy<IHomeAPI> HomeAPI { get; private set; }

        public Lazy<IProductsAPI> ProductsAPI { get; private set; }

        public RestPoolService()
        {
            UpdateApiUrl(Settings.Settings.RootApiUrl);
        }

        public void UpdateApiUrl(string newApiUrl)
        {
            ProfilesAPI = new Lazy<IProfilesAPI>(
                () => RestService.For<IProfilesAPI>(HttpClientFactory.Create(newApiUrl)));
            HomeAPI = new Lazy<IHomeAPI>(
                () => RestService.For<IHomeAPI>(HttpClientFactory.Create(newApiUrl)));
            ProductsAPI = new Lazy<IProductsAPI>(
                () => RestService.For<IProductsAPI>(HttpClientFactory.Create(newApiUrl)));
        }
    }
}
