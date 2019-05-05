using System;
using System.Collections.Generic;
using System.Text;

namespace TailwindTraders.Mobile.Features.Product.Cart
{
    public class ProductCartLineDTO
    {
        public int Quantity { get; set; }

        public ProductDTO Product { get; set; }
    }
}
