using System.Collections.Generic;
using Newtonsoft.Json;
using TailwindTraders.Mobile.Helpers;

namespace TailwindTraders.Mobile.Features.Product
{
    public static class FakeProducts
    {
        public static IEnumerable<ProductDTO> Fakes => 
            JsonConvert.DeserializeObject<IEnumerable<ProductDTO>>(
                EmbeddedResourceHelper.Load("TailwindTraders.Mobile.Features.Product.FakeProducts.json"));
    }
}
