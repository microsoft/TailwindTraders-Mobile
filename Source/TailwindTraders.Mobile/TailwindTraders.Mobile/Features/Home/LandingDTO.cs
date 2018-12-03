using System.Collections.Generic;
using TailwindTraders.Mobile.Features.Product;

namespace TailwindTraders.Mobile.Features.Home
{
    public class LandingDTO
    {
        public IEnumerable<ProductDTO> PopularProducts { get; set; }
    }
}