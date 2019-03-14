using System.Threading.Tasks;

namespace TailwindTraders.Mobile.Features.LogIn
{
    public class FakeLoginAPI : ILoginAPI
    {
        public Task<LoginResponseDTO> LoginAsync(TokenRequestDTO request)
        {
            return Task.FromResult<LoginResponseDTO>(
                new LoginResponseDTO
                {
                    AccessToken = "Faketoken",
                    ExpiresIn = 1000,
                    TokenType = "tokenfaketype",
                });
        }
    }
}