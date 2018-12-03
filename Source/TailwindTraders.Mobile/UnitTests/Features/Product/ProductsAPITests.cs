using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using NUnit.Framework;
using Refit;
using TailwindTraders.Mobile.Features.Common;
using TailwindTraders.Mobile.Features.Product;
using TailwindTraders.Mobile.Features.Settings;

namespace UnitTests.Features.Product
{
    public class ProductsAPITests
    {
        private IProductsAPI productsAPI;

        [SetUp]
        public void Init()
        {
            productsAPI = RestService.For<IProductsAPI>(HttpClientFactory.Create(Settings.ProductApiUrl));
        }

        [Test]
        public async Task GetDetailAsync()
        {
            var product = await productsAPI.GetDetailAsync(Settings.AnonymousToken, "1");

            Assert.AreEqual(product.Id, 1);
        }

        [Test]
        public async Task GetProductsAsync()
        {
            var products = await productsAPI.GetProductsAsync(Settings.AnonymousToken, "1");

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

                var products = await productsAPI.GetSimilarProductsAsync(Settings.AnonymousToken, streamPart);

                Assert.IsNotEmpty(products);
            }
        }
    }
}
