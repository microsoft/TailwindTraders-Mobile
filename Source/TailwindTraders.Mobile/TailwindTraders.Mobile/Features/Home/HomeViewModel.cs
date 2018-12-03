using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TailwindTraders.Mobile.Features.Common;
using TailwindTraders.Mobile.Features.LogIn;
using TailwindTraders.Mobile.Features.Product;
using TailwindTraders.Mobile.Features.Scanning.Photo;
using Xamarin.Forms;

namespace TailwindTraders.Mobile.Features.Home
{
    public class HomeViewModel : BaseStateAwareViewModel<HomeViewModel.State>
    {
        private readonly IHomeAPI homeAPI;

        public enum State
        { 
            EverythingOK,
            Error,
        }

        private IEnumerable<Tuple<string, string, ICommand>> recommendedProducts;
        private IEnumerable<Tuple<ProductDTO, ICommand>> popularProducts;
        private IEnumerable<ProductDTO> previouslySeenProducts;

        public HomeViewModel()
        {
            homeAPI = DependencyService.Get<IRestPoolService>().HomeAPI.Value;

            IsBusy = true;
        }

        public bool IsNoOneLoggedIn => !AuthenticationService.IsAnyOneLoggedIn;

        public IEnumerable<Tuple<string, string, ICommand>> RecommendedProducts
        {
            get => recommendedProducts;
            set => SetAndRaisePropertyChanged(ref recommendedProducts, value);
        }

        public IEnumerable<Tuple<ProductDTO, ICommand>> PopularProducts
        {
            get => popularProducts;
            set => SetAndRaisePropertyChanged(ref popularProducts, value);
        }

        public IEnumerable<ProductDTO> PreviouslySeenProducts
        {
            get => previouslySeenProducts;
            set => SetAndRaisePropertyChanged(ref previouslySeenProducts, value);
        }

        public ICommand PhotoCommand => new AsyncCommand(_ => App.NavigateModallyToAsync(
            new CameraPage(), 
            animated: false));

        public ICommand ARCommand => FeatureNotAvailableCommand;

        public ICommand LoadCommand => new AsyncCommand(_ => LoadDataAsync());

        public override async Task InitializeAsync()
        {
            MessagingCenter.Subscribe<LoginViewModel>(
                this,
                LoginViewModel.LogInFinishedMessage,
                _ => LoadCommand.Execute(null));

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

            MessagingCenter.Unsubscribe<LoginViewModel>(this, LoginViewModel.LogInFinishedMessage);
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

            var homeResult = await ExecuteWithLoadingIndicatorsAsync(
                () => homeAPI.GetAsync(AuthenticationService.AuthorizationHeader));

            if (!homeResult.IsSucceded || homeResult.Result == null || homeResult.Result.PopularProducts == null)
            {
                CurrentState = State.Error;
                return;
            }

            var popularProductsRaw = homeResult.Result.PopularProducts;
            var popularProductsWithCommand = popularProductsRaw.Select(
                item => Tuple.Create(item, FeatureNotAvailableCommand));
            PopularProducts = new List<Tuple<ProductDTO, ICommand>>(popularProductsWithCommand);

            var randomProducts = popularProductsRaw.Shuffle().Take(3);
            PreviouslySeenProducts = new List<ProductDTO>(randomProducts);
        }
    }
}
