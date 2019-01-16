using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Plugin.Permissions.Abstractions;
using TailwindTraders.Mobile.Features.Common;
using TailwindTraders.Mobile.Features.Product;
using TailwindTraders.Mobile.Framework;
using Xamarin.Forms;

namespace TailwindTraders.Mobile.Features.Scanning.AR
{
    public class CameraPreviewViewModel : BaseViewModel
    {
        public const string AddCameraControlMessage = nameof(AddCameraControlMessage);

        private readonly PhotoService photoService;
        private readonly IProductsAPI productsAPI;
        private static readonly TimeSpan minSameMessageLabelTime = TimeSpan.FromSeconds(3);

        private IEnumerable<ProductViewModel> recommendedProducts;
        private string lastMessageLabel = string.Empty;
        private string lastProcessedMessageLabel = string.Empty;
        private DateTime lastMessageDate = DateTime.MinValue;
        private TimeSpan sameMessageLabelTime = TimeSpan.Zero;
        private Task loadingTask = Task.CompletedTask;

        public CameraPreviewViewModel()
        {
            photoService = DependencyService.Get<PhotoService>();
            productsAPI = DependencyService.Get<IRestPoolService>().ProductsAPI.Value;
        }

        public IEnumerable<ProductViewModel> RecommendedProducts
        {
            get => recommendedProducts;
            set => SetAndRaisePropertyChanged(ref recommendedProducts, value);
        }

        public override async Task InitializeAsync()
        {
            MessagingCenter.Instance.Subscribe<TensorflowLiteService, DetectionMessage>(
                this, 
                TensorflowLiteService.ObjectDetectedMessage, 
                (_, message) => GatherRecommendedProducts(message));

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
            MessagingCenter.Instance.Unsubscribe<TensorflowLiteService, DetectionMessage>(
                this,
                TensorflowLiteService.ObjectDetectedMessage);

            return base.UninitializeAsync();
        }

        private void GatherRecommendedProducts(DetectionMessage message)
        {
            if (lastProcessedMessageLabel == message.Label || !loadingTask.IsCompleted)
            {
                return;
            }

            var now = DateTime.UtcNow;

            if (lastMessageLabel == message.Label)
            {
                sameMessageLabelTime += now - lastMessageDate;
            }
            else
            {
                lastMessageLabel = message.Label;
                sameMessageLabelTime = TimeSpan.Zero;
            }

            if (sameMessageLabelTime >= minSameMessageLabelTime)
            {
                // TODO rely on message.Label when having final network
                loadingTask = LoadRecommendedProductsAsync("1");
                lastProcessedMessageLabel = lastMessageLabel;
                lastMessageLabel = string.Empty;
                sameMessageLabelTime = -(minSameMessageLabelTime + minSameMessageLabelTime);
            }

            lastMessageDate = now;
        }

        private async Task LoadRecommendedProductsAsync(string productType)
        {
            var result = await TryExecuteWithLoadingIndicatorsAsync(
                () => productsAPI.GetProductsAsync(Settings.Settings.AnonymousToken, productType));

            if (result.IsSucceded)
            {
                RecommendedProducts = result.Result.Products.Select(
                    item => new ProductViewModel(item, FeatureNotAvailableCommand));
            }
        }
    }
}
