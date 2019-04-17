using System.Collections.Generic;
using System.Threading.Tasks;

namespace TailwindTraders.Mobile.Features.Product.Cart
{
    public interface IProductCartAPI
    {
        Task<List<ProductCartLineDTO>> GetCartLinesAsync();

        Task<ProductCartLineDTO> AddProductAsync(ProductDTO product); 
    }
}
