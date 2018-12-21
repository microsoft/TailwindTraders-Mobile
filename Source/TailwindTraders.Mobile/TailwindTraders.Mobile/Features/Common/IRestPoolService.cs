using TailwindTraders.Mobile.Features.Home;
using TailwindTraders.Mobile.Features.LogIn;
using TailwindTraders.Mobile.Features.Product;

namespace TailwindTraders.Mobile.Features.Common
{
    public interface IRestPoolService
    {
        IProfilesAPI ProfilesAPI { get; }

        IHomeAPI HomeAPI { get; }

        IProductsAPI ProductsAPI { get; }

        void UpdateApiUrl(string newApiUrl);
    }
}