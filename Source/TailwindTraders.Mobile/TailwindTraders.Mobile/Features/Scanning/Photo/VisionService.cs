using Refit;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TailwindTraders.Mobile.Features.Common;
using TailwindTraders.Mobile.Features.LogIn;
using TailwindTraders.Mobile.Features.Product;
using TailwindTraders.Mobile.Features.Scanning.Photo;
using Xamarin.Forms;

[assembly: Dependency(typeof(VisionService))]

namespace TailwindTraders.Mobile.Features.Scanning.Photo
{
    public class VisionService : IVisionService
    {
        private readonly IAuthenticationService authenticationService;
        private readonly IProductsAPI productsAPI;

        public VisionService()
        {
            authenticationService = DependencyService.Get<IAuthenticationService>();
            productsAPI = DependencyService.Get<IRestPoolService>().ProductsAPI.Value;
        }

        public async Task<IEnumerable<ProductDTO>> GetRecommendedProductsFromPhotoAsync(string photoPath)
        {
            using (var photoStream = File.Open(photoPath, FileMode.Open))
            {
                var streamPart = new StreamPart(photoStream, "photo.jpg", "image/jpeg");

                return await productsAPI.GetSimilarProductsAsync(authenticationService.AuthorizationHeader, streamPart);
            }
        }
    }
}
