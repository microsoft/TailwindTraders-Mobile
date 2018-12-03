namespace TailwindTraders.Mobile.Features.Product.Detail
{
    public partial class ProductDetailPage
    {
        public ProductDetailPage(int productId)
        {
            InitializeComponent();

            BindingContext = new ProductDetailViewModel(productId);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            NavigationProxy.Inner = App.NavigationRoot.NavigationProxy;
        }
    }
}
