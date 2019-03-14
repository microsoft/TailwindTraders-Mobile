using System.Collections.Generic;
using System.Threading.Tasks;
using Refit;
using TailwindTraders.Mobile.Features.Settings;

namespace TailwindTraders.Mobile.Features.LogIn
{
    public interface IProfilesAPI
    {
        [Get("/profiles")]        
        Task<IEnumerable<ProfileDTO>> GetAsync(
            [Header(DefaultSettings.ApiAuthorizationHeader)] string authorizationHeader);
    }
}
