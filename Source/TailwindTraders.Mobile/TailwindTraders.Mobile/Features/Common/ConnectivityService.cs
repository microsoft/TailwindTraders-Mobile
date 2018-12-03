using TailwindTraders.Mobile.Features.Common;
using Xamarin.Essentials;
using Xamarin.Forms;

[assembly: Dependency(typeof(ConnectivityService))]

namespace TailwindTraders.Mobile.Features.Common
{
    internal class ConnectivityService : IConnectivityService
    {
        public bool IsThereInternet => Connectivity.NetworkAccess == NetworkAccess.Internet;
    }
}
