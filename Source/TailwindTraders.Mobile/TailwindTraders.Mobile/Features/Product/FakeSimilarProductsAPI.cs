using System.Collections.Generic;
using System.Threading.Tasks;
using Refit;

namespace TailwindTraders.Mobile.Features.Product
{
    public class FakeSimilarProductsAPI : ISimilarProductsAPI
    {
        public Task<IEnumerable<ProductDTO>> GetSimilarProductsAsync(
            [Header("Authorization")] string authorizationHeader,
            [AliasAs("file")] StreamPart stream)
        {
            var result = new List<ProductDTO>();
            result.Add(new ProductDTO());

            return Task.FromResult<IEnumerable<ProductDTO>>(result);
        }
    }
}