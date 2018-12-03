using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using TailwindTraders.Mobile.Features.Common;
using TailwindTraders.Mobile.Features.Localization;
using TailwindTraders.Mobile.Features.Product;
using Xamarin.Forms;

namespace TailwindTraders.Mobile.Features.Scanning.Photo
{
    public class CameraViewModel : BaseViewModel
    {
        private readonly PhotoService photoService;
        private readonly IVisionService visionService;

        public ICommand CloseCommand => new AsyncCommand(App.NavigationRoot.Navigation.PopModalAsync);

        public ICommand AddCommand => FeatureNotAvailableCommand;

        public ICommand TakePhotoCommand => new AsyncCommand(TryTakePhotoAsync);

        private string cameraImage;

        public string CameraImage
        {
            get => cameraImage;
            set => SetAndRaisePropertyChangedIfDifferentValues(ref cameraImage, value);
        }

        private List<ProductDTO> recommendedProducts;

        public List<ProductDTO> RecommendedProducts
        {
            get => recommendedProducts;
            set => SetAndRaisePropertyChangedIfDifferentValues(ref recommendedProducts, value);
        }

        public CameraViewModel()
        {
            photoService = DependencyService.Get<PhotoService>();
            visionService = DependencyService.Get<IVisionService>();
        }

        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();

            await TryTakePhotoAsync();
        }

        private async Task TryTakePhotoAsync()
        {
            var isInitialized = await photoService.InitializeCameraAsync();
            if (!isInitialized)
            {
                await Application.Current.MainPage.DisplayAlert(
                   Resources.Alert_Title_NoCameraSupport,
                   Resources.Alert_Message_NoCameraSupport,
                   Resources.Alert_OK);

                await App.NavigationRoot.Navigation.PopModalAsync();

                return;
            }

            var photoTakeResult = await photoService.TakePhotoAsync();
            if (!photoTakeResult.IsSucceded)
            {
                await Application.Current.MainPage.DisplayAlert(
                   Resources.Alert_Title_NoCameraAccess,
                   Resources.Alert_Message_NoCameraAccess,
                   Resources.Alert_OK);

                await App.NavigationRoot.Navigation.PopModalAsync();

                return;
            }

            var wasPhotoTaken = !string.IsNullOrEmpty(photoTakeResult.Result);
            if (!wasPhotoTaken)
            {
                return;
            }

            CameraImage = photoTakeResult.Result;

            var visionResult = await ExecuteWithLoadingIndicatorsAsync(
                () => visionService.GetRecommendedProductsFromPhotoAsync(CameraImage));

            var gotRecommendedProducts = visionResult.IsSucceded && visionResult.Result != 
                default(IEnumerable<ProductDTO>);
            if (!gotRecommendedProducts)
            {
                return;
            }

            RecommendedProducts = new List<ProductDTO>(visionResult.Result);
        }
    }
}
