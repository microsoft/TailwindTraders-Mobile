using System.Threading.Tasks;

namespace TailwindTraders.Mobile.Features.LogIn
{
    public interface IAuthenticationService
    {
        string AuthorizationHeader { get; }

        bool IsAnyOneLoggedIn { get; }

        Task LogInAsync(string email, string password);
    }
}