using System;
using System.Linq;
using System.Threading.Tasks;
using TailwindTraders.Mobile.Features.Common;
using Xamarin.Forms;

namespace TailwindTraders.Mobile.Features.LogIn
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IProfilesAPI profilesAPI;

        private string authenticatedUser;

        public AuthenticationService()
        {
            profilesAPI = DependencyService.Get<IRestPoolService>().ProfilesAPI.Value;
        }

        public string AuthorizationHeader => $"Email {authenticatedUser}";

        public bool IsAnyOneLoggedIn => authenticatedUser != null;

        public async Task LogInAsync(string email, string password)
        {
            var profiles = await profilesAPI.GetAsync(Settings.Settings.AnonymousToken);

            if (!profiles.Any())
            {
                throw new InvalidOperationException("No profile could be retrieved.");
            }

            authenticatedUser = profiles.First().Email;
        }
    }
}
