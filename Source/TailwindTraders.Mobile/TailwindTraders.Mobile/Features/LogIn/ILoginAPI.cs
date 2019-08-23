using System.Threading.Tasks;
using Refit;

namespace TailwindTraders.Mobile.Features.LogIn
{
    public interface ILoginAPI
    {
        [Post("/login")]
        Task<LoginResponseDTO> LoginAsync(LoginRequestDTO request);
    }
}
