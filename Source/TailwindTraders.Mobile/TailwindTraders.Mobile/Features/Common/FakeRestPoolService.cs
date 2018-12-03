using System;
using TailwindTraders.Mobile.Features.Home;
using TailwindTraders.Mobile.Features.LogIn;
using TailwindTraders.Mobile.Features.Product;

namespace TailwindTraders.Mobile.Features.Common
{
    public class FakeRestPoolService : IRestPoolService
    {
        public Lazy<IProfilesAPI> ProfilesAPI => new Lazy<IProfilesAPI>(() => new FakeProfilesAPI());

        public Lazy<IHomeAPI> HomeAPI => new Lazy<IHomeAPI>(() => new FakeHomeAPI());

        public Lazy<IProductsAPI> ProductsAPI => new Lazy<IProductsAPI>(() => new FakeProductsAPI());
    }
}
