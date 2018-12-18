using System;
using TailwindTraders.Mobile.Features.Home;
using TailwindTraders.Mobile.Features.LogIn;
using TailwindTraders.Mobile.Features.Product;

namespace TailwindTraders.Mobile.Features.Common
{
    public class FakeRestPoolService : IRestPoolService
    {
        public Lazy<IProfilesAPI> ProfilesAPI { get; } = new Lazy<IProfilesAPI>(() => new FakeProfilesAPI());

        public Lazy<IHomeAPI> HomeAPI { get; } = new Lazy<IHomeAPI>(() => new FakeHomeAPI());

        public Lazy<IProductsAPI> ProductsAPI { get; } = new Lazy<IProductsAPI>(() => new FakeProductsAPI());

        public void UpdateApiUrl(string newApiUrl)
        {
            // Intentionally blank
        }
    }
}
