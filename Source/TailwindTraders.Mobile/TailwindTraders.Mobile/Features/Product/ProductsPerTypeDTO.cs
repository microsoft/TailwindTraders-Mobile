using System.Collections.Generic;

namespace TailwindTraders.Mobile.Features.Product
{
    public class ProductsPerTypeDTO
    {
        public IEnumerable<ProductDTO> Products { get; set; }

        public IEnumerable<BrandDTO> Brands { get; set; }

        public IEnumerable<TypeDTO> Types { get; set; }
    }
}
