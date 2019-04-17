using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TailwindTraders.Mobile.Features.LogIn;
using TailwindTraders.Mobile.Features.Product;
using TailwindTraders.Mobile.Features.Product.Cart;
using TailwindTraders.Mobile.Features.Scanning.AR;
using TailwindTraders.Mobile.Features.Scanning.Photo;
using TailwindTraders.Mobile.Framework;
using TailwindTraders.Mobile.Helpers;
using Xamarin.Forms;

namespace TailwindTraders.Mobile.Features.Home
{
    public class HomeViewModel : BaseStateAwareViewModel<HomeViewModel.State>
    {
        public enum State
        { 
            EverythingOK,
            Error,
        }

        private IEnumerable<Tuple<string, string, ICommand>> recommendedProducts;
        private IEnumerable<ProductViewModel> popularProducts;
        private IEnumerable<ProductDTO> previouslySeenProducts;

        public HomeViewModel()
        {
            IsBusy = true;

            MessagingCenter.Subscribe<LoginViewModel>(
                this,
                LoginViewModel.LogInFinishedMessage,
                _ => LoadCommand.Execute(null));
        }

        public bool IsNoOneLoggedIn => !AuthenticationService.IsAnyOneLoggedIn;

        public IEnumerable<Tuple<string, string, ICommand>> RecommendedProducts
        {
            get => recommendedProducts;
            set => SetAndRaisePropertyChanged(ref recommendedProducts, value);
        }

        public IEnumerable<ProductViewModel> PopularProducts
        {
            get => popularProducts;
            set => SetAndRaisePropertyChanged(ref popularProducts, value);
        }

        public IEnumerable<ProductDTO> PreviouslySeenProducts
        {
            get => previouslySeenProducts;
            set => SetAndRaisePropertyChanged(ref previouslySeenProducts, value);
        }

        public ICommand PhotoCommand => new AsyncCommand(_ => App.NavigateToAsync(
            new CameraPreviewTakePhotoPage()));

        public ICommand ARCommand => new AsyncCommand(_ => App.NavigateToAsync(new CameraPreviewPage()));

        public ICommand LoadCommand => new AsyncCommand(_ => LoadDataAsync());

        public ICommand CartCommand => new AsyncCommand(_ => App.NavigateToAsync(new ProductCartPage()));

        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();

            if (IsNoOneLoggedIn)
            {
                await App.NavigateModallyToAsync(new LogInPage());
                IsBusy = false;
            }
        }

        public override async Task UninitializeAsync()
        {
            await base.UninitializeAsync();
        }

        private async Task LoadDataAsync()
        {
            CurrentState = State.EverythingOK;

            RecommendedProducts = new List<Tuple<string, string, ICommand>>
            {
                Tuple.Create("Power Tools", "recommended_powertools.jpg", FeatureNotAvailableCommand),
                Tuple.Create("Plants", "recommended_plants.jpg", FeatureNotAvailableCommand),
                Tuple.Create("Bathrooms", "recommended_bathrooms.jpg", FeatureNotAvailableCommand),
                Tuple.Create("Lighting", "recommended_lighting.jpg", FeatureNotAvailableCommand),
            };

            var homeResult = await TryExecuteWithLoadingIndicatorsAsync(
                RestPoolService.HomeAPI.GetAsync(AuthenticationService.AuthorizationHeader));

            if (homeResult.IsError || homeResult.Value == null || homeResult.Value.PopularProducts == null)
            {
                CurrentState = State.Error;
                return;
            }

            var popularProductsRaw = homeResult.Value.PopularProducts;
            var popularProductsWithCommand = popularProductsRaw.Select(
                item => new ProductViewModel(item, FeatureNotAvailableCommand));
            PopularProducts = new List<ProductViewModel>(popularProductsWithCommand);

            var randomProducts = popularProductsRaw.Shuffle().Take(3);
            PreviouslySeenProducts = new List<ProductDTO>(randomProducts);
        }
    }
}
