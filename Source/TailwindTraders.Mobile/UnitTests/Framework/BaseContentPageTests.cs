using Moq;
using NUnit.Framework;
using TailwindTraders.Mobile.Framework;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Mocks;

namespace UnitTests.Framework
{
    public class BaseContentPageTests
    {
        private Mock<BaseViewModel> mockViewModel;
        private TestPage page;

        [SetUp]
        public void SetUp()
        {
            MockForms.Init();

            mockViewModel = new Mock<BaseViewModel>();
            page = new TestPage { BindingContext = mockViewModel.Object };
        }

        [Test]
        public void ViewModelInitializedJustOnce()
        {
            page.SimulateDoubleAppearing();

            mockViewModel.Verify(viewModel => viewModel.InitializeAsync(), Times.Once);
        }

        [Test]
        public void ViewModelUninitializedJustOnce()
        {
            page.SimulateDoubleDisappearing();

            mockViewModel.Verify(viewModel => viewModel.UninitializeAsync(), Times.Once);
        }

        private class TestPage : BaseContentPage<BaseViewModel>
        {
            public TestPage()
            {
                NavigationProxy.Inner = new NavigationProxy();
            }

            internal void SimulateDoubleAppearing()
            {
                OnAppearing();
                OnAppearing();
            }

            internal void SimulateDoubleDisappearing()
            {
                OnDisappearing();
                OnDisappearing();
            }
        }
    }
}
