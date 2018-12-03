using System.Collections.Generic;
using System.Threading.Tasks;
using Refit;

namespace TailwindTraders.Mobile.Features.Product
{
    public interface IProductsAPI
    {
        [Get("/{id}")]
        Task<ProductDTO> GetDetailAsync(
            [Header(Settings.Settings.ApiAuthorizationHeader)] string authorizationHeader, string id);

        [Get("/")]
        Task<ProductsPerTypeDTO> GetProductsAsync(
            [Header(Settings.Settings.ApiAuthorizationHeader)] string authorizationHeader, string type);

        [Multipart]
        [Post("/imageclassifier")]
        Task<IEnumerable<ProductDTO>> GetSimilarProductsAsync(
            [Header(Settings.Settings.ApiAuthorizationHeader)] string authorizationHeader,
            [AliasAs("file")] StreamPart stream);
    }
}
