using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TailwindTraders.Mobile.Framework;

namespace TailwindTraders.Mobile.Features.Product.Cart
{
    public class ProductCartViewModel : BaseStateAwareViewModel<ProductCartViewModel.State>
    {
        public enum State
        {
            Initial,
            CartHasItems,
            Error,
            Empty,
        }

        private ObservableCollection<ProductCartLineDTO> cartLines;
        private float cartTotal;
        private int cartLinesCount;

        public ObservableCollection<ProductCartLineDTO> CartLines
        {
            get => cartLines;
            set => SetAndRaisePropertyChanged(ref cartLines, value);
        }

        public float CartTotal
        {
            get => cartTotal;
            set => SetAndRaisePropertyChanged(ref cartTotal, value);
        }

        public int CartLinesCount
        {
            get => cartLinesCount;
            set => SetAndRaisePropertyChanged(ref cartLinesCount, value);
        }

        public ICommand LoadCommand => new AsyncCommand(_ => LoadDataAsync());

        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();

            PropertyChanged += ProductCartViewModel_PropertyChanged;

            CurrentState = State.Initial;

            LoadCommand.Execute(null);
        }

        private void ProductCartViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(CartLines))
            {
                OnCartLinesChanged();
            }
        }

        private void CartLines_CollectionChanged(
            object sender, 
            System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnCartLinesChanged();
        }

        private void OnCartLinesChanged()
        {
            CartTotal = CartLines.Aggregate(0f, (result, cartLine) => result + cartLine.Product.Price);
            CartLinesCount = CartLines.Count;
        }        

        private async Task LoadDataAsync()
        {
            var productsInCartResult = await TryExecuteWithLoadingIndicatorsAsync(
                RestPoolService.ProductCartAPI.GetCartLinesAsync());

            if (productsInCartResult.IsError || productsInCartResult.Value == null)
            {
                CurrentState = State.Error;
                return;
            }

            CartLines = new ObservableCollection<ProductCartLineDTO>(productsInCartResult.Value);
            CartLines.CollectionChanged += CartLines_CollectionChanged;

            if (!CartLines.Any())
            {
                CurrentState = State.Empty;
                return;
            }

            CurrentState = State.CartHasItems;
        }
    }
}
