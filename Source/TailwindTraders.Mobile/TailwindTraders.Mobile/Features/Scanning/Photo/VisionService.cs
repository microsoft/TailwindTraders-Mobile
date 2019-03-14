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
        private readonly IRestPoolService restPoolService;

        public VisionService()
        {
            authenticationService = DependencyService.Get<IAuthenticationService>();
            restPoolService = DependencyService.Get<IRestPoolService>();
        }

        public async Task<IEnumerable<ProductDTO>> GetRecommendedProductsFromPhotoAsync(string photoPath)
        {
            using (var photoStream = File.Open(photoPath, FileMode.Open))
            {
                var streamPart = new StreamPart(photoStream, "photo.jpg", "image/jpeg");

                return await restPoolService.SimilarProductsAPI.GetSimilarProductsAsync(
                    authenticationService.AuthorizationHeader, streamPart);
            }
        }
    }
}
