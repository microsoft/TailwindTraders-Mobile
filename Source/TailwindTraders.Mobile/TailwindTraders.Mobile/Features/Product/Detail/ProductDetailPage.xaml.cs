namespace TailwindTraders.Mobile.Features.Product.Detail
{
    public partial class ProductDetailPage
    {
        public ProductDetailPage(int productId)
        {
            InitializeComponent();

            BindingContext = new ProductDetailViewModel(productId);
        }
    }
}
