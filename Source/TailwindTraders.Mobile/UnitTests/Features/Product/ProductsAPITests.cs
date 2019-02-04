using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using NUnit.Framework;
using Refit;
using TailwindTraders.Mobile.Features.Product;
using TailwindTraders.Mobile.Features.Settings;
using TailwindTraders.Mobile.Helpers;

namespace UnitTests.Features.Product
{
#if !DEBUG
    [Ignore(Constants.IgnoreReason)]
#endif
    public class ProductsAPITests
    {
        private IProductsAPI productsAPI;

        [SetUp]
        public void Init()
        {
            productsAPI = RestService.For<IProductsAPI>(HttpClientFactory.Create(DefaultSettings.ProductApiUrl));
        }

        [Test]
        public async Task GetDetailAsync()
        {
            var product = await productsAPI.GetDetailAsync(DefaultSettings.AnonymousToken, "1");

            Assert.AreEqual(product.Id, 1);
        }

        [Test]
        public async Task GetProductsAsync()
        {
            var products = await productsAPI.GetProductsAsync(DefaultSettings.AnonymousToken, "1");

            Assert.IsNotEmpty(products.Products);
        }

        [Test]
        public async Task GetRecommendedProductsAsync()
        {
            var assemblyPath = Path.GetDirectoryName(Assembly.GetAssembly(typeof(ProductsAPITests)).Location);

            var img = Path.Combine(
                assemblyPath,
                "Assets/Vision/Test/Multitool/Multitool1.jpg");
            using (var photoStream = File.Open(img, FileMode.Open))
            {
                var streamPart = new StreamPart(photoStream, "photo.jpg", "image/jpeg");

                var products = await productsAPI.GetSimilarProductsAsync(DefaultSettings.AnonymousToken, streamPart);

                Assert.IsNotEmpty(products);
            }
        }
    }
}
