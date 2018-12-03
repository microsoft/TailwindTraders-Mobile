using System;
using Refit;
using TailwindTraders.Mobile.Features.Home;
using TailwindTraders.Mobile.Features.LogIn;
using TailwindTraders.Mobile.Features.Product;

namespace TailwindTraders.Mobile.Features.Common
{
    public class RestPoolService : IRestPoolService
    {
        public Lazy<IProfilesAPI> ProfilesAPI => new Lazy<IProfilesAPI>(
            () => RestService.For<IProfilesAPI>(HttpClientFactory.Create(Settings.Settings.ProfilesApiUrl)));

        public Lazy<IHomeAPI> HomeAPI => new Lazy<IHomeAPI>(
            () => RestService.For<IHomeAPI>(HttpClientFactory.Create(Settings.Settings.ProductApiUrl)));

        public Lazy<IProductsAPI> ProductsAPI => new Lazy<IProductsAPI>(
            () => RestService.For<IProductsAPI>(HttpClientFactory.Create(Settings.Settings.ProductApiUrl)));
    }
}
