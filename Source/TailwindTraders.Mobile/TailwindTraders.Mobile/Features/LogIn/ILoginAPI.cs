using System.Collections.Generic;
using System.Threading.Tasks;
using Refit;
using TailwindTraders.Mobile.Features.Settings;

namespace TailwindTraders.Mobile.Features.LogIn
{
    public interface ILoginAPI
    {
        [Post("/")]
        Task<LoginResponseDTO> LoginAsync(TokenRequestDTO request);
    }
}
