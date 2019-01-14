using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Plugin.Permissions.Abstractions;
using PubSub.Extension;
using TailwindTraders.Mobile.Features.Common;
using TailwindTraders.Mobile.Features.Product;
using TailwindTraders.Mobile.Framework;
using TailwindTraders.Mobile.Helpers;
using Xamarin.Forms;

namespace TailwindTraders.Mobile.Features.Scanning.AR
{
    public class CameraPreviewViewModel : BaseViewModel
    {
        public const string AddCameraControlMessage = nameof(AddCameraControlMessage);

        private readonly PhotoService photoService;
        private readonly IProductsAPI productsAPI;

        private IEnumerable<ProductDTO> recommendedProducts;

        public CameraPreviewViewModel()
        {
            photoService = DependencyService.Get<PhotoService>();
            productsAPI = DependencyService.Get<IRestPoolService>().ProductsAPI.Value;
        }

        public IEnumerable<ProductDTO> RecommendedProducts
        {
            get => recommendedProducts;
            set => SetAndRaisePropertyChanged(ref recommendedProducts, value);
        }

        public override async Task InitializeAsync()
        {
            // TODO rely on message.Label when having final network
            this.Subscribe<DetectionMessage>(message => LoadRecommendedProductsAsync("1").ConfigureAwait(true));

            await base.InitializeAsync();

            var cameraHasInitializedAndAdded = await photoService.CheckPermissionsAsync(
                Permission.Storage,
                Permission.Camera);
            if (!cameraHasInitializedAndAdded)
            {
                await App.NavigateBackAsync();
                return;
            }

            MessagingCenter.Send(this, AddCameraControlMessage);
        }

        public override Task UninitializeAsync()
        {
            this.Unsubscribe<DetectionMessage>();

            return base.UninitializeAsync();
        }

        private async Task LoadRecommendedProductsAsync(string productType)
        {
            var result = await TryExecuteWithLoadingIndicatorsAsync(
                () => productsAPI.GetProductsAsync(Settings.Settings.AnonymousToken, productType));

            if (result.IsSucceded)
            {
                var randomProducts = result.Result.Products.Shuffle().Take(3);
                RecommendedProducts = new List<ProductDTO>(randomProducts);
            }
        }
    }
}
