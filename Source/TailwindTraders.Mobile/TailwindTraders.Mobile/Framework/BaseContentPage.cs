using Xamarin.Forms;

namespace TailwindTraders.Mobile.Framework
{
    public abstract class BaseContentPage<T> : ContentPage
        where T : BaseViewModel
    {
        protected virtual T ViewModel => BindingContext as T;

        protected override void OnAppearing()
        {
            base.OnAppearing();

            NavigationProxy.Inner = App.NavigationRoot.NavigationProxy;

            ViewModel.InitializeAsync();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            ViewModel.UninitializeAsync();
        }
    }
}
