using System.Collections.Generic;
using System.Threading.Tasks;
using Refit;

namespace TailwindTraders.Mobile.Features.LogIn
{
    public interface IProfilesAPI
    {
        [Get("/")]
        Task<IEnumerable<ProfileDTO>> GetAsync(
            [Header(Settings.Settings.ApiAuthorizationHeader)] string authorizationHeader);
    }
}
