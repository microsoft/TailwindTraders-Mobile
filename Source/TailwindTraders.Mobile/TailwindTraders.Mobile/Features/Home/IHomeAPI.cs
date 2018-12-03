using System.Threading.Tasks;
using Refit;

namespace TailwindTraders.Mobile.Features.Home
{
    public interface IHomeAPI
    {
        [Get("/landing")]
        Task<LandingDTO> GetAsync([Header(Settings.Settings.ApiAuthorizationHeader)] string authorizationHeader);
    }
}
