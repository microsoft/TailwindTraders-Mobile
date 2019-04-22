using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TailwindTraders.Mobile.Features.Product.Cart
{
    public class FakeProductCartAPI : IProductCartAPI
    {
        private readonly List<ProductCartLineDTO> lines;

        public FakeProductCartAPI()
        {
            lines = new List<ProductCartLineDTO>();
        }

        public async Task<ProductCartLineDTO> AddProductAsync(ProductDTO product)
        {
            // Simulate delay
            await Task.Delay(TimeSpan.FromSeconds(1));

            if (lines.Where(line => line.Product.Id == product.Id).Any())
            {
                var existingLine = lines.Where(line => line.Product.Id == product.Id).FirstOrDefault();
                existingLine.Quantity++;

                return await Task.FromResult(existingLine);
            }
            else
            {
                var newLine = new ProductCartLineDTO
                {
                    Quantity = 1,
                    Product = product,
                };

                lines.Add(newLine);
                return await Task.FromResult(newLine);
            }
        }

        public async Task<List<ProductCartLineDTO>> GetCartLinesAsync()
        {
            // Simulate delay
            await Task.Delay(TimeSpan.FromSeconds(1));

            return await Task.FromResult(lines);
        }
    }
}
