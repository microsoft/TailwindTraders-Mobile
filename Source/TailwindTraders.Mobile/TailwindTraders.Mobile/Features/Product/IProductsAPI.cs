using System.Collections.Generic;
using System.Threading.Tasks;
using Refit;
using TailwindTraders.Mobile.Features.Settings;

namespace TailwindTraders.Mobile.Features.Product
{
    public interface IProductsAPI
    {
        [Get("/{id}")]
        Task<ProductDTO> GetDetailAsync(
            [Header(DefaultSettings.ApiAuthorizationHeader)] string authorizationHeader, string id);

        [Get("/")]
        Task<ProductsPerTypeDTO> GetProductsAsync(
            [Header(DefaultSettings.ApiAuthorizationHeader)] string authorizationHeader, string type);

        [Multipart]
        [Post("/imageclassifier")]
        Task<IEnumerable<ProductDTO>> GetSimilarProductsAsync(
            [Header(DefaultSettings.ApiAuthorizationHeader)] string authorizationHeader,
            [AliasAs("file")] StreamPart stream);
    }
}
