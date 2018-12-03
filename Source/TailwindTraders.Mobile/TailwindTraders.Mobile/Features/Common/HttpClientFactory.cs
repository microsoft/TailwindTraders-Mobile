using System;
using System.Net.Http;

namespace TailwindTraders.Mobile.Features.Common
{
    public static class HttpClientFactory
    {
        public static HttpClient Create(string baseAddress) => new HttpClient
        {
            BaseAddress = new Uri(baseAddress),
            Timeout = TimeSpan.FromSeconds(5),
        };
    }
}
