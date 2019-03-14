using NUnit.Framework;
using Refit;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TailwindTraders.Mobile.Features.LogIn;
using TailwindTraders.Mobile.Features.Settings;

namespace UnitTests.Framework
{
    public abstract class BaseAPITest
    {
        protected readonly ILoginAPI LoginApi;
        protected string authenticationBearer;
        protected bool isSuccess;
        protected Exception failureException = new Exception();

        public BaseAPITest()
        {   
            LoginApi = RestService.For<ILoginAPI>(DefaultSettings.RootApiUrl);
        }

        protected Task PreauthenticateAsync(
            Func<Task> actualTestAsync,
            string username = null,
            string password = null) =>
            PreauthenticateAsync<bool>(
                async () =>
                {
                    await actualTestAsync();

                    return true;
                },
                username,
                password);

        protected async Task<T> PreauthenticateAsync<T>(
            Func<Task<T>> actualTestAsync,
            string username = null,
            string password = null)
        {
            T result = default(T);
            var actualUsername = username ?? Constants.User;
            var actualPassword = password ?? Constants.Pass;

            try
            {
                var request = new TokenRequestDTO()
                {
                    GrantType = "password",
                    Password = Constants.Pass,
                    Username = Constants.User,
                };

                var authResult = await LoginApi.LoginAsync(request);

                DefaultSettings.AccessToken = authResult.AccessToken;
                authenticationBearer = $"Bearer {DefaultSettings.AccessToken}";

                result = await actualTestAsync();
                isSuccess = true;
            }
            catch (Exception exception)
            {
                isSuccess = false;
                failureException = exception;
            }

            Assert.True(isSuccess, $"It shouldn't have thrown any exception, but got {failureException}");

            return result;
        }
    }
}
