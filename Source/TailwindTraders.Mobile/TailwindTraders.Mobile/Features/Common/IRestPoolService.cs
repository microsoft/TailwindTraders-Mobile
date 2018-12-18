using System;
using TailwindTraders.Mobile.Features.Home;
using TailwindTraders.Mobile.Features.LogIn;
using TailwindTraders.Mobile.Features.Product;

namespace TailwindTraders.Mobile.Features.Common
{
    public interface IRestPoolService
    {
        Lazy<IProfilesAPI> ProfilesAPI { get; }

        Lazy<IHomeAPI> HomeAPI { get; }

        Lazy<IProductsAPI> ProductsAPI { get; }

        void UpdateApiUrl(string newApiUrl);
    }
}