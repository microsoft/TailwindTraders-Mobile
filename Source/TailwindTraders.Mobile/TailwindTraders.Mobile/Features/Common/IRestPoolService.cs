using TailwindTraders.Mobile.Features.Home;
using TailwindTraders.Mobile.Features.LogIn;
using TailwindTraders.Mobile.Features.Product;
using TailwindTraders.Mobile.Features.Product.Cart;

namespace TailwindTraders.Mobile.Features.Common
{
    public interface IRestPoolService
    {
        IProfilesAPI ProfilesAPI { get; }

        IHomeAPI HomeAPI { get; }

        IProductsAPI ProductsAPI { get; }

        ILoginAPI LoginAPI { get; }

        ISimilarProductsAPI SimilarProductsAPI { get; }

        IProductCartAPI ProductCartAPI { get; }

        void UpdateApiUrl(string newApiUrl);
    }
}