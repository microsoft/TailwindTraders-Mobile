using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Xamarin.Forms;

namespace TailwindTraders.Mobile.Features.Product.Category
{
    [QueryProperty("Category", "category")]
    public partial class ProductCategoryPage
    {
        private string category;

        public string Category
        {
            get
            {
                return category;
            }

            set
            {
                category = value;
                BindingContext = new ProductCategoryViewModel(Category);
            }
        }

        public ProductCategoryPage()
        {
            InitializeComponent();           
        }

        internal override IEnumerable<VisualElement> GetStateAwareVisualElements() => new VisualElement[]
        {
            infoLabel,
            refreshButton,
        };

        protected override void OnAppearing()
        {
            ViewModel.PropertyChanged += ViewModel_PropertyChanged;

            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            ViewModel.PropertyChanged -= ViewModel_PropertyChanged;

            base.OnDisappearing();
        }

        private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ViewModel.Products))
            {
                WireProductsUpWithCollectionView();
            }
        }

        private void WireProductsUpWithCollectionView()
        {
            IList products;

            if (ViewModel.Products == null)
            {
                products = null;
            }
            else
            {
                var rawProducts = ViewModel.Products
                    .Select(item => new ProductViewModel(item, ViewModel.DetailCommand))
                    .ToList();

                if (rawProducts.Count() % 2 == 1)
                {
                    rawProducts.Add(null);
                }

                var evenProducts = rawProducts.Where((_, index) => index % 2 == 0);
                var oddProducts = rawProducts.Where((_, index) => index % 2 == 1);

                var groupedProducts = evenProducts.Zip(oddProducts, Tuple.Create);
                products = new List<Tuple<ProductViewModel, ProductViewModel>>(groupedProducts);
            }

            productsCollectionView.ItemsSource = products;
        }
    }
}
