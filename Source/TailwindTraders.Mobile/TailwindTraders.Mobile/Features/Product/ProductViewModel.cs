using System.Windows.Input;

namespace TailwindTraders.Mobile.Features.Product
{
    public class ProductViewModel
    {
        public ProductViewModel(ProductDTO product, ICommand command)
        {
            Product = product;
            Command = command;
        }

        public ProductDTO Product { get; }

        public ICommand Command { get; }
    }
}
