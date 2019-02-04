using System.Collections.Generic;
using System.ComponentModel;
using Xamarin.Forms;

namespace TailwindTraders.Mobile.Framework
{
    public abstract class BaseStateAwareContentPage<TViewModel, TEnum> : BaseContentPage<TViewModel>
        where TViewModel : BaseStateAwareViewModel<TEnum>
        where TEnum : struct
    {
        private readonly List<VisualElement> statefullVisualElements = new List<VisualElement>();

        protected override void OnAppearing()
        {
            statefullVisualElements.AddRange(GetStateAwareVisualElements());
            ViewModel.PropertyChanged += ViewModel_PropertyChanged;

            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            ViewModel.PropertyChanged -= ViewModel_PropertyChanged;
            statefullVisualElements.Clear();

            base.OnDisappearing();
        }

        internal abstract IEnumerable<VisualElement> GetStateAwareVisualElements();

        private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ViewModel.CurrentState))
            {
                UpdateVisualStateManager(ViewModel.CurrentState.ToString());
            }
        }

        private void UpdateVisualStateManager(string name)
        {
            foreach (var item in statefullVisualElements)
            {
                VisualStateManager.GoToState(item, name);
            }
        }
    }
}
