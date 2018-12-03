using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace TailwindTraders.Mobile.Features.Common
{
    public static class FakeNetwork
    {
        private static readonly Random random = new Random();

        public static async Task<T> ReturnAsync<T>(T result)
        {
            await Task.Delay(TimeSpan.FromSeconds(1));

            if (random.Next() % 4 == 0)
            {
                throw new HttpRequestException();
            }

            return result;
        }
    }
}
