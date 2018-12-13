using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TailwindTraders.Mobile.Features.Common;
using TailwindTraders.Mobile.Framework;
using Xamarin.Forms;

namespace TailwindTraders.Mobile.Features.Product.Detail
{
    public class ProductDetailViewModel : BaseViewModel
    {
        private readonly IProductsAPI productsAPI;
        private readonly int productId;

        private string title;
        private IEnumerable<string> pictures;
        private string brand;
        private string name;
        private string price;
        private IEnumerable<FeatureDTO> features;
        private IEnumerable<ProductViewModel> similarProducts;

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

        public ProductDetailViewModel(int productId)
        {
            productsAPI = DependencyService.Get<IRestPoolService>().ProductsAPI.Value;

            this.productId = productId;
        }

        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();

            await LoadDataAsync(productId).ConfigureAwait(false);
        }

        private async Task LoadDataAsync(int product)
        {
            var detailResponse = await TryExecuteWithLoadingIndicatorsAsync(
                () => productsAPI.GetDetailAsync(AuthenticationService.AuthorizationHeader, product.ToString()));

            if (!detailResponse.IsSucceded || detailResponse.Result == null)
            {
                await App.NavigateBackAsync();
                return;
            }

            var result = detailResponse.Result;
            var brandName = result.Brand.Name;
            var productName = result.Name;
            Title = $"{brandName}. {productName}";
            Pictures = new List<string> { result.ImageUrl };
            Brand = brandName;
            Name = productName;
            Price = $"${result.Price}";
            Features = result.Features;

            if (result.Type != null)
            {
                var type = result.Type.Id.ToString();
                var similarResponse = await TryExecuteWithLoadingIndicatorsAsync(
                    () => productsAPI.GetProductsAsync(AuthenticationService.AuthorizationHeader, type));

                if (similarResponse.IsSucceded && similarResponse.Result != null)
                {
                    SimilarProducts = similarResponse.Result.Products
                        .Select(item => new ProductViewModel(item, FeatureNotAvailableCommand));
                }
            }
        }
    }
}
