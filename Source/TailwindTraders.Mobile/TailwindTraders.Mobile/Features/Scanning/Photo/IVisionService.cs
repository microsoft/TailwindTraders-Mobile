using System.Collections.Generic;
using System.Threading.Tasks;
using TailwindTraders.Mobile.Features.Product;

namespace TailwindTraders.Mobile.Features.Scanning.Photo
{
    public interface IVisionService
    {
        Task<IEnumerable<ProductDTO>> GetRecommendedProductsFromPhotoAsync(string photoPath);
    }
}
