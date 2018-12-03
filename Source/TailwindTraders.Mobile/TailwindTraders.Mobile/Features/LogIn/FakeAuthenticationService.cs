using System;
using System.Threading.Tasks;

namespace TailwindTraders.Mobile.Features.LogIn
{
    public class FakeAuthenticationService : IAuthenticationService
    {
        public string AuthorizationHeader => string.Empty;

        public bool IsAnyOneLoggedIn { get; private set; } = false;

        public async Task LogInAsync(string email, string password)
        {
            await Task.Delay(TimeSpan.FromSeconds(2));

            IsAnyOneLoggedIn = true;
        }

        public async Task LogInWithMicrosoftAsync()
        {
            await Task.Delay(TimeSpan.FromSeconds(2));

            IsAnyOneLoggedIn = true;
        }

        public void LogOut()
        {
        }

        public async Task RefreshSessionAsync()
        {
            await Task.Delay(TimeSpan.FromSeconds(2));
        }
    }
}
