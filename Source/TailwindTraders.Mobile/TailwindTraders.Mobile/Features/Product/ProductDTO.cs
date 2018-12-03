using System.Collections.Generic;

namespace TailwindTraders.Mobile.Features.Product
{
    public class ProductDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public float Price { get; set; }

        public string ImageUrl { get; set; }

        public BrandDTO Brand { get; set; }

        public TypeDTO Type { get; set; }

        public IEnumerable<FeatureDTO> Features { get; set; }
    }
}
