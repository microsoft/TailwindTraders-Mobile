using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TailwindTraders.Mobile.Framework;
using TailwindTraders.Mobile.Helpers;
using Xamarin.Forms;

namespace TailwindTraders.Mobile.Features.Product.Detail
{
    public class ProductDetailViewModel : BaseStateAwareViewModel<ProductDetailViewModel.State>
    {
        private readonly int productId;

        private int productTypeId;
        private string title;
        private IEnumerable<string> pictures;
        private string brand;
        private string name;
        private string price;
        private IEnumerable<FeatureDTO> features;
        private IEnumerable<ProductViewModel> similarProducts;
        private IEnumerable<ProductDTO> alsoBoughtProducts;

        public enum State
        {
            EverythingOK,
            Error,
        }

        public string Title
        {
            get => title;
            set => SetAndRaisePropertyChanged(ref title, value);
        }

        public IEnumerable<string> Pictures
        {
            get => pictures;
            set => SetAndRaisePropertyChanged(ref pictures, value);
        }
        
        public string Brand
        {
            get => brand;
            set => SetAndRaisePropertyChanged(ref brand, value);
        }

        public string Name
        {
            get => name;
            set => SetAndRaisePropertyChanged(ref name, value);
        }

        public string Price
        {
            get => price;
            set => SetAndRaisePropertyChanged(ref price, value);
        }

        public IEnumerable<FeatureDTO> Features
        {
            get => features;
            set => SetAndRaisePropertyChanged(ref features, value);
        }

        public IEnumerable<ProductViewModel> SimilarProducts
        {
            get => similarProducts;
            set => SetAndRaisePropertyChanged(ref similarProducts, value);
        }

        public IEnumerable<ProductDTO> AlsoBoughtProducts
        {
            get => alsoBoughtProducts;
            set => SetAndRaisePropertyChanged(ref alsoBoughtProducts, value);
        }

        public ICommand RefreshCommand { get; }

        public ProductDetailViewModel(int productId)
        {
            this.productId = productId;

            RefreshCommand = new Command(async () => await LoadSecondaryDataAsync());
        }

        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();

            await LoadDataAsync().ConfigureAwait(false);
        }

        private async Task LoadDataAsync()
        {
            var status = await TryExecuteWithLoadingIndicatorsAsync(RequestProductDetailAsync());

            if (status.IsError)
            {
                await App.NavigateBackAsync();
            }

            await LoadSecondaryDataAsync();
        }

        private async Task LoadSecondaryDataAsync()
        {
            var status = await TryExecuteWithLoadingIndicatorsAsync(RequestSimilarAndAlsoBoughtProductsAsync());
            CurrentState = status ? State.EverythingOK : State.Error;
        }

        private async Task RequestProductDetailAsync()
        {
            var product = await RestPoolService.ProductsAPI.GetDetailAsync(
                AuthenticationService.AuthorizationHeader, productId.ToString());

            if (product != null)
            {
                UpdateProduct(product);
            }
        }

        private async Task RequestSimilarAndAlsoBoughtProductsAsync()
        {
            var productsPerType = await RestPoolService.ProductsAPI.GetProductsAsync(
                AuthenticationService.AuthorizationHeader, productTypeId.ToString());

            if (productsPerType != null)
            {
                SimilarProducts = productsPerType.Products
                    .Select(item => new ProductViewModel(item, FeatureNotAvailableCommand));

                var randomProducts = productsPerType.Products.Shuffle().Take(3);
                AlsoBoughtProducts = randomProducts.ToList();
            }
        }

        private void UpdateProduct(ProductDTO product)
        {
            productTypeId = product.Type.Id;

            var brandName = product.Brand.Name;
            var productName = product.Name;
            Title = $"{brandName}. {productName}";
            Pictures = new List<string> { product.ImageUrl };
            Brand = brandName;
            Name = productName;
            Price = $"${product.Price}";
            Features = product.Features;
        }
    }
}
