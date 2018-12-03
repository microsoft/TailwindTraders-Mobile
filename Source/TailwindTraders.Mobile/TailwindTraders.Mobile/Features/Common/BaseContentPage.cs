using Xamarin.Forms;

namespace TailwindTraders.Mobile.Features.Common
{
    public abstract class BaseContentPage<T> : ContentPage
        where T : BaseViewModel
    {
        protected virtual T ViewModel => BindingContext as T;

        protected override void OnAppearing()
        {
            base.OnAppearing();

            ViewModel.InitializeAsync();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            ViewModel.UninitializeAsync();
        }
    }
}
