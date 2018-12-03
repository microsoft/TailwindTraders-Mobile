using System.Collections.Generic;
using System.Threading.Tasks;
using TailwindTraders.Mobile.Features.Common;
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
            var response = await ExecuteWithLoadingIndicatorsAsync(
                () => productsAPI.GetDetailAsync(AuthenticationService.AuthorizationHeader, product.ToString()));

            if (!response.IsSucceded || response.Result == null)
            {
                await App.NavigateBackAsync();
                return;
            }

            var result = response.Result;
            var brandName = result.Brand.Name;
            var productName = result.Name;
            Title = $"{brandName}. {productName}";
            Pictures = new List<string> { result.ImageUrl };
            Brand = brandName;
            Name = productName;
            Price = $"${result.Price}";
            Features = result.Features;
        }
    }
}
